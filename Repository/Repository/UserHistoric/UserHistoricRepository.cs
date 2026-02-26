using Domain.Constants;
using Domain.Data.Entities;
using Domain.Exceptions;
using Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Repository.Repository._Base;

namespace Repository.Repository;

public class UserHistoricRepository : BaseRepository<UserHistoric>, IUserHistoricRepository
{
    public UserHistoricRepository(AppDbContext context) : base(context, context.UserHistorics) { }

    public async Task<UserHistoric> GetUserHistoricAsync(string document, DateTime dtHistoricStart, DateTime dtHistoricEnd, CancellationToken cancellationToken = default)
    {
        UserHistoric response;

        try
        {
            response = await _entity
                .Where(x => x.Document == document && x.DateStart >= dtHistoricStart && x.DateEnd <= dtHistoricEnd)
                .AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<UserHistoric> GetUserHistoricByIdAsync(string userHistoricId, CancellationToken cancellationToken = default)
    {
        UserHistoric response;

        try
        {
            response = await _entity
                .Where(x => x.Id == userHistoricId)
                .AsNoTracking().FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<UserHistoric> GetUserHistoricCurrentAsync(string document, CancellationToken cancellationToken = default)
    {
        UserHistoric response;

        try
        {
            response = await _entity
                .Where(x => x.Document == document)
                .AsNoTracking().OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<List<UserHistoric>> GetAllUserHistoricAsync(string document, CancellationToken cancellationToken = default)
    {
        List<UserHistoric> response;

        try
        {
            response = await _entity
                .Where(x => x.Document == document)
                .OrderByDescending(x => x.DateStart)
                .AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<UserHistoric> InsertUserHistoricAsync(UserHistoric entity)
    {
        try
        {
            entity.Id = Guid.NewGuid().ToString();
            entity.CreatedAt = DateTimeOffset.UtcNow;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            entity.CreatedBy = entity.CreatedBy ?? Constant.Settings.SystemId;
            entity.UpdatedBy = entity.UpdatedBy ?? Constant.Settings.SystemId;

            _context.Entry(entity).State = EntityState.Added;
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }

        return entity;
    }

    public async Task<string> GetUserHistoricByIdUserAsync(string idUser, CancellationToken cancellationToken = default)
    {
        string response;

        try
        {
            response = await _entity
                .Where(x => x.IdUser == idUser)
                .AsNoTracking().OrderByDescending(x => x.CreatedAt).Select(x => x.Id).FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }
}