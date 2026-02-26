using Domain.Data.Entities;
using Domain.Data.Models;
using Domain.Repository;
using Domain.SearchParameters.UserHstoric;

namespace Domain.Services;

public class UserHistoricService : IUserHistoricService
{
    private readonly IUserHistoricRepository _userHistoricRepository;
    private readonly IUserRepository _userRepository;

    public UserHistoricService(IUserHistoricRepository userHistoricRepository,
        IUserRepository userRepository) : base(userHistoricRepository)
    {
        _userHistoricRepository = userHistoricRepository;
        _userRepository = userRepository;
    }

    public override async Task<UserHistoric> GetUserHistoricAsync(string document, DateTime dtHistoricStart, DateTime dtHistoricEnd, CancellationToken cancellationToken = default)
    {
        var response = await _Repository.GetUserHistoricAsync(document, dtHistoricStart, dtHistoricEnd, cancellationToken);
        
        return response;
    }

    public override async Task<UserHistoric> GetUserHistoricByIdAsync(string userHistoricId, CancellationToken cancellationToken = default)
    {
        var response = await _Repository.GetUserHistoricByIdAsync(userHistoricId, cancellationToken);

        return response;
    }

    public override async Task<UserHistoric> GetUserHistoricCurrentAsync(string document, CancellationToken cancellationToken = default)
    {
        var response = await _Repository.GetUserHistoricCurrentAsync(document, cancellationToken);
     
        return response;
    }

    public override async Task<PagedResult<UserHistoric>> GetAllUserHistoricAsync(UserHistoricSearchParameter searchParameter, CancellationToken cancellationToken = default)
    {
        Boolean isOrdered = true;
        IQueryable<UserHistoric> query = _Repository.GetByExpression(
            x => x.Document == searchParameter.Document);

        query = query.OrderByDescending(x => x.DateStart);

        var userHistoricPaginated = await _Repository.PaginateAsync(query, searchParameter.Page, searchParameter.PageSize, searchParameter.IsDesc, searchParameter.OrderBy, isOrdered);

        if (userHistoricPaginated?.Rows != null)
            foreach (var historic in userHistoricPaginated.Rows)
            {
                historic.DateStart = historic.DateStart.AddHours(-3);
                var user = await _userRepository.GetUserAsync(historic.UpdatedBy, cancellationToken);

                if (user is not null)
                    historic.UpdatedBy = user.Email;
            }

        return userHistoricPaginated;
    }

    public override async Task<UserHistoric> InsertUserHistoricAsync(UserHistoric entity)
    {
        var response = await _Repository.InsertUserHistoricAsync(entity);
        
        return response;
    }

    public override async Task<string> GetUserHistoricByIdUserAsync(string idUser, CancellationToken cancellationToken = default)
    {
        var response = await _Repository.GetUserHistoricByIdUserAsync(idUser, cancellationToken);

        return response;
    }
}
