using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository.User;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository.User;

public class UserSecurityInfoRepository : BaseRepository<UserSecurityInfo>, IUserSecurityInfoRepository
{
    public UserSecurityInfoRepository(AppDbContext context) : base(context, context.UserSecurityInfos) { }

    public List<UserSecurityInfo> GetByUserId(string id)
    {
        try
        {
            var response = _entity
            .Where(x => x.DeletedAt == null && x.UserId == id)
            .Where(x => x.CreatedAt.Date == DateTime.Today.Date &&
            x.Moment == SecurityInfoMoment.LOGIN)
            .AsNoTracking().ToList();

            return response;
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
