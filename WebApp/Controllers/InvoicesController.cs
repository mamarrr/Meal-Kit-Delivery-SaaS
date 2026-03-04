using App.Contracts.BLL.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

[Authorize(Policy = "SystemBilling")]
public class InvoicesController : Controller
{
    private readonly IPlatformSubscriptionService _platformSubscriptionService;

    public InvoicesController(IPlatformSubscriptionService platformSubscriptionService)
    {
        _platformSubscriptionService = platformSubscriptionService;
    }

    // GET: Invoices
    public async Task<IActionResult> Index()
    {
        var subscriptions = await _platformSubscriptionService.GetAllForBillingAsync();

        return View(subscriptions);
    }
}
