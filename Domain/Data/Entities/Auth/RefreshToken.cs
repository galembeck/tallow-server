using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBRefreshToken")]
public class RefreshToken : BaseEntity, IBaseEntity<RefreshToken>
{
    public string UserId { get; set; }
    public User User { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }



    #region .: HELPER METHODS :.

    public RefreshToken WithoutRelations(RefreshToken entity)
    {
        if (entity == null)
            return null;

        var newEntity = new RefreshToken()
        {
            UserId = entity.UserId,
            ExpiresAt = entity.ExpiresAt,
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }

    #endregion .: HELPER METHODS :.
}