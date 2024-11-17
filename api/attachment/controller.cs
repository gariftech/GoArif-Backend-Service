

using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Goarif.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/Attachment")]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _IAttachmentService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationMasterDto _masterValidationService;
        private readonly ConvertJWT _ConvertJwt;
        public AttachmentController(IAttachmentService roleService, ConvertJWT convert)
        {
            _IAttachmentService = roleService;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationMasterDto();
            _ConvertJwt = convert;
        }

        [HttpDelete]
        [Route("")]
        public async Task<object> GetAll([FromBody] string url)
        {
            try
            {
                var data = await _IAttachmentService.DeleteFileAsync(url);
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
        [HttpGet]
        [Route("GetAll")]
        public async Task<object> Get()
        {
            try
            {
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                
                var data = await _IAttachmentService.Get(idUser);
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
        [HttpPost]
        [Route("Upload")]
        public async Task<object> Upload(IFormFile file)
        {

            try
            {
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);

                if (file == null || file.Length == 0)
                {
                    throw new CustomException(400, "Message", "File Not Found");
                }

                // Define allowed MIME types for images and videos
                var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                var allowedVideoTypes = new[] { "video/mp4", "video/mpeg", "video/quicktime" };

                // Define max file size: 2 MB for images, 50 MB for videos
                const long maxImageSize = 2 * 1024 * 1024; // 2 MB
                const long maxVideoSize = 50 * 1024 * 1024; // 50 MB

                // Check if the file is an image
                if (Array.Exists(allowedImageTypes, type => type.Equals(file.ContentType, StringComparison.OrdinalIgnoreCase)))
                {
                    if (file.Length > maxImageSize)
                    {
                        throw new CustomException(400, "Message", "Image size must not exceed 2 MB.");
                    }
                }
                // Check if the file is a video
                else if (Array.Exists(allowedVideoTypes, type => type.Equals(file.ContentType, StringComparison.OrdinalIgnoreCase)))
                {
                    if (file.Length > maxVideoSize)
                    {
                        throw new CustomException(400, "Message", "Video size must not exceed 50 MB.");
                    }
                }
                else
                {
                    throw new CustomException(400, "Message", "Only image (JPEG, PNG, GIF) or video (MP4, MPEG, MOV) files are allowed.");
                }
                // Generate unique file name
                var fileName = $"{Guid.NewGuid()}-{file.FileName}";
                // Upload file
                var result = await _IAttachmentService.Upload(file, fileName, idUser);
                return Ok(new
                {
                    status = true,
                    fileName = result.FileName,
                    path = result.Url
                });
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }
    }
}