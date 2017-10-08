using System;
using System.IO;
using System.Threading.Tasks;
using Tunr.Models;
using Tunr.Utilities.FFmpeg;
using Microsoft.Extensions.DependencyInjection;

namespace Tunr.Services
{
    public class FFMpegAudioInfoReaderService : IAudioInfoReaderService
    {
#pragma warning disable CS1998
        public async Task<AudioInfo> GetAudioInfoAsync(Stream fileStream, string fileName = "")
        {
            using (var ffmpegContext = FFmpegHelper.ReadSoundFileInfo(fileStream))
            {
                var audioInfo = new AudioInfo()
                {
                    BitrateKbps = (short)ffmpegContext.BitrateKbps,
                    Channels = (byte)ffmpegContext.Channels,
                    DurationSeconds = (short)ffmpegContext.DurationSeconds,
                    SampleRateHz = ffmpegContext.SampleRateHz
                };
                return audioInfo;
            }
        }
#pragma warning restore CS1998
    }

    public static class FFMpegAudioInfoReaderServiceExtensionMethods
    {
        public static IServiceCollection AddFFMpegAudioInfoReaderService(this IServiceCollection services)
        {
            // Add the service.
            return services.AddTransient<IAudioInfoReaderService, FFMpegAudioInfoReaderService>();
        }
    }
}