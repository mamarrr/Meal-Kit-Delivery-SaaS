using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.Domain.Menu;

namespace App.BLL.Menu;

public class CustomerPreferenceService : BaseService<CustomerPreference, ICustomerPreferenceRepository>, ICustomerPreferenceService
{
    public CustomerPreferenceService(ICustomerPreferenceRepository repository) : base(repository)
    {
    }

    public async Task<ICollection<CustomerPreference>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<CustomerPreference?> GetByIdAsync(Guid id, Guid companyId)
    {
        return await Repository.GetByIdAsync(id, companyId);
    }

    public async Task<CustomerPreference> AddAsync(CustomerPreference entity, Guid companyId)
    {
        var customerInCompany = await Repository.IsCustomerInCompanyAsync(entity.CustomerId, companyId);
        var dietaryCategoryInCompany = await Repository.IsDietaryCategoryInCompanyAsync(entity.DietaryCategoryId, companyId);

        if (!customerInCompany || !dietaryCategoryInCompany)
        {
            throw new KeyNotFoundException("Customer preference references are outside company scope.");
        }

        return await base.AddAsync(entity);
    }

    public async Task<CustomerPreference> UpdateAsync(CustomerPreference entity, Guid companyId)
    {
        var existing = await Repository.GetByIdAsync(entity.Id, companyId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Customer preference {entity.Id} was not found in company scope {companyId}.");
        }

        var customerInCompany = await Repository.IsCustomerInCompanyAsync(entity.CustomerId, companyId);
        var dietaryCategoryInCompany = await Repository.IsDietaryCategoryInCompanyAsync(entity.DietaryCategoryId, companyId);

        if (!customerInCompany || !dietaryCategoryInCompany)
        {
            throw new KeyNotFoundException("Customer preference references are outside company scope.");
        }

        return await base.UpdateAsync(entity);
    }

    public async Task<CustomerPreference> RemoveAsync(Guid id, Guid companyId)
    {
        var existing = await Repository.GetByIdAsync(id, companyId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Customer preference {id} was not found in company scope {companyId}.");
        }

        return await base.RemoveAsync(id);
    }
}

