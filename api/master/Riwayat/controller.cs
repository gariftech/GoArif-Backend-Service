

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Goarif.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("/api/v1/riwayat")]
    public class RiwayatController : ControllerBase
    {
        private readonly IRiwayatService _IRiwayatService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationRiwayatDto _masterValidationService;
        private readonly ConvertJWT _ConvertJwt;
        public RiwayatController(IRiwayatService RiwayatService, ConvertJWT convert)
        {
            _IRiwayatService = RiwayatService;
             _ConvertJwt = convert;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationRiwayatDto();
        }

        [HttpGet]
        public async Task<object> GetById()
        {
            try
            {
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IRiwayatService.GetById(idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        // [Authorize]
        [HttpPost]
        public async Task<object> Post([FromBody] CreateRiwayatDto item)
        {
            try
            {
                var validationErrors = _masterValidationService.ValidateCreateInput(item);
                if (validationErrors.Count > 0)
                {
                    var errorResponse = new { code = 400, errorMessage = validationErrors };
                    return BadRequest(errorResponse);
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _IRiwayatService.Post(item, idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        // [Authorize]
        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] string id)
        {
            try
            {
                var data = await _IRiwayatService.Delete(id);
                return Ok(data);
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