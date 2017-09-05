using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Tunr.Models;
using Tunr.Models.Library;

namespace Tunr.Services
{
    public class EntityFrameworkMusicMetadataStore : IMusicMetadataStore
    {
        private ApplicationDbContext dbContext;

        public EntityFrameworkMusicMetadataStore(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public Task AddTrackAsync(Track track)
        {
            throw new NotImplementedException();
        }

        public Task<Track> GetTrackAsync(Guid trackId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTrackAsync(Guid trackId)
        {
            throw new NotImplementedException();
        }
    }

    public static class EntityFrameworkMusicMetadataStoreExtensionMethods
    {
        public static IServiceCollection AddEntityFrameworkMusicMetadataStore(this IServiceCollection services)
        {
            // Add the service.
            return services.AddTransient<IMusicMetadataStore, EntityFrameworkMusicMetadataStore>();
        }
    }
}