using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;

namespace Domain.Services;

public class UserAddressService(
    IUserAddressRepository repository) : IUserAddressService(repository)
{
    public override async Task<UserAddress> CreateAsync(UserAddress userAddress, string actorId, CancellationToken cancellationToken = default)
    {
        userAddress.UserId = actorId;

        var cleanedEntity = userAddress.WithoutRelations(userAddress);

        var addressSaved = await _Repository.InsertAsync(cleanedEntity, actorId);

        return addressSaved;
    }

    public override async Task<UserAddress> UpdateAsync(
        string addressId,
        string? addressTitle,
        string? receiverName,
        string? receiverLastname,
        string? contactCellphone,
        string? zipcode,
        string? address,
        string? number,
        string? complement,
        string? neighborhood,
        string? city,
        string? state,
        CancellationToken cancellationToken = default)
    {
        return await _Repository.UpdatePartialAsync(
            new UserAddress { Id = addressId },
            userAddress =>
            {
                if (!string.IsNullOrWhiteSpace(addressTitle))
                    userAddress.AddressTitle = addressTitle;
                if (!string.IsNullOrWhiteSpace(receiverName))
                    userAddress.ReceiverName = receiverName;
                if (!string.IsNullOrWhiteSpace(receiverLastname))
                    userAddress.ReceiverLastname = receiverLastname;
                if (!string.IsNullOrWhiteSpace(contactCellphone))
                    userAddress.ContactCellphone = contactCellphone;
                if (!string.IsNullOrWhiteSpace(zipcode))
                    userAddress.Zipcode = zipcode;
                if (!string.IsNullOrWhiteSpace(address))
                    userAddress.Address = address;
                if (!string.IsNullOrWhiteSpace(number))
                    userAddress.Number = number;
                if (!string.IsNullOrWhiteSpace(complement))
                    userAddress.Complement = complement;
                if (!string.IsNullOrWhiteSpace(neighborhood))
                    userAddress.Neighborhood = neighborhood;
                if (!string.IsNullOrWhiteSpace(city))
                    userAddress.City = city;
                if (!string.IsNullOrWhiteSpace(state))
                    userAddress.State = state;
            },
            addressId);
    }

    public override async Task<List<UserAddress>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _Repository.GetByUserIdAsync(userId, cancellationToken);
    }
}
