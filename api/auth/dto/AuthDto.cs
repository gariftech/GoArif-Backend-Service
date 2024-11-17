public class LoginDto
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class RegisterDto
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }

}

public class UpdatePasswordDto
{
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }

}

public class ForgotPasswordDto
{
    public string? CodeOtp { get; set; }
    public string? Password { get; set; }

}

public class UpdatePinDto
{
    public string? Pin { get; set; }
    public string? ConfirmPin { get; set; }

}

public class PinDto
{
    public string? Pin { get; set; }

}

public class OtpDto
{
    public string? Email { get; set; }
    public string? Otp { get; set; }
}

public class ReqOtpDto
{
    public string? Email { get; set; }
}

public class ChangeUserPasswordDto {
    public string? currentPassword { get; set; }

    public string? newPassword { get; set; }

}