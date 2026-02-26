using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository.User;

public interface IUserSecurityInfoRepository : IRepository<UserSecurityInfo>
{
    List<UserSecurityInfo> GetByUserId(string id);
}
