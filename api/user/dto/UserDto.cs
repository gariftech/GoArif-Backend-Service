using MongoDB.Bson.Serialization.Attributes;

namespace Goarif.Shared.Dto
{
    public class CreateUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Username")]
        public string? Username {get; set;}
        
        [BsonElement("Email")]
        public string? Email {get; set;}

        [BsonElement("Password")]
        public string? Password {get; set;}

        [BsonElement("Image")]
        public string? Image {get; set;}

        [BsonElement("Pin")]
        public string? Pin {get; set;}

        [BsonElement("Role")]
        public string? Role {get; set;}
    }

    public class CreateApi
    {
        [BsonElement("Pin")]
        public string? Pin {get; set;}
    }
}