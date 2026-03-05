using App.Contracts.DAL.Menu;
using App.Domain.Menu;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Menu;

public class CustomerPreferenceRepository : BaseRepository<CustomerPreference, AppDbContext>, ICustomerPreferenceRepository
{
    public CustomerPreferenceRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ICollection<CustomerPreference>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Include(x => x.Customer)
            .Include(x => x.DietaryCategory)
            .Where(x => x.Customer != null && x.Customer.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<CustomerPreference?> GetByIdAsync(Guid id, Guid companyId)
    {
        return await RepositoryDbSet
            .Include(x => x.Customer)
            .Include(x => x.DietaryCategory)
            .FirstOrDefaultAsync(x => x.Id == id && x.Customer != null && x.Customer.CompanyId == companyId);
    }

    public async Task<bool> IsCustomerInCompanyAsync(Guid customerId, Guid companyId)
    {
        return await RepositoryDbContext.Customers.AnyAsync(c => c.Id == customerId && c.CompanyId == companyId);
    }

    public async Task<bool> IsDietaryCategoryInCompanyAsync(Guid dietaryCategoryId, Guid companyId)
    {
        return await RepositoryDbContext.DietaryCategories
            .AnyAsync(dc => dc.Id == dietaryCategoryId
                            && dc.CompanyId == companyId
                            && dc.DeletedAt == null);
    }
}

