using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using Domain.Enumerators;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Xml.Linq;
using UAParser;

namespace Domain.Data.Entities;

[Table("TBUserHistoric")]
public class UserHistoric : BaseEntity, IBaseEntity<UserHistoric>
{
    public string IdUser { get; set; }



    public DateTime DateStart { get; set; }
    public DateTime? DateEnd { get; set; }



    public string Name { get; set; }
    public string Email { get; set; }
    public string Cellphone { get; set; }
    public string Document { get; set; }
    public ProfileType? ProfileType { get; set; }

    
    
    public string Password { get; set; }



    public bool? ReceiveWhatsappOffers { get; set; }
    public bool? ReceiveEmailOffers { get; set; }



    public DateTimeOffset? LastAccessAt { get; set; }
    public string? PasswordChangeToken { get; set; }
    public DateTimeOffset? PasswordChangeTokenExpiresAt { get; set; }

    [NotMapped]
    public string HashId { get; set; }

    public string UpdatedColumn { get; set; }



    #region .: METHODS :.

    public UserHistoric WithoutRelations(UserHistoric entity)
    {
        if (entity == null)
            return null;

        var newEntity = new UserHistoric()
        {
            IdUser = entity.IdUser,
            DateStart = entity.DateStart,
            DateEnd = entity.DateEnd,
            Name = entity.Name,
            Email = entity.Email,
            Cellphone = entity.Cellphone,
            Document = entity.Document,
            Password = entity.Password,
            ProfileType = entity.ProfileType,
            ReceiveWhatsappOffers = entity.ReceiveWhatsappOffers,
            ReceiveEmailOffers = entity.ReceiveEmailOffers,
            LastAccessAt = entity.LastAccessAt,
            PasswordChangeToken = entity.PasswordChangeToken,
            PasswordChangeTokenExpiresAt = entity.PasswordChangeTokenExpiresAt,
            UpdatedColumn = entity.UpdatedColumn,
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }
    #endregion
}
