using API.Public.DTOs._Base;
using Domain.Data.Entities;

namespace API.Public.DTOs;

public class AddressResponseDTO : PublicBaseDTO<UserAddress>
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

    public DateTimeOffset? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }



    public AddressResponseDTO() { }

    public AddressResponseDTO(UserAddress o) : base(o)
    {
        if (o == null) return;

        Id = o.Id;
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
        CreatedAt = o.CreatedAt;
        CreatedBy = o.CreatedBy;
        UpdatedAt = o.UpdatedAt;
        UpdatedBy = o.UpdatedBy;
    }

    public static AddressResponseDTO ModelToDTO(UserAddress o) => o == null ? null : new AddressResponseDTO(o);

    public static List<AddressResponseDTO> ModelToDTO(IEnumerable<UserAddress> addresses) => addresses.Select(address => new AddressResponseDTO(address)).ToList();
}
