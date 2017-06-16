using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tunr.Models;
using Tunr.Models.Library;
using Tunr.Services;

namespace Tunr.Tests
{
    [TestClass]
    public class SqlPageBlobLibraryStoreTests
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

        private const string fileRelativePath = "/Music/Artist/Album/Folder/";

        private static readonly string[] fileExtensions = new string[]
        {
            "mp3",
            "m4a",
            "flac"
        };

        [TestMethod]
        public async Task TestSongInsertion()
        {
            // Init DB
            var dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TunrTest")
                .Options;
            var dbContext = new ApplicationDbContext(dbOptions);

            // Add user
            var user = new TunrUser()
            {
                Id = Guid.NewGuid(),
                Email = "test@tunr.io",
                EmailConfirmed = true,
                LibraryOffset = 0,
                UserName = "test@tunr.io"
            };
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            // Init store
            var storeOptions = Options.Create(new SqlPageBlobLibraryStoreOptions()
            {
                StorageAccountConnectionString = "UseDevelopmentStorage=true"
            });
            var libraryStore = new SqlPageBlobLibraryStore(storeOptions, dbContext);

            // Insert a bunch of songs
            foreach (var artist in artists)
            {
                foreach (var album in albums)
                {
                    foreach (var title in titles)
                    {
                        foreach (var genre in genres)
                        {
                            var track = new Track()
                            {
                                TrackId = Guid.NewGuid(),
                                UserId = user.Id,
                                StorageLocation = 0,
                                FileRelativePath = fileRelativePath,
                                FileName = "file.mp3",
                                FileExtension = "mp3",
                                FileSizeBytes = 4194304,
                                FileSha256Hash = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                                AudioChannels = 2,
                                AudioBitrateKbps = 256,
                                AudioSampleRateHz = 48000,
                                AudioDurationSeconds = 200,
                                TagTitle = title,
                                TagPerformers = new string[] { artist },
                                TagAlbumArtist = artist,
                                TagAlbum = album,
                                TagComposers = new string[] { artist },
                                TagGenres = new string[] { genre },
                                TagComment = "This is a comment.",
                                TagYear = 2017,
                                TagBeatsPerMinute = 128,
                                TagTrackNumber = 1,
                                TagAlbumTrackCount = 16,
                                TagDiscNumber = 1,
                                TagAlbumDiscCount = 1,
                                LibraryPlays = 0,
                                LibraryRating = 0,
                                LibraryDateTimeModified = DateTimeOffset.Now.ToUnixTimeSeconds(),
                                LibraryDateTimeAdded = DateTimeOffset.Now.ToUnixTimeSeconds()
                            };
                            await libraryStore.AddTrackAsync(track);
                        }
                    }
                }
            }

            // Check our library offset
            user = await dbContext.Users.FindAsync(user.Id);
            Assert.AreEqual(user.LibraryOffset, artists.Length * albums.Length * titles.Length * genres.Length * 2048);
        }
    }
}
