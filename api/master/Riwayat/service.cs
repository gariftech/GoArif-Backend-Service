using MongoDB.Driver;
using Goarif.Shared.Models;

namespace RepositoryPattern.Services.RiwayatService
{
    public class RiwayatService : IRiwayatService
    {
        private readonly IMongoCollection<Riwayat> dataUser;
        private readonly string key;

        public RiwayatService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Goarif");
            dataUser = database.GetCollection<Riwayat>("Riwayat");
            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }

        public async Task<Object> GetById(string id)
        {
            try
            {
                // Sort by 'CreatedAt' field in descending order (latest first)
                var items = await dataUser
                    .Find(_ => _.UserId == id && _.IsActive == true)  // Filter by 'IsActive' field
                    .SortByDescending(item => item.CreatedAt)  // Sort by 'CreatedAt' in descending order
                    .ToListAsync();

                return new { code = 200, data = items, message = "Data retrieved successfully" };
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Catch any other exceptions to avoid exposing raw errors to the client
                return new { code = 500, message = $"An error occurred: {ex.Message}" };
            }
        }

        public async Task<object> Post(CreateRiwayatDto item, string idUser)
        {
            try
            {
                var RiwayatData = new Riwayat()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = item.Title,
                    Type = item.Type,
                    File = item.File,
                    Result = item.Result,
                    Prompt = item.Prompt,
                    Size = item.Size,
                    UserId = idUser,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                };
                await dataUser.InsertOneAsync(RiwayatData);
                return new { code = 200, id = RiwayatData.Id, message = "Data Add Complete" };
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
                var RiwayatData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (RiwayatData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                RiwayatData.IsActive = false;
                await dataUser.ReplaceOneAsync(x => x.Id == id, RiwayatData);
                return new { code = 200, id = RiwayatData.Id.ToString(), message = "Data Deleted" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}