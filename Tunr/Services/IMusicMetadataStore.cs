using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tunr.Models.Library;

namespace Tunr.Services
{
    public interface IMusicMetadataStore
    {
        Task AddTrackAsync(Track track);
        Task RemoveTrackAsync(Guid trackId);
        Task<Track> GetTrackAsync(Guid trackId);
        Task<IEnumerable<string>> FetchUniqueUserTrackPropertyValuesAsync(
            Guid userId,
            string propertyName,
            Dictionary<string, string> filters);
    }
}