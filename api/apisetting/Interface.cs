public interface IApiSettingService
{
    Task<Object> Get();
    Task<Object> GetById(string id);
    Task<Object> Post(CreateApiSettingDto items);
    Task<Object> Put(string id, CreateApiSettingDto items);
    Task<Object> Delete(string id);
}