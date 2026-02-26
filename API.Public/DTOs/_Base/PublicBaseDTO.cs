using Domain.Data.Entities._Base;

namespace API.Public.DTOs._Base;

public class PublicBaseDTO<T> where T : BaseEntity
{
    public string Id { get; set; } = string.Empty;

    public PublicBaseDTO() { }

    public PublicBaseDTO(T o)
    {
        if (o == null) return;

        Id = o.Id;
    }

    public T InitializeInstance(T o)
    {
        o.Id = Id;

        return o;
    }
}
