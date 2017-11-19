using System;
using System.Linq;
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

        public async Task AddTrackAsync(Track track)
        {
            // Insert normalized rows
            track.DbTagPerformers
                 = track.TagPerformers.Select(
                     tp => new TrackPerformer() { TrackId = track.TrackId, Performer = tp }
                 ).ToList();
            track.DbTagComposers
                = track.TagComposers.Select(
                    tc => new TrackComposer() { TrackId = track.TrackId, Composer = tc }
                ).ToList();
            track.DbTagGenres
                = track.TagGenres.Select(
                    tg => new TrackGenre() { TrackId = track.TrackId, Genre = tg }
                ).ToList();
            await dbContext.Tracks.AddAsync(track);
            await dbContext.SaveChangesAsync();
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