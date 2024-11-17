using Microsoft.IdentityModel.Tokens;
using Goarif.Shared.Dto;

public class ValidationUserDto
{
    public List<object> ValidateCreateInput(CreateUser items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.Username))
        {
            errors.Add(new { Username = "Username is a required field." });
        }
        return errors;
    }
}