using System.Text.Json;

public class ErrorResponse
{
    public int Code { get; set; }
    public IDictionary<string, string> ErrorMessage { get; set; }

    public ErrorResponse(int errorCode, string errorHeader, string errorMessage)
    {
        Code = errorCode;
        ErrorMessage = new Dictionary<string, string>
        {
            { errorHeader, errorMessage }
        };
    }
}

public class ErrorMessageItem
{
    public string error { get; set; }
}

public class ErrorDto
{
    public int code { get; set; }
    public ErrorMessageItem errorMessage { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}