using MongoDB.Driver;
using pergisafar.Shared.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryPattern.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IMongoCollection<Chat> dataUser;
        private readonly string key;

        public ChatService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Goarif");
            dataUser = database.GetCollection<Chat>("Chat");
            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }
        public async Task<Object> Get()
        {
            try
            {
                var items = await dataUser.Find(_ => _.IsActive == true).ToListAsync();
                return new { code = 200, data = items, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<Object> GetById(string id, string idUser)
        {
            try
            {
                var items = await dataUser.Find(_ => _.Id == id && _.IdUser == idUser).FirstOrDefaultAsync();

                return new { code = 200, data = items, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
        public async Task<object> Post(CreateChatDto item, string idUser)
        {
            try
            {
                var filter = Builders<Chat>.Filter.Eq(u => u.TextChat, item.TextChat);
                var ChatData = new Chat()
                {
                    Id = Guid.NewGuid().ToString(),
                    TextChat = item.TextChat,
                    // IdRoom = item.IdRoom,
                    IdUser = idUser,
                    IsActive = true,
                    IsVerification = false,
                    CreatedAt = DateTime.Now
                };
                // await dataUser.InsertOneAsync(ChatData);

                var apiKey = "AIzaSyDjO7GLq9crBdve2pfAxvJoscTkxflpL8k";
                var requestUri = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={apiKey}";
                var payload = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = item.TextChat}
                            }
                        }
                    }
                };

                var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

                var client = new HttpClient();
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(requestUri, content);

                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var geminiResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<GeminiResponse>(responseContent);
                    var data = geminiResponse?.Candidates[0].Content.Parts[0].Text;

                    var GeminiChatData = new Chat()
                    {
                        Id = Guid.NewGuid().ToString(),
                        TextChat = data,
                        // IdRoom = item.IdRoom,
                        IdUser = "Gemini",
                        IsActive = true,
                        IsVerification = false,
                        CreatedAt = DateTime.Now
                    };
                    // await dataUser.InsertOneAsync(GeminiChatData);
                    return new { code = 200, id = ChatData.Id, message = data };


                }
                catch (System.Exception)
                {
                    throw;
                }
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> Put(string id, CreateChatDto item)
        {
            try
            {
                var ChatData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (ChatData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                ChatData.TextChat = item.TextChat;
                await dataUser.ReplaceOneAsync(x => x.Id == id, ChatData);
                return new { code = 200, id = ChatData.Id.ToString(), message = "Data Updated" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
        public async Task<object> Delete(string id)
        {
            try
            {
                var ChatData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (ChatData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                ChatData.IsActive = false;
                await dataUser.ReplaceOneAsync(x => x.Id == id, ChatData);
                return new { code = 200, id = ChatData.Id.ToString(), message = "Data Deleted" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}