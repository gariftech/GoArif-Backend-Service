
public interface IAuthService
{
    Task<Object> LoginAsync(LoginDto model);
    Task<Object> RegisterAsync(RegisterDto model);
    Task<string> Aktifasi(string id);
    Task<Object> UpdatePassword(string id, ChangeUserPasswordDto model);
    Task<string> ForgotPasswordAsync(UpdateUserAuthDto model);
    Task<Object> UpdatePin(string id, UpdatePinDto model);
    Task<Object> VerifyOtp(OtpDto otp);
    Task<Object> VerifyPin(string id);
    Task<Object> CheckPin(PinDto pin, string id);
    Task<string> CheckMail(string email);

    Task<Object> RequestOtpEmail(string id);
    Task<object> Recaptcha(string token);
}