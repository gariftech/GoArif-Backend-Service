public class CustomException : Exception
{
    public string ErrorHeader { get; private set; }
    public int ErrorCode { get; private set; }

    public CustomException(int errorCode, string header, string message) : base(message)
    {
        ErrorHeader = header;
        ErrorCode = errorCode;
    }
}