namespace Core.Interfaces;

public interface IS3Service
{
    string GeneratePreSignedUrl(string objectKey, TimeSpan expiration);
}