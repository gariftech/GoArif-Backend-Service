using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace Goarif.Shared.Models
{
    public class Transcribe : BaseModel
    {
        [BsonId]
        public string? Id { get; set; }

        [BsonElement("Name")]
        public string? Name { get; set; }
    }

    public class UploadDocument : BaseModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int? Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public object Result { get; set; }
        public object File {get; set; }
        public byte[] Data { get; set; }

    }

    public class YouTubeUrl
    {
        public string? Url { get; set; }
        public string? Languange { get; set; } = "id";
        public YouTubeUrl()
        {
            Languange = "id";
        }


    }
}