using Domain.Constants;
using Domain.Data.Entities._Base;
using Domain.Enumerators;
using Domain.Exceptions;
using Domain.Repository._Base;
using Domain.SearchParameters._Base;

namespace Domain.Services._Base;

public abstract class IService<E, R, S>
        where E : BaseEntity
        where R : IRepository<E>
        where S : BaseSearchParameter
{
    protected readonly R _Repository;

    public IService(R repository)
    {
        _Repository = repository;
    }

    public async Task<E> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _Repository.GetAsync(id, cancellationToken);
    }
    public async Task<IEnumerable<E>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _Repository.GetAllAsync(cancellationToken);
    }

    public async Task<E> InsertAsync(E entity, string? actorId = null)
    {
        return await _Repository.InsertAsync(
            entity,
            actorId
            );
    }

    public async Task<IEnumerable<E>> InsertListAsync(IEnumerable<E> entityList, string? actorId = null)
    {
        return await _Repository.InsertListAsync(
            entityList,
            actorId ?? Constant.Settings.SystemId
            );
    }

    public async Task<E> Update(E entity, string? actorId = null)
    {
        return await _Repository.UpdateAsync(
            entity,
            actorId ?? Constant.Settings.SystemId
            );
    }

    public async Task<IEnumerable<E>> UpdateList(IEnumerable<E> entityList, string? actorId = null)
    {
        return await _Repository.UpdateListAsync(
            entityList,
            actorId ?? Constant.Settings.SystemId
            );
    }

    public async Task<E> DeleteAsync(E entity, string? actorId = null)
    {
        return await _Repository.DeleteAsync(
            entity,
            actorId ?? Constant.Settings.SystemId
            );
    }

    public async Task<IEnumerable<E>> DeleteListAsync(IEnumerable<String> idList, string? actorId = null)
    {
        return await _Repository.DeleteListAsync(
            idList,
            actorId ?? Constant.Settings.SystemId
            );
    }

    //public IEnumerable<E> Pagination(IQueryable<E> query, S searchParameter, out int count, Boolean isOrdered = false)
    //{
    //    if (searchParameter.Page < 1) { throw new BusinessException(BusinessErrorMessage.INVALID_PAGE); }

    //    IEnumerable<E> returnList = null;

    //    if (!isOrdered)
    //    {
    //        query = (searchParameter.OrderBy.ToUpper()) switch
    //        {
    //            "ID" => searchParameter.IsDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
    //            "CREATEDBY" => searchParameter.IsDesc ? query.OrderByDescending(x => x.CreatedBy) : query.OrderBy(x => x.CreatedBy),
    //            "CREATEDAT" => searchParameter.IsDesc ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
    //            "UPDATEDBY" => searchParameter.IsDesc ? query.OrderByDescending(x => x.UpdatedBy) : query.OrderBy(x => x.UpdatedBy),
    //            "UPDATEDAT" => searchParameter.IsDesc ? query.OrderByDescending(x => x.UpdatedAt) : query.OrderBy(x => x.UpdatedAt),
    //            "DELETEDAT" => searchParameter.IsDesc ? query.OrderByDescending(x => x.DeletedAt) : query.OrderBy(x => x.DeletedAt),
    //            _ => searchParameter.IsDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id),
    //        };
    //    }

    //    try
    //    {
    //        count = query.Count();
    //        returnList = query
    //            .Skip((searchParameter.Page - 1) * searchParameter.PageSize)
    //            .Take(searchParameter.PageSize)
    //            .ToList();
    //    }
    //    catch (System.Exception)
    //    {
    //        throw;
    //    }

    //    return returnList;
    //}
}