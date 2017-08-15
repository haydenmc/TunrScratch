using System;
using System.IO;
using System.Threading.Tasks;
using Tunr.Models;
using Tunr.Utilities.FFmpeg;
using Microsoft.Extensions.DependencyInjection;

namespace Tunr.Services
{
    public class FFMpegAudioInfoService : IAudioInfoService
    {
        public async Task<AudioInfo> GetAudioInfoAsync(Stream fileStream, string fileName = "")
        {
            var ffmpegContext = FFmpegHelper.ReadSoundFileInfo(fileStream);
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

    public static class FFMpegAudioInfoServiceExtensionMethods
    {
        public static IServiceCollection AddFFMpegAudioInfoService(this IServiceCollection services)
        {
            // Add the service.
            return services.AddTransient<IAudioInfoService, FFMpegAudioInfoService>();
        }
    }
}