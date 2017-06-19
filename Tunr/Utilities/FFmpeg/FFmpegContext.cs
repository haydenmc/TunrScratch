using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace Tunr.Utilities.FFmpeg
{
    /// <summary>
    /// Helper class for FFmpeg functions
    /// </summary>
    /// <remarks>
    /// Credit to Pulsus (https://github.com/GoaLitiuM/Pulsus)
    /// </remarks>
    public unsafe class FFmpegContext
    {
        // Source stream
        private Stream stream;

        // FFmpeg references
        private AVFormatContext* formatContext = null;

        private AVIOContext* avioContext = null;

        private AVCodecContext* codecContext = null;

        private AVFrame* frame = null;

        private AVMediaType type;

        // Media data
        public int SampleRateHz { get; set; }
        public int Channels { get; set; }
        public int BitrateKbps { get; set; }
        public int DurationSeconds { get; set; }

        private int streamIndex = -1;

        private FFmpegContext(Stream stream)
        {
            this.stream = stream;
        }

        public static FFmpegContext Read(Stream stream)
        {
            var ffmpeg = new FFmpegContext(stream);
            ffmpeg.OpenInput();
            return ffmpeg;
        }

        private void OpenInput()
        {
            formatContext = ffmpeg.avformat_alloc_context();
            fixed (AVFormatContext** formatContextPtr = &formatContext)
            {
                int error;
                AVInputFormat* inputFormat = null;
                if (stream != null)
                {
                    const int bufferSize = 8192;

                    avio_alloc_context_read_packet readStream = (void* opaque, byte* buf, int bufSize) =>
                    {
                        byte[] temp = new byte[bufSize];
                        int read = stream.Read(temp, 0, bufSize);
                        Marshal.Copy(temp, 0, (IntPtr)buf, read);
                        return read;
                    };

                    avio_alloc_context_seek seekStream = (void* opaque, Int64 offset, int whence) =>
                    {
                        if (whence == ffmpeg.AVSEEK_SIZE)
                        {
                            return stream.Length;
                        }
                        if (!stream.CanSeek)
                        {
                            return -1;
                        }
                        stream.Seek(offset, (SeekOrigin)whence);
                        return stream.Position;
                    };

                    byte* readBuffer = (byte*)ffmpeg.av_malloc((ulong)bufferSize + (ulong)ffmpeg.FF_INPUT_BUFFER_PADDING_SIZE);
                    avioContext = ffmpeg.avio_alloc_context(readBuffer, bufferSize, 0, null, readStream, null, seekStream);

                    formatContext->pb = avioContext;
                    formatContext->flags |= ffmpeg.AVFMT_FLAG_CUSTOM_IO;
                    if ((error = ffmpeg.av_probe_input_buffer(formatContext->pb, &inputFormat, null, null, 0, 0)) != 0)
                    {
                        throw new Exception($"FFmpeg error: {error}");
                    }
                }
                frame = ffmpeg.av_frame_alloc();
            }
        }

        public void SelectStream(AVMediaType type)
        {
            this.type = type;
            streamIndex = ffmpeg.av_find_best_stream(formatContext, type, -1, -1, null, 0);
            if (streamIndex < 0)
            {
                throw new Exception($"Could not find stream for type {type.ToString()}");
            }
            SetupCodecContext(formatContext->streams[streamIndex]);
        }

        private void SetupCodecContext(AVStream* avStream)
        {
            AVCodec* decoder = ffmpeg.avcodec_find_decoder(avStream->codec->codec_id);
            if (decoder == null)
            {
                throw new Exception($"Could not find decoder for {avStream->codec->codec_id.ToString()}");
            }

            codecContext = ffmpeg.avcodec_alloc_context3(decoder);
            if (codecContext == null)
            {
                throw new Exception("Failed to allocate codec context");
            }

            int error;
            // TODO: Apparently this is obsolete
            if ((error = ffmpeg.avcodec_copy_context(codecContext, avStream->codec)) != 0)
            {
                throw new Exception($"Failed to copy codec context: {error}");
            }

            if ((error = ffmpeg.avcodec_open2(codecContext, decoder, null)) < 0)
            {
                throw new Exception($"Failed to open decoder for {codecContext->codec_id.ToString()}");
            }

            DurationSeconds = (int) avStream->duration / (avStream->time_base.num / avStream->time_base.den);
            Channels = codecContext->channels;
            SampleRateHz = codecContext->sample_rate;
            BitrateKbps = (int)(codecContext->bit_rate / 1024);
        }
    }
}