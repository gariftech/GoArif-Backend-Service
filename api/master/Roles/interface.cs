public interface IRoleService
{
    Task<Object> Get();
    Task<Object> GetById(string id);
    Task<Object> Post(CreateRoleDto items);
    Task<Object> Put(string id, CreateRoleDto items);
    Task<Object> Delete(string id);
}