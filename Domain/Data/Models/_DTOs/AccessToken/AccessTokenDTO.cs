using Domain.Data.Entities;
using Domain.Data.Models.DTOs._Base;

namespace Domain.Data.Models.DTOs;

public class AccessTokenDTO : BaseDTO
{
    public string UserId { get; set; }
    public User User { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }

    public AccessTokenDTO(string id, DateTimeOffset createdAt, DateTimeOffset? expiresAt)
    {
        Id = id;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public AccessTokenDTO(AccessToken accessToken)
    {
        Id = accessToken.Id;
        CreatedAt = accessToken.CreatedAt;
        CreatedBy = accessToken.CreatedBy;
        UpdatedBy = accessToken.UpdatedBy;
        UpdatedAt = accessToken.UpdatedAt;
        DeletedAt = accessToken.DeletedAt;
        UserId = accessToken.UserId;
        ExpiresAt = accessToken.ExpiresAt;
        User = accessToken.User;
    }
}