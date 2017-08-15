using System;
using System.IO;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace Tunr.Utilities.FFmpeg
{
    public unsafe static class FFmpegHelper
    {
        private static bool isInitialized = false;
        public static bool IsInitialized
        {
            get
            {
                return isInitialized;
            }
            private set
            {
                isInitialized = value;
            }
        }

        public static void Init()
        {
            if (IsInitialized)
            {
                return;
            }
            FFmpegBinariesHelper.RegisterFFmpegBinaries();
            ffmpeg.av_register_all();
            ffmpeg.avcodec_register_all();
            ffmpeg.avformat_network_init();
            Console.WriteLine($"FFmpeg version info: {ffmpeg.av_version_info()}");
            // setup logging
            ffmpeg.av_log_set_level(ffmpeg.AV_LOG_VERBOSE);
            av_log_set_callback_callback logCallback = (p0, level, format, vl) =>
            {
                var lineSize = 1024;
                var lineBuffer = stackalloc byte[lineSize];
                var printPrefix = 1;
                ffmpeg.av_log_format_line(p0, level, format, vl, lineBuffer, lineSize, &printPrefix);
                var line = Marshal.PtrToStringAnsi((IntPtr)lineBuffer);
                Console.WriteLine(line);
            };
            ffmpeg.av_log_set_callback(logCallback);
            IsInitialized = true;
        }

        public static FFmpegContext ReadSoundFileInfo(Stream stream)
        {
            Init();
            var ffmpegContext = FFmpegContext.Read(stream);
            ffmpegContext.FindStreamInfo();
            ffmpegContext.SelectStream(AVMediaType.AVMEDIA_TYPE_AUDIO);
            return ffmpegContext;
        }
    }
}