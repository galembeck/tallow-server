using Domain.Data.Entities;
using Domain.Data.Models.DTOs;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class AccessTokenRepository : BaseRepository<AccessToken>, IAccessTokenRepository
{
    public AccessTokenRepository(AppDbContext context) : base(context, context.AccessTokens) { }

    public async Task<AccessTokenDTO> GetByToken(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _entity
                .Where(x => x.Id == token && x.DeletedAt == null)
                .Select(x => new AccessTokenDTO(x))
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            throw new PersistenceException(ex);
        }
    }
}
