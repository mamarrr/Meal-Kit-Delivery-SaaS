using App.Contracts.DAL.Menu;
using App.Domain.Menu;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Menu;

public class CustomerExclusionRepository : BaseRepository<CustomerExclusion, AppDbContext>, ICustomerExclusionRepository
{
    public CustomerExclusionRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ICollection<CustomerExclusion>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Include(x => x.Customer)
            .Include(x => x.Ingredient)
            .Where(x => x.Customer != null && x.Customer.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<CustomerExclusion?> GetByIdAsync(Guid id, Guid companyId)
    {
        return await RepositoryDbSet
            .Include(x => x.Customer)
            .Include(x => x.Ingredient)
            .FirstOrDefaultAsync(x => x.Id == id && x.Customer != null && x.Customer.CompanyId == companyId);
    }

    public async Task<bool> IsCustomerInCompanyAsync(Guid customerId, Guid companyId)
    {
        return await RepositoryDbContext.Customers.AnyAsync(c => c.Id == customerId && c.CompanyId == companyId);
    }

    public async Task<bool> IsIngredientInCompanyAsync(Guid ingredientId, Guid companyId)
    {
        return await RepositoryDbContext.Ingredients.AnyAsync(i => i.Id == ingredientId && i.CompanyId == companyId);
    }

    /// <inheritdoc />
    public async Task<ICollection<CustomerExclusion>> GetAllByCustomerIdAsync(Guid customerId)
    {
        return await RepositoryDbSet
            .Include(x => x.Ingredient)
            .Where(x => x.CustomerId == customerId)
            .ToListAsync();
    }
}

