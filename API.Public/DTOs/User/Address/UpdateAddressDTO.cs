using API.Public.DTOs._Base;
using Domain.Data.Entities;

namespace API.Public.DTOs;

public class UpdateAddressDTO : PublicBaseDTO<UserAddress>
{
    public string? AddressTitle { get; set; }

    public string? ReceiverName { get; set; }
    public string? ReceiverLastname { get; set; }
    public string? ContactCellphone { get; set; }

    public string? Zipcode { get; set; }
    public string? Address { get; set; }
    public string? Number { get; set; }
    public string? Complement { get; set; }
    public string? Neighborhood { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }



    public UpdateAddressDTO() { }

    public static UserAddress ApplyToModel(UpdateAddressDTO dto, UserAddress existing)
    {
        if (dto.AddressTitle != null) existing.AddressTitle = dto.AddressTitle;
        if (dto.ReceiverName != null) existing.ReceiverName = dto.ReceiverName;
        if (dto.ReceiverLastname != null) existing.ReceiverLastname = dto.ReceiverLastname;
        if (dto.ContactCellphone != null) existing.ContactCellphone = dto.ContactCellphone;
        if (dto.Zipcode != null) existing.Zipcode = dto.Zipcode;
        if (dto.Address != null) existing.Address = dto.Address;
        if (dto.Number != null) existing.Number = dto.Number;
        if (dto.Complement != null) existing.Complement = dto.Complement;
        if (dto.Neighborhood != null) existing.Neighborhood = dto.Neighborhood;
        if (dto.City != null) existing.City = dto.City;
        if (dto.State != null) existing.State = dto.State;

        return existing;
    }
}
