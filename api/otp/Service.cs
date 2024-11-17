using MongoDB.Driver;
using SendingEmail;

namespace RepositoryPattern.Services.OtpService
{
    public class OtpService : IOtpService
    {
        private readonly IMongoCollection<OtpModel> _otpCollection;
        private readonly IEmailService _mailerService;

        public OtpService(IConfiguration configuration, IEmailService mailerService)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("Goarif");
            _otpCollection = database.GetCollection<OtpModel>("Otps");
            _mailerService = mailerService;
        }

        public async Task<string> SendOtpAsync(CreateOtpDto dto)
        {
            // Hapus OTP yang sudah ada sebelumnya
            var existingOtps = await _otpCollection.Find(otp => otp.Email == dto.Email).ToListAsync();
            foreach (var otps in existingOtps)
            {
                await _otpCollection.DeleteOneAsync(o => o.Id == otps.Id);
            }

            // Generate OTP baru
            var otpCode = new Random().Next(1000, 9999).ToString();
            var otp = new OtpModel
            {
                Email = dto.Email,
                CodeOtp = otpCode,
                TypeOtp = dto.TypeOtp,
                CreatedAt = DateTime.UtcNow
            };

            // Simpan OTP ke database
            await _otpCollection.InsertOneAsync(otp);

            // Siapkan email
            var emailSubject = dto.TypeOtp == "signUp" ? "Account Activation OTP" : "Forgot Password OTP";
            var emailBody = $"Your OTP code is: {otpCode}";

            // Kirim email
            try
            {
                var emailForm = new EmailForm()
                {
                    Email = dto.Email,
                    Subject = "Request OTP",
                    Message = "OTP",
                    Otp = otpCode
                };
                await _mailerService.SendEmailAsync(emailForm);
                return "OTP sent to your email";
            }
            catch (Exception)
            {
                throw new CustomException(400, "Message", "Failed to send OTP email");
            }
        }

        public async Task<string> ValidateOtpAsync(ValidateOtpDto dto)
        {
            // Cari OTP berdasarkan email
            var otp = await _otpCollection.Find(o => o.Email == dto.Email).FirstOrDefaultAsync();

            if (otp == null)
                throw new CustomException(400, "Message", "OTP not found");

            if (otp.CodeOtp != dto.CodeOtp)
                throw new CustomException(400, "Message", "OTP invalid");

            // Hapus OTP setelah validasi
            await _otpCollection.DeleteOneAsync(o => o.Id == otp.Id);
            return "OTP valid";
        }

        
    }
}
