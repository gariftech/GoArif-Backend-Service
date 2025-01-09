using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Goarif.Shared.Models
{
    public class Riwayat : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }
        [BsonElement("Title")]
        public string? Title { get; set; }
        [BsonElement("Result")]
        public object? Result { get; set; }
        [BsonElement("Prompt")]
        public string? Prompt { get; set; }
        [BsonElement("File")]
        public List<string>? File { get; set; }
        [BsonElement("Type")]
        public string? Type { get; set; }
        [BsonElement("Size")]
        public long? Size { get; set; }
        [BsonElement("UserId")]
        public string? UserId { get; set; }
    }
}