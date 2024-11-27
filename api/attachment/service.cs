using Goarif.Shared.Models;
using Google.Cloud.Storage.V1;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace RepositoryPattern.Services.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly string bucketName = "twillink";
        private readonly StorageClient storageClient;
        private readonly IMongoCollection<Attachments> AttachmentLink;
        private readonly IMongoCollection<User> users;
        private readonly IConfiguration _conf;

        private readonly string key;

        public AttachmentService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Goarif");
            AttachmentLink = database.GetCollection<Attachments>("Attachment");
            this.key = configuration.GetSection("AppSettings")["JwtKey"];
            _conf = configuration;
        }
        public async Task<Object> Get(string Username)
        {
            try
            {
                var items = await AttachmentLink.Find(_ => _.UserId == Username).ToListAsync();
                return new { code = 200, data = items, message = "Complete" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        public async Task<Riwayat> Upload(IFormFile file, string idUser)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new CustomException(400, "Message", "File not found");
                }

                // Define max file size: 300 MB
                const long maxFileSize = 300 * 1024 * 1024; // 300 MB

                // Validate file size
                if (file.Length > maxFileSize)
                {
                    throw new CustomException(400, "Message", "File size must not exceed 300 MB.");
                }

                // Initialize GridFSBucket
                var client = new MongoClient(_conf.GetConnectionString("ConnectionURI"));
                var database = client.GetDatabase("Goarif");
                var gridFSBucket = new GridFSBucket(database);

                // Upload file to GridFS
                ObjectId fileId;
                using (var stream = file.OpenReadStream())
                {
                    var options = new GridFSUploadOptions
                    {
                        Metadata = new BsonDocument
                {
                    { "FileName", file.FileName },
                    { "ContentType", file.ContentType },
                    { "UploadedBy", idUser },
                    { "UploadedAt", DateTime.UtcNow }
                }
                    };

                    fileId = await gridFSBucket.UploadFromStreamAsync(file.FileName, stream, options);
                }
                return new Riwayat
                {
                    File = [$"https://app.goarif.co/api/v1/Attachment/Download/{fileId}"],
                };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        // public async Task<(string FileName, string Url)> Upload(IFormFile file, string fileName, string idUser)
        // {
        //     var bucket = bucketName;
        //     var folderName = "uploads";
        //     var fileSize = file.Length;

        //     // Baca file sebagai stream
        //     using var memoryStream = new MemoryStream();
        //     await file.CopyToAsync(memoryStream);
        //     memoryStream.Position = 0;

        //     // Tentukan metadata file
        //     var contentType = file.ContentType;

        //     // Unggah file ke Google Cloud Storage
        //     var gcsFile = storageClient.UploadObject(bucket, fileName, contentType, memoryStream);

        //     // URL akses file
        //     var url = $"https://storage.googleapis.com/{bucket}/{fileName}";
        //     var uuid = Guid.NewGuid().ToString();

        //     var otp = new Attachments
        //     {
        //         Id = uuid,
        //         fileName = fileName,
        //         type = file.ContentType,
        //         path = url,
        //         UserId = idUser,
        //         size = fileSize,
        //         CreatedAt = DateTime.Now,
        //     };

        //     // Simpan OTP ke database
        //     await AttachmentLink.InsertOneAsync(otp);

        //     return (gcsFile.Name, url);
        // }

        public async Task<object> DeleteFileAsync(string fileUrl)
        {
            try
            {
                // Extract bucket name and file path from the URL
                var uri = new Uri(fileUrl);
                var bucketName = uri.Host.Split('.')[0]; // e.g., "my-bucket" from "https://storage.googleapis.com/my-bucket/filename"
                var filePath = uri.AbsolutePath.TrimStart('/'); // e.g., "folder/filename.jpg"
                string fileName = filePath.Split('/').Last();

                // Delete the file from GCS
                await storageClient.DeleteObjectAsync("twillink", fileName);

                var ChatData = await AttachmentLink.Find(x => x.path == fileUrl).FirstOrDefaultAsync();
                if (ChatData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                await AttachmentLink.DeleteOneAsync(x => x.Id == ChatData.Id);
                return "Berhasil";
            }
            catch (Exception ex)
            {
                throw new CustomException(400, "Error", ex.Message);
            }
        }
    }
}