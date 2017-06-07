using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Tunr.Models;
using Tunr.Models.Library;
using Tunr.Services;
using Xunit;

namespace Tunr.Tests
{
    public class SqlPageBlobLibraryStoreTest
    {
        private static readonly string[] artists = new string[]
        {
            "Under the Influence of Giants",
            "Bruce Springsteen",
            "Barenaked Ladies",
            "Electric Light Orchestra",
            "权振东"
        };

        private static readonly string[] titles = new string[]
        {
            "Got Nothing",
            "Born To Run",
            "Enid",
            "Evil Woman",
            "错爱"
        };

        private static readonly string[] albums = new string[]
        {
            "This is an album"
        };

        private static readonly string[] genres = new string[]
        {
            "Rock",
            "Techno"
        };

        private static readonly string[] folderNames = new string[]
        {
            "backup",
            "music",
            "audio",
            "thisisalongfoldername"
        };

        private static readonly string[] fileExtensions = new string[]
        {
            "mp3",
            "m4a",
            "flac"
        };

        [Fact]
        public void Test1()
        {
            // Init DB
            var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TunrTest")
                .Options;
            var dbContext = new ApplicationDbContext(dbOptions);

            // Init store
            var storeOptions = Options.Create(new SqlPageBlobLibraryStoreOptions()
            {
                StorageAccountConnectionString = "UseDevelopmentStorage=true"
            });

            var libraryStore = new SqlPageBlobLibraryStore(storeOptions, dbContext);
        }

        private Track summonRandomTrack(Guid? userId = null)
        {
            var random = new Random();
            var track = new Track()
            {
                TrackId = Guid.NewGuid(),
                UserId = userId == null ? Guid.Empty : (Guid)userId,
                StorageLocation = 0,
                FileRelativePath = "/" + String.Join("/", folderNames.OrderBy(o => random.Next()).Take(random.Next(1, folderNames.Length - 1)).ToArray()) + "/",
                FileName = summonRandomString(random.Next(16, 64)) + "." + fileExtensions[random.Next(0, fileExtensions.Length - 1)],
                FileExtension = fileExtensions[random.Next(0, fileExtensions.Length - 1)],
                FileSizeBytes = random.Next(2048, 20480),
                FileSha256Hash = new byte[] { },
                AudioChannels = (byte)random.Next(1, 2),
                AudioBitrateKbps = (short)random.Next(96, 320),
                AudioSampleRateHz = 48000,
                AudioDurationSeconds = (short)random.Next(60, 240),
                TagTitle = titles[random.Next(0, titles.Length - 1)],
                TagPerformers = artists.OrderBy(o => random.Next()).Take(random.Next(1, artists.Length - 1)).ToArray(),
                TagAlbumArtist = artists[random.Next(0, artists.Length - 1)],
                TagAlbum = albums[random.Next(0, albums.Length - 1)],
                TagComposers = artists.OrderBy(o => random.Next()).Take(random.Next(1, artists.Length - 1)).ToArray(),
                TagGenres = genres.OrderBy(o => random.Next()).Take(random.Next(1, genres.Length - 1)).ToArray(),
                TagComment = summonRandomString(random.Next(0, 128)),
                TagYear = (short)random.Next(1600, 2017),
                TagBeatsPerMinute = (short)random.Next(30, 600),
                TagTrackNumber = (short)random.Next(1, 48),
                TagAlbumTrackCount = (short)random.Next(1, 48),
                TagDiscNumber = (short)random.Next(1, 8),
                TagAlbumDiscCount = (short)random.Next(1, 8),
                LibraryPlays = 0,
                LibraryRating = 0,
                LibraryDateTimeAdded = DateTimeOffset.Now.ToUnixTimeSeconds(),
                LibraryDateTimeModified = DateTimeOffset.Now.ToUnixTimeSeconds()
            };

            return track;
        }

        private string summonRandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
