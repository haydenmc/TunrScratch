namespace Tunr.Models
{
    public class AudioInfo
    {
        public byte Channels { get; set; }

        public short BitrateKbps { get; set; }

        public int SampleRateHz { get; set; }

        public short DurationSeconds { get; set; }
    }
}