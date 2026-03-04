using App.Contracts.BLL.Menu;
using App.Contracts.DAL.Menu;
using App.Domain.Menu;

namespace App.BLL.Menu;

public class CustomerExclusionService : BaseService<CustomerExclusion, ICustomerExclusionRepository>, ICustomerExclusionService
{
    public CustomerExclusionService(ICustomerExclusionRepository repository) : base(repository)
    {
    }

    public async Task<ICollection<CustomerExclusion>> GetAllByCompanyIdAsync(Guid companyId)
    {
        return await Repository.GetAllByCompanyIdAsync(companyId);
    }

    public async Task<CustomerExclusion?> GetByIdAsync(Guid id, Guid companyId)
    {
        return await Repository.GetByIdAsync(id, companyId);
    }

    public async Task<CustomerExclusion> AddAsync(CustomerExclusion entity, Guid companyId)
    {
        var customerInCompany = await Repository.IsCustomerInCompanyAsync(entity.CustomerId, companyId);
        var ingredientInCompany = await Repository.IsIngredientInCompanyAsync(entity.IngredientId, companyId);

        if (!customerInCompany || !ingredientInCompany)
        {
            throw new KeyNotFoundException("Customer exclusion references are outside company scope.");
        }

        return await base.AddAsync(entity);
    }

    public async Task<CustomerExclusion> UpdateAsync(CustomerExclusion entity, Guid companyId)
    {
        var existing = await Repository.GetByIdAsync(entity.Id, companyId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Customer exclusion {entity.Id} was not found in company scope {companyId}.");
        }

        var customerInCompany = await Repository.IsCustomerInCompanyAsync(entity.CustomerId, companyId);
        var ingredientInCompany = await Repository.IsIngredientInCompanyAsync(entity.IngredientId, companyId);

        if (!customerInCompany || !ingredientInCompany)
        {
            throw new KeyNotFoundException("Customer exclusion references are outside company scope.");
        }

        return await base.UpdateAsync(entity);
    }

    public async Task<CustomerExclusion> RemoveAsync(Guid id, Guid companyId)
    {
        var existing = await Repository.GetByIdAsync(id, companyId);
        if (existing == null)
        {
            throw new KeyNotFoundException($"Customer exclusion {id} was not found in company scope {companyId}.");
        }

        return await base.RemoveAsync(id);
    }
}

