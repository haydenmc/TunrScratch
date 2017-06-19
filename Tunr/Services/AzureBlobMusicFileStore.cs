using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;

namespace Tunr.Services
{
    public class AzureBlobMusicFileStore : IMusicFileStore
    {
        private const string ContainerName = "musicfiles";

        private readonly IOptions<AzureBlobMusicFileStoreOptions> options;

        public AzureBlobMusicFileStore(IOptions<AzureBlobMusicFileStoreOptions> options)
        {
            this.options = options;
        }

        public Task<Uri> GetFileLocationAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetFileStreamAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task PutFileAsync(Guid id, Stream fileStream)
        {
            var storageAccount = createStorageAccountFromConnectionString(options.Value.StorageAccountConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var libraryContainer = blobClient.GetContainerReference(ContainerName);
            await libraryContainer.CreateIfNotExistsAsync();
            var musicFileBlob = libraryContainer.GetBlockBlobReference(id.ToString());
            await musicFileBlob.UploadFromStreamAsync(fileStream);
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

    public class AzureBlobMusicFileStoreOptions
    {
        public string StorageAccountConnectionString { get; set; }
    }

    public static class AzureBlobMusicFileStoreExtensionMethods
    {
        public static IServiceCollection AddAzureBlobMusicFileStore(this IServiceCollection services, Action<AzureBlobMusicFileStoreOptions> setupAction)
        {
            // Configure the options manually.
            services.Configure(setupAction);
            // Add the service.
            return services.AddTransient<IMusicFileStore, AzureBlobMusicFileStore>();
        }
    }
}