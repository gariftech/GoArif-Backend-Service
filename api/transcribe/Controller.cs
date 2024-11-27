using Goarif.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Goarif.Server.Controllers
{
    [ApiController]
    [Route("/api/v1/Transcribe")]
    public class TranscribeController : ControllerBase
    {
        private readonly ITranscribeService _ITranscribeService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationTranscribeDto _TranscribeValidationService;
         private readonly ConvertJWT _ConvertJwt;
        public TranscribeController(ITranscribeService TranscribeService, ConvertJWT convert)
        {
            _ITranscribeService = TranscribeService;
            _errorUtility = new ErrorHandlingUtility();
            _TranscribeValidationService = new ValidationTranscribeDto();
            _ConvertJwt = convert;
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
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                // Check if the uploaded file is not null and is a PDF
                if (file == null || !IsAudioFile(file))
                {
                    var errorResponse = new { code = 400, errorMessage = "Please upload a audio file." };
                    return BadRequest(errorResponse);
                }
                var data = await _ITranscribeService.PostAudio(file, languange, idUser);
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
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _ITranscribeService.PostAudioYoutubeUrl(url, idUser);
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
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _ITranscribeService.PostAudioUrl(url,idUser);
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
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _ITranscribeService.PostAudioUrlDrive(url,idUser);
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

            // List of common video MIME types
            var videoMimeTypes = new[]
            {
                "video/mp4",
                "video/ogg",
                "video/webm",
                "video/x-m4v",
                "video/quicktime"
            };

            // List of common video file extensions
            var videoExtensions = new[]
            {
                ".mp4",
                ".ogg",
                ".webm",
                ".m4v",
                ".mov"
            };

            // Check if the file content type indicates an audio or video file
            var isMimeTypeValid = audioMimeTypes.Contains(file.ContentType.ToLower()) || videoMimeTypes.Contains(file.ContentType.ToLower());

            // Check if the file extension indicates an audio or video file
            var isExtensionValid = audioExtensions.Contains(Path.GetExtension(file.FileName).ToLower()) || videoExtensions.Contains(Path.GetExtension(file.FileName).ToLower());

            // Return true if either the MIME type or file extension is valid for audio or video
            return isMimeTypeValid || isExtensionValid;
        }



    }
}