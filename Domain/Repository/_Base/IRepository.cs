using Domain.Data.Entities._Base;
using Domain.Data.Models;
using System.Linq.Expressions;


namespace Domain.Repository._Base;

public interface IRepository<E> where E : BaseEntity
{
    Task<E> GetAsync(string id, CancellationToken cancellationToken = default);

    Task<E> InsertAsync(E entity, string? actorId = null);

    Task<E> UpdateAsync(E entity, string? actorId = null);

    Task<E> DeleteAsync(E entity, string? actorId = null);

    Task HardDeleteAsync(E entity);

    Task<IEnumerable<E>> GetByIdListAsync(IEnumerable<string> idList, CancellationToken cancellationToken = default);

    Task<IEnumerable<E>> InsertListAsync(IEnumerable<E> entityList, string? actorId = null);

    Task<IEnumerable<E>> UpdateListAsync(IEnumerable<E> entityList, string? actorId = null);

    Task<IEnumerable<E>> DeleteListAsync(IEnumerable<string> idList, string? actorId = null);

    Task<IEnumerable<E>> HardDeleteListAsync(IEnumerable<string> idList);

    Task<IEnumerable<E>> GetAllAsync(CancellationToken cancellationToken = default);


    IQueryable<E> GetByExpression(Expression<Func<E, bool>> expression);

    IQueryable<E> GetDeletionByExpression(Expression<Func<E, bool>> expression);

    Task<PagedResult<E>> PaginateAsync(IQueryable<E> query, int page, int pageSize, bool isDesc, string orderBy, bool isOrdered = false, CancellationToken cancellationToken = default);

    Task<PagedResult<E>> GetPagedAsync(IQueryable<E> query, int page, int pageSize, CancellationToken cancellationToken = default);

    Task<PagedResult<E>> GetAllPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    Task<T> GetMaxAsync<T>(Expression<Func<E, T>> selector, CancellationToken cancellationToken = default);

    Task<T> GetMaxAsync<T>(Expression<Func<E, bool>> query, Expression<Func<E, T>> selector, CancellationToken cancellationToken = default);

    Task<T> GetMinAsync<T>(Expression<Func<E, bool>> query, Expression<Func<E, T>> selector, CancellationToken cancellationToken = default);

    Task<T> GetMinAsync<T>(Expression<Func<E, T>> selector, CancellationToken cancellationToken = default);

    Task<bool> Exists(Expression<Func<E, bool>> expression, CancellationToken cancellationToken = default);

    Task<int> CountAll(CancellationToken cancellationToken = default);

    Task<int> CountWhere(Expression<Func<E, bool>> expression, CancellationToken cancellationToken = default);

    Task<E> UpdatePartialAsync(E entry, Action<E> updateAction, string? actorId = null);
}