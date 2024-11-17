using MongoDB.Bson.Serialization.Attributes;

namespace Goarif.Shared.Models
{
    public class User : BaseModel
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Username")]
        public string? Username {get; set;}
        
        [BsonElement("Email")]
        public string? Email {get; set;}

        [BsonElement("Password")]
        public string? Password {get; set;}

        [BsonElement("FullName")]
        public string? FullName {get; set;}
        [BsonElement("PhoneNumber")]
        public string? PhoneNumber {get; set;}

        [BsonElement("Image")]
        public string? Image {get; set;}

        [BsonElement("IdRole")]
        public string? IdRole {get; set;}
        [BsonElement("Pin")]
        public string? Pin {get; set;}
        [BsonElement("Otp")]
        public string? Otp {get; set;}
    
    }
}