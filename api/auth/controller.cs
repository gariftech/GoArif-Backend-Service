
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Goarif.Server.Controllers
{
    [ApiController]
    [Route("api/v1/user-auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _IAuthService;
        private readonly IEmailService _emailService;

        private readonly ConvertJWT _ConvertJwt;
        private readonly ErrorHandlingUtility _errorUtility;

        private readonly ValidationAuthDto _masterValidationService;
        public AuthController(IEmailService EmailService, IAuthService authService, ConvertJWT convert, ValidationAuthDto MasterValidationService)
        {
            _IAuthService = authService;
            _emailService = EmailService;
            _ConvertJwt = convert;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = MasterValidationService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto login)
        {
            try
            {
                var validationErrors = _masterValidationService.ValidateRegister(login);
                if (validationErrors.Count > 0)
                {
                    var errorResponse = new { code = 400, errorMessage = validationErrors };
                    return BadRequest(errorResponse);
                }
                var dataList = await _IAuthService.RegisterAsync(login);
                return Ok(dataList);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }
        
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<object> LoginAsync([FromBody] LoginDto login)
        {
            try
            {
                var validationErrors = _masterValidationService.ValidateLogin(login);
                if (validationErrors.Count > 0)
                {
                    var errorResponse = new { code = 400, errorMessage = validationErrors };
                    return BadRequest(errorResponse);
                }
                var response = await _IAuthService.LoginAsync(login);
                return Ok(response);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<object> ForgotPassword([FromBody] UpdateUserAuthDto item)
        {
            try
            {
                var dataList = await _IAuthService.ForgotPasswordAsync(item);
                return Ok(dataList);
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
        [Route("change-password")]
        public async Task<object> UpdatePassword([FromBody] ChangeUserPasswordDto item)
        {
            try
            {
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var dataList = await _IAuthService.UpdatePassword(idUser, item);
                return Ok(dataList);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpGet]
        [Route("check-mail-registered/{email}")]
        public async Task<object> CheckMail([FromRoute] string email)
        {
            try
            {
                var dataList = await _IAuthService.CheckMail(email);
                return Ok(dataList);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        // [AllowAnonymous]
        // [HttpGet]
        // [Route("activation/{UID}")]
        // public async Task<object> VerifySeasonsAsync([FromRoute] string UID)
        // {
        //     try
        //     {
        //         var dataList = await _IAuthService.Aktifasi(UID);
        //         return Ok(dataList);
        //     }
        //     catch (CustomException ex)
        //     {
        //         int errorCode = ex.ErrorCode;
        //         var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
        //         return _errorUtility.HandleError(errorCode, errorResponse);
        //     }
        // }

        [Authorize]
        [HttpGet]
        [Route("verifySessions")]
        public object Aktifasi()
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                return new { code = 200, message = "not expired" };
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