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
using Tunr.Helpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using Tunr.Filters;
using System.Text;

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

        private readonly FormOptions formOptions = new FormOptions();

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
        [DisableFormValueModelBinding]
        public async Task<IActionResult> Upload()
        {
            var user = await userManager.GetUserAsync(User);

            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                return BadRequest($"Expected a multipart request, but got {Request.ContentType}");
            }

            // Used to accumulate all the form url encoded key value pairs in the
            // request.
            var boundary = MultipartRequestHelper.GetBoundary(
                MediaTypeHeaderValue.Parse(Request.ContentType),
                formOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader(boundary, HttpContext.Request.Body);

            var tracks = new List<Track>();
            var section = await reader.ReadNextSectionAsync();
            while (section != null)
            {
                ContentDispositionHeaderValue contentDisposition;
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out contentDisposition);
                string targetFilePath = null;

                if (hasContentDispositionHeader)
                {
                    if (MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        targetFilePath = Path.GetTempFileName();
                        using (var targetStream = System.IO.File.Create(targetFilePath))
                        {
                            await section.Body.CopyToAsync(targetStream);
                        }
                        string fileName = "";
                        if (contentDisposition.FileName.HasValue)
                        {
                            fileName = HeaderUtilities.RemoveQuotes(contentDisposition.FileName).Value;
                        }
                        var track = await processTempFileUpload(user.Id, targetFilePath, fileName);
                        System.IO.File.Delete(targetFilePath);
                        tracks.Add(track);
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            return Ok(tracks);
        }

        private async Task<Track> processTempFileUpload(Guid userId, string filePath, string fileName)
        {
            // Initialize some vars
            Stream stream;
            Guid trackId = Guid.NewGuid();

            // Get tags
            TrackTags tags;
            using (stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                tags = await tagService.GetTagsAsync(stream, fileName);
            }

            // Get audio data
            AudioInfo audioInfo;
            using (stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                audioInfo = await audioInfoService.GetAudioInfoAsync(stream, fileName);
            }

            // Upload file to store
            using (stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                await musicFileStore.PutFileAsync(trackId, stream);
            }

            // Get file info
            var fileSizeBytes = new FileInfo(filePath).Length;

            // Add to library
            var track = new Track()
            {
                // Internal info
                TrackId = trackId,
                // UserId = Guid.Empty, // user.Id,
                UserId = userId, // Temporarily hard-coded
                StorageLocation = 0,
                // File info
                FileRelativePath = "", // TODO
                FileName = fileName,
                FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1),
                FileSizeBytes = (int)fileSizeBytes,
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
                    throw new ApplicationException($"Could not store track metadata. Error: {e.Message}" +
                        $"\nAdditionally, could not remove track from music file store. Error: {ie.Message}");
                }
                throw new ApplicationException($"Could not store track metadata. Error: {e.Message}");
            }

            return track;
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

        private static Encoding GetEncoding(MultipartSection section)
        {
            MediaTypeHeaderValue mediaType;
            var hasMediaTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out mediaType);
            // UTF-7 is insecure and should not be honored. UTF-8 will succeed in 
            // most cases.
            if (!hasMediaTypeHeader || Encoding.UTF7.Equals(mediaType.Encoding))
            {
                return Encoding.UTF8;
            }
            return mediaType.Encoding;
        }
    }
}