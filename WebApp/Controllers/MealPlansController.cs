using App.Contracts.BLL.Core;
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
        private readonly IMealSubscriptionService _mealSubscriptionService;
        private readonly ICustomerAppUserService _customerAppUserService;
        private readonly ICustomerService _customerService;

        public MealPlansController(
            IBoxPriceService boxPriceService,
            IMealSubscriptionService mealSubscriptionService,
            ICustomerAppUserService customerAppUserService,
            ICustomerService customerService)
        {
            _boxPriceService = boxPriceService;
            _mealSubscriptionService = mealSubscriptionService;
            _customerAppUserService = customerAppUserService;
            _customerService = customerService;
        }

        // GET: MealPlans
        public async Task<IActionResult> Index()
        {
            var companyId = GetCurrentCompanyId();
            if (companyId == null)
            {
                return Forbid();
            }

            var customerId = await GetCurrentCustomerIdAsync(companyId.Value);
            if (customerId == null)
            {
                return Forbid();
            }

            var subscribedBoxIds = (await _mealSubscriptionService.GetAllByCustomerIdAsync(customerId.Value, companyId.Value))
                .Where(ms => ms.IsActive)
                .Select(ms => ms.BoxId)
                .ToHashSet();

            var boxPrices = await _boxPriceService.GetAllByCompanyIdAsync(companyId.Value);
            var plans = boxPrices
                .Where(bp => bp.IsActive && !subscribedBoxIds.Contains(bp.BoxId))
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

        private async Task<Guid?> GetCurrentCustomerIdAsync(Guid companyId)
        {
            var userIdRaw = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
                            ?? User.FindFirst("sub")?.Value
                            ?? User.FindFirst("user_id")?.Value;

            if (!Guid.TryParse(userIdRaw, out var userId))
            {
                return null;
            }

            var customers = await _customerService.GetAllByCompanyIdAsync(companyId);
            var allowedCustomerIds = customers.Select(c => c.Id).ToHashSet();

            var customerLinks = await _customerAppUserService.GetAllByAppUserIdAsync(userId);
            var linkedCustomerId = customerLinks
                .Select(link => link.CustomerId)
                .FirstOrDefault(customerId => allowedCustomerIds.Contains(customerId));

            if (linkedCustomerId != Guid.Empty)
            {
                return linkedCustomerId;
            }

            var userEmail = User.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(userEmail))
            {
                var emailMatch = customers.FirstOrDefault(c =>
                    string.Equals(c.Email, userEmail, StringComparison.OrdinalIgnoreCase));

                if (emailMatch != null)
                {
                    return emailMatch.Id;
                }
            }

            return null;
        }
    }
}
