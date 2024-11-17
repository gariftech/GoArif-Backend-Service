using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class OtpModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    [BsonElement("Email")]
    public string Email { get; set; }
    [BsonElement("CodeOtp")]
    public string CodeOtp { get; set; }
    [BsonElement("TypeOtp")]
    public string TypeOtp { get; set; }
    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; }
}
