using MongoDB.Driver;
using Goarif.Shared.Models;

namespace RepositoryPattern.Services.PromptService
{
    public class PromptService : IPromptService
    {
        private readonly IMongoCollection<Prompt> dataUser;
        private readonly string key;

        public PromptService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Goarif");
            dataUser = database.GetCollection<Prompt>("Prompts");
            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }

        public async Task<Object> GetById(string id)
        {
            try
            {
                var items = await dataUser.Find(_ => _.Type == id).ToListAsync();
                return new { code = 200, data = items, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
        public async Task<object> Post(CreatePromptDto item)
        {
            try
            {
                var filter = Builders<Prompt>.Filter.Eq(u => u.Name, item.Name);
                var user = await dataUser.Find(filter).SingleOrDefaultAsync();
                if (user != null)
                {
                    throw new CustomException(400, "Error", "Nama sudah digunakan.");
                }
                var PromptData = new Prompt()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = item.Name,
                    Type = item.Type,
                };
                await dataUser.InsertOneAsync(PromptData);
                return new { code = 200, id = PromptData.Id, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> Put(string id, CreatePromptDto item)
        {
            try
            {
                var PromptData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (PromptData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                PromptData.Name = item.Name;
                await dataUser.ReplaceOneAsync(x => x.Id == id, PromptData);
                return new { code = 200, id = PromptData.Id.ToString(), message = "Data Updated" };
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
                var PromptData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (PromptData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                // PromptData.IsActive = false;≥
                await dataUser.ReplaceOneAsync(x => x.Id == id, PromptData);
                return new { code = 200, id = PromptData.Id.ToString(), message = "Data Deleted" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}