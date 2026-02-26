using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using Domain.Enumerators;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;

namespace Domain.Data.Entities;

[Table("TBUserSecurityInfo")]
public class UserSecurityInfo : BaseEntity, IBaseEntity<UserSecurityInfo>
{
    public string Ip { get; set; } = string.Empty;
    public string MacAdress { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public SecurityInfoMoment? Moment { get; set; }
    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;



    [NotMapped]
    public IList<KeyValuePair<string, StringValues>>? Header { get; set; }



    #region Methods

    public UserSecurityInfo WithoutRelations(UserSecurityInfo entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var newEntity = new UserSecurityInfo()
        {
            UserId = entity.UserId,
            Ip = entity.Ip,
            MacAdress = entity.MacAdress,
            Browser = entity.Browser,
            Hash = entity.Hash,
            Moment = entity.Moment,
            User = null!
        };

        newEntity.InitializeInstance(entity);
        return newEntity;
    }

    #endregion Methods
}