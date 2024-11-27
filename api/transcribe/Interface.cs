using Goarif.Shared.Models;
using MongoDB.Bson;
using static RepositoryPattern.Services.RoleService.RoleService;

public interface ITranscribeService
{
    Task<Object> Get();
    Task<Object> GetById(string id);
    Task<UploadDocument> PostAudio(IFormFile items, string languange, string idUser);
    Task<Object> PostAudioYoutubeUrl(YouTubeUrl items, string idUser);
    Task<Object> PostAudioUrl(YouTubeUrl items, string idUser);
    Task<Object> PostAudioUrlDrive(YouTubeUrl items, string idUser);
}