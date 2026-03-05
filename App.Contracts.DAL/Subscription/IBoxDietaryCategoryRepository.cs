using App.Domain.Subscription;
using Contracts.DAL;

namespace App.Contracts.DAL.Subscription;

public interface IBoxDietaryCategoryRepository : IRepository<BoxDietaryCategory>
{
    Task<ICollection<BoxDietaryCategory>> GetAllByCompanyIdAsync(Guid companyId);
    Task<ICollection<BoxDietaryCategory>> GetAllByBoxIdAsync(Guid boxId, Guid companyId);
    Task RemoveAllByBoxIdAsync(Guid boxId, Guid companyId);
}
