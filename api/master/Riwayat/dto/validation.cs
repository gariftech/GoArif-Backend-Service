using Microsoft.IdentityModel.Tokens;

public class ValidationRiwayatDto
{
    public List<object> ValidateCreateInput(CreateRiwayatDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.Title))
        {
            errors.Add(new { Title = "Title is a required field." });
        }
        return errors;
    }
}