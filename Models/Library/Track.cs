using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tunr.Models.Library
{
    /// <summary>
    /// Generic representation of a track as understood by Tunr
    /// </summary>
    public class Track
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid TrackId { get; set; }

        /// <summary>
        /// ID of the owner of this track
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Location of the track in library storage
        /// </summary>
        public ulong StorageLocation { get; set; }

        /// <summary>
        /// Relative path of the file on the original client
        /// </summary>
        public string FileRelativePath { get; set; }

        /// <summary>
        /// Name of the file on the original client
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Extension of the original file
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        public uint FileSizeBytes { get; set; }

        /// <summary>
        /// SHA-256 hash of the file
        /// </summary>
        public byte[] FileSha256Hash { get; set; }

        /// <summary>
        /// Number of audio channels
        /// </summary>
        public byte AudioChannels { get; set; }

        /// <summary>
        /// Compressed bitrate of the audio file in kilobits per second
        /// </summary>
        public ushort AudioBitrateKbps { get; set; }

        /// <summary>
        /// Sample rate of the audio file in hz
        /// </summary>
        public uint AudioSampleRateHz { get; set; }

        /// <summary>
        /// Duration of the audio file in seconds
        /// </summary>
        public ushort AudioDurationSeconds { get; set; }

        /// <summary>
        /// Title of this track
        /// </summary>
        public string TagTitle { get; set; }

        /// <summary>
        /// Performers of this track
        /// </summary>
        public string[] TagPerformers { get; set; }

        /// <summary>
        /// Artist credited with producing the album this track belongs to
        /// </summary>
        public string TagAlbumArtist { get; set; }

        /// <summary>
        /// Name of the album this track belongs to
        /// </summary>
        public string TagAlbum { get; set; }

        /// <summary>
        /// Composers of this track
        /// </summary>
        public string[] TagComposers { get; set; }

        /// <summary>
        /// Genres that this track belongs to
        /// </summary>
        public string[] TagGenres { get; set; }

        /// <summary>
        /// Comment tag of this track
        /// </summary>
        public string TagComment { get; set; }

        /// <summary>
        /// Year this track was released
        /// </summary>
        public ushort TagYear { get; set; }

        /// <summary>
        /// Beats per minute of this track
        /// </summary>
        public ushort TagBeatsPerMinute { get; set; }

        /// <summary>
        /// Track number of this track on the album it belongs to
        /// </summary>
        public ushort TagTrackNumber { get; set; }

        /// <summary>
        /// Total track number of the album this track belongs to
        /// </summary>
        public ushort TagAlbumTrackCount { get; set; }

        /// <summary>
        /// Disc number of this track on the album it belongs to
        /// </summary>
        public ushort TagDiscNumber { get; set; }

        /// <summary>
        /// Total disc count of the album this track belongs to
        /// </summary>
        public ushort TagAlbumDiscCount { get; set; }

        /// <summary>
        /// Number of times this track has been played
        /// </summary>
        public uint LibraryPlays { get; set; }

        /// <summary>
        /// User rating of this track
        /// </summary>
        public byte LibraryRating { get; set; }

        /// <summary>
        /// Time this track was added to the library
        /// </summary>
        public ulong LibraryDateTimeAdded { get; set; }

        /// <summary>
        /// Time this track was last modified in the library
        /// </summary>
        public ulong LibraryDateTimeModified { get; set; }
    }
}