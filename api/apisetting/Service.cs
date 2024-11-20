using Goarif.Shared.Models;
using MongoDB.Driver;

namespace RepositoryPattern.Services.ApiSettingService
{
    public class ApiSettingService : IApiSettingService
    {
        private readonly IMongoCollection<ApiSetting> dataUser;
        private readonly string key;

        public ApiSettingService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Goarif");
            dataUser = database.GetCollection<ApiSetting>("ApiSettings");
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

        public async Task<Object> GetById(string id)
        {
            try
            {
                var items = await dataUser.Find(_ => _.Id == id).FirstOrDefaultAsync();
                return new { code = 200, data = items, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
        public async Task<object> Post(CreateApiSettingDto item)
        {
            try
            {
                var filter = Builders<ApiSetting>.Filter.Eq(u => u.Key, item.Key);
                var user = await dataUser.Find(filter).SingleOrDefaultAsync();
                if (user != null)
                {
                    throw new CustomException(400, "Error", "Nama sudah digunakan.");
                }
                var ApiSettingData = new ApiSetting()
                {
                    Id = Guid.NewGuid().ToString(),
                    Key = item.Key,
                    Value = item.Value,
                    IsActive = true,
                    IsVerification = false,
                    CreatedAt = DateTime.Now
                };
                await dataUser.InsertOneAsync(ApiSettingData);
                return new { code = 200, id = ApiSettingData.Id, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> Put(string id, CreateApiSettingDto item)
        {
            try
            {
                var ApiSettingData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (ApiSettingData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                ApiSettingData.Key = item.Key;
                ApiSettingData.Value = item.Value;
                await dataUser.ReplaceOneAsync(x => x.Id == id, ApiSettingData);
                return new { code = 200, id = ApiSettingData.Id.ToString(), message = "Data Updated" };
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
                var ApiSettingData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (ApiSettingData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                ApiSettingData.IsActive = false;
                await dataUser.ReplaceOneAsync(x => x.Id == id, ApiSettingData);
                return new { code = 200, id = ApiSettingData.Id.ToString(), message = "Data Deleted" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}