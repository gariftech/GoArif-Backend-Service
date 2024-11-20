using Goarif.Shared.Models;
using MongoDB.Bson;
using static RepositoryPattern.Services.RoleService.RoleService;

public interface ITranscribeService
{
    Task<Object> Get();
    Task<Object> GetById(string id);
    Task<UploadDocument> PostAudio(IFormFile items, string languange);
    Task<Object> PostAudioYoutubeUrl(YouTubeUrl items);
    Task<Object> PostAudioUrl(YouTubeUrl items);
    Task<Object> PostAudioUrlDrive(YouTubeUrl items);
}