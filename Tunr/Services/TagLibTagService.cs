using System;
using System.IO;
using System.Threading.Tasks;
using Tunr.Models;
using static TagLib.File;

namespace Tunr.Services
{
    public class TagLibTagService : ITagService
    {
#pragma warning disable CS1998
        public async Task<TrackTags> GetTagsAsync(Stream fileStream, string fileName)
        {
            var tagLibFile = TagLib.File.Create(new StreamFileAbstraction(fileStream, fileName));
            var trackTags = new TrackTags()
            {
                Title = tagLibFile.Tag.Title,
                Performers = tagLibFile.Tag.Performers,
                AlbumArtist = tagLibFile.Tag.FirstAlbumArtist,
                Album = tagLibFile.Tag.Album,
                Composers = tagLibFile.Tag.Composers,
                Genres = tagLibFile.Tag.Genres,
                Comment = tagLibFile.Tag.Comment,
                Year = (short)tagLibFile.Tag.Year,
                BeatsPerMinute = (short)tagLibFile.Tag.BeatsPerMinute,
                TrackNumber = (short)tagLibFile.Tag.Track,
                AlbumTrackCount = (short)tagLibFile.Tag.TrackCount,
                DiscNumber = (short)tagLibFile.Tag.Disc,
                AlbumDiscCount = (short)tagLibFile.Tag.DiscCount
            };
            return trackTags;
        }
#pragma warning restore CS1998

        private class StreamFileAbstraction : IFileAbstraction
        {
            private Stream stream;

            private string fileName;

            public string Name => fileName;

            public Stream ReadStream => stream;

            public Stream WriteStream => stream;

            public StreamFileAbstraction(Stream stream, string fileName = "")
            {
                this.stream = stream;
                this.fileName = fileName;
            }

            public void CloseStream(Stream stream)
            {
                stream.Close();
            }
        }
    }
}