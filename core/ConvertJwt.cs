using System.IdentityModel.Tokens.Jwt;
public class ConvertJWT
{
    public async Task<string> ConvertString(string accessToken)
    {
        var tokenAccess = accessToken.Substring("Bearer ".Length);
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.ReadJwtToken(tokenAccess);
        string idUser = token.Subject;
        return idUser;//
    }
}