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
        private readonly IMusicMetadataStore metadataStore;

        private readonly UserManager<TunrUser> userManager;

        private readonly ITagReaderService tagService;

        private readonly IMusicFileStore musicFileStore;

        private readonly IAudioInfoReaderService audioInfoService;

        public LibraryController(
            UserManager<TunrUser> userManager,
            IMusicMetadataStore metadataStore,
            ITagReaderService tagService,
            IMusicFileStore musicFileStore,
            IAudioInfoReaderService audioInfoService)
        {
            this.userManager = userManager;
            this.metadataStore = metadataStore;
            this.tagService = tagService;
            this.musicFileStore = musicFileStore;
            this.audioInfoService = audioInfoService;
        }

        [HttpPost]
        //[Authorize] // TODO: Turn on when we're done writing this...
        public async Task<IActionResult> Upload(IList<IFormFile> files)
        {
            var user = await userManager.GetUserAsync(User);
            for (var i = 0; i < files.Count; i++)
            {
                var file = files[i];
                if (file.Length > 0)
                {
                    if (file.Length > 1)
                    {
                        return BadRequest("Only one file upload per request is supported.");
                    }
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
                        UserId = Guid.Empty, // user.Id,
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
                    try
                    {
                        await metadataStore.AddTrackAsync(track);
                    }
                    catch (Exception e)
                    {
                        return BadRequest($"Could not store track metadata. Error: {e.Message}");
                    }
                    return Ok(track);
                }
            }
            return Ok();
        }
    }
}