using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Track> GetTrackAsync(Guid trackId)
        {
            return await dbContext.Tracks.FindAsync(trackId);
        }

        public Task RemoveTrackAsync(Guid trackId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> FetchUniqueUserTrackPropertyValuesAsync(Guid userId, string propertyName)
        {
            // CONSIDER: A faster (but less concise) way of doing this would involve generics everywhere.
            // Use reflection to determine whether this field is valid
            var property = typeof(Track).GetProperty(propertyName);
            if (property == null)
            {
                throw new ArgumentOutOfRangeException("The specified property is invalid");
            }
            // Grab all tracks, select the value of the specified property
            var tracks = await dbContext.Tracks.Where(t => t.UserId == userId).ToListAsync();
            var trackPropertyValues = tracks.Select(t => property.GetValue(t) as string).Distinct();
            return trackPropertyValues;
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