using MongoDB.Bson.Serialization.Attributes;

namespace Goarif.Shared.Models
{
    public class Widget
    {
        public object[] WidgetList {get; set;}
        public object user {get; set;}
    }
    public class AddLink : BaseModelUser 
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Title")]
        public string? Title {get; set;}
        
        [BsonElement("UrlLink")]
        public string? UrlLink {get; set;}

        [BsonElement("UrlTumbnail")]
        public string? UrlTumbnail {get; set;}
    }

    public class AddText : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Text")]
        public string? Text {get; set;}
    }

    public class AddImage : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Caption")]
        public string? Caption {get; set;}
        
        [BsonElement("UrlImage")]
        public string? UrlImage {get; set;}
    }

    public class AddProfile : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Caption")]
        public string? Caption {get; set;}
        
        [BsonElement("UrlImage")]
        public string? UrlImage {get; set;}
    }

    public class AddVideo: BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Caption")]
        public string? Caption {get; set;}
        
        [BsonElement("UrlVideo")]
        public string? UrlVideo {get; set;}
    }

    public class AddContent : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("UrlCover")]
        public string? UrlCover {get; set;}
        
        [BsonElement("Title")]
        public string? Title {get; set;}

        [BsonElement("Content")]
        public string? Content {get; set;}
    }

    public class AddContact : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Email")]
        public string? Email {get; set;}
        
        [BsonElement("PhoneNumber")]
        public string? PhoneNumber {get; set;}
    }

    public class AddCarausel : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Caption")]
        public string? Caption {get; set;}
        
        [BsonElement("Image")]
        public string? Image {get; set;}
    }

    public class AddWebinar : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("Title")]
        public string? Title {get; set;}

        [BsonElement("UrlLink")]
        public string? UrlLink {get; set;}

        [BsonElement("Passcode")]
        public string? Passcode {get; set;}
        
        [BsonElement("StartDate")]
        public DateTime? StartDate {get; set;}

        [BsonElement("EndDate")]
        public DateTime? EndDate {get; set;}

        [BsonElement("Desc")]
        public string? Desc {get; set;}
    }

    public class AddBanner : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("UrlImage")]
        public string? UrlImage {get; set;}
    }

    public class AddSocial : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("UrlLink")]
        public string? UrlLink {get; set;}

        [BsonElement("Model")]
        public int? Model {get; set;}
    }

    public class Attachments : BaseModelUser
    {
        [BsonId]
        public string? Id {get; set;}

        [BsonElement("fileName")]
        public string? fileName {get; set;}

        [BsonElement("type")]
        public string? type {get; set;}

        [BsonElement("path")]
        public string? path {get; set;}
    }
}