using UserEntity = Domain.Data.Entities.User;
using Domain.Enumerators;
using Domain.Repository._Base;

namespace Domain.Repository;

public interface IUserRepository : IRepository<UserEntity>
{
    Task<UserEntity> GetByDocumentAsync(string document, CancellationToken cancellationToken = default);
    Task<UserEntity> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserEntity> GetByDocumentAndEmailAsync(string document, string email, CancellationToken cancellationToken = default);
    Task<UserEntity> GetByDocumentPasswordAsync(string document, string password, CancellationToken cancellationToken = default);
    Task<UserEntity> GetByEmailCellphoneAsync(string email, string cellphone, CancellationToken cancellationToken = default);
    Task<UserEntity> GetUserAsync(string id, CancellationToken cancellationToken = default);
    Task<List<UserEntity>> GetAllUserAsync(CancellationToken cancellationToken = default);
    Task<List<UserEntity>> GetAllByProfileTypeAsync(ProfileType profileType, CancellationToken cancellationToken = default);

    //Task<PagedResult<User>> GetAllUserQueryAsync(IQueryable<User> query, int page, int pageSize, bool isDesc, string orderBy, bool isOrdered, CancellationToken cancellationToken = default);
    //Task<PagedResult<ClientGrouped>> GetAllGroupedAsync(BackOfficeClientSearchParameter sp, CancellationToken cancellationToken = default);
    //Task<PagedResult<ClientLogins>> GetAllClientsLoginsAsync(ClientLoginsSearchParameter sp, CancellationToken cancellationToken = default);
    //Task<List<ClientLogins>> GetAllClientsLoginsCSVAsync(ClientLoginsSearchParameter sp, CancellationToken cancellationToken = default);

    Task<UserEntity> GetByDocumentAndTokenAsync(string document, string changeToken, CancellationToken cancellationToken = default);
    Task<UserEntity> GetByEmailAndTokenAsync(string email, string changeToken, CancellationToken cancellationToken = default);

    //Task<PagedResult<User>> GetUserPaginate(IQueryable<User> query, int page, int pageSize, bool isDesc, string orderBy, bool isOrdered, CancellationToken cancellationToken = default);

    Task<string> GetByPrimaryDocument(string primaryDocument, CancellationToken cancellationToken = default);
    Task<bool> GetByEmailOrCellphoneForOtherUserAsync(string currentUserId, string email, string cellphone, CancellationToken cancellationToken = default);
    Task<UserEntity> GetNameAndPrimaryDocumentAsync(string id, CancellationToken cancellationToken = default);

    //Task<List<ClientPrimaryDocumentLogins>> GetUserByPrimaryDocumentAsync(string primaryDocument, CancellationToken cancellationToken = default);

    Task<List<UserEntity>> GetUsersByDocumentAsync(string document, CancellationToken cancellationToken = default);
}
