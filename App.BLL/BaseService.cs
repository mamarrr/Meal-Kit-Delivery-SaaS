using App.Contracts.BLL;
using Base.Contracts.Domain;
using Contracts.DAL;

namespace App.BLL;

public abstract class BaseService<TEntity, TRepository> : IBaseEntityService<TEntity>
    where TEntity : class, IBaseEntity
    where TRepository : IRepository<TEntity>
{
    protected readonly TRepository Repository;

    protected BaseService(TRepository repository)
    {
        Repository = repository;
    }

    public virtual async Task<ICollection<TEntity>> GetAllAsync()
    {
        return await Repository.GetAllAsync();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await Repository.GetByIdAsync(id);
    }

    public virtual Task<TEntity> AddAsync(TEntity entity)
    {
        return Task.FromResult(Repository.Add(entity));
    }

    public virtual Task<TEntity> UpdateAsync(TEntity entity)
    {
        return Task.FromResult(Repository.Update(entity));
    }

    public virtual async Task<TEntity> RemoveAsync(Guid id)
    {
        return await Repository.RemoveAsync(id);
    }
}

