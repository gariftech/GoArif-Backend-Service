using Goarif.Shared.Models;

public interface IAttachmentService
{
    Task<Object> Get(string Username);
    Task<Riwayat> Upload(IFormFile file,string idUser);


    Task<object> DeleteFileAsync(string FileName);

}