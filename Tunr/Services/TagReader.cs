using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tunr.Models;

namespace Tunr.Services
{
    public class TagReader
    {
        public TrackTags ReadTags(Stream fileStream)
        {
            TrackTags tags;
            if (tryReadId3v1Tags(fileStream, out tags))
            {
                return tags;
            }
            return null;
        }

#region Tag reading constants

        private static readonly Dictionary<byte, string> id3v1Genres
            = new Dictionary<byte, string>()
            {
                { 0, "Blues" },
                { 1, "Classic Rock" },
                { 2, "Country" },
                { 3, "Dance" },
                { 4, "Disco" },
                { 5, "Funk" },
                { 6, "Grunge" },
                { 7, "Hip-Hop" },
                { 8, "Jazz" },
                { 9, "Metal" },
                { 10, "New Age" },
                { 11, "Oldies" },
                { 12, "Other" },
                { 13, "Pop" },
                { 14, "R&B" },
                { 15, "Rap" },
                { 16, "Reggae" },
                { 17, "Rock" },
                { 18, "Techno" },
                { 19, "Industrial" },
                { 20, "Alternative" },
                { 21, "Ska" },
                { 22, "Death Metal" },
                { 23, "Pranks" },
                { 24, "Soundtrack" },
                { 25, "Euro-Techno" },
                { 26, "Ambient" },
                { 27, "Trip-Hop" },
                { 28, "Vocal" },
                { 29, "Jazz+Funk" },
                { 30, "Fusion" },
                { 31, "Trance" },
                { 32, "Classical" },
                { 33, "Instrumental" },
                { 34, "Acid" },
                { 35, "House" },
                { 36, "Game" },
                { 37, "Sound Clip" },
                { 38, "Gospel" },
                { 39, "Noise" },
                { 40, "AlternRock" },
                { 41, "Bass" },
                { 42, "Soul" },
                { 43, "Punk" },
                { 44, "Space" },
                { 45, "Meditative" },
                { 46, "Instrumental Pop" },
                { 47, "Instrumental Rock" },
                { 48, "Ethnic" },
                { 49, "Gothic" },
                { 50, "Darkwave" },
                { 51, "Techno-Industrial" },
                { 52, "Electronic" },
                { 53, "Pop-Folk" },
                { 54, "Eurodance" },
                { 55, "Dream" },
                { 56, "Southern Rock" },
                { 57, "Comedy" },
                { 58, "Cult" },
                { 59, "Gangsta Rap" },
                { 60, "Top 40" },
                { 61, "Christian Rap" },
                { 62, "Pop / Funk" },
                { 63, "Jungle" },
                { 64, "Native American" },
                { 65, "Cabaret" },
                { 66, "New Wave" },
                { 67, "Psychedelic" },
                { 68, "Rave" },
                { 69, "Showtunes" },
                { 70, "Trailer" },
                { 71, "Lo-Fi" },
                { 72, "Tribal" },
                { 73, "Acid Punk" },
                { 74, "Acid Jazz" },
                { 75, "Polka" },
                { 76, "Retro" },
                { 77, "Musical" },
                { 78, "Rock & Roll" },
                { 79, "Hard Rock" },
                { 80, "Folk" },
                { 81, "Folk-Rock" },
                { 82, "National Folk" },
                { 83, "Swing" },
                { 84, "Fast Fusion" },
                { 85, "Bebob" },
                { 86, "Latin" },
                { 87, "Revival" },
                { 88, "Celtic" },
                { 89, "Bluegrass" },
                { 90, "Avantgarde" },
                { 91, "Gothic Rock" },
                { 92, "Progressive Rock" },
                { 93, "Psychedelic Rock" },
                { 94, "Symphonic Rock" },
                { 95, "Slow Rock" },
                { 96, "Big Band" },
                { 97, "Chorus" },
                { 98, "Easy Listening" },
                { 99, "Acoustic" },
                { 100, "Humour" },
                { 101, "Speech" },
                { 102, "Chanson" },
                { 103, "Opera" },
                { 104, "Chamber Music" },
                { 105, "Sonata" },
                { 106, "Symphony" },
                { 107, "Booty Bass" },
                { 108, "Primus" },
                { 109, "Porn Groove" },
                { 110, "Satire" },
                { 111, "Slow Jam" },
                { 112, "Club" },
                { 113, "Tango" },
                { 114, "Samba" },
                { 115, "Folklore" },
                { 116, "Ballad" },
                { 117, "Power Ballad" },
                { 118, "Rhythmic Soul" },
                { 119, "Freestyle" },
                { 120, "Duet" },
                { 121, "Punk Rock" },
                { 122, "Drum Solo" },
                { 123, "A Cappella" },
                { 124, "Euro-House" },
                { 125, "Dance Hall" },
                { 126, "Goa" },
                { 127, "Drum & Bass" },
                { 128, "Club-House" },
                { 129, "Hardcore" },
                { 130, "Terror" },
                { 131, "Indie" },
                { 132, "BritPop" },
                { 133, "Negerpunk" },
                { 134, "Polsk Punk" },
                { 135, "Beat" },
                { 136, "Christian Gangsta Rap" },
                { 137, "Heavy Metal" },
                { 138, "Black Metal" },
                { 139, "Crossover" },
                { 140, "Contemporary Christian" },
                { 141, "Christian Rock" },
                { 142, "Merengue" },
                { 143, "Salsa" },
                { 144, "Thrash Metal" },
                { 145, "Anime" },
                { 146, "JPop" },
                { 147, "Synthpop" },
                { 148, "Abstract" },
                { 149, "Art Rock" },
                { 150, "Baroque" },
                { 151, "Bhangra" },
                { 152, "Big Beat" },
                { 153, "Breakbeat" },
                { 154, "Chillout" },
                { 155, "Downtempo" },
                { 156, "Dub" },
                { 157, "EBM" },
                { 158, "Eclectic" },
                { 159, "Electro" },
                { 160, "Electroclash" },
                { 161, "Emo" },
                { 162, "Experimental" },
                { 163, "Garage" },
                { 164, "Global" },
                { 165, "IDM" },
                { 166, "Illbient" },
                { 167, "Industro-Goth" },
                { 168, "Jam Band" },
                { 169, "Krautrock" },
                { 170, "Leftfield" },
                { 171, "Lounge" },
                { 172, "Math Rock" },
                { 173, "New Romantic" },
                { 174, "Nu-Breakz" },
                { 175, "Post-Punk" },
                { 176, "Post-Rock" },
                { 177, "Psytrance" },
                { 178, "Shoegaze" },
                { 179, "Space Rock" },
                { 180, "Trop Rock" },
                { 181, "World Music" },
                { 182, "Neoclassical" },
                { 183, "Audiobook" },
                { 184, "Audio Theatre" },
                { 185, "Neue Deutsche Welle" },
                { 186, "Podcast" },
                { 187, "Indie Rock" },
                { 188, "G-Funk" },
                { 189, "Dubstep" },
                { 190, "Garage Rock" },
                { 191, "Psybient" }
            };

#endregion

#region Tag reading implementations

        /// <summary>
        /// Attempts to read ID3v1 tags from a given file stream
        /// </summary>
        /// <param name="fileStream">File stream of MP3 to read</param>
        /// <param name="outTags">Output tags if successful</param>
        /// <returns>True on success, false on failure</returns>
        private bool tryReadId3v1Tags(Stream fileStream, out TrackTags outTags)
        {
            // ID3V1 TAG LAYOUT
            // header       3           "TAG"
            // title        30          30 characters of the title
            // artist       30          30 characters of the artist name
            // album        30          30 characters of the album name
            // year         4           A four-digit year
            // comment      28 or 30    The comment.
            // zero-byte    1           If a track number is stored, this byte contains a binary 0.
            // track        1           The number of the track on the album, or 0. Invalid, if previous byte is not a binary 0.
            // genre        1           Index in a list of genres, or 255

            // ID3v1 tags begin at 128 bytes before the end of the file.
            // Let's seek there and see if we find the tag.
            fileStream.Seek(-128, SeekOrigin.End);

            // Some pointers that we'll use for reading the stream
            byte[] buffer;
            string str;

            // The first 3 characters should be "TAG"
            buffer = new byte[3];
            fileStream.Read(buffer, 0, 3);
            str = Encoding.ASCII.GetString(buffer);
            if (str != "TAG")
            {
                outTags = null;
                return false;
            }

            // Instantiate our tags object
            var tags = new TrackTags();

            // Title
            buffer = new byte[30];
            fileStream.Read(buffer, 0, 30);
            tags.Title = Encoding.ASCII.GetString(buffer);

            // Artist
            buffer = new byte[30];
            fileStream.Read(buffer, 0, 30);
            var artist = Encoding.ASCII.GetString(buffer);
            tags.Performers = new string[] { artist };
            tags.AlbumArtist = artist;

            // Album
            buffer = new byte[30];
            fileStream.Read(buffer, 0, 30);
            tags.Album = Encoding.ASCII.GetString(buffer);

            // Year
            buffer = new byte[4];
            fileStream.Read(buffer, 0, 4);
            var yearStr = Encoding.ASCII.GetString(buffer);
            short year = 0;
            short.TryParse(yearStr, out year);

            // Comment
            buffer = new byte[28];
            fileStream.Read(buffer, 0, 28);
            tags.Comment = Encoding.ASCII.GetString(buffer);

            // Zero-byte
            buffer = new byte[1];
            fileStream.Read(buffer, 0, 1);
            if ((byte)buffer[0] == 0)
            {
                // If zero-byte, then read track number
                fileStream.Read(buffer, 0, 1);
                tags.TrackNumber = (byte)buffer[0];
            }
            else
            {
                // Otherwise skip forward
                fileStream.Seek(1, SeekOrigin.Current);
            }

            // Genre
            buffer = new byte[1];
            fileStream.Read(buffer, 0, 1);
            string genreStr = "";
            id3v1Genres.TryGetValue(buffer[0], out genreStr);
            tags.Genres = new string[] { genreStr };

            outTags = tags;
            return true;
        }

        /// <summary>
        /// Attempts to read ID3v2 tags from a given file stream
        /// </summary>
        /// <remarks>
        /// Supports ID3v2.4.0 as per http://id3.org/id3v2.4.0-structure
        /// </remarks>
        /// <param name="fileStream">File stream of MP3 to read</param>
        /// <param name="outTags">Output tags if successful</param>
        /// <returns>True on success, false on failure</returns>
        private bool tryReadId3v2Tags(Stream fileStream, out TrackTags outTags)
        {
            // ID3v2 tags begin at the start of the file.
            // Let's seek there and see if we find the tag.
            fileStream.Seek(0, SeekOrigin.Begin);

            // Some pointers that we'll use for reading the stream
            byte[] buffer;
            string str;

            // The first 3 characters should be "ID3"
            //ID3v2/file identifier    "ID3"
            buffer = new byte[3];
            fileStream.Read(buffer, 0, 3);
            str = Encoding.ASCII.GetString(buffer);
            if (str != "ID3")
            {
                outTags = null;
                return false;
            }

            // Next four bytes are the version. ID3v2.major.revision
            // ID3v2 version    $04 00
            buffer = new byte[2];
            fileStream.Read(buffer, 0, 2);
            int majorVersion = readBigEndianInt16(buffer);
            fileStream.Read(buffer, 0, 2);
            int revisionVersion = readBigEndianInt16(buffer);

            // We only support ID3v2.4.x and below
            if (majorVersion > 4)
            {
                outTags = null;
                return false;
            }

            // Next byte is flags
            // ID3v2 flags    %abcd0000
            buffer = new byte[1];
            fileStream.Read(buffer, 0, 1);
            var flagsBitArray = new BitArray(buffer);
            bool unsynchronisation = flagsBitArray[7];
            bool extendedHeader = flagsBitArray[6];
            bool experimental = flagsBitArray[5];
            bool footerPresent = flagsBitArray[4];
            // The rest of these should be zero... otherwise something's wrong
            for (var i = 3; i >= 0; i--)
            {
                if (flagsBitArray[i] != false)
                {
                    outTags = null;
                    return false;
                }
            }

            // Next 4 bytes are size
            // ID3v2 size    4 * %0xxxxxxx
            buffer = new byte[4];
            fileStream.Read(buffer, 0, 4);
            // TODO: Read "synchsafe" 32-bit int
            var tagSize = readBigEndianInt32(buffer);

            // Read extended header
            if (extendedHeader)
            {

            }

        }

#endregion

#region Tag reading helper methods
        private Int16 readBigEndianInt16(byte[] bytes)
        {
            // If we're a little endian machine (we likely are) we need to reverse the bits before we read them
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt16(bytes, 0);
        }

        private Int32 readBigEndianInt32(byte[] bytes)
        {
            // If we're a little endian machine (we likely are) we need to reverse the bits before we read them
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToInt32(bytes, 0);
        }
#endregion
    }
}