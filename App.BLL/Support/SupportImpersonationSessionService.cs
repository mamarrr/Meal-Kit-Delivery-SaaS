using App.Contracts.BLL.Support;
using App.Contracts.DAL.Support;
using App.Domain.Support;

namespace App.BLL.Support;

public class SupportImpersonationSessionService : BaseService<SupportImpersonationSession, ISupportImpersonationSessionRepository>, ISupportImpersonationSessionService
{
    public SupportImpersonationSessionService(ISupportImpersonationSessionRepository repository) : base(repository)
    {
    }
}
