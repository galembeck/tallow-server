using Domain.Data.Entities;
using Domain.Repository;
using Domain.SearchParameters;
using Domain.Services._Base;

namespace Domain.Services;

public abstract class IUserAddressService : IService<UserAddress, IUserAddressRepository, UserSearchParameter>
{
    public IUserAddressService(IUserAddressRepository repository) : base(repository) { }

    public abstract Task<UserAddress> CreateAsync(UserAddress userAddress, string actorId, CancellationToken cancellationToken = default);
    public abstract Task<UserAddress> UpdateAsync(string addressId, string? addressTitle, string? receiverName, string? receiverLastname, string? contactCellphone, string? zipcode, string? address, string? number, string? complement, string? neighborhood, string? city, string? state, CancellationToken cancellationToken = default);
    public abstract Task<List<UserAddress>> GetUserAddressesAsync(string userId, CancellationToken cancellationToken = default);
}
