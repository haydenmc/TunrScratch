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

        public async Task<IEnumerable<string>> FetchUniqueUserTrackPropertyValuesAsync(
            Guid userId,
            string propertyName,
            Dictionary<string, string> filters)
        {
            var query = dbContext.Tracks.Where(t => t.UserId == userId);

            if (filters != null)
            {
                foreach (var filterPropertyName in filters.Keys)
                {
                    var filterPropertyValue = filters[filterPropertyName];
                    query = processTrackPropertyFilter(query, filterPropertyName, filterPropertyValue);
                }
            }

            switch (propertyName.ToLowerInvariant())
            {
                case "tagperformers":
                    return await query
                        .SelectMany(t => t.DbTagPerformers.Select(tp => tp.Performer))
                        .Distinct()
                        .ToArrayAsync();
                case "tagalbum":
                    return await query
                        .Select(t => t.TagAlbum)
                        .Distinct()
                        .ToArrayAsync();
                case "tagtitle":
                    return await query
                        .Select(t => t.TagTitle)
                        .Distinct()
                        .ToArrayAsync();
                default:
                    throw new ArgumentException("This property is not supported.");
            }
        }

        public async Task<IEnumerable<Track>> FetchUniqueUserTracksAsync(
            Guid userId,
            Dictionary<string, string> filters)
        {
            var query = dbContext.Tracks
                .Include(t => t.DbTagPerformers)
                .Include(t => t.DbTagComposers)
                .Include(t => t.DbTagGenres)
                .Where(t => t.UserId == userId);

            if (filters != null)
            {
                foreach (var filterPropertyName in filters.Keys)
                {
                    var filterPropertyValue = filters[filterPropertyName];
                    query = processTrackPropertyFilter(query, filterPropertyName, filterPropertyValue);
                }
            }

            return await query.ToListAsync();
        }

        private IQueryable<Track> processTrackPropertyFilter(IQueryable<Track> inQuery, string filterPropertyName, string filterPropertyValue)
        {
            switch (filterPropertyName.ToLowerInvariant())
            {
                case "tagperformers":
                    return inQuery.Where(
                        t => t.DbTagPerformers
                            .Where(tp => tp.Performer.ToLowerInvariant() == filterPropertyValue.ToLowerInvariant())
                            .FirstOrDefault() != null);
                case "tagalbum":
                    return inQuery.Where(t => t.TagAlbum.ToLowerInvariant() == filterPropertyValue.ToLowerInvariant());
                case "tagtitle":
                    return inQuery.Where(t => t.TagTitle.ToLowerInvariant() == filterPropertyValue.ToLowerInvariant());
                default:
                    throw new ArgumentException("This filter property is not supported.");
            }
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