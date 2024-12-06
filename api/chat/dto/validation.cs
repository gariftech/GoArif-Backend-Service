using Microsoft.IdentityModel.Tokens;

public class ValidationChatDto
{
    public List<object> ValidateApiCreateInput(CreateChatDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.TextChat))
        {
            errors.Add(new { TextChat = "Text is a required field." });
        }
        return errors;
    }
}