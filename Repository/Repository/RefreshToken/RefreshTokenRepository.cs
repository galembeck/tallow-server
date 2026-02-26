using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context, context.RefreshTokens) { }

    public async Task<RefreshToken> GetByUserIdAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.UserId == id && x.DeletedAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}