public interface IPromptService
{
    Task<Object> Get(string type);
    Task<Object> GetById(string id);
    Task<Object> Post(CreatePromptDto items);
    Task<Object> Put(string id, CreatePromptDto items);
    Task<Object> Delete(string id);
}