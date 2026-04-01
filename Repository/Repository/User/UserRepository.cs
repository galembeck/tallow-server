using UserEntity = Domain.Data.Entities.User;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class UserRepository : BaseRepository<UserEntity>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context, context.Users) { }

    public async Task<UserEntity> GetByDocumentAsync(string document, CancellationToken cancellationToken = default)
    {
        UserEntity response;

        try
        {
            response = await _entity
                .Where(x => x.Document == document && x.DeletedAt == null)
                .AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<UserEntity> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        UserEntity response;

        try
        {
            response = await _entity
                .Where(x => x.Email == email && x.DeletedAt == null)
                .AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }
        return response;
    }

    public async Task<UserEntity> GetByDocumentAndEmailAsync(string document, string email, CancellationToken cancellationToken = default)
    {
        UserEntity response;

        try
        {
            response = await _entity.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Document == document
                && x.Email == email
                && x.DeletedAt == null, cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<UserEntity> GetByDocumentPasswordAsync(string document, string password, CancellationToken cancellationToken = default)
    {
        UserEntity response;

        try
        {
            response = await _entity
                .Where(x =>
                    x.Document == document
                    && x.Password == password
                    && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<UserEntity> GetByEmailCellphoneAsync(string email, string cellphone, CancellationToken cancellationToken = default)
    {
        UserEntity response;

        try
        {
            response = await _entity
                .Where(x =>
                       (String.IsNullOrEmpty(email) || x.Email == email)
                    || (String.IsNullOrEmpty(cellphone) || x.Cellphone == cellphone)
                    && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<UserEntity> GetUserAsync(string id, CancellationToken cancellationToken = default)
    {
        UserEntity response;

        try
        {
            response = await _entity
                .Where(x => x.Id == id && x.DeletedAt == null)
                .AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<List<UserEntity>> GetAllUserAsync(CancellationToken cancellationToken = default)
    {
        List<UserEntity> response;

        try
        {
            response = await _entity
                .Where(x => x.DeletedAt == null)
                .AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    //public async Task<PagedResult<User>> GetAllUserQueryAsync(IQueryable<User> query, int page, int pageSize, bool isDesc, string orderBy, bool isOrdered, CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        query = query
    //            .Where(x => x.DeletedAt == null)
    //            .Include(x => x.ActiveInstallation);
    //    }
    //    catch (System.Exception e)
    //    {
    //        throw new PersistenceException(e);
    //    }

    //    return await PaginateAsync(query, page, pageSize, isDesc, orderBy, isOrdered, cancellationToken);
    //}

    public async Task<UserEntity> GetByDocumentAndTokenAsync(string document, string changeToken, CancellationToken cancellationToken = default)
    {
        UserEntity response;

        try
        {
            response = await _entity
                .Where(x => x.Document == document
                    && x.PasswordChangeToken == changeToken
                    && x.PasswordChangeTokenExpiresAt > DateTimeOffset.UtcNow
                    && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<UserEntity> GetByEmailAndTokenAsync(string email, string changeToken, CancellationToken cancellationToken = default)
    {
        UserEntity response;

        try
        {
            response = await _entity
                .Where(x => x.Email == email
                    && x.PasswordChangeToken == changeToken
                    && x.PasswordChangeTokenExpiresAt > DateTimeOffset.UtcNow
                    && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    //public async Task<PagedResult<ClientGrouped>> GetAllGroupedAsync(
    //BackOfficeClientSearchParameter sp,
    //CancellationToken cancellationToken = default)
    //{
    //    var measure = new Stopwatch();
    //    measure.Start();

    //    PagedResult<ClientGrouped> response = new PagedResult<ClientGrouped>();

    //    try
    //    {
    //        var query = _context.Set<ClientGrouped>().FromSqlRaw("SELECT * FROM VW_CLIENT_DATE");

    //        if (sp.InitialClientDate != null && sp.EndClientDate == null)
    //        {
    //            query = query.Where(x => x.ClientDate.Value.Date >= sp.InitialClientDate.Value.Date);
    //        }

    //        if (sp.InitialClientDate == null && sp.EndClientDate != null)
    //        {
    //            query = query.Where(x => x.ClientDate.Value.Date <= sp.EndClientDate.Value.Date);
    //        }

    //        if (sp.InitialClientDate != null && sp.EndClientDate != null)
    //        {
    //            query = query.Where(x => x.ClientDate.Value.Date >= sp.InitialClientDate.Value.Date
    //                                  && x.ClientDate.Value.Date <= sp.EndClientDate.Value.Date);
    //        }

    //        if (sp.CodEmpresa != null)
    //        {
    //            query = query.Where(x => x.DescriptionDistributorStatus == (int)sp.CodEmpresa);
    //        }

    //        query = query.OrderByDescending(x => x.ClientDate);

    //        if (sp.Page < 1)
    //        {
    //            throw new BusinessException(BusinessErrorMessage.INVALID_PAGE);
    //        }

    //        query = query.AsNoTracking();

    //        var rows = await query.ToListAsync(cancellationToken);

    //        measure.Stop();
    //        await _logger.Log(
    //            $@"Timer de consulta, no metodo {nameof(GetAllGroupedAsync)}, da classe {nameof(UserRepository)}: {measure.Elapsed} com uma consulta que trouxe {rows.Count} registros.",
    //            EnumLogType.APPLICATION);

    //        response.Rows = rows;
    //        return CreateAndOrderPagedResultByDate(response, sp.PageSize);
    //    }
    //    catch (System.Exception e)
    //    {
    //        throw new PersistenceException(e);
    //    }
    //}

    //private PagedResult<ClientGrouped> CreateAndOrderPagedResultByDate(PagedResult<ClientGrouped> pagedResult, int pageSize)
    //{
    //    var pageResult = new PagedResult<ClientGrouped>();
    //    var recordsKeyDatas = pagedResult.Rows.Select(x => x.CreatedAt).Distinct();
    //    foreach (var key in recordsKeyDatas)
    //    {
    //        var recordsClients = pagedResult.Rows.Where(x => x.CreatedAt == key).ToList();
    //        DateTime clientDate = DateTime.Parse(key, new CultureInfo("pt-BR"));
    //        pageResult.Rows.Add(new ClientGrouped { ClientDate = clientDate, TotalClients = recordsClients.Count });
    //        pageResult.RowCount++;
    //    }
    //    pageResult.PageCount = (int)Math.Ceiling((double)pageResult.RowCount / pageSize);
    
    //    return pageResult;
    //}

    //public async Task<List<ClientGrouped>> GetAllGroupedCSVAsync(BackOfficeClientSearchParameter sp, CancellationToken cancellationToken = default)
    //{
    //    List<ClientGrouped> response = new List<ClientGrouped>();

    //    try
    //    {
    //        var query = _context.Set<ClientGrouped>().FromSqlRaw("SELECT * FROM VW_CLIENT_DATE");
    //        if (sp.InitialClientDate != null && sp.EndClientDate == null) { query = query.Where(x => x.ClientDate.Value.Date >= sp.InitialClientDate.Value.Date); }
    //        if (sp.InitialClientDate == null && sp.EndClientDate != null) { query = query.Where(x => x.ClientDate.Value.Date <= sp.EndClientDate.Value.Date); }
    //        if (sp.InitialClientDate != null && sp.EndClientDate != null) { query = query.Where(x => x.ClientDate.Value.Date >= sp.InitialClientDate.Value.Date && x.ClientDate.Value.Date <= sp.EndClientDate.Value.Date); }
    //        // if (sp.BankId != null)                                                                   ClientDate                                                    ClientDate
    //        //     query = query.Where(x => x.BankIdContract == sp.BankId || x.BankIdPreContract == sp.BankId);
    //        response = await query.AsNoTracking().ToListAsync(cancellationToken);

    //        return response;
    //    }
    //    catch (System.Exception e)
    //    {
    //        throw new PersistenceException(e);
    //    }
    //}

    //public async Task<PagedResult<User>> GetUserPaginate(IQueryable<User> query, int page, int pageSize, bool isDesc, string orderBy, bool isOrdered, CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        query = query
    //           .Include(x => x.Contracts)
    //           .Include(x => x.PreContracts)
    //           .Include(x => x.ActiveInstallation.Distributor.DistributorDescription);


    //        return await PaginateAsync(query, page, pageSize, isDesc, orderBy, isOrdered, cancellationToken);
    //    }
    //    catch (System.Exception e)
    //    {
    //        throw new PersistenceException(e);
    //    }
    //}

//    public async Task<PagedResult<ClientLogins>> GetAllClientsLoginsAsync(
//ClientLoginsSearchParameter sp,
//CancellationToken cancellationToken = default)
//    {
//        var measure = new Stopwatch();
//        measure.Start();

//        PagedResult<ClientLogins> response = new PagedResult<ClientLogins>();

//        try
//        {
//            var query = _context.Set<ClientLogins>().FromSqlRaw("SELECT * FROM VW_ALL_CLIENT_LOGINS");

//            if (sp.InitialDate != null && sp.EndDate == null)
//            {
//                query = query.Where(x => x.Day.Value.Date >= sp.InitialDate.Value.Date);
//            }

//            if (sp.InitialDate == null && sp.EndDate != null)
//            {
//                query = query.Where(x => x.Day.Value.Date <= sp.EndDate.Value.Date);
//            }

//            if (sp.InitialDate != null && sp.EndDate != null)
//            {
//                query = query.Where(x => x.Day.Value.Date >= sp.InitialDate.Value.Date
//                                      && x.Day.Value.Date <= sp.EndDate.Value.Date);
//            }

//            query = query.OrderByDescending(x => x.Day);

//            if (sp.Page < 1)
//            {
//                throw new BusinessException(BusinessErrorMessage.INVALID_PAGE);
//            }

//            query = query.AsNoTracking();

//            response.RowCount = await query.CountAsync(cancellationToken);

//            measure.Stop();
//            await _logger.Log(
//                $@"Timer de consulta, no metodo {nameof(GetAllClientsLoginsAsync)}, da classe {nameof(UserRepository)}: {measure.Elapsed} com uma consulta que trouxe {response.RowCount} registros.",
//                EnumLogType.APPLICATION);

//            response.Rows = await query.Skip((sp.Page - 1) * sp.PageSize).Take(sp.PageSize).ToListAsync(cancellationToken);
//            response.PageCount = (int)Math.Ceiling((double)response.RowCount / sp.PageSize);

//            return response;
//        }
//        catch (System.Exception e)
//        {
//            throw new PersistenceException(e);
//        }
//    }
    
    //public async Task<List<ClientLogins>> GetAllClientsLoginsCSVAsync(ClientLoginsSearchParameter sp, CancellationToken cancellationToken = default)
    //{
    //    List<ClientLogins> response = new List<ClientLogins>();

    //    try
    //    {
    //        var query = _context.Set<ClientLogins>().FromSqlRaw("SELECT * FROM VW_ALL_CLIENT_LOGINS");
    //        if (sp.InitialDate != null && sp.EndDate == null) { query = query.Where(x => x.Day.Value.Date >= sp.InitialDate.Value.Date); }
    //        if (sp.InitialDate == null && sp.EndDate != null) { query = query.Where(x => x.Day.Value.Date <= sp.EndDate.Value.Date); }
    //        if (sp.InitialDate != null && sp.EndDate != null) { query = query.Where(x => x.Day.Value.Date >= sp.InitialDate.Value.Date && x.Day.Value.Date <= sp.EndDate.Value.Date); }

    //        response = await query.AsNoTracking().ToListAsync(cancellationToken);

    //        return response;
    //    }
    //    catch (System.Exception e)
    //    {
    //        throw new PersistenceException(e);
    //    }
    //}

    public async Task<string> GetByPrimaryDocument(string primaryDocument, CancellationToken cancellationToken = default)
    {
        string response;

        try
        {
            response = await _entity
                    .Where(x => x.Document == primaryDocument
                     && x.DeletedAt == null)
                    .Select(x => x.Id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<bool> GetByEmailOrCellphoneForOtherUserAsync(string currentUserId, string email, string cellphone, CancellationToken cancellationToken = default)
    {
        List<UserEntity> response;

        try
        {
            response = await _entity
                .Where(x =>
                 ((string.IsNullOrEmpty(email) || x.Email == email) ||
                 (string.IsNullOrEmpty(cellphone) || x.Cellphone == cellphone))
                 && x.Id != currentUserId
                 && x.DeletedAt == null)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        if (response.Count > 0)
            return true;

        return false;
    }

    public async Task<UserEntity> GetNameAndPrimaryDocumentAsync(string id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _entity
                .Where(x => x.Id == id && x.DeletedAt == null)
                .Select(x => new UserEntity
                {
                    Name = x.Name,
                    Document = x.Document
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            return response;
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    //public async Task<List<ClientPrimaryDocumentLogins>> GetUserByPrimaryDocumentAsync(string primaryDocument, CancellationToken cancellationToken = default)
    //{
    //    List<ClientPrimaryDocumentLogins> response;

    //    try
    //    {
    //        response = await _entity
    //            .Where(x => x.PrimaryDocument == primaryDocument && x.DeletedAt == null)
    //            .Select(x => new ClientPrimaryDocumentLogins
    //            {
    //                clientId = x.Id,
    //                userOrigin = x.UserOrigin,
    //            })
    //            .AsNoTracking().ToListAsync(cancellationToken);
    //    }
    //    catch (System.Exception e)
    //    {
    //        throw new PersistenceException(e);
    //    }

    //    return response;
    //}

    public async Task<List<UserEntity>> GetUsersByDocumentAsync(string document, CancellationToken cancellationToken = default)
    {
        List<UserEntity> response;

        try
        {
            response = await _entity
                .Where(x => x.Document == document && x.DeletedAt == null)
                .AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<List<UserEntity>> GetAllByProfileTypeAsync(ProfileType profileType, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => x.ProfileType == profileType && x.DeletedAt == null)
                .OrderByDescending(x => x.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }
    }
}
