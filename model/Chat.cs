using Goarif.Shared.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace pergisafar.Shared.Models
{
    public class Chat : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("IdRoom")]
        public string? IdRoom { get; set; }
        [BsonElement("IdUser")]
        public string? IdUser { get; set; }

        [BsonElement("TextChat")]
        public string? TextChat { get; set; }
    }
}