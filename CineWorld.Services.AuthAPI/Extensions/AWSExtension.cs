using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using CineWorld.Services.AuthAPI.Utilities.AWSS3;

namespace CineWorld.Services.AuthAPI.Extensions
{
    public static class AWSExtension
    {
        public static void ConfigureAWSS3(this IServiceCollection services, IConfiguration configuration)
        {
            var s3Settings = configuration.GetSection("AWSS3").Get<AWSS3Settings>();
            AWSOptions aWSOptions = new AWSOptions
            {
                Credentials = new BasicAWSCredentials(s3Settings.AccessKey, s3Settings.SecretKey)
            };
            services.AddDefaultAWSOptions(aWSOptions);
            services.AddAWSService<IAmazonS3>();
        }
    }
}
