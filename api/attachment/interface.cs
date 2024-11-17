public interface IAttachmentService
{
    Task<Object> Get(string Username);
    Task<(string FileName, string Url)> Upload(IFormFile file, string FileName, String idUser);

    Task<object> DeleteFileAsync(string FileName);

}