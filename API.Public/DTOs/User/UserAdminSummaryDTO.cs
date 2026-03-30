using Domain.Data.Entities;

namespace API.Public.DTOs;

public class UserAdminSummaryDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Document { get; set; }
    public string Cellphone { get; set; }
    public string ProfileType { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastAccessAt { get; set; }

    public static UserAdminSummaryDTO ToDTO(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Document = user.Document,
        Cellphone = user.Cellphone,
        ProfileType = user.ProfileType?.ToString(),
        CreatedAt = user.CreatedAt,
        LastAccessAt = user.LastAccessAt
    };

    public static List<UserAdminSummaryDTO> ToDTO(IEnumerable<User> users)
        => users.Select(ToDTO).ToList();
}
