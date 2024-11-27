using Microsoft.IdentityModel.Tokens;

public class ValidationPromptDto
{
    public List<object> ValidateCreateInput(CreatePromptDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.Name))
        {
            errors.Add(new { Name = "Name is a required field." });
        }
        return errors;
    }
}