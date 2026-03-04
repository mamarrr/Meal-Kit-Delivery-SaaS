using Base.Contracts.Domain;

namespace App.Contracts.BLL;

public interface ITenantEntityService<TEntity>
    where TEntity : class, IBaseEntity, ITenantProvider
{
    Task<ICollection<TEntity>> GetAllByCompanyIdAsync(Guid companyId);
    Task<TEntity?> GetByIdAsync(Guid id, Guid companyId);
    Task<TEntity> AddAsync(TEntity entity, Guid companyId);
    Task<TEntity> UpdateAsync(TEntity entity, Guid companyId);
    Task<TEntity> RemoveAsync(Guid id, Guid companyId);
}

