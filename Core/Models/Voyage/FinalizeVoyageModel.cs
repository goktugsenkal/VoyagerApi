namespace Core.Models;

public class FinalizeVoyageModel
{
    public int VoyageId { get; set; }
    public List<ImageUploadStatus> ImageStatuses { get; set; }
}