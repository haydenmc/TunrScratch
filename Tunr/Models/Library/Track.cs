using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;

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
        public long StorageLocation { get; set; }

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
        public int FileSizeBytes { get; set; }

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
        public short AudioBitrateKbps { get; set; }

        /// <summary>
        /// Sample rate of the audio file in hz
        /// </summary>
        public int AudioSampleRateHz { get; set; }

        /// <summary>
        /// Duration of the audio file in seconds
        /// </summary>
        public short AudioDurationSeconds { get; set; }

        /// <summary>
        /// Title of this track
        /// </summary>
        public string TagTitle { get; set; }

        /// <summary>
        /// Backing field for TagPerformers
        /// </summary>
        private string[] tagPerformers;

        /// <summary>
        /// Performers of this track
        /// </summary>
        public string[] TagPerformers
        {
            get
            {
                if (tagPerformers == null)
                {
                    if (DbTagPerformers == null)
                    {
                        return null;
                    }
                    tagPerformers = DbTagPerformers.Select(g => g.Performer).ToArray();
                }
                return tagPerformers;
            }
            set
            {
                tagPerformers = value;
            }
        }

        /// <summary>
        /// Normalized Performers for Entity Framework
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<TrackPerformer> DbTagPerformers { get; set; }

        /// <summary>
        /// Artist credited with producing the album this track belongs to
        /// </summary>
        public string TagAlbumArtist { get; set; }

        /// <summary>
        /// Name of the album this track belongs to
        /// </summary>
        public string TagAlbum { get; set; }

        /// <summary>
        /// Backing field for TagComposers
        /// </summary>
        private string[] tagComposers;

        /// <summary>
        /// Composers of this track
        /// </summary>
        public string[] TagComposers
        {
            get
            {
                if (tagComposers == null)
                {
                    if (DbTagComposers == null)
                    {
                        return null;
                    }
                    tagComposers = DbTagComposers.Select(g => g.Composer).ToArray();
                }
                return tagComposers;
            }
            set
            {
                tagComposers = value;
            }
        }

        /// <summary>
        /// Normalized Composers for Entity Framework
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<TrackComposer> DbTagComposers { get; set; }

        /// <summary>
        /// Backing field for TagGenres
        /// </summary>
        private string[] tagGenres;

        /// <summary>
        /// Genres that this track belongs to
        /// </summary>
        public string[] TagGenres
        {
            get
            {
                if (tagGenres == null)
                {
                    if (DbTagGenres == null)
                    {
                        return null;
                    }
                    tagGenres = DbTagGenres.Select(g => g.Genre).ToArray();
                }
                return tagGenres;
            }
            set
            {
                tagGenres = value;
            }
        }

        /// <summary>
        /// Normalized Genres for Entity Framework
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<TrackGenre> DbTagGenres { get; set; }

        /// <summary>
        /// Comment tag of this track
        /// </summary>
        public string TagComment { get; set; }

        /// <summary>
        /// Year this track was released
        /// </summary>
        public short TagYear { get; set; }

        /// <summary>
        /// Beats per minute of this track
        /// </summary>
        public short TagBeatsPerMinute { get; set; }

        /// <summary>
        /// Track number of this track on the album it belongs to
        /// </summary>
        public short TagTrackNumber { get; set; }

        /// <summary>
        /// Total track number of the album this track belongs to
        /// </summary>
        public short TagAlbumTrackCount { get; set; }

        /// <summary>
        /// Disc number of this track on the album it belongs to
        /// </summary>
        public short TagDiscNumber { get; set; }

        /// <summary>
        /// Total disc count of the album this track belongs to
        /// </summary>
        public short TagAlbumDiscCount { get; set; }

        /// <summary>
        /// Number of times this track has been played
        /// </summary>
        public int LibraryPlays { get; set; }

        /// <summary>
        /// User rating of this track
        /// </summary>
        public byte LibraryRating { get; set; }

        /// <summary>
        /// Time this track was added to the library
        /// </summary>
        public long LibraryDateTimeAdded { get; set; }

        /// <summary>
        /// Time this track was last modified in the library
        /// </summary>
        public long LibraryDateTimeModified { get; set; }
    }
}