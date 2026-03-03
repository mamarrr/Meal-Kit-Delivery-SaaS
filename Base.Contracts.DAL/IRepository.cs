using Base.Contracts.Domain;

namespace Contracts.DAL;

public interface IRepository<TEntity>
    where TEntity : class, IBaseEntity
{
    TEntity Add(TEntity entity);

    TEntity Remove(TEntity entity);

    Task<TEntity> RemoveAsync(Guid Id);

    TEntity Update(TEntity entity);

    Task<ICollection<TEntity>> GetAllAsync();

    Task<TEntity> GetByIdAsync(Guid id);

}