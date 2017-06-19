using System.IO;
using System.Threading.Tasks;
using Tunr.Models;

namespace Tunr.Services
{
    public interface ITagService
    {
        Task<TrackTags> GetTagsAsync(Stream fileStream, string fileName);
    }
}