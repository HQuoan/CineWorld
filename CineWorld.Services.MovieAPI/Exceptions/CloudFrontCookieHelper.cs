using System.Security.Cryptography;
using System.Text;

namespace CineWorld.Services.MovieAPI.Exceptions
{
    public class CloudFrontCookieHelper
    {
        public static void SetSignedCookies(
               HttpResponse response,
               string resourceUrl,
               DateTime expiresOn,
               string privateKeyPath,
               string keyPairId)
        {

            string policy = CreatePolicy(resourceUrl, expiresOn);


            string signature = SignPolicy(policy, privateKeyPath);


            response.Cookies.Append("CloudFront-Policy", Convert.ToBase64String(Encoding.UTF8.GetBytes(policy)), new CookieOptions
            {

                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1),



            });
            response.Cookies.Append("CloudFront-Signature", signature, new CookieOptions
            {

                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1),



            });
            response.Cookies.Append("CloudFront-Key-Pair-Id", keyPairId, new CookieOptions
            {

                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1),


            });
        }
        //public static object GenerateSignedCookies(
        //    string resourceUrl,
        //    DateTime expiresOn,
        //    string privateKeyPath,
        //    string keyPairId)
        //{
        //    // Tạo chính sách
        //    string policy = CreatePolicy(resourceUrl, expiresOn);

        //    // Ký chính sách bằng private key
        //    string signature = SignPolicy(policy, privateKeyPath);

        //    // Trả về các cookie values dưới dạng JSON
        //    return new
        //    {
        //        CloudFrontPolicy = Convert.ToBase64String(Encoding.UTF8.GetBytes(policy)),
        //        CloudFrontSignature = signature,
        //        CloudFrontKeyPairId = keyPairId,
        //        Expires = DateTime.UtcNow.AddHours(1).ToString("o") // Đảm bảo trả về thời gian hết hạn cookie theo định dạng chuẩn
        //    };
        //}

        private static string CreatePolicy(string resourceUrl, DateTime expiresOn)
        {
            return $@"
        {{
            ""Statement"": [
                {{
                    ""Resource"": ""{resourceUrl}"",
                    ""Condition"": {{
                        ""DateLessThan"": {{ ""AWS:EpochTime"": {new DateTimeOffset(expiresOn).ToUnixTimeSeconds()} }}
                    }}
                }}
            ]
        }}";
        }
        public static string GenerateSignedUrl(
            string resourceUrl,
            DateTime expiresOn,
            string privateKeyPath,
            string keyPairId)
        {
            // Tạo chính sách
            string policy = CreatePolicy(resourceUrl, expiresOn);

            // Ký chính sách bằng private key
            string signature = SignPolicy(policy, privateKeyPath);

            // Tạo và trả về signed URL
            return $"{resourceUrl}?Policy={Convert.ToBase64String(Encoding.UTF8.GetBytes(policy))}&Signature={Uri.EscapeDataString(signature)}&Key-Pair-Id={keyPairId}";
        }
        private static string SignPolicy(string policy, string privateKeyPath)
        {
            byte[] policyBytes = Encoding.UTF8.GetBytes(policy);
            using RSA rsa = RSA.Create();
            rsa.ImportFromPem(System.IO.File.ReadAllText(privateKeyPath));

            byte[] signedBytes = rsa.SignData(policyBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
            return Convert.ToBase64String(signedBytes);
        }
    }
}
