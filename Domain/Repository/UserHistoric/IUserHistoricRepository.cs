using Domain.Data.Entities;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IUserHistoricRepository : IRepository<UserHistoric>
{
    Task<UserHistoric> GetUserHistoricAsync(string document, DateTime dtHistoricStart, DateTime dtHistoricEnd, CancellationToken cancellationToken = default);
    Task<UserHistoric> GetUserHistoricByIdAsync(string userHistoricId, CancellationToken cancellationToken = default);
    Task<UserHistoric> GetUserHistoricCurrentAsync(string document, CancellationToken cancellationToken = default);
    Task<List<UserHistoric>> GetAllUserHistoricAsync(string document, CancellationToken cancellationToken = default);
    Task<UserHistoric> InsertUserHistoricAsync(UserHistoric entity);
    Task<string> GetUserHistoricByIdUserAsync(string idUser, CancellationToken cancellationToken = default);
}
