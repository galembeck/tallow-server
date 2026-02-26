namespace Domain.Data.Entities._Base;

public interface IBaseEntity<T> where T : BaseEntity
{
    T WithoutRelations(T entity);
}
