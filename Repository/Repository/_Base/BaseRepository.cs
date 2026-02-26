using Domain.Constants;
using Domain.Data.Entities._Base;
using Domain.Data.Models;
using Domain.Enumerators;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Repository.Repository._Base;

public abstract class BaseRepository<E> where E : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<E> _entity;

    protected BaseRepository(AppDbContext dbContext, DbSet<E> dbEntity)
    {
        _context = dbContext;
        _entity = dbEntity;
    }

    public async Task<E> GetAsync(string id, CancellationToken cancellationToken = default)
    {
        E? response;

        try
        {
            response = await _entity
                .Where(x => x.Id == id && x.DeletedAt == null)
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        if (response is null)
            throw new PersistenceException($"Entity with ID {id} not found.");

        return response;
    }

    public async Task<E> InsertAsync(E entity, string? actorId = null)
    {
        try
        {
            entity.Id = string.IsNullOrWhiteSpace(entity.Id) ? Guid.NewGuid().ToString() : entity.Id;
            entity.CreatedAt = DateTimeOffset.UtcNow;
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            entity.CreatedBy = actorId ?? Constant.Settings.SystemId;
            entity.UpdatedBy = actorId ?? Constant.Settings.SystemId;

            await _entity.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return entity;
    }

    public async Task<E> UpdateAsync(E entry, string? actorId = null)
    {
        try
        {
            var dbentry = await _entity.Where(e => e.Id == entry.Id).FirstOrDefaultAsync();

            if (dbentry == null)
                throw new PersistenceException($"Entity with ID {entry.Id} not found.");

            _context.Entry(dbentry).CurrentValues.SetValues(entry);

            _context.Entry(dbentry).Property(x => x.CreatedAt).IsModified = false;
            _context.Entry(dbentry).Property(x => x.CreatedBy).IsModified = false;
            _context.Entry(dbentry).Property(x => x.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
            _context.Entry(dbentry).Property(x => x.UpdatedBy).CurrentValue = actorId ?? Constant.Settings.SystemId;

            await _context.SaveChangesAsync();
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return entry;
    }

    public async Task<E> DeleteAsync(E entity, string? actorId = null)
    {
        try
        {
            _context.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
            _context.Entry(entity).Property(x => x.CreatedAt).IsModified = false;
            _context.Entry(entity).Property(x => x.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
            _context.Entry(entity).Property(x => x.DeletedAt).CurrentValue = DateTimeOffset.UtcNow;
            _context.Entry(entity).Property(x => x.UpdatedBy).CurrentValue = actorId ?? Constant.Settings.SystemId;

            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return entity;
    }

    public async Task HardDeleteAsync(E entity)
    {
        try
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<IEnumerable<E>> GetByIdListAsync(IEnumerable<String> idList, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _entity
                .Where(x => idList.Contains(x.Id) && x.DeletedAt == null)
                .AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<IEnumerable<E>> InsertListAsync(IEnumerable<E> entityList, string? actorId = null)
    {
        try
        {
            foreach (E entity in entityList)
            {
                entity.Id = Guid.NewGuid().ToString();
                entity.CreatedAt = DateTimeOffset.UtcNow;
                entity.UpdatedAt = DateTimeOffset.UtcNow;
                entity.CreatedBy = actorId ?? Constant.Settings.SystemId;
                entity.UpdatedBy = actorId ?? Constant.Settings.SystemId;
            }

            await _context.AddRangeAsync(entityList);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return entityList;
    }

    public async Task<IEnumerable<E>> UpdateListAsync(IEnumerable<E> entityList, string? actorId = null)
    {
        try
        {
            List<String> idStringList = entityList.Select(m => m.Id).ToList();

            IList<E> existingList = await _entity
                .Where(x => idStringList.Contains(x.Id)
                            && x.DeletedAt == null)
                .ToListAsync();

            DateTimeOffset createdAt;
            string createdBy;
            E matchingEntity;

            foreach (E existing in existingList)
            {
                matchingEntity = entityList.Where(ex => ex.Id == existing.Id).FirstOrDefault();

                if (matchingEntity != null)
                {
                    createdAt = existing.CreatedAt;
                    createdBy = existing.CreatedBy;

                    _context.Entry(existing).CurrentValues.SetValues(matchingEntity);

                    existing.CreatedAt = createdAt;
                    existing.CreatedBy = createdBy;
                    existing.UpdatedAt = DateTimeOffset.UtcNow;
                    existing.UpdatedBy = actorId ?? Constant.Settings.SystemId;
                }
            }

            await _context.SaveChangesAsync();

            return existingList;
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<IEnumerable<E>> DeleteListAsync(IEnumerable<String> idList, string? actorId = null)
    {
        try
        {
            IList<E> existingList = await _entity
                .Where(x => idList.Contains(x.Id)
                            && x.DeletedAt == null)
                .ToListAsync();

            foreach (E existing in existingList)
            {
                _context.Entry(existing).Property(x => x.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
                _context.Entry(existing).Property(x => x.DeletedAt).CurrentValue = DateTimeOffset.UtcNow;
                _context.Entry(existing).Property(x => x.UpdatedBy).CurrentValue =
                    actorId ?? Constant.Settings.SystemId;
            }

            await _context.SaveChangesAsync();
            return existingList;
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    public async Task<IEnumerable<E>> HardDeleteListAsync(IEnumerable<String> idList)
    {
        IEnumerable<E> response;

        try
        {
            response = await _entity.Where(x => idList.Contains(x.Id)).ToListAsync();
            _context.RemoveRange(response);
            await _context.SaveChangesAsync();
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<IEnumerable<E>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        IEnumerable<E> response;

        try
        {
            response = await _entity
                .Where(e => e.DeletedAt == null)
                .AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public IQueryable<E> GetByExpression(Expression<Func<E, bool>> expression)
    {
        IQueryable<E> response;

        try
        {
            response = _entity.Where(expression)
                .Where(e => e.DeletedAt == null)
                .AsNoTracking();
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public IQueryable<E> GetDeletionByExpression(Expression<Func<E, bool>> expression)
    {
        IQueryable<E> response;

        try
        {
            response = _entity.Where(expression)
                .Where(e => e.DeletedAt.HasValue)
                .AsNoTracking();
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<PagedResult<E>> GetPagedAsync(IQueryable<E> query, Int32 page, Int32 pageSize, CancellationToken cancellationToken = default)
    {
        PagedResult<E> result = new PagedResult<E>();

        result.RowCount = await query.CountAsync(cancellationToken);
        result.Rows = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        result.PageCount = (int)Math.Ceiling((double)result.RowCount / pageSize);

        return result;
    }

    public async Task<PagedResult<E>> PaginateAsync(IQueryable<E> query, Int32 page, Int32 pageSize, Boolean isDesc, string orderBy, Boolean isOrdered = false, CancellationToken cancellationToken = default)
    {
        var response = new PagedResult<E>();

        if (!isOrdered)
        {
            query = (orderBy.ToUpper()) switch
            {
                "ID" => isDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
                "CREATEDBY" => isDesc ? query.OrderByDescending(x => x.CreatedBy) : query.OrderBy(x => x.CreatedBy),
                "CREATEDAT" => isDesc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
                "UPDATEDBY" => isDesc ? query.OrderByDescending(x => x.UpdatedBy) : query.OrderBy(x => x.UpdatedBy),
                "UPDATEDAT" => isDesc ? query.OrderByDescending(x => x.UpdatedAt) : query.OrderBy(x => x.UpdatedAt),
                "DELETEDAT" => isDesc ? query.OrderByDescending(x => x.DeletedAt) : query.OrderBy(x => x.DeletedAt),
                _ => isDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
            };
        }

        if (page < 1) { throw new BusinessException(BusinessErrorMessage.INVALID_PAGE); }

        try
        {
            query = query.AsNoTracking();
            response.RowCount = await query.CountAsync(cancellationToken);
            response.Rows = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            response.PageCount = (int)Math.Ceiling((double)response.RowCount / pageSize);
        }
        catch (System.Exception ex)
        {
            throw new PersistenceException(ex);
        }

        return response;
    }

    public PagedResult<E> Paginate(IQueryable<E> query, Int32 page, Int32 pageSize, Boolean isDesc, string orderBy, Boolean isOrdered = false)
    {
        var response = new PagedResult<E>();

        if (!isOrdered)
        {
            query = (orderBy.ToUpper()) switch
            {
                "ID" => isDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
                "CREATEDBY" => isDesc ? query.OrderByDescending(x => x.CreatedBy) : query.OrderBy(x => x.CreatedBy),
                "CREATEDAT" => isDesc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
                "UPDATEDBY" => isDesc ? query.OrderByDescending(x => x.UpdatedBy) : query.OrderBy(x => x.UpdatedBy),
                "UPDATEDAT" => isDesc ? query.OrderByDescending(x => x.UpdatedAt) : query.OrderBy(x => x.UpdatedAt),
                "DELETEDAT" => isDesc ? query.OrderByDescending(x => x.DeletedAt) : query.OrderBy(x => x.DeletedAt),
                _ => isDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
            };
        }

        if (page < 1) { throw new BusinessException(BusinessErrorMessage.INVALID_PAGE); }

        try
        {
            query = query.AsNoTracking();
            response.RowCount = query.Count();
            response.Rows = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            response.PageCount = (int)Math.Ceiling((double)response.RowCount / pageSize);
        }
        catch (System.Exception ex)
        {
            throw new PersistenceException(ex);
        }

        return response;
    }

    public async Task<PagedResult<E>> GetAllPagedAsync(Int32 page, Int32 pageSize, CancellationToken cancellationToken = default)
    {
        PagedResult<E> result = new PagedResult<E>();

        IQueryable<E> query = _entity
            .Where(e => e.DeletedAt == null)
            .AsNoTracking();

        result.RowCount = await query.CountAsync(cancellationToken);
        result.Rows = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        result.PageCount = (int)Math.Ceiling((double)result.RowCount / pageSize);

        return result;
    }

    public async Task<T> GetMaxAsync<T>(Expression<Func<E, bool>> query, Expression<Func<E, T>> selector, CancellationToken cancellationToken = default)
    {
        T response;

        try
        {
            response = await _entity
                .Where(query)
                .Where(x => x.DeletedAt == null)
                .MaxAsync(selector, cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<T> GetMaxAsync<T>(Expression<Func<E, T>> selector, CancellationToken cancellationToken = default)
    {
        T response;

        try
        {
            response = await _entity
                .Where(x => x.DeletedAt == null)
                .MaxAsync(selector, cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<T> GetMinAsync<T>(Expression<Func<E, bool>> query, Expression<Func<E, T>> selector, CancellationToken cancellationToken = default)
    {
        T response;

        try
        {
            response = await _entity
                .Where(query)
                .Where(x => x.DeletedAt == null)
                .MinAsync(selector, cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<T> GetMinAsync<T>(Expression<Func<E, T>> selector, CancellationToken cancellationToken = default)
    {
        T response;

        try
        {
            response = await _entity
                .Where(x => x.DeletedAt == null)
                .MinAsync(selector, cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<Boolean> Exists(Expression<Func<E, bool>> expression, CancellationToken cancellationToken = default)
    {
        Boolean response;

        try
        {
            response = await _entity.Where(expression).AsNoTracking().AnyAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<Int32> CountAll(CancellationToken cancellationToken = default)
    {
        Int32 response;

        try
        {
            response = await _entity.CountAsync(cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<Int32> CountWhere(Expression<Func<E, bool>> expression, CancellationToken cancellationToken = default)
    {
        Int32 response;

        try
        {
            response = await _entity.CountAsync(expression, cancellationToken);
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }

        return response;
    }

    public async Task<E> UpdatePartialAsync(E entry, Action<E> updateAction, string? actorId = null)
    {
        try
        {
            var dbEntry = await _entity
                                .Where(e => e.Id == entry.Id)
                                .FirstOrDefaultAsync() ?? throw new PersistenceException($"Entity with ID {entry.Id} not found.");

            updateAction(dbEntry);

            _context.Entry(dbEntry).Property(x => x.CreatedAt).IsModified = false;
            _context.Entry(dbEntry).Property(x => x.CreatedBy).IsModified = false;
            _context.Entry(dbEntry).Property(x => x.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
            _context.Entry(dbEntry).Property(x => x.UpdatedBy).CurrentValue = actorId ?? Constant.Settings.SystemId;

            await _context.SaveChangesAsync();

            return dbEntry;
        }
        catch (System.Exception e)
        {
            throw new PersistenceException(e);
        }
    }

    //public PagedResult<T> PaginateDto<T>(IQueryable<T> query, int page, int pageSize, bool isDesc, string orderBy, bool isOrdered = false) where T : class
    //{
    //    if (page < 1)
    //        throw new BusinessException(BusinessErrorMessage.INVALID_PAGE);

    //    var response = new PagedResult<T>();

    //    if (!isOrdered && !string.IsNullOrEmpty(orderBy))
    //    {
    //        query = isDesc
    //            ? query.OrderByDescending(x => EF.Property<object>(x, orderBy))
    //            : query.OrderBy(x => EF.Property<object>(x, orderBy));
    //    }

    //    query = query.AsNoTracking();

    //    response.RowCount = query.Count();
    //    response.Rows = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
    //    response.PageCount = (int)Math.Ceiling((double)response.RowCount / pageSize);

    //    return response;
    //}
}