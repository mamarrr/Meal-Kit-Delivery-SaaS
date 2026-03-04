using App.Contracts.BLL;
using Base.Contracts.Domain;
using Contracts.DAL;

namespace App.BLL;

public abstract class BaseTenantService<TEntity, TRepository> : ITenantEntityService<TEntity>
    where TEntity : class, IBaseEntity, ITenantProvider
    where TRepository : IRepository<TEntity>
{
    protected readonly TRepository Repository;

    protected BaseTenantService(TRepository repository)
    {
        Repository = repository;
    }

    protected abstract Task<ICollection<TEntity>> GetAllByCompanyIdCoreAsync(Guid companyId);

    public virtual async Task<ICollection<TEntity>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await GetAllByCompanyIdCoreAsync(companyId);
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, Guid companyId)
    {
        var entity = await Repository.GetByIdAsync(id);
        return entity != null && entity.CompanyId == companyId ? entity : null;
    }

    public virtual Task<TEntity> AddAsync(TEntity entity, Guid companyId)
    {
        entity.CompanyId = companyId;
        return Task.FromResult(Repository.Add(entity));
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, Guid companyId)
    {
        var existing = await Repository.GetByIdAsync(entity.Id);
        if (existing == null || existing.CompanyId != companyId)
        {
            throw new KeyNotFoundException($"Entity {entity.Id} was not found in company scope {companyId}.");
        }

        entity.CompanyId = companyId;
        return Repository.Update(entity);
    }

    public virtual async Task<TEntity> RemoveAsync(Guid id, Guid companyId)
    {
        var entity = await Repository.GetByIdAsync(id);
        if (entity == null || entity.CompanyId != companyId)
        {
            throw new KeyNotFoundException($"Entity {id} was not found in company scope {companyId}.");
        }

        return Repository.Remove(entity);
    }
}

