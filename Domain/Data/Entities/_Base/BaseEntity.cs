using System.ComponentModel.DataAnnotations;

namespace Domain.Data.Entities._Base;

public class BaseEntity
{
    [Key]
    public string Id { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }
}
