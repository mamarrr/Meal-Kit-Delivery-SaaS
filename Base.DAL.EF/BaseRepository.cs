using Base.Contracts.Domain;
using Contracts.DAL;
using Microsoft.EntityFrameworkCore;

namespace Base.DAL.EF;

public class BaseRepository<TEntity, TDbContext> : IRepository<TEntity>
    where TEntity : class, IBaseEntity
    where TDbContext : DbContext
{
    protected TDbContext RepositoryDbContext;
    protected DbSet<TEntity> RepositoryDbSet;
    
    
    public BaseRepository(TDbContext context)
    {
        RepositoryDbContext = context;
        RepositoryDbSet = RepositoryDbContext.Set<TEntity>();
    }
    
    public virtual TEntity Add(TEntity entity)
    {
        return RepositoryDbSet.Add(entity).Entity;
    }

    public virtual TEntity Remove(TEntity entity)
    {
        return RepositoryDbSet.Remove(entity).Entity;
    }

    public virtual async Task<TEntity> RemoveAsync(Guid Id)
    {
        var entity = await GetByIdAsync(Id);
        return RepositoryDbSet.Remove(entity!).Entity;
    }

    public virtual TEntity Update(TEntity entity)
    {
        return RepositoryDbSet.Update(entity).Entity;
    }

    public virtual async Task<ICollection<TEntity>> GetAllAsync()
    {
        return await RepositoryDbSet.ToListAsync();
    }

    public virtual async Task<TEntity> GetByIdAsync(Guid id)
    {
        return (await RepositoryDbSet.FindAsync(id))!;
    }
}