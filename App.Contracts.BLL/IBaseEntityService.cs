using Base.Contracts.Domain;

namespace App.Contracts.BLL;

public interface IBaseEntityService<TEntity>
    where TEntity : class, IBaseEntity
{
    Task<ICollection<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> RemoveAsync(Guid id);
}

