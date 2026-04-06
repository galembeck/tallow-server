using Domain.Data.Entities;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Domain.Repository.User;
using Domain.Utils;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Domain.Services;

public class UserService(
    IUserRepository repository,
    IUserRepository userRepository,
    IUserSecurityInfoRepository userSecurityInfoRepository,
    IUserHistoricService userHistoricService,
    IFileStorageService fileStorageService,
    IBackgroundJobClient backgroundJobClient) : IUserService(repository)
{
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly IBackgroundJobClient _backgroundJobClient = backgroundJobClient;
    public override async Task<User> CreateAsync(User user, UserSecurityInfo securityInfo)
    {
        try
        {
            CheckAndSanitizeCellphone(user);

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var userSaved = await InsertClientAndCheckSecurityInfo(user, securityInfo);

            scope.Complete();

            _backgroundJobClient.Enqueue<IEmailService>(s =>
                s.SendWelcomeEmailAsync(userSaved.Name, userSaved.Email));

            return userSaved;
        }
        catch
        {
            throw;
        }
    }

    public override async Task<User> UpdateAsync(User user, string userId)
    {
        try
        {
            CheckAndSanitizeCellphone(user);

            var existingUser = await _Repository.GetUserAsync(userId);
            if (existingUser is null)
                throw new BusinessException(BusinessErrorMessage.USER_NOT_FOUND);

            if (!CellPhoneUtil.CheckDDDNumberIsValid(user.Cellphone))
                throw new BusinessException(BusinessErrorMessage.INVALID_DDD_NUMBER);

            var changes = VerifyUserChanges(existingUser, user);

            existingUser.Email = user.Email;
            existingUser.Cellphone = user.Cellphone;
            existingUser.UpdatedBy = existingUser.Id;

            var userSaved = await _Repository.UpdateAsync(existingUser, userId);

            await UpdateAndOrInsertUserHistoric(userSaved);
            return userSaved;
        }
        catch
        {
            throw;
        }
    }

    public override async Task<User> GetClientDetailAsync(string id, User user, CancellationToken cancellationToken = default)
    {
        var response = await _Repository.GetUserAsync(id, cancellationToken);

        if (response is null)
            throw new BusinessException(BusinessErrorMessage.USER_NOT_FOUND);

        if (user.ProfileType == ProfileType.ADMIN)
            return response;

        response.PasswordChangeToken = null;
        response.PasswordChangeTokenExpiresAt = null;

        return response;
    }

    public override async Task<User> GetUserAsync(string id, UserSecurityInfo securityInfo, CancellationToken cancellationToken = default)
    {
        var response = await _Repository.GetUserAsync(id, cancellationToken);

        if (response is null)
            throw new BusinessException(BusinessErrorMessage.USER_NOT_FOUND);

        response.LastAccessAt = DateTimeOffset.UtcNow;
        
        await _Repository.UpdateAsync(response);

        var loginUser = userSecurityInfoRepository.GetByUserId(id);

        if (loginUser.Count != 0)
            return response;

        securityInfo.UserId = id;
        securityInfo.Moment = SecurityInfoMoment.LOGIN;

        await userSecurityInfoRepository.InsertAsync(securityInfo.WithoutRelations(securityInfo));

        return response;
    }

    public override async Task<User> GetByDocumentAsync(string document, CancellationToken cancellationToken = default)
    {
        var response = await _Repository.GetByDocumentAsync(document, cancellationToken);
        return response;
    }

    public override async Task<User> GetByDocumentAndEmailAsync(string document, string email, CancellationToken cancellationToken = default)
    {
        var response = await _Repository.GetByDocumentAndEmailAsync(document, email, cancellationToken);
        return response;
    }

    public override async Task<bool> CheckIfPrimaryDocumentAlreadyExist(string primaryDocument, CancellationToken cancellationToken = default)
    {
        try
        {
            User user = await userRepository.GetByDocumentAsync(primaryDocument, cancellationToken);

            if (user is not null)
                return true;

            return false;
        }
        catch
        {
            throw;
        }
    }

    public override async Task<List<User>> GetUsersByEmail(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _Repository.GetByExpression(
                                          x => x.Email == email
                                          && x.DeletedAt == null)
                                         .ToListAsync(cancellationToken);

            return users;
        }
        catch (Exception ex)
        {
            throw new PersistenceException(ex);
        }
    }



    public override async Task<User> UpdateProfileAsync(
        string userId,
        string? name,
        string? email,
        string? cellphone,
        string? document,
        string? password,
        bool? receiveEmailOffers,
        bool? receiveWhatsappOffers,
        IFormFile? avatar,
        CancellationToken cancellationToken = default)
    {
        string? newAvatarPath = null;
        string? newAvatarUrl = null;

        if (avatar != null)
        {
            using var stream = avatar.OpenReadStream();
            newAvatarPath = await _fileStorageService.UploadFileAsync(stream, avatar.FileName, "avatars");
            newAvatarUrl = _fileStorageService.GetFileUrl(newAvatarPath);
        }

        var hashedPassword = !string.IsNullOrWhiteSpace(password)
            ? StringUtil.SHA512(password)
            : null;

        return await _Repository.UpdatePartialAsync(
            new User { Id = userId },
            user =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                    user.Name = name;
                if (!string.IsNullOrWhiteSpace(email))
                    user.Email = email;
                if (!string.IsNullOrWhiteSpace(cellphone))
                    user.Cellphone = cellphone;
                if (!string.IsNullOrWhiteSpace(document))
                    user.Document = document;
                if (hashedPassword != null)
                    user.Password = hashedPassword;
                if (receiveEmailOffers.HasValue)
                    user.ReceiveEmailOffers = receiveEmailOffers;
                if (receiveWhatsappOffers.HasValue)
                    user.ReceiveWhatsappOffers = receiveWhatsappOffers;
                if (newAvatarPath != null)
                {
                    user.AvatarPath = newAvatarPath;
                    user.AvatarUrl = newAvatarUrl;
                }
            },
            userId);
    }

    public override async Task<List<User>> GetAllByProfileTypeAsync(ProfileType profileType, CancellationToken cancellationToken = default)
    {
        return await _Repository.GetAllByProfileTypeAsync(profileType, cancellationToken);
    }

    public override async Task<User> GetUserDetailByAdminAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _Repository.GetUserAsync(id, cancellationToken);

        if (user is null)
            throw new BusinessException(BusinessErrorMessage.USER_NOT_FOUND);

        return user;
    }

    public override async Task<User> UpdateUserByAdminAsync(string id, string? name, string? email, CancellationToken cancellationToken = default)
    {
        return await _Repository.UpdatePartialAsync(
            new User { Id = id },
            user =>
            {
                if (!string.IsNullOrWhiteSpace(name))
                    user.Name = name;
                if (!string.IsNullOrWhiteSpace(email))
                    user.Email = email;
            });
    }

    public override async Task<User> ChangeUserProfileTypeAsync(string id, ProfileType newProfileType, CancellationToken cancellationToken = default)
    {
        return await _Repository.UpdatePartialAsync(
            new User { Id = id },
            user => user.ProfileType = newProfileType);
    }

    #region .: HELPER METHODS :.
    private void CheckAndSanitizeCellphone(User user)
    {
        if (user.Cellphone.Substring(2, 1) != "9")
            throw new System.Exception("Número do celular após o DDD precisa iniciar com 9");
        user.Cellphone = user.Cellphone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
    }

    private async Task<User> InsertClientAndCheckSecurityInfo(User user, UserSecurityInfo securityInfo)
    {
        user.Password = StringUtil.SHA512(user.Password);
        user.LastAccessAt = DateTimeOffset.UtcNow;

        var userSaved = await InsertAndGetNewClient(user);

        await SaveSecurityInformation(securityInfo, userSaved);
        return userSaved;
    }

    private async Task<User> InsertAndGetNewClient(User user)
    {
        try
        {
            return await _Repository.InsertAsync(user.WithoutRelations(user));
        }
        catch (Exception ex)
        {
            if (ex.InnerException!.InnerException!.Message.ToLower().Contains("O dcumento já está cadastrado!"))
                return null;

            throw;
        }
    }

    private async Task SaveSecurityInformation(UserSecurityInfo securityInfo, User userSaved)
    {
        securityInfo.UserId = userSaved.Id;
        securityInfo.Moment = SecurityInfoMoment.REGISTER;

        await userSecurityInfoRepository.InsertAsync(securityInfo.WithoutRelations(securityInfo));
    }

    private string VerifyUserChanges(User existingUser, User user)
    {
        string changes = string.Empty;

        if (!string.IsNullOrEmpty(user?.Name) && existingUser.Name != user.Name)
            changes += GenerateChangesString(" Nome");

        if (!string.IsNullOrEmpty(user?.Email) && existingUser.Email != user.Email)
            changes += GenerateChangesString(" Email");

        if (!string.IsNullOrEmpty(user?.Cellphone) && existingUser.Cellphone != user.Cellphone)
            changes += GenerateChangesString(" Celular");

        if (!string.IsNullOrEmpty(changes))
            changes = changes.Substring(1, changes.Length - 2);
        else
            changes = "Sem alteração";

        return changes;
    }

    private string GenerateChangesString(string property)
    {
        return property += " |";
    }

    private async Task UpdateAndOrInsertUserHistoric(User user)
    {
        var userHistoricCurrent = await userHistoricService.GetUserHistoricCurrentAsync(user.Document);

        var userHistoric = new UserHistoric
        {
            IdUser = user.Id,
            DateStart = DateTime.UtcNow.AddMinutes(1),
            DateEnd = null,
            Name = user.Name,
            Email = user.Email,
            Cellphone = user.Cellphone,
            Document = user.Document,
            LastAccessAt = user.LastAccessAt,
            ProfileType = user.ProfileType,
            ReceiveWhatsappOffers = user.ReceiveWhatsappOffers,
            ReceiveEmailOffers = user.ReceiveEmailOffers,
            CreatedBy = user.CreatedBy,
            UpdatedBy = user.UpdatedBy,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            DeletedAt = user.DeletedAt,
            Password = user.Password,
            PasswordChangeToken = user.PasswordChangeToken,
            PasswordChangeTokenExpiresAt = user.PasswordChangeTokenExpiresAt,
        };

        if (userHistoricCurrent is null)
            await userHistoricService.InsertUserHistoricAsync(userHistoric);
        else
        {
            userHistoricCurrent.DateEnd = DateTime.UtcNow;

            await userHistoricService.Update(userHistoricCurrent, userHistoricCurrent.UpdatedBy);
            await userHistoricService.InsertUserHistoricAsync(userHistoric);
        }
    }

    #endregion .: HELPER METHODS :.
}
