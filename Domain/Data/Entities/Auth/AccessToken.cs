using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBAccessToken")]
public class AccessToken : BaseEntity, IBaseEntity<AccessToken>
{
    public string UserId { get; set; }
    public User User { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }



    #region .: HELPER METHODS :.

    public AccessToken WithoutRelations(AccessToken entity)
    {
        if (entity == null)
            return null;

        var newEntity = new AccessToken()
        {
            UserId = entity.UserId,
            ExpiresAt = entity.ExpiresAt,
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }

    #endregion .: HELPER MTHODS :.
}