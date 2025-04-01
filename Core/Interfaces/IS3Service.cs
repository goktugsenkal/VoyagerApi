namespace Core.Interfaces;

public interface IS3Service
{
    Task<bool> ObjectExistsAsync(string key);
    string GeneratePreSignedUploadUrl(string objectKey, TimeSpan expiration);
    string GeneratePreSignedDownloadUrl(string objectKey, TimeSpan expiration);
}