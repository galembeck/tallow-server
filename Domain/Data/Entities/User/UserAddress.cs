using Domain.Data.Entities._Base;
using Domain.Data.Entities._Base.Extension;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Data.Entities;

[Table("TBUserAddress")]
public class UserAddress : BaseEntity, IBaseEntity<UserAddress>
{
    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public User User { get; set; }

    public string AddressTitle { get; set; }

    public string ReceiverName { get; set; }
    public string ReceiverLastname { get; set; }
    public string ContactCellphone { get; set; }

    public string Zipcode { get; set; }
    public string Address { get; set; }
    public string Number { get; set; }
    public string? Complement { get; set; }
    public string Neighborhood { get; set; }
    public string City { get; set; }
    public string State { get; set; }



    #region .: METHODS :.

    public UserAddress WithoutRelations(UserAddress entity)
    {
        if (entity == null)
            return null;

        var newEntity = new UserAddress()
        {
            UserId = entity.UserId,
            AddressTitle = entity.AddressTitle,
            ReceiverName = entity.ReceiverName,
            ReceiverLastname = entity.ReceiverLastname,
            ContactCellphone = entity.ContactCellphone,
            Zipcode = entity.Zipcode,
            Address = entity.Address,
            Number = entity.Number,
            Complement = entity.Complement,
            Neighborhood = entity.Neighborhood,
            City = entity.City,
            State = entity.State,
        };

        newEntity.InitializeInstance(entity);

        return newEntity;
    }

    #endregion .: METHODS :.
}
