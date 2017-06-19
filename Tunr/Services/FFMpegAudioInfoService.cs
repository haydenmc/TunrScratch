using System;
using System.IO;
using System.Threading.Tasks;
using Tunr.Models;

namespace Tunr.Services
{
    public class FFMpegAudioInfoService : IAudioInfoService
    {
        public Task<AudioInfo> GetAudioInfoAsync(Stream fileStream, string fileName = "")
        {
            throw new NotImplementedException();
        }
    }
}