namespace Domain.Data.Entities._Base.Extension;

public static class BaseEntityExtension
{
    public static BaseEntity InitializeInstance(this BaseEntity current, BaseEntity i)
    {
        current.Id = i.Id;

        current.CreatedBy = i.CreatedBy;
        current.UpdatedBy = i.UpdatedBy;
        current.CreatedAt = i.CreatedAt;
        current.UpdatedAt = i.UpdatedAt;
        current.DeletedAt = i.DeletedAt;

        return current;
    }
}
