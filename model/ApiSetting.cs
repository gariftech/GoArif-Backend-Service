using Goarif.Shared.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Goarif.Shared.Models
{
    public class ApiSetting : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("Key")]
        public string? Key { get; set; }

        [BsonElement("Value")]
        public string? Value { get; set; }
    }
}