using App.DAL.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Controllers;

[Authorize(Policy = "SystemBilling")]
public class InvoicesController : Controller
{
    private readonly AppDbContext _appDbContext;

    public InvoicesController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    // GET: Invoices
    public async Task<IActionResult> Index()
    {
        var subscriptions = await _appDbContext.PlatformSubscriptions
            .Include(ps => ps.Company)
            .Include(ps => ps.PlatformSubscriptionTier)
            .Include(ps => ps.PlatformSubscriptionStatus)
            .Where(ps => ps.DeletedAt == null)
            .OrderByDescending(ps => ps.ValidFrom)
            .ToListAsync();

        return View(subscriptions);
    }
}
