using Domain.Data.Entities;
using Domain.Data.Models;
using Domain.Repository;
using Domain.SearchParameters;
using Domain.SearchParameters.UserHstoric;
using Domain.Services._Base;

namespace Domain.Services;

public abstract class IUserHistoricService : IService<UserHistoric, IUserHistoricRepository, UserSearchParameter>
{
    public IUserHistoricService(IUserHistoricRepository repository) : base(repository) { }

    public abstract Task<UserHistoric> GetUserHistoricAsync(string document, DateTime dtHistoricStart, DateTime dtHistoricEnd, CancellationToken cancellationToken = default);
    public abstract Task<UserHistoric> GetUserHistoricByIdAsync(string userHistoricId, CancellationToken cancellationToken = default);
    public abstract Task<UserHistoric> GetUserHistoricCurrentAsync(string document, CancellationToken cancellationToken = default);
    public abstract Task<UserHistoric> InsertUserHistoricAsync(UserHistoric entity);
    public abstract Task<string> GetUserHistoricByIdUserAsync(string idUser, CancellationToken cancellationToken = default);
    public abstract Task<PagedResult<UserHistoric>> GetAllUserHistoricAsync(UserHistoricSearchParameter searchParameter, CancellationToken cancellationToken = default);
}
