using System.Security.Claims;
using App.Contracts.BLL.Delivery;
using App.DAL.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels.Delivery;

namespace WebApp.Controllers;

[Authorize(Policy = "CustomerAccess")]
public class CustomerDeliveriesController(
    IDeliveryService deliveryService,
    AppDbContext dbContext,
    ILogger<CustomerDeliveriesController> logger) : Controller
{
    [HttpGet("/customer/deliveries")]
    public async Task<IActionResult> Index()
    {
        var customer = await ResolveCustomerAsync();
        if (customer == null)
        {
            return Forbid();
        }

        var deliveries = (await deliveryService.GetAllByCustomerIdAsync(customer.Id))
            .Where(d => d.DeletedAt == null)
            .OrderByDescending(d => d.ScheduledTime)
            .ToList();

        var model = new CustomerDeliveriesIndexViewModel
        {
            Deliveries = deliveries.Select(d => new CustomerDeliveryListItemViewModel
            {
                DeliveryId = d.Id,
                ScheduledTime = d.ScheduledTime,
                DeliveredAt = d.DeliveredAt,
                StatusLabel = d.DeliveryStatus?.Label ?? "Unknown",
                BoxName = d.Box?.DisplayName ?? "-",
                MealsCount = d.Box?.MealsCount ?? 0,
                PeopleCount = d.Box?.PeopleCount ?? 0,
                WindowLabel = FormatWindow(d.DeliveryWindow),
                FailureReason = d.FailureReason
            }).ToList()
        };

        return View(model);
    }

    [HttpGet("/customer/deliveries/{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var customer = await ResolveCustomerAsync();
        if (customer == null)
        {
            return Forbid();
        }

        var delivery = await dbContext.Deliveries
            .Include(d => d.DeliveryStatus)
            .Include(d => d.Box)
            .Include(d => d.DeliveryWindow)
            .FirstOrDefaultAsync(d => d.Id == id && d.CustomerId == customer.Id && d.DeletedAt == null);

        if (delivery == null)
        {
            return NotFound();
        }

        var model = new CustomerDeliveryDetailsViewModel
        {
            DeliveryId = delivery.Id,
            ScheduledTime = delivery.ScheduledTime,
            DeliveredAt = delivery.DeliveredAt,
            StatusLabel = delivery.DeliveryStatus?.Label ?? "Unknown",
            BoxName = delivery.Box?.DisplayName ?? "-",
            MealsCount = delivery.Box?.MealsCount ?? 0,
            PeopleCount = delivery.Box?.PeopleCount ?? 0,
            WindowLabel = FormatWindow(delivery.DeliveryWindow),
            AddressLine = delivery.AddressLine,
            City = delivery.City,
            PostalCode = delivery.PostalCode,
            Country = delivery.Country,
            FailureReason = delivery.FailureReason
        };

        return View(model);
    }

    private async Task<App.Domain.Core.Customer?> ResolveCustomerAsync()
    {
        var userId = GetCurrentUserId();

        var mappedCustomers = await dbContext.CustomerAppUsers
            .Where(link => link.AppUserId == userId && link.Customer != null)
            .Select(link => new
            {
                link.CustomerId,
                CompanyId = link.Customer!.CompanyId,
                CustomerDeletedAt = link.Customer!.DeletedAt
            })
            .ToListAsync();

        logger.LogInformation(
            "CustomerDeliveries.ResolveCustomer start: userId={UserId}, claimCompanyId={ClaimCompanyId}, claimCompanySlug={ClaimCompanySlug}, mappings=[{Mappings}]",
            userId,
            User.FindFirstValue("company_id") ?? "<null>",
            User.FindFirstValue("company_slug") ?? "<null>",
            string.Join(",", mappedCustomers.Select(x => $"{x.CustomerId}@{x.CompanyId}:deleted={x.CustomerDeletedAt != null}")));

        var customerId = await dbContext.CustomerAppUsers
            .Where(link => link.AppUserId == userId && link.Customer != null && link.Customer.DeletedAt == null)
            .Select(link => link.CustomerId)
            .FirstOrDefaultAsync();

        if (customerId == Guid.Empty)
        {
            logger.LogWarning("CustomerDeliveries.ResolveCustomer unresolved mapping: userId={UserId}", userId);
            return null;
        }

        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId && c.DeletedAt == null);

        logger.LogInformation(
            "CustomerDeliveries.ResolveCustomer result: userId={UserId}, customerId={CustomerId}, resolved={Resolved}",
            userId,
            customerId,
            customer != null);

        return customer;
    }

    private Guid GetCurrentUserId()
    {
        var userIdRaw = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub")
                        ?? User.FindFirstValue("user_id");

        if (!Guid.TryParse(userIdRaw, out var userId))
        {
            throw new UnauthorizedAccessException("Unable to resolve current user id.");
        }

        return userId;
    }

    private static string? FormatWindow(App.Domain.Delivery.DeliveryWindow? window)
    {
        if (window == null)
        {
            return null;
        }

        var day = Enum.GetName(typeof(DayOfWeek), window.DayOfWeek) ?? "";
        return $"{day} {window.StartTime:hh\\:mm}-{window.EndTime:hh\\:mm}";
    }
}
