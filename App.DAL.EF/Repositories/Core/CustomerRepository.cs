using App.Contracts.DAL.Core;
using App.Domain.Core;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Core;

/// <summary>
/// EF Core repository implementation for Customer aggregate.
/// </summary>
public class CustomerRepository : BaseRepository<Customer, AppDbContext>, ICustomerRepository
{
    public CustomerRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<Customer>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await RepositoryDbSet
            .Where(c => c.CompanyId == companyId)
            .ToListAsync();
    }
}
