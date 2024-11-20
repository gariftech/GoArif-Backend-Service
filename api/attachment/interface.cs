public interface IAttachmentService
{
    Task<Object> Get(string Username);

    Task<object> DeleteFileAsync(string FileName);

}