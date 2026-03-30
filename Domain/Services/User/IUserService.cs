using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Repository;
using Domain.SearchParameters;
using Domain.Services._Base;

namespace Domain.Services;

public abstract class IUserService : IService<User, IUserRepository, UserSearchParameter>
{
    public IUserService(IUserRepository repository) : base(repository) { }

    public abstract Task<User> CreateAsync(User user, UserSecurityInfo securityInfo);
    public abstract Task<User> UpdateAsync(User user, string userId);
    public abstract Task<User> GetClientDetailAsync(string id, User user, CancellationToken cancellationToken = default);
    public abstract Task<User> GetUserAsync(string id, UserSecurityInfo securityInfo, CancellationToken cancellationToken = default);
    public abstract Task<User> GetByDocumentAsync(string document, CancellationToken cancellationToken = default);
    public abstract Task<User> GetByDocumentAndEmailAsync(string document, string email, CancellationToken cancellationToken = default);
    public abstract Task<bool> CheckIfPrimaryDocumentAlreadyExist(string primaryDocument, CancellationToken cancellationToken = default);
    public abstract Task<List<User>> GetUsersByEmail(string email, CancellationToken cancellationToken = default);

    // Admin management
    public abstract Task<List<User>> GetAllByProfileTypeAsync(ProfileType profileType, CancellationToken cancellationToken = default);
    public abstract Task<User> GetUserDetailByAdminAsync(string id, CancellationToken cancellationToken = default);
    public abstract Task<User> UpdateUserByAdminAsync(string id, string? name, string? email, CancellationToken cancellationToken = default);
    public abstract Task<User> ChangeUserProfileTypeAsync(string id, ProfileType newProfileType, CancellationToken cancellationToken = default);
}
