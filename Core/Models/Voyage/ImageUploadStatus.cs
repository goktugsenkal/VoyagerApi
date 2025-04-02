namespace Core.Models;

public class ImageUploadStatus
{
    // For example, the S3 object key or an identifier used during pre-signed URL generation.
    public string ImageKey { get; set; }
    public bool UploadSuccessful { get; set; }
}