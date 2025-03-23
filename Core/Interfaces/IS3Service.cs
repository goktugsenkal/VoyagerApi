namespace Core.Interfaces;

public interface IS3Service
{
    string GeneratePreSignedUploadUrl(string objectKey, TimeSpan expiration);
    string GeneratePreSignedDownloadUrl(string objectKey, TimeSpan expiration);
}