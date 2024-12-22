namespace CineWorld.Services.AuthAPI.Services.IService
{
    public interface IS3Service
    {
        Task DeleteFileAsync(string key);
        Task<string> UploadFileAsync(Stream fileStream, string fileName, string userId, string? folder = null);
        string GetFileUrl(string key);
        string GetBucketName();
    }
}
