using Microsoft.AspNetCore.Mvc;

public class ErrorHandlingUtility
{
    public IActionResult HandleError(int errorCode, ErrorResponse errorResponse)
    {
        switch (errorCode)
        {
            case 400:
                return new ObjectResult(errorResponse) { StatusCode = 400 };
            case 401:
                return new ObjectResult(errorResponse) { StatusCode = 401 };
            case 403:
                return new ObjectResult(errorResponse) { StatusCode = 403 };
            case 404:
                return new ObjectResult(errorResponse) { StatusCode = 404 };
            case 500:
                return new ObjectResult(errorResponse) { StatusCode = 500 };
            default:
                return new ObjectResult(errorResponse) { StatusCode = 500 };
        }
    }
}