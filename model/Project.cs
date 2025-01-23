using Goarif.Shared.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Goarif.Shared.Models
{
    public class Project : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string? Name { get; set; }

        [BsonElement("UserId")]
        public string? UserId { get; set; }
    }
}