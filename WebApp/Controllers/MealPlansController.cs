using App.Contracts.BLL.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels.MealPlans;

namespace WebApp.Controllers
{
    [Authorize(Policy = "CustomerAccess")]
    public class MealPlansController : Controller
    {
        private readonly IBoxPriceService _boxPriceService;

        public MealPlansController(IBoxPriceService boxPriceService)
        {
            _boxPriceService = boxPriceService;
        }

        // GET: MealPlans
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var boxPrices = await _boxPriceService.GetAllByCompanyIdAsync(companyId.Value);
            var plans = boxPrices
                .Where(bp => bp.IsActive)
                .OrderBy(bp => bp.PriceAmount)
                .Select(bp => new MealPlanListItemViewModel
                {
                    BoxPriceId = bp.Id,
                    BoxId = bp.BoxId,
                    BoxDisplayName = bp.Box?.DisplayName ?? "Meal plan",
                    MealsCount = bp.Box?.MealsCount ?? 0,
                    PeopleCount = bp.Box?.PeopleCount ?? 0,
                    PriceAmount = bp.PriceAmount,
                    ValidFrom = bp.ValidFrom,
                    ValidTo = bp.ValidTo,
                    SubscribeUrl = Url.Action("Create", "MealSubscriptions", new { boxId = bp.BoxId }) ?? "/MealSubscriptions/Create"
                })
                .ToList();

            return View(new MealPlanDiscoveryViewModel
            {
                CompanyId = companyId.Value,
                Plans = plans
            });
        }

        private Guid? GetCurrentCompanyId()
        {
            var companyIdRaw = User.FindFirst("company_id")?.Value
                               ?? User.FindFirst("tenant_id")?.Value
                               ?? User.FindFirst("companyId")?.Value;

            return Guid.TryParse(companyIdRaw, out var companyId)
                ? companyId
                : null;
        }
    }
}
