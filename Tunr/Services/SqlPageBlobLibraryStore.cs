using System;
using System.IO;
using System.Linq;
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
        
        public SqlPageBlobLibraryStore(IOptions<SqlPageBlobLibraryStoreOptions> options, ApplicationDbContext dbContext)
        {
            this.options = options;
            this.dbContext = dbContext;
        }

        public async Task AddTrackAsync(Track track)
        {
            // Prep our blob reference
            var storageAccount = createStorageAccountFromConnectionString(options.Value.StorageAccountConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var libraryContainer = blobClient.GetContainerReference(ContainerName);
            await libraryContainer.CreateIfNotExistsAsync();
            var userLibraryPageBlob = libraryContainer.GetPageBlobReference(track.UserId.ToString());
            if (!(await userLibraryPageBlob.ExistsAsync()))
            {
                await userLibraryPageBlob.CreateAsync(InitialBlobSizeBytes);
            }

            // Update our page offset in our SQL store
            long pageOffset;
            for (uint attempts = 0;;attempts++)
            {
                try
                {
                    var dbUser = await dbContext.Users.FindAsync(track.UserId);
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
            track.StorageLocation = pageOffset;
            track.DbTagPerformers = track.TagPerformers.Select(tp => new TrackPerformer() { TrackId = track.TrackId, Performer = tp }).ToList();
            track.DbTagComposers = track.TagComposers.Select(tc => new TrackComposer() { TrackId = track.TrackId, Composer = tc }).ToList();
            track.DbTagGenres = track.TagGenres.Select(tg => new TrackGenre() { TrackId = track.TrackId, Genre = tg }).ToList();
            dbContext.Tracks.Add(track);
            await dbContext.SaveChangesAsync();
            // TODO: On failure here, we need to roll-back our byte offset allocation (or mark it as available)

            // Write track to page blob
            var trackBlobStream = TrackToStream(track);
            await userLibraryPageBlob.WritePagesAsync(trackBlobStream, (long)pageOffset, ""); // TODO: Calculate MD5
            // TODO: On failure here, we need to remove from DB and roll-back our byte offset allocation (or mark it as available)
        }

#pragma warning disable CS1998
        public async Task RemoveTrackAsync(Guid trackId)
        {
            throw new NotImplementedException();
        }
#pragma warning restore CS1998

        public async Task<Track> GetTrackAsync(Guid trackId)
        {
            // Fetch track from DB
            var dbTrack = await dbContext
                .Tracks
                .Include(t => t.DbTagPerformers)
                .Include(t => t.DbTagComposers)
                .Include(t => t.DbTagGenres)
                .SingleOrDefaultAsync(t => t.TrackId == trackId);
            return dbTrack;
        }

        private Stream TrackToStream(Track track)
        {
            byte[] buffer; // Buffer for converting data
            var stream = new MemoryStream(new byte[SongBlobSizeBytes]);
            // SchemaVersion, 2 bytes
            stream.Write(BitConverter.GetBytes(CurrentBlobSchemaVersion), 0, 2);
            // GUID, 16 bytes
            byte[] guidArray = track.TrackId.ToByteArray();
            stream.Write(track.TrackId.ToByteArray(), 0, 16);
            // FileRelativePath, 256 bytes
            buffer = Encoding.UTF8.GetBytes(track.FileRelativePath);
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(256 - buffer.Length, SeekOrigin.Current);
            // FileName, 128 bytes
            buffer = Encoding.UTF8.GetBytes(track.FileName);
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(128 - buffer.Length, SeekOrigin.Current);
            // FileExtension, 32 bytes
            buffer = Encoding.UTF8.GetBytes(track.FileExtension);
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(32 - buffer.Length, SeekOrigin.Current);
            // FileSizeBytes, 4 bytes
            stream.Write(BitConverter.GetBytes(track.FileSizeBytes), 0, 4);
            // FileSha256Hash, 32 bytes
            stream.Write(track.FileSha256Hash, 0, 32);
            // AudioChannels, 1 byte
            stream.Write(BitConverter.GetBytes(track.AudioChannels), 0, 1);
            // AudioBitrateKbps, 2 bytes
            stream.Write(BitConverter.GetBytes(track.AudioBitrateKbps), 0, 2);
            // AudioSampleRateHz, 4 bytes
            stream.Write(BitConverter.GetBytes(track.AudioSampleRateHz), 0, 4);
            // AudioDurationSeconds, 2 bytes
            stream.Write(BitConverter.GetBytes(track.AudioDurationSeconds), 0, 2);
            // TagTitle, 256 bytes
            buffer = Encoding.UTF8.GetBytes(track.TagTitle);
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(256 - buffer.Length, SeekOrigin.Current);
            // TagPerformers, 256 bytes
            // We're gonna use the BELL (U+0007) character as a delimiter. Because why not.
            buffer = Encoding.UTF8.GetBytes(String.Join("\u0007", track.TagPerformers));
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(256 - buffer.Length, SeekOrigin.Current);
            // TagAlbumArtist, 256 bytes
            buffer = Encoding.UTF8.GetBytes(track.TagAlbumArtist);
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(256 - buffer.Length, SeekOrigin.Current);
            // TagAlbum, 256 bytes
            buffer = Encoding.UTF8.GetBytes(track.TagAlbum);
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(256 - buffer.Length, SeekOrigin.Current);
            // TagComposers, 256 bytes
            buffer = Encoding.UTF8.GetBytes(String.Join("\u0007", track.TagComposers));
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(256 - buffer.Length, SeekOrigin.Current);
            // TagGenres, 128 bytes
            buffer = Encoding.UTF8.GetBytes(String.Join("\u0007", track.TagGenres));
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(128 - buffer.Length, SeekOrigin.Current);
            // TagComment, 128 bytes
            buffer = Encoding.UTF8.GetBytes(track.TagComment);
            stream.Write(buffer, 0, buffer.Length);
            stream.Seek(128 - buffer.Length, SeekOrigin.Current);
            // TagYear, 2 bytes
            stream.Write(BitConverter.GetBytes(track.TagYear), 0, 2);
            // TagBeatsPerMinute, 2 bytes
            stream.Write(BitConverter.GetBytes(track.TagBeatsPerMinute), 0, 2);
            // TagTrackNumber, 2 bytes
            stream.Write(BitConverter.GetBytes(track.TagTrackNumber), 0, 2);
            // TagAlbumTrackCount, 2 bytes
            stream.Write(BitConverter.GetBytes(track.TagAlbumTrackCount), 0, 2);
            // TagDiscNumber, 2 bytes
            stream.Write(BitConverter.GetBytes(track.TagDiscNumber), 0, 2);
            // TagAlbumDiscCount, 2 bytes
            stream.Write(BitConverter.GetBytes(track.TagAlbumDiscCount), 0, 2);
            // LibraryPlays, 4 bytes
            stream.Write(BitConverter.GetBytes(track.LibraryPlays), 0, 4);
            // LibraryRating, 1 byte
            stream.Write(BitConverter.GetBytes(track.LibraryRating), 0, 1);
            // LibraryDateTimeAdded, 8 bytes
            stream.Write(BitConverter.GetBytes(track.LibraryDateTimeAdded), 0, 8);
            // LibraryDateTimeModified, 8 bytes
            stream.Write(BitConverter.GetBytes(track.LibraryDateTimeModified), 0, 8);

            // Rewind to beginning of stream before returning
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
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

    public static class SqlPageBlobLibraryStoreExtensionMethods
    {
        public static IServiceCollection AddSqlPageBlobLibraryStore(this IServiceCollection services, Action<SqlPageBlobLibraryStoreOptions> setupAction)
        {
            // Configure the options manually.
            services.Configure(setupAction);
            // Add the service.
            return services.AddTransient<ILibraryStore, SqlPageBlobLibraryStore>();
        }
    }
}