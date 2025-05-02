using Core.Entities.Chat;

namespace Core.Results;

public record CreateChatRoomResult(string? ImageUploadUrl, string? BannerUploadUrl);