using Microsoft.IdentityModel.Tokens;

public class ValidationMasterDto
{
    public List<object> ValidateCreateInput(CreateRoleDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.Name))
        {
            errors.Add(new { Name = "Name is a required field." });
        }
        return errors;
    }

    public List<object> ValidateApiCreateInput(CreateApiSettingDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.Key))
        {
            errors.Add(new { Key = "Key is a required field." });
        }
        if (items == null || string.IsNullOrEmpty(items.Value))
        {
            errors.Add(new { Value = "Value is a required field." });
        }
        return errors;
    }

    public List<object> ValidateSettingCreateInput(CreateSettingDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.Key))
        {
            errors.Add(new { Key = "Key is a required field." });
        }
        if (items == null || string.IsNullOrEmpty(items.Value))
        {
             errors.Add(new { Value = "Value is a required field." });
        }
        return errors;
    }

    public List<object> ValidateComparisonCreateInput(CreateComparisonDto items)
    {
        var errors = new List<object>();

        if (items == null || string.IsNullOrEmpty(items.Key))
        {
            errors.Add(new { Key = "Key is a required field." });
        }
        if (items == null || string.IsNullOrEmpty(items.Value))
        {
             errors.Add(new { Value = "Value is a required field." });
        }
        return errors;
    }
}