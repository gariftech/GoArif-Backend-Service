using MongoDB.Bson;
using static RepositoryPattern.Services.ChatService.ChatService;

public interface IChatService
{
    Task<Object> Get();
    Task<Object> GetById(string id, string idUser);
    Task<Object> Post(CreateChatDto items, string idUser);
    Task<Object> Put(string id, CreateChatDto items);
    Task<Object> Delete(string id);
}