public interface IRiwayatService
{
    Task<Object> GetById(string id);
    Task<Object> Post(CreateRiwayatDto items, string idUser);
    Task<Object> Delete(string id);
}