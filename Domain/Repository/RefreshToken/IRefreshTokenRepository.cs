using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken> GetByUserIdAsync(string id, CancellationToken cancellationToken = default);
}