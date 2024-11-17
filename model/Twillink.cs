using MongoDB.Bson.Serialization.Attributes;
namespace Goarif.Shared.Models
{
public class AddUrlGoarif : BaseModel
    {
        [BsonId]
        public string? Title {get; set;}

        [BsonElement("UserId")]
        public string? UserId {get; set;}
    }
}