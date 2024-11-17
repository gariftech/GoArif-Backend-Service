public class CreateOtpDto
{
    public string Email { get; set; }
    public string TypeOtp { get; set; }
}

public class UpdateUserAuthDto
{
    public string CodeOtp { get; set; }
    public string Password { get; set; }
}