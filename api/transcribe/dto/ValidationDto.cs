using Microsoft.IdentityModel.Tokens;

public class ValidationTranscribeDto
{
    public List<object> ValidateCreateInput(CreateTranscribeDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.Name))
        {
            errors.Add(new { Name = "Name is a required field." });
        }
        return errors;
    }
}