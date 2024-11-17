

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CheckId;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using SendingEmail;
using Goarif.Shared.Models;

namespace RepositoryPattern.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IMongoCollection<User> dataUser;
        private readonly IMongoCollection<OtpModel> dataOtp;

        private readonly IEmailService _emailService;
        private readonly string key;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IConfiguration configuration, IEmailService emailService, ILogger<AuthService> logger)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Goarif");
            dataUser = database.GetCollection<User>("Users");
            dataOtp = database.GetCollection<OtpModel>("Otps");
            this.key = configuration.GetSection("AppSettings")["JwtKey"];
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Object> LoginAsync([FromBody] LoginDto login)
        {
            try
            {
                var user = await dataUser.Find(u => u.Email == login.Email).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new CustomException(400, "Email", "Email tidak ditemukan");
                }
                bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(login.Password, user.Password);
                if (!isPasswordCorrect)
                {
                    throw new CustomException(400, "Password", "Password Salah");
                }
                if (user.IsActive == false)
                {
                    throw new CustomException(400, "Message", "Akun anda tidak perbolehkan akses");
                }
                if (user.IsVerification == false)
                {
                    throw new CustomException(400, "Message", "Akun anda belum aktif, silahkan aktifasi melalui link kami kirimkan di email anda");
                }

                var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                var jwtService = new JwtService(configuration);
                string userId = user.Id;
                string token = jwtService.GenerateJwtToken(userId);
                string idAsString = user.Id.ToString();
                return new { code = 200, id = idAsString, accessToken = token };
            }
            catch (CustomException ex)
            {

                throw;
            }
        }

        public async Task<object> RegisterAsync([FromBody] RegisterDto data)
        {
            try
            {
                var user = await dataUser.Find(u => u.Email == data.Email).FirstOrDefaultAsync();

                if (user != null)
                {
                    throw new CustomException(400, "Email", "Email Sudah digunakan");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Password);
                var uuid = Guid.NewGuid().ToString();

                var roleData = new User()
                {
                    Id = uuid,
                    Username = data.Username,
                    FullName = data.FullName,
                    PhoneNumber = data.PhoneNumber,
                    Email = data.Email,
                    Password = hashedPassword,
                    IsActive = true,
                    IsVerification = false,
                    IdRole = Roles.User,
                    CreatedAt = DateTime.Now
                };

                await dataUser.InsertOneAsync(roleData);
                string roleIdAsString = roleData.Id.ToString();

                var email = new EmailForm()
                {
                    Id = uuid,
                    Email = data.Email,
                    Subject = "Activation Goarif",
                    Message = "Activation"
                };
                // var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                // var jwtService = new JwtService(configuration);
                // string userId = roleData.Id;
                // string token = jwtService.GenerateJwtToken(userId);
                var sending = _emailService.SendEmailAsync(email);
                return new { code = 200, id = roleIdAsString, message = "Register Success, Please activate your account via email"};
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message);;
            }
        }

        public async Task<object> UpdatePassword(string id, ChangeUserPasswordDto item)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Error", "Data tidak ada");
                }
                if (roleData.Password != item.currentPassword)
                {
                    throw new CustomException(400, "Error", "Data tidak ada");
                }
                if (item.newPassword.Length < 8)
                {
                    throw new CustomException(400, "Password", "Password harus 8 karakter");
                }
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(item.newPassword);
                roleData.Password = hashedPassword;
                await dataUser.ReplaceOneAsync(x => x.Id == id, roleData);
                return new { code = 200, Message = "Update Password Berhasil" };
            }
            catch (CustomException ex)
            {

                throw;
            }
        }

        public async Task<object> UpdatePin(string id, UpdatePinDto item)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Error", "Data tidak ada");
                }
                if (item.Pin.Length < 6)
                {
                    throw new CustomException(400, "Password", "Pin harus 6 karakter");
                }
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(item.Pin);
                roleData.Pin = hashedPassword;
                await dataUser.ReplaceOneAsync(x => x.Id == id, roleData);
                return new { code = 200, Message = "Update Pin Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> RequestOtpEmail(string id)
        {
            try
            {

                var roleData = await dataUser.Find(x => x.Email == id).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Email", "Data not found");
                }
                Random random = new Random();
                string otp = random.Next(10000, 99999).ToString();
                var emailForm = new EmailForm()
                {
                    Id = roleData.Id,
                    Email = roleData.Email,
                    Subject = "Request OTP",
                    Message = "OTP",
                    Otp = otp
                };
                var sending = _emailService.SendEmailAsync(emailForm);
                roleData.Otp = otp;
                await dataUser.ReplaceOneAsync(x => x.Email == id, roleData);
                return new { code = 200, Message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw;
            }
        }

        public async Task<object> VerifyOtp(OtpDto otp)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Email == otp.Email).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Error", "Data not found");
                }
                if (roleData.Otp != otp.Otp)
                {
                    throw new CustomException(400, "Error", "Otp anda salah");
                }
                var data = new LoginDto();
                {
                    data.Email = roleData.Email;
                }
                var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                var jwtService = new JwtService(configuration);
                string userId = roleData.Id;
                string token = jwtService.GenerateJwtToken(userId);
                return new { code = 200, message = "Berhasil", accessToken = token };
            }
            catch (CustomException ex)
            {

                throw;
            }
        }


        public async Task<object> Aktifasi(string id)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Error", "Data not found");
                }
                roleData.IsVerification = true;
                await dataUser.ReplaceOneAsync(x => x.Id == id, roleData);
                return new { code = 200, Message = "Email berhasil di verifikasi" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> VerifyPin(string id)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Message", "Data Not Found");
                }
                if (roleData.Pin == null)
                {
                    return new CustomException(400, "Message", "Pin Not Set");
                }
                return new { code = 200, Message = "Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<Object> CheckPin([FromBody] PinDto pin, string id)
        {
            try
            {
                var user = await dataUser.Find(u => u.Id == id).FirstOrDefaultAsync();
                if (user == null)
                {
                    throw new CustomException(400, "Email", "Email tidak ditemukan");
                }
                bool isPinCorrect = BCrypt.Net.BCrypt.Verify(pin.Pin, user.Pin);
                if (!isPinCorrect)
                {
                    throw new CustomException(400, "Pin", "Pin Salah");
                }
                string idAsString = user.Id.ToString();
                return new { code = 200, Message = "Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<string> ForgotPasswordAsync(UpdateUserAuthDto dto)
        {
            try
            {
                var codeOtp = dto.CodeOtp;
                var newPassword = dto.Password;

                // Cek OTP di database
                var userOtp = await dataOtp.Find(x => x.CodeOtp.Equals(dto.CodeOtp, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();

                if (userOtp == null)
                    throw new CustomException(400,"Message", "Otp not found");

                // Cari user berdasarkan email dari OTP
                var roleData = await dataUser.Find(x => x.Email.Equals(userOtp.Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
                if (roleData == null)
                    throw new CustomException(400,"Message", "User not found");

                // Hash password baru
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                roleData.Password = passwordHash;
                await dataUser.ReplaceOneAsync(x => x.Id == roleData.Id, roleData);
                // Hapus OTP setelah berhasil update password
                await dataOtp.DeleteOneAsync(o => o.Id == userOtp.Id);

                return "Update Password Successfully";
            }
            catch (Exception ex)
            {
                throw new CustomException(400,"Message", $"ForgotPassword Error: {ex.Message}");
            }
        }

        public async Task<string> CheckMail(string email)
        {
            try
            {
                var userOtp = await dataUser.Find(x => x.Email == email).FirstOrDefaultAsync();
                if (userOtp == null)
                {
                    return "Email Not Found";
                }
                return "Available";
            }
            catch (Exception ex)
            {

                throw new CustomException(400,"Message", $"ForgotPassword Error: {ex.Message}");
            }
        }
    }
}