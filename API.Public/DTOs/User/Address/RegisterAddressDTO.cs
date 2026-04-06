using API.Public.DTOs._Base;
using Domain.Data.Entities;

namespace API.Public.DTOs;

public class RegisterAddressDTO : PublicBaseDTO<UserAddress>
{
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

    public RegisterAddressDTO() { }

    public RegisterAddressDTO(UserAddress o) : base(o)
    {
        if (o == null) return;

        AddressTitle = o.AddressTitle;
        ReceiverName = o.ReceiverName;
        ReceiverLastname = o.ReceiverLastname;
        ContactCellphone = o.ContactCellphone;
        Zipcode = o.Zipcode;
        Address = o.Address;
        Number = o.Number;
        Complement = o.Complement;
        Neighborhood = o.Neighborhood;
        City = o.City;
        State = o.State;
    }

    public static RegisterAddressDTO ModelToDTO(UserAddress o) => o == null ? null : new RegisterAddressDTO(o);

    public static List<RegisterAddressDTO> ModelToDTO(IEnumerable<UserAddress> addresses) => addresses.Select(address => new RegisterAddressDTO(address)).ToList();

    public static UserAddress DTOToModel(RegisterAddressDTO o)
    {
        if (o == null) return null;

        var model = new UserAddress()
        {
            AddressTitle = o.AddressTitle,
            ReceiverName = o.ReceiverName,
            ReceiverLastname = o.ReceiverLastname,
            ContactCellphone = o.ContactCellphone,
            Zipcode = o.Zipcode,
            Address = o.Address,
            Number = o.Number,
            Complement = o.Complement,
            Neighborhood = o.Neighborhood,
            City = o.City,
            State = o.State,
        };

        return o.InitializeInstance(model);
    }
}
