using Domain.Data.Entities._Base;

namespace API.Public.DTOs._Base;

public class PrivateBaseDTO<T> where T : BaseEntity
{
    public string Id { get; private set; } = string.Empty;
    public string CreatedBy { get; private set; } = string.Empty;
    public string UpdatedBy { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }

    public PrivateBaseDTO() { }

    public PrivateBaseDTO(T o)
    {
        if (o == null) return;

        Id = o.Id;
        CreatedBy = o.CreatedBy;
        UpdatedBy = o.UpdatedBy;
        CreatedAt = o.CreatedAt;
        UpdatedAt = o.UpdatedAt;
        DeletedAt = o.DeletedAt;
    }

    public T InitializeInstance(T o)
    {
        o.Id = Id;
        o.CreatedBy = CreatedBy;
        o.UpdatedBy = UpdatedBy;
        o.CreatedAt = CreatedAt;
        o.UpdatedAt = UpdatedAt;
        o.DeletedAt = DeletedAt;

        return o;
    }
}
