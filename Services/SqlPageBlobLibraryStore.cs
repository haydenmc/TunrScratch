using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Tunr.Models;
using Tunr.Models.Library;

namespace Tunr.Services
{
    public class SqlPageBlobLibraryStore : ILibraryStore
    {
        private const string ContainerName = "library";

        private const ushort SongBlobSizeBytes = 2048;

        private const uint InitialBlobSizeBytes = SongBlobSizeBytes * 256; // Songs are in 2048 byte blocks

        private const uint BlobSizeIncrementBytes = InitialBlobSizeBytes;

        private const ushort MaxSqlConcurrencyRetryAttempts = 5;

        private const ushort CurrentBlobSchemaVersion = 1;

        private readonly IOptions<SqlPageBlobLibraryStoreOptions> options;

        private readonly ApplicationDbContext dbContext;
        
        SqlPageBlobLibraryStore(IOptions<SqlPageBlobLibraryStoreOptions> options, ApplicationDbContext dbContext)
        {
            this.options = options;
            this.dbContext = dbContext;
        }

        public async Task AddTrackAsync(Track track, TunrUser user)
        {
            // Prep our blob reference
            var storageAccount = createStorageAccountFromConnectionString(options.Value.StorageAccountConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var libraryContainer = blobClient.GetContainerReference(ContainerName);
            await libraryContainer.CreateIfNotExistsAsync();
            var userLibraryPageBlob = libraryContainer.GetPageBlobReference(user.Id.ToString());
            if (!(await userLibraryPageBlob.ExistsAsync()))
            {
                await userLibraryPageBlob.CreateAsync(InitialBlobSizeBytes);
            }

            // Update our page offset in our SQL store
            ulong pageOffset;
            for (uint attempts = 0;;attempts++)
            {
                try
                {
                    var dbUser = await dbContext.Users.FindAsync(user.Id);
                    pageOffset = dbUser.LibraryOffset;
                    dbUser.LibraryOffset += SongBlobSizeBytes;
                    await dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (attempts >= MaxSqlConcurrencyRetryAttempts)
                    {
                        throw new Exception("Could not increment page offset in SQL store", e);
                    }
                    continue;
                }
                break;
            }

            // TODO: Check if we need to grow our page blob to accomodate the new entry

            // Insert track into SQL store
            track.TrackId = Guid.NewGuid();
            track.StorageLocation = pageOffset;
            track.UserId = user.Id;
            dbContext.Tracks.Add(track);
            await dbContext.SaveChangesAsync();
            // TODO: On failure here, we need to roll-back our byte offset allocation (or mark it as available)

            // Write track to page blob
        }

        public async Task RemoveTrackAsync(Guid trackId)
        {

        }

        public async Task<Track> GetTrackAsync(Guid trackId)
        {

        }

        public static void AddSqlPageBlobLibraryStore(this IServiceCollection services, Action<SqlPageBlobLibraryStoreOptions> setupAction)
        {
            // Add the service.
            services.AddTransient<ILibraryStore, SqlPageBlobLibraryStore>();

            // Configure the options manually.
            services.Configure(setupAction);
        }

        private Stream TrackToStream(Track track)
        {
            byte[] buffer; // Buffer for converting data
            var stream = new MemoryStream(new byte[SongBlobSizeBytes]);
            // SchemaVersion, 2 bytes
            stream.Write(BitConverter.GetBytes(CurrentBlobSchemaVersion), 0, 2);
            // GUID, 16 bytes
            stream.Write(track.TrackId.ToByteArray(), 2, 16);
            // FileRelativePath, 256 bytes
            buffer = Encoding.UTF8.GetBytes(track.FileRelativePath);
            stream.Write(buffer, 18, buffer.Length);
            // FileName, 128 bytes
            buffer = Encoding.UTF8.GetBytes(track.FileName);
            stream.Write(buffer, 274, buffer.Length);
            // FileExtension, 32 bytes
            buffer = Encoding.UTF8.GetBytes(track.FileExtension);
            stream.Write(buffer, 402, buffer.Length);
            // FileSizeBytes, 4 bytes
            stream.Write(BitConverter.GetBytes(track.FileSizeBytes), 434, 4);
            // FileSha256Hash, 32 bytes
            stream.Write(track.FileSha256Hash, 438, 32);
            // AudioChannels, 1 byte
            stream.Write(new byte[] { track.AudioChannels }, 470, 1);
            // AudioBitrateKbps, 2 bytes
            stream.Write(BitConverter.GetBytes(track.AudioBitrateKbps), 471, 2);
            // AudioSampleRateHz, 4 bytes
            stream.Write(BitConverter.GetBytes(track.AudioSampleRateHz), 473, 4);
            // AudioDurationSeconds, 2 bytes
            stream.Write(BitConverter.GetBytes(track.AudioDurationSeconds), 477, 2);
            // TagTitle, 256 bytes
            buffer = Encoding.UTF8.GetBytes(track.TagTitle);
            stream.Write(buffer, 479, buffer.Length);
            // TagPerformers, 256 bytes
            buffer = Encoding.UTF8.GetBytes(track.TagTitle);
            stream.Write(buffer, 479, buffer.Length);
        }

        private CloudStorageAccount createStorageAccountFromConnectionString(string connectionString)
        {
            CloudStorageAccount storageAccount;
            try
            {
                storageAccount = CloudStorageAccount.Parse(connectionString);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid.");
                Console.ReadLine(); 
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid.");
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }
    }

    public class SqlPageBlobLibraryStoreOptions
    {
        public string StorageAccountConnectionString { get; set; }
    }
}