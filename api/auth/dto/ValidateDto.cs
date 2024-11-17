using Microsoft.IdentityModel.Tokens;

public class ValidationAuthDto
{
    public Dictionary<string, string> ValidateLogin(LoginDto items)
    {
        var errors = new Dictionary<string, string>();

        if (items == null || string.IsNullOrEmpty(items.Email))
        {
            errors["Email"] = "Email is a required field.";

        }
        else if (!IsValidEmail(items.Email))
        {
            errors["Email"] = "Email not valid.";
        }

        if (items == null || string.IsNullOrEmpty(items.Password))
        {
            errors["Password"] = "Password is a required field.";
        }
        else if (items.Password.Length < 8)
        {
            errors["Password"] = "Password must 8 character";
        }

        return errors;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public Dictionary<string, string> ValidateRegister(RegisterDto items)
    {
        var errors = new Dictionary<string, string>();

        if (items == null || string.IsNullOrEmpty(items.Username))
        {
            errors["Username"] = "Username is a required field.";
        }

        if (items == null || string.IsNullOrEmpty(items.Email))
        {
            errors["Email"] = "Email is a required field.";

        }
        else
        {
            if (!IsValidEmail(items.Email))
            {
                errors["Email"] = "Email not valid.";
            }
        }

        if (items == null || string.IsNullOrEmpty(items.Password))
        {
            errors["Password"] = "Password is a required field.";
        }
        else if (items.Password.Length < 8)
        {
            errors["Password"] = "Password must 8 character";
        }

        return errors;
    }

    public Dictionary<string, string> ValidateUpdatePassword(UpdatePasswordDto items)
    {
        var errors = new Dictionary<string, string>();

        if (items == null || string.IsNullOrEmpty(items.Password))
        {
            errors["Password"] = "Password is a required field.";
        }
        else if (items.Password.Length < 8)
        {
            errors["Password"] = "Password must 8 character";
        }

        if (items == null || string.IsNullOrEmpty(items.ConfirmPassword))
        {
            errors["ConfirmPassword"] = "ConfirmPassword is a required field.";
        }
        else if (items.Password.Length < 8)
        {
            errors["ConfirmPassword"] = "ConfirmPassword must 8 character";
        }
        else if (items.Password != items.ConfirmPassword)
        {
            errors["ConfirmPassword"] = "Password and confirmation do not match.";
        }

        return errors;
    }

}