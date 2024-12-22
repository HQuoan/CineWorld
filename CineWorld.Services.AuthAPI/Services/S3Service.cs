using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using CineWorld.Services.AuthAPI.Services.IService;
using CineWorld.Services.AuthAPI.Utilities;
using CineWorld.Services.AuthAPI.Utilities.AWSS3;

namespace CineWorld.Services.AuthAPI.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly IConfigAWSS3 _config;
        public S3Service(IAmazonS3 s3Client, IConfigAWSS3 config)
        {
            _s3Client = s3Client;
            _config = config;
        }
        public async Task DeleteFileAsync(string key)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _config.getAWSBucketName(),
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
        }
        public string GetBucketName()
        {
            return _config.getAWSBucketName();
        }
        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string fileId, string? folder = null)
        {
            const long maxFileSize = 2 * 1024 * 1024;
            if (fileStream == null || fileStream.Length == 0)
            {
                throw new ArgumentException("No file uploaded or the file is empty!");
            }
            if (fileStream.Length > maxFileSize)
            {
                throw new ArgumentException("File exceeds the maximum allowed size!");
            }
            var allowedExtensions = new[] { FileExtensions.Jpg, FileExtensions.Jpeg, FileExtensions.Png };
            var fileExtension = Path.GetExtension(fileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Unsupported file type!");
            }
            if(folder != FolderNameS3.USER_AVATARS && folder != FolderNameS3.MOVIE_IMAGES)
            {
                throw new ArgumentException("Invalid Foldername");
            }    
            var key = string.IsNullOrEmpty(folder)
                ? $"{Guid.NewGuid()}_{fileName}"
                : $"{folder}/{fileId}";

            var request = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = key,
                BucketName = _config.getAWSBucketName(),
                ContentType = "image/jpeg",
                CannedACL = S3CannedACL.PublicRead
            };

            using (var transferUtility = new TransferUtility(_s3Client))
            {
                await transferUtility.UploadAsync(request);
            }
            return key;
        }
        public string GetFileUrl(string key)
        {
            return $"https://{_config.getAWSBucketName()}.s3.{_config.getAWSRegion()}.amazonaws.com/{key}";
        }

    }
}
