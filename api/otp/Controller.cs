using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Goarif.Server.Controllers
{
    [ApiController]
    [Route("api/v1/otp")]
    public class OtpController : ControllerBase
    {
        private readonly IOtpService _otpService;

        public OtpController(IOtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendOtp([FromBody] CreateOtpDto dto)
        {
            try
            {
                var result = await _otpService.SendOtpAsync(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpDto dto)
        {
            try
            {
                var result = await _otpService.ValidateOtpAsync(dto);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
