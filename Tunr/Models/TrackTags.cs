namespace Tunr.Models
{
    public class TrackTags
    {
        public string Title { get; set; }

        public string[] Performers { get; set; }

        public string AlbumArtist { get; set; }

        public string Album { get; set; }

        public string[] Composers { get; set; }

        public string[] Genres { get; set; }

        public string Comment { get; set; }

        public short Year { get; set; }

        public short BeatsPerMinute { get; set; }

        public short TrackNumber { get; set; }

        public short AlbumTrackCount { get; set; }

        public short DiscNumber { get; set; }

        public short AlbumDiscCount { get; set; }
    }
}