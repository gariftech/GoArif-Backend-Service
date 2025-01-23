public interface IProjectService
{
    Task<Object> Get(string idUser);
    Task<Object> GetById(string id);
    Task<Object> Post(CreateProjectDto items, string idUser);
    Task<Object> Put(string id, CreateProjectDto items, string idUser);
    Task<Object> Delete(string id);
}