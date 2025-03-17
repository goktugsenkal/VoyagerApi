using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Service(IConfiguration configuration)
    {
        var awsOptions = configuration.GetSection("AWS");
        var accessKey = awsOptions["AccessKey"];
        var secretKey = awsOptions["SecretKey"];
        var region = awsOptions["Region"];
        _bucketName = awsOptions["BucketName"] ?? throw new InvalidOperationException();

        // Create the S3 client using the provided credentials
        _s3Client = new AmazonS3Client(accessKey, secretKey, RegionEndpoint.GetBySystemName(region));
    }
    public string GeneratePreSignedUrl(string objectKey, TimeSpan expiration)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = objectKey,
            Expires = DateTime.UtcNow.Add(expiration),
            Verb = HttpVerb.PUT  // For upload operations
        };

        return _s3Client.GetPreSignedURL(request);
    }
}