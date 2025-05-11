using Core.Entities.Chat;

namespace Core.Models.Chat;

public class CreateChatRoomModel
{
    public Guid ClientId { get; set; }
    // list of users to be added as participants
    public List<CreateChatRoomParticipantModel> ParticipantModels { get; set; } = [];
    public required string Title { get; set; }
    public string Description { get; set; } = string.Empty;

    // should be left empty if there is no intent to upload an image
    public string ImageType { get; set; } = string.Empty;
    public string BannerType { get; set; } = string.Empty;
}
