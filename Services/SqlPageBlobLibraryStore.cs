using System;
using System.Threading.Tasks;
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

        private const Int64 InitialBlobSize = 1024 * 2 * 256; // Songs are in 2048 byte blocks

        private const Int64 BlobSizeIncrement = InitialBlobSize;

        private readonly IOptions<SqlPageBlobLibraryStoreOptions> options;

        private readonly ApplicationDbContext dbContext;
        
        SqlPageBlobLibraryStore(IOptions<SqlPageBlobLibraryStoreOptions> options, ApplicationDbContext dbContext)
        {
            this.options = options;
            this.dbContext = dbContext;
        }

        public async Task AddTrackAsync(Track track, TunrUser user)
        {
            var storageAccount = createStorageAccountFromConnectionString(options.Value.StorageAccountConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var libraryContainer = blobClient.GetContainerReference(ContainerName);
            await libraryContainer.CreateIfNotExistsAsync();
            var userLibraryPageBlob = libraryContainer.GetPageBlobReference(user.Id.ToString());
            if (!(await userLibraryPageBlob.ExistsAsync()))
            {
                await userLibraryPageBlob.CreateAsync(InitialBlobSize);
            }

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