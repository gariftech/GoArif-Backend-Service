using Goarif.Shared.Models;
using MongoDB.Driver;

namespace RepositoryPattern.Services.ProjectService
{
    public class ProjectService : IProjectService
    {
        private readonly IMongoCollection<Project> dataUser;
        private readonly string key;

        public ProjectService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Goarif");
            dataUser = database.GetCollection<Project>("Projects");
            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }
        public async Task<Object> Get(string idUser)
        {
            try
            {
                var items = await dataUser.Find(_ => _.IsActive == true && idUser == _.UserId).ToListAsync();
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
        public async Task<object> Post(CreateProjectDto item, string idUser)
        {
            try
            {
                var filter = Builders<Project>.Filter.Eq(u => u.Name, item.Name);
                var user = await dataUser.Find(filter).SingleOrDefaultAsync();
                if (user != null)
                {
                    throw new CustomException(400, "Error", "Nama sudah digunakan.");
                }
                var ProjectData = new Project()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = item.Name,
                    UserId = idUser,
                    IsActive = true,
                    IsVerification = false,
                    CreatedAt = DateTime.Now
                };
                await dataUser.InsertOneAsync(ProjectData);
                return new { code = 200, id = ProjectData.Id, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> Put(string id, CreateProjectDto item, string idUser)
        {
            try
            {
                var ProjectData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (ProjectData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                ProjectData.Name = item.Name;
                await dataUser.ReplaceOneAsync(x => x.Id == id, ProjectData);
                return new { code = 200, id = ProjectData.Id.ToString(), message = "Data Updated" };
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
                var ProjectData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (ProjectData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                ProjectData.IsActive = false;
                await dataUser.ReplaceOneAsync(x => x.Id == id, ProjectData);
                return new { code = 200, id = ProjectData.Id.ToString(), message = "Data Deleted" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}