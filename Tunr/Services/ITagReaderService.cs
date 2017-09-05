using System.IO;
using System.Threading.Tasks;
using Tunr.Models;

namespace Tunr.Services
{
    public interface ITagReaderService
    {
        Task<TrackTags> GetTagsAsync(Stream fileStream, string fileName);
    }
}