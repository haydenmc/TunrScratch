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

        [Route("")]
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
                    // Initialize some vars
                    Stream stream;
                    Guid trackId = Guid.NewGuid();

                    // Get tags
                    TrackTags tags;
                    try
                    {
                        using (stream = file.OpenReadStream())
                        {
                            tags = await tagService.GetTagsAsync(stream, file.FileName);
                        }
                    }
                    catch (Exception e)
                    {
                        return BadRequest($"Could not read tags from audio file. Error: {e.Message}");
                    }

                    // Get audio data
                    AudioInfo audioInfo;
                    try
                    {
                        using (stream = file.OpenReadStream())
                        {
                            audioInfo = await audioInfoService.GetAudioInfoAsync(stream, file.FileName);
                        }
                    }
                    catch (Exception e)
                    {
                        return BadRequest($"Could not read audio info from audio file. Error: {e.Message}");
                    }

                    // Upload to store
                    try
                    {
                        using (stream = file.OpenReadStream())
                        {
                            await musicFileStore.PutFileAsync(trackId, stream);
                        }
                    }
                    catch (Exception e)
                    {
                        return BadRequest($"Could not upload audio file to music store. Error: {e.Message}");
                    }

                    // Add to library
                    var track = new Track()
                    {
                        // Internal info
                        TrackId = trackId,
                        // UserId = Guid.Empty, // user.Id,
                        UserId = new Guid("C4AB3147-07FF-4C21-BFE6-C8CEF5561D06"), // Temporarily hard-coded
                        StorageLocation = 0,
                        // File info
                        FileRelativePath = "", // TODO
                        FileName = file.FileName,
                        FileExtension = file.FileName.Substring(file.FileName.LastIndexOf('.') + 1),
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
                        // Uh oh, something went wrong.
                        // Now we need to remove this track from the music file store.
                        try
                        {
                            await musicFileStore.DeleteFileAsync(trackId);
                        }
                        catch (Exception ie)
                        {
                            return BadRequest($"Could not store track metadata. Error: {e.Message}" +
                                $"\nAdditionally, could not remove track from music file store. Error: {ie.Message}");
                        }
                        return BadRequest($"Could not store track metadata. Error: {e.Message}");
                    }

                    // All done!
                    return Ok(track);
                }
            }
            return Ok();
        }

        [Route("track/{propertyName}")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> FetchTrackPropertyValues(
            [FromQuery] string propertyName)
        {
            var user = await userManager.GetUserAsync(User);
            var userId = user.Id;
            var propertyValues = metadataStore.FetchUniqueUserTrackPropertyValuesAsync(userId, propertyName);
            return Ok(propertyValues);
        }
    }
}