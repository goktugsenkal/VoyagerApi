namespace Core.Entities;

public abstract class UpdatableBaseEntity : BaseEntity
{
    public DateTime UpdatedAt { get; set; }
}