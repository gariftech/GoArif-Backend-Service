using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Goarif.Shared.Models
{
    public class Prompt
    {
        [BsonId]
        public string? Id { get; set; }
        [BsonElement("Title")]
        public string? Title { get; set; }

        [BsonElement("Name")]
        public string? Name { get; set; }
        [BsonElement("Type")]
        public string? Type { get; set; }
    }
}