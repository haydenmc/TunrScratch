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

        private readonly IAudioInfoService audioInfoService;

        public LibraryController(
            UserManager<TunrUser> userManager,
            ILibraryStore libraryStore,
            ITagService tagService,
            IMusicFileStore musicFileStore,
            IAudioInfoService audioInfoService)
        {
            this.userManager = userManager;
            this.libraryStore = libraryStore;
            this.tagService = tagService;
            this.musicFileStore = musicFileStore;
            this.audioInfoService = audioInfoService;
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
                    Stream stream;
                    // Get tags
                    TrackTags tags;
                    try
                    {
                        stream = file.OpenReadStream();
                        tags = await tagService.GetTagsAsync(stream, file.FileName);
                    }
                    catch (Exception e)
                    {
                        return BadRequest($"Could not read tags from audio file. Error: {e.Message}");
                    }
                    // Get audio data
                    AudioInfo audioInfo;
                    try
                    {
                        stream = file.OpenReadStream();
                        audioInfo = await audioInfoService.GetAudioInfoAsync(stream, file.FileName);
                    }
                    catch (Exception e)
                    {
                        return BadRequest($"Could not read audio info from audio file. Error: {e.Message}");
                    }
                    // Add to library
                    var track = new Track()
                    {
                        // Internal info
                        TrackId = Guid.NewGuid(),
                        UserId = user.Id,
                        StorageLocation = 0,
                        // File info
                        FileRelativePath = "", // TODO
                        FileName = file.FileName,
                        FileExtension = file.FileName.Substring(file.FileName.LastIndexOf('.')),
                        FileSizeBytes = (int)file.Length,
                        FileSha256Hash = new byte[256], // TODO
                        // Audio info
                        AudioChannels = audioInfo.Channels,
                        AudioBitrateKbps = audioInfo.BitrateKbps,
                        AudioSampleRateHz = audioInfo.SampleRateHz,
                        AudioDurationSeconds = audioInfo.DurationSeconds,
                        // Tags
                        TagTitle = tags.Title,
                        TagPerformers = tags.Performers,
                        TagAlbumArtist = tags.AlbumArtist,
                        TagAlbum = tags.Album,
                        TagComposers = tags.Composers,
                        TagGenres = tags.Genres,
                        TagComment = tags.Comment,
                        TagYear = tags.Year,
                        TagBeatsPerMinute = tags.BeatsPerMinute,
                        TagTrackNumber = tags.TrackNumber,
                        TagAlbumTrackCount = tags.AlbumTrackCount,
                        TagDiscNumber = tags.DiscNumber,
                        TagAlbumDiscCount = tags.AlbumDiscCount,
                        // Library info
                        LibraryPlays = 0,
                        LibraryRating = 0,
                        LibraryDateTimeAdded = DateTimeOffset.Now.ToUnixTimeSeconds(),
                        LibraryDateTimeModified = DateTimeOffset.Now.ToUnixTimeSeconds()
                    };
                    return Ok(track);
                }
            }
            return Ok();
        }
    }
}