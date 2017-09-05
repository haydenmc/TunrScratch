using System.IO;
using System.Threading.Tasks;
using Tunr.Models;

namespace Tunr.Services
{
    public interface IAudioInfoReaderService
    {
        Task<AudioInfo> GetAudioInfoAsync(Stream fileStream, string fileName = "");
    }
}