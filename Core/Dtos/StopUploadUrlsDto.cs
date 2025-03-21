namespace Core.Dtos;

public class StopUploadUrlsDto
{
    public Guid StopId { get; set; }
    public List<string> UploadUrls { get; set; }
}
