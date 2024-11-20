using Goarif.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Goarif.Server.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class TranscribeController : ControllerBase
    {
        private readonly ITranscribeService _ITranscribeService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationTranscribeDto _TranscribeValidationService;
        public TranscribeController(ITranscribeService TranscribeService)
        {
            _ITranscribeService = TranscribeService;
            _errorUtility = new ErrorHandlingUtility();
            _TranscribeValidationService = new ValidationTranscribeDto();
        }

        [Authorize]
        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                var data = await _ITranscribeService.Get();
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<object> GetById([FromRoute] string id)
        {
            try
            {
                var data = await _ITranscribeService.GetById(id);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("SpeechFileToText/{languange}")]
        public async Task<object> PostSpeech(IFormFile file, [FromRoute] string languange)
        {
            try
            {
                // Check if the uploaded file is not null and is a PDF
                if (file == null || !IsAudioFile(file))
                {
                    var errorResponse = new { code = 400, errorMessage = "Please upload a audio file." };
                    return BadRequest(errorResponse);
                }
                var data = await _ITranscribeService.PostAudio(file, languange);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("SpeechYoutubeToText")]
        public async Task<object> PostYoutubeSpeech([FromBody] YouTubeUrl url)
        {
            try
            {
                var data = await _ITranscribeService.PostAudioYoutubeUrl(url);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("SpeechUrlGeneralToText")]
        public async Task<object> PostURLGeneralSpeech([FromBody] YouTubeUrl url)
        {
            try
            {
                var data = await _ITranscribeService.PostAudioUrl(url);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("SpeechUrlGoogleDriveShareToText")]
        public async Task<object> PostURLDriveSpeech([FromBody] YouTubeUrl url)
        {
            try
            {
                var data = await _ITranscribeService.PostAudioUrlDrive(url);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        private bool IsAudioFile(IFormFile file)
        {
            // List of common audio MIME types
            var audioMimeTypes = new[]
            {
                "audio/mpeg",
                "audio/wav",
                "audio/ogg",
                "audio/mp4",
                "audio/aac",
                "audio/x-aac"
            };

            // List of common audio file extensions
            var audioExtensions = new[]
            {
                ".mp3",
                ".wav",
                ".ogg",
                ".m4a",
                ".aac"
            };

            // Check if the file content type indicates an audio file
            var isMimeTypeValid = audioMimeTypes.Contains(file.ContentType.ToLower());

            // Check if the file extension indicates an audio file
            var isExtensionValid = audioExtensions.Contains(Path.GetExtension(file.FileName).ToLower());

            // Return true if either the MIME type or file extension is valid
            return isMimeTypeValid || isExtensionValid;
        }


    }
}