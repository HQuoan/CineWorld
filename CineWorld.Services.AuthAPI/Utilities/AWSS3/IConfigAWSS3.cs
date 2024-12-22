namespace CineWorld.Services.AuthAPI.Utilities.AWSS3
{
    public interface IConfigAWSS3
    {
        string getAWSAccessKey();
        string getAWSSecretKey();
        string getAWSRegion();
        string getAWSBucketName();
    }
}
