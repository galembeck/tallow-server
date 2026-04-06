using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class UserAddressRepository : BaseRepository<UserAddress>, IUserAddressRepository
{
    public UserAddressRepository(AppDbContext context) : base(context, context.UserAddresses) { }

    public async Task<List<UserAddress>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Set<UserAddress>()
                .Where(w => w.UserId == userId && w.DeletedAt == null)
                .OrderByDescending(w => w.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
