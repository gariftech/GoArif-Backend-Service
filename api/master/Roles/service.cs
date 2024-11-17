using MongoDB.Driver;
using Goarif.Shared.Models;

namespace RepositoryPattern.Services.RoleService
{
    public class RoleService : IRoleService
    {
        private readonly IMongoCollection<Role> dataUser;
        private readonly string key;

        public RoleService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Goarif");
            dataUser = database.GetCollection<Role>("Roles");
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
        public async Task<object> Post(CreateRoleDto item)
        {
            try
            {
                var filter = Builders<Role>.Filter.Eq(u => u.Name, item.Name);
                var user = await dataUser.Find(filter).SingleOrDefaultAsync();
                if (user != null)
                {
                    throw new CustomException(400, "Error", "Nama sudah digunakan.");
                }
                var roleData = new Role()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = item.Name,
                    IsActive = true,
                    IsVerification = false,
                    CreatedAt = DateTime.Now
                };
                await dataUser.InsertOneAsync(roleData);
                return new { code = 200, id = roleData.Id, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> Put(string id, CreateRoleDto item)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                roleData.Name = item.Name;
                await dataUser.ReplaceOneAsync(x => x.Id == id, roleData);
                return new { code = 200, id = roleData.Id.ToString(), message = "Data Updated" };
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
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (roleData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                roleData.IsActive = false;
                await dataUser.ReplaceOneAsync(x => x.Id == id, roleData);
                return new { code = 200, id = roleData.Id.ToString(), message = "Data Deleted" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}