using App.Contracts.DAL.Core;
using App.Domain.Core;
using Base.DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories.Core;

/// <summary>
/// EF Core repository implementation for CustomerAppUser aggregate.
/// </summary>
public class CustomerAppUserRepository : BaseRepository<CustomerAppUser, AppDbContext>, ICustomerAppUserRepository
{
    public CustomerAppUserRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<ICollection<CustomerAppUser>> GetAllByCustomerIdAsync(Guid customerId)
    {
        return await RepositoryDbSet
            .Where(cau => cau.CustomerId == customerId)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<ICollection<CustomerAppUser>> GetAllByAppUserIdAsync(Guid appUserId)
    {
        return await RepositoryDbSet
            .Where(cau => cau.AppUserId == appUserId)
            .ToListAsync();
    }
}
