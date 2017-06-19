using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tunr.Services;
using Microsoft.AspNetCore.Identity;
using Tunr.Models;
using System.Threading.Tasks;
using Tunr.Models.Library;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace Tunr.Controllers
{
    [Route("Library")]
    public class LibraryController: Controller
    {
        private readonly ILibraryStore libraryStore;

        private readonly UserManager<TunrUser> userManager;

        private readonly ITagService tagService;

        private readonly IMusicFileStore musicFileStore;

        public LibraryController(UserManager<TunrUser> userManager, ILibraryStore libraryStore, ITagService tagService, IMusicFileStore musicFileStore)
        {
            this.userManager = userManager;
            this.libraryStore = libraryStore;
            this.tagService = tagService;
            this.musicFileStore = musicFileStore;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Upload(IList<IFormFile> files)
        {
            var user = await userManager.GetUserAsync(User);
            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.Length > 0)
                {
                    var tempFilePath = Path.GetTempFileName();
                    var stream = new FileStream(tempFilePath, FileMode.Create);
                    // Get tags
                    try
                    {
                        var tags = tagService.GetTagsAsync(stream, file.FileName);
                    }
                    catch (Exception e)
                    {
                        return BadRequest($"Could not read tags from audio file. Error: {e.Message}");
                    }
                    // TODO: Add to library
                }
            }

            Console.WriteLine(user.Email);
            var newTrack = new Track() 
            {
                TrackId = Guid.NewGuid(),
                UserId = user.Id,
                FileRelativePath = "/Fleetwood Mac/Greatest Hits/",
                FileName = "06. Dreams.mp3",
                FileExtension = "mp3",
                FileSizeBytes = 12345,
                FileSha256Hash = new byte[32],
                AudioChannels = 2,
                AudioBitrateKbps = 256,
                AudioSampleRateHz = 48000,
                AudioDurationSeconds = 148,
                TagTitle = "Dreams",
                TagPerformers = new string[] { "Fleetwood Mac", "Also Fleetwood Mac" },
                TagAlbumArtist = "Fleetwood Mac",
                TagAlbum = "Greatest Hits",
                TagComposers = new string[] { "Stevie Nicks", "Lindsay Buckingham" },
                TagGenres = new string[] { "Rock", "Awesome" },
                TagComment = "This song is good I like it I am Hayden this is Hayden speaking",
                TagYear = 1988,
                TagBeatsPerMinute = 0,
                TagTrackNumber = 9,
                TagAlbumTrackCount = 16,
                TagDiscNumber = 1,
                TagAlbumDiscCount = 1,
                LibraryPlays = 0,
                LibraryRating = 0,
                LibraryDateTimeAdded = 0,
                LibraryDateTimeModified = 0
            };
            await libraryStore.AddTrackAsync(newTrack);
            return Ok();
        }
    }
}