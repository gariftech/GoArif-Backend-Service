public interface IOtpService
{
    Task<string> SendOtpAsync(CreateOtpDto dto);
    Task<string> ValidateOtpAsync(ValidateOtpDto dto);
}