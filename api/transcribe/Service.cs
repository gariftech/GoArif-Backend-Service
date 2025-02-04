

using System.Diagnostics;
using System.Text.RegularExpressions;
using Deepgram;
using Deepgram.Models.Listen.v1.REST;
using Goarif.Shared.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YoutubeExplode;

namespace RepositoryPattern.Services.TranscribeService
{
    public class TranscribeService : ITranscribeService
    {
        private readonly IMongoCollection<Transcribe> dataUser;
        private readonly IMongoCollection<ApiSetting> _apiSetting;
        private readonly IMongoCollection<Riwayat> _riwayat;
        private readonly IAttachmentService _IAttachmentService;


        private readonly string key;
        private readonly string _apiKey = "cd126858ce8cfafa9e32e79ea04117fff14d4e2b"; // Your Deepgram API key

        public TranscribeService(IConfiguration configuration, IAttachmentService roleService)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Goarif");
            dataUser = database.GetCollection<Transcribe>("Transcribes");
            _apiSetting = database.GetCollection<ApiSetting>("ApiSettings");
            _riwayat = database.GetCollection<Riwayat>("Riwayat");
            _IAttachmentService = roleService;

            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }

        public async Task<Object> Get()
        {
            try
            {
                var items = await dataUser.Find(_ => _.IsActive == true).ToListAsync();
                return new { code = 200, data = items, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<Object> GetById(string id)
        {
            try
            {
                var items = await dataUser.Find(_ => _.Id == id).FirstOrDefaultAsync();
                return new { code = 200, data = items, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<UploadDocument> PostAudio(IFormFile file, string languange, string idUser)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Invalid file");

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var audioData = memoryStream.ToArray();
                object Result = await ConvertAudioToTextAsync(audioData, languange);


                Riwayat resultUpoad = await _IAttachmentService.Upload(file, idUser);

                var uploadedFile = new UploadDocument
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Result = Result, // Store the text result,
                    File = resultUpoad?.File
                };

                // var RiwayatData = new Riwayat()
                // {
                //     Id = Guid.NewGuid().ToString(),
                //     Title = file.FileName,
                //     Type = "Transcribe",
                //     File = resultUpoad?.File,
                //     Result = Result.ToString(),
                //     Prompt = resultUpoad?.Prompt,
                //     UserId = idUser,
                //     CreatedAt = DateTime.Now,
                //     IsActive = true,
                // };
                // await _riwayat.InsertOneAsync(RiwayatData);
                
                return uploadedFile;
            }
        }

        public async Task<object> PostAudioYoutubeUrl(YouTubeUrl youtubeUrl, string idUser)
        {
            try
            {
                var youtubeClient = new YoutubeClient();

                // Get video metadata
                var video = await youtubeClient.Videos.GetAsync(youtubeUrl.Url);

                // Get audio stream manifest
                var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(video.Id);

                // Get all audio streams and select the one with the highest bitrate
                var audioStream = streamManifest.GetAudioOnlyStreams()
                    .OrderByDescending(stream => stream.Bitrate)
                    .FirstOrDefault();

                if (audioStream == null)
                {
                    return new { code = 404, message = "No audio stream found." };
                }

                // Download the selected audio stream into memory
                using var audioStreamData = await youtubeClient.Videos.Streams.GetAsync(audioStream);
                using var memoryStream = new MemoryStream();
                await audioStreamData.CopyToAsync(memoryStream);

                memoryStream.Position = 0; // Reset stream position for reading

                // Convert audio stream to text
                var transcript = await ConvertAudioToTextAsyncWithLang(memoryStream.ToArray(), youtubeUrl);
                var uploadedFile = new UploadDocument
                {
                    FileName = youtubeUrl.Url,
                    ContentType = youtubeUrl.Url,
                    Result = transcript // Store the text result
                };

                var RiwayatData = new Riwayat()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = youtubeUrl.Url,
                    Type = "Transcribe UrlYoutube",
                    File = [youtubeUrl.Url],
                    Result = transcript.ToString(),
                    Prompt = "",
                    UserId = idUser,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                };
                await _riwayat.InsertOneAsync(RiwayatData);

                return uploadedFile;
            }
            catch (Exception ex)
            {
                // Handle errors and return error message
                return new { code = 500, message = $"Error processing audio: {ex.Message}" };
            }
        }

        public async Task<object> PostAudioUrl(YouTubeUrl audioUrl, string idUser)
        {
            try
            {
                // Validate the URL (You can use more complex validation based on requirements)
                if (string.IsNullOrEmpty(audioUrl.Url) || !Uri.IsWellFormedUriString(audioUrl.Url, UriKind.Absolute))
                {
                    return new { code = 400, message = "Invalid URL." };
                }

                // Download the audio stream into memory
                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(audioUrl.Url);
                if (!response.IsSuccessStatusCode)
                {
                    return new { code = (int)response.StatusCode, message = "Failed to download audio." };
                }

                using var audioStreamData = await response.Content.ReadAsStreamAsync();
                using var memoryStream = new MemoryStream();
                await audioStreamData.CopyToAsync(memoryStream);

                memoryStream.Position = 0; // Reset stream position for reading

                // Convert audio stream to text
                var transcript = await ConvertAudioToTextAsyncWithLang(memoryStream.ToArray(), audioUrl);
                var uploadedFile = new UploadDocument
                {
                    FileName = audioUrl.Url,
                    ContentType = "audio/mpeg",
                    Result = transcript // Store the text result
                };

                var RiwayatData = new Riwayat()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = audioUrl.Url,
                    Type = "Transcribe UrlAudio",
                    File = [audioUrl.Url],
                    Result = transcript.ToString(),
                    Prompt = "",
                    UserId = idUser,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                };
                await _riwayat.InsertOneAsync(RiwayatData);

                return uploadedFile;
            }
            catch (Exception ex)
            {
                // Handle errors and return error message
                return new { code = 500, message = $"Error processing audio: {ex.Message}" };
            }
        }

        public async Task<object> PostAudioUrlDrive(YouTubeUrl googleDriveLink, string idUser)
        {
            try
            {
                // Convert Google Drive shareable link to direct download link
                var directDownloadUrl = ConvertGoogleDriveLinkToDirectUrl(googleDriveLink.Url);

                if (string.IsNullOrEmpty(directDownloadUrl) || !Uri.IsWellFormedUriString(directDownloadUrl, UriKind.Absolute))
                {
                    return new { code = 400, message = "Invalid Google Drive link." };
                }

                // Download the audio stream into memory
                using var httpClient = new HttpClient();
                using var response = await httpClient.GetAsync(directDownloadUrl);
                if (!response.IsSuccessStatusCode)
                {
                    return new { code = (int)response.StatusCode, message = "Failed to download audio." };
                }

                using var audioStreamData = await response.Content.ReadAsStreamAsync();
                using var memoryStream = new MemoryStream();
                await audioStreamData.CopyToAsync(memoryStream);

                memoryStream.Position = 0; // Reset stream position for reading

                // Convert audio stream to text
                var transcript = await ConvertAudioToTextAsync(memoryStream.ToArray(), "audio/mpeg");

                var uploadedFile = new UploadDocument
                {
                    FileName = googleDriveLink.Url,
                    ContentType = "audio/mpeg",
                    Result = transcript // Store the text result
                };

                var RiwayatData = new Riwayat()
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = googleDriveLink.Url,
                    Type = "Transcribe UrlDrive",
                    File = [googleDriveLink.Url],
                    Result = transcript.ToString(),
                    Prompt = "",
                    UserId = idUser,
                    CreatedAt = DateTime.Now,
                    IsActive = true,
                };
                await _riwayat.InsertOneAsync(RiwayatData);

                return uploadedFile;
            }
            catch (Exception ex)
            {
                // Handle errors and return error message
                return new { code = 500, message = $"Error processing audio: {ex.Message}" };
            }
        }

        // Method to convert Google Drive shareable link to direct download URL
        private string ConvertGoogleDriveLinkToDirectUrl(string shareableLink)
        {
            // Extract file ID from Google Drive shareable link
            var fileId = ExtractGoogleDriveFileId(shareableLink);
            if (string.IsNullOrEmpty(fileId))
            {
                return null;
            }

            // Convert to direct download URL
            return $"https://drive.google.com/uc?export=download&id={fileId}";
        }

        // Method to extract file ID from Google Drive shareable link
        private string ExtractGoogleDriveFileId(string link)
        {
            var pattern = @"(?<=id=|/)([a-zA-Z0-9_-]{33})(?=[/?]|$)";
            var match = Regex.Match(link, pattern);
            return match.Success ? match.Value : null;
        }

        private async Task<object> ConvertAudioToTextAsync(byte[] audioData, string languange)
        {
            var items = await _apiSetting.Find(_ => _.Key == "DeepGram").FirstOrDefaultAsync();
            // Create Deepgram client
            var deepgramClient = new PreRecordedClient(items.Value);

            // Create a temporary file path
            var tempFilePath = Path.GetTempFileName();
            try
            {
                // Write audio data to the temporary file
                await File.WriteAllBytesAsync(tempFilePath, audioData);

                // Perform transcription
                using var fileStream = File.OpenRead(tempFilePath); // Ensure proper disposal of file stream
                CancellationTokenSource cancelToken = new CancellationTokenSource(600000);
                var response = await deepgramClient.TranscribeFile(
                    fileStream,
                    new PreRecordedSchema
                    {
                        Punctuate = true,
                        Model = "nova-2",
                        Language = languange,
                        SmartFormat = true,
                        Paragraphs = true,
                        Diarize=true
                    }, cancelToken);

                // Process and return the transcription result
                var transcript = response?.Results?.Channels?[0]?.Alternatives?[0]?.Paragraphs;
                return transcript;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw new Exception($"Error in transcription: {ex.Message}", ex);
            }
            finally
            {
                // Clean up temporary file
                if (File.Exists(tempFilePath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch (IOException ex)
                    {
                        // Log or handle the exception if the file cannot be deleted
                        Debug.WriteLine($"Error deleting temporary file: {ex.Message}");
                    }
                }
            }
        }

        private async Task<object> ConvertAudioToTextAsyncWithLang(byte[] audioData, YouTubeUrl youtubeUrl)
        {
            var items = await _apiSetting.Find(_ => _.Key == "DeepGram").FirstOrDefaultAsync();
            // Create Deepgram client
            var deepgramClient = new PreRecordedClient(items.Value);

            // Create a temporary file path
            var tempFilePath = Path.GetTempFileName();
            try
            {
                // Write audio data to the temporary file
                await File.WriteAllBytesAsync(tempFilePath, audioData);

                // Perform transcription
                using var fileStream = File.OpenRead(tempFilePath); // Ensure proper disposal of file stream
                CancellationTokenSource cancelToken = new CancellationTokenSource(600000);
                var response = await deepgramClient.TranscribeFile(
                    fileStream,
                    new PreRecordedSchema
                    {
                        Punctuate = true,
                        Model = "nova-2",
                        Language = youtubeUrl.Languange,
                        SmartFormat = true,
                        Paragraphs = true,
                        Diarize=true
                    }, cancelToken);

                // Process and return the transcription result
                var transcript = response?.Results?.Channels?[0]?.Alternatives?[0]?.Paragraphs;
                return transcript;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                throw new Exception($"Error in transcription: {ex.Message}", ex);
            }
            finally
            {
                // Clean up temporary file
                if (File.Exists(tempFilePath))
                {
                    try
                    {
                        File.Delete(tempFilePath);
                    }
                    catch (IOException ex)
                    {
                        // Log or handle the exception if the file cannot be deleted
                        Debug.WriteLine($"Error deleting temporary file: {ex.Message}");
                    }
                }
            }
        }

    }
}