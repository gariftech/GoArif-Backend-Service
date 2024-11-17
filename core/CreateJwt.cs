using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly string key;

    public JwtService(IConfiguration configuration)
    {
        this.key = configuration.GetSection("AppSettings")["JwtKey"];
    }

    public string GenerateJwtToken(string id)
    {
        var tokenDescriptor = CreateTokenDescriptor(id);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private SecurityTokenDescriptor CreateTokenDescriptor(string id)
    {
        var keys = Encoding.ASCII.GetBytes(key);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, id), // NOTE: this will be the "User.Identity.Name" value
                new Claim(JwtRegisteredClaimNames.Sub, id),
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keys), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "Goarif.com",
            Audience = "Goarif.com",
        };

        return tokenDescriptor;
    }
}