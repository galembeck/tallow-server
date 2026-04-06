using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IUserAddressRepository : IRepository<UserAddress>
{
    Task<List<UserAddress>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
