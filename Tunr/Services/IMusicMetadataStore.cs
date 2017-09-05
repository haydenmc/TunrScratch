using System;
using System.Threading.Tasks;
using Tunr.Models.Library;

namespace Tunr.Services
{
    public interface IMusicMetadataStore
    {
        Task AddTrackAsync(Track track);
        Task RemoveTrackAsync(Guid trackId);
        Task<Track> GetTrackAsync(Guid trackId);
    }
}