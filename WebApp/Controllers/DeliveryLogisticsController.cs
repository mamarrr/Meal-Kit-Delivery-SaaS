using System.Security.Claims;
using App.Contracts.BLL.Core;
using App.Contracts.BLL.Delivery;
using App.Contracts.BLL.Menu;
using App.Contracts.BLL.Subscription;
using App.DAL.EF;
using App.Domain.Core;
using App.Domain.Delivery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels.Delivery;

namespace WebApp.Controllers;

[Authorize(Policy = "CompanyEmployee")]
public class DeliveryLogisticsController(
    ICustomerService customerService,
    IDeliveryZoneService deliveryZoneService,
    IDeliveryWindowService deliveryWindowService,
    IDeliveryService deliveryService,
    IDeliveryAttemptService deliveryAttemptService,
    IOperationalLookupService operationalLookupService,
    IMealSubscriptionService mealSubscriptionService,
    IMealSelectionService mealSelectionService,
    IWeeklyMenuService weeklyMenuService,
    IBoxService boxService,
    ICompanySettingsService companySettingsService,
    AppDbContext dbContext) : Controller
{
    [HttpGet("/{slug}/delivery-logistics")]
    public async Task<IActionResult> Index(string slug, DateTime? deliveryDate)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        var targetDate = (deliveryDate ?? DateTime.UtcNow).Date;
        var model = await BuildViewModelAsync(companyId, slug, targetDate);
        return View(model);
    }

    [HttpPost("/{slug}/delivery-logistics/window")]
    [Authorize(Policy = "CompanyManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveWindow(string slug, DeliveryLogisticsIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        var form = model.WindowForm;
        if (form.EndTime <= form.StartTime)
        {
            TempData["ErrorMessage"] = "Window end time must be later than start time.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        if (form.Capacity is < 1 or > 5000)
        {
            TempData["ErrorMessage"] = "Capacity must be between 1 and 5000.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        var zone = await deliveryZoneService.GetByIdAsync(form.DeliveryZoneId, companyId);
        if (zone == null)
        {
            TempData["ErrorMessage"] = "Zone is outside company scope.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        DeliveryWindow entity;
        if (form.DeliveryWindowId.HasValue)
        {
            var existing = await deliveryWindowService.GetByIdAsync(form.DeliveryWindowId.Value);
            if (existing == null || existing.DeliveryZoneId != form.DeliveryZoneId)
            {
                TempData["ErrorMessage"] = "Delivery window is outside company scope.";
                return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
            }

            existing.DayOfWeek = form.DayOfWeek;
            existing.StartTime = form.StartTime;
            existing.EndTime = form.EndTime;
            existing.Capacity = form.Capacity;
            existing.IsActive = form.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            entity = await deliveryWindowService.UpdateAsync(existing);
        }
        else
        {
            entity = await deliveryWindowService.AddAsync(new DeliveryWindow
            {
                DeliveryZoneId = form.DeliveryZoneId,
                DayOfWeek = form.DayOfWeek,
                StartTime = form.StartTime,
                EndTime = form.EndTime,
                Capacity = form.Capacity,
                IsActive = form.IsActive,
                CreatedAt = DateTime.UtcNow,
                CreatedByAppUserId = GetCurrentUserId()
            });
        }

        await dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = $"Delivery window {(form.DeliveryWindowId.HasValue ? "updated" : "created")} (Id: {entity.Id}).";
        return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
    }

    [HttpPost("/{slug}/delivery-logistics/zone")]
    [Authorize(Policy = "CompanyManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveZone(string slug, DeliveryLogisticsIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        var form = model.ZoneForm;
        if (string.IsNullOrWhiteSpace(form.Name))
        {
            TempData["ErrorMessage"] = "Zone name is required.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        if (form.DeliveryZoneId.HasValue)
        {
            var existing = await deliveryZoneService.GetByIdAsync(form.DeliveryZoneId.Value, companyId);
            if (existing == null)
            {
                TempData["ErrorMessage"] = "Zone is outside company scope.";
                return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
            }

            existing.Name = form.Name.Trim();
            existing.Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim();
            existing.IsActive = form.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;
            if (!form.IsActive)
            {
                existing.DeletedAt ??= DateTime.UtcNow;
            }
            else
            {
                existing.DeletedAt = null;
            }

            await deliveryZoneService.UpdateAsync(existing, companyId);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Delivery zone updated.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        var canCreate = await deliveryZoneService.CanCreateZoneAsync(companyId);
        if (!canCreate)
        {
            TempData["ErrorMessage"] = "Current subscription limit does not allow more zones.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        await deliveryZoneService.AddAsync(new DeliveryZone
        {
            Name = form.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(form.Description) ? null : form.Description.Trim(),
            IsActive = form.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedByAppUserId = GetCurrentUserId(),
            DeletedAt = form.IsActive ? null : DateTime.UtcNow
        }, companyId);

        await dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Delivery zone created.";
        return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
    }

    [HttpPost("/{slug}/delivery-logistics/zone/delete")]
    [Authorize(Policy = "CompanyManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateZone(string slug, Guid deliveryZoneId, DateTime deliveryDate)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        var zone = await deliveryZoneService.GetByIdAsync(deliveryZoneId, companyId);
        if (zone == null)
        {
            TempData["ErrorMessage"] = "Zone is outside company scope.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = deliveryDate.ToString("yyyy-MM-dd") });
        }

        zone.IsActive = false;
        zone.DeletedAt ??= DateTime.UtcNow;
        zone.UpdatedAt = DateTime.UtcNow;
        await deliveryZoneService.UpdateAsync(zone, companyId);
        await dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Delivery zone deactivated.";
        return RedirectToAction(nameof(Index), new { slug, deliveryDate = deliveryDate.ToString("yyyy-MM-dd") });
    }

    [HttpPost("/{slug}/delivery-logistics/delivery")]
    [Authorize(Policy = "CompanyManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDelivery(string slug, DeliveryLogisticsIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        var form = model.DeliveryCreateForm;
        var zone = await deliveryZoneService.GetByIdAsync(form.DeliveryZoneId, companyId);
        if (zone == null || !zone.IsActive || zone.DeletedAt != null)
        {
            TempData["ErrorMessage"] = "Selected zone is invalid for company scope.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        var window = await deliveryWindowService.GetByIdAsync(form.DeliveryWindowId);
        if (window == null || window.DeliveryZoneId != zone.Id)
        {
            TempData["ErrorMessage"] = "Selected delivery window is outside selected zone scope.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        var customer = await customerService.GetByIdAsync(form.CustomerId, companyId);
        if (customer == null)
        {
            TempData["ErrorMessage"] = "Selected customer is outside company scope.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        var mealSubscription = (await mealSubscriptionService.GetAllByCustomerIdAsync(customer.Id, companyId))
            .Where(x => x.IsActive && x.DeletedAt == null)
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefault();
        if (mealSubscription == null)
        {
            TempData["ErrorMessage"] = "Customer has no active subscription to create delivery from.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        var weekStart = NormalizeWeekStart(form.ScheduledDate.Date);
        var selection = await ResolveMealSelectionForWeekAsync(mealSubscription.Id, companyId, weekStart);
        if (selection == null)
        {
            TempData["ErrorMessage"] = "No meal selection found for the selected delivery week.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        var weeklyMenu = await weeklyMenuService.GetByIdAsync(selection.WeeklyMenuId, companyId);
        var box = await boxService.GetByIdAsync(mealSubscription.BoxId, companyId);
        if (weeklyMenu == null || box == null)
        {
            TempData["ErrorMessage"] = "Delivery context could not be resolved within company scope.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        var statuses = await operationalLookupService.GetDeliveryStatusesAsync();
        var scheduledStatusId = statuses.FirstOrDefault(x => x.Code == "scheduled")?.Id
                                ?? statuses.FirstOrDefault()?.Id;
        if (!scheduledStatusId.HasValue)
        {
            TempData["ErrorMessage"] = "Delivery statuses are not configured.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        var scheduledTimeUtc = ResolveScheduledUtc(form.ScheduledDate.Date, window.StartTime);
        await deliveryService.AddAsync(new App.Domain.Delivery.Delivery
        {
            CompanyId = companyId,
            DeliveryStatusId = scheduledStatusId.Value,
            CustomerId = customer.Id,
            WeeklyMenuId = weeklyMenu.Id,
            DeliveryZoneId = zone.Id,
            DeliveryWindowId = window.Id,
            BoxId = box.Id,
            MealSelectionId = selection.Id,
            MealSubscriptionId = mealSubscription.Id,
            ScheduledTime = scheduledTimeUtc,
            AddressLine = customer.AddressLine,
            City = customer.City,
            PostalCode = customer.PostalCode,
            Country = customer.Country,
            CreatedAt = DateTime.UtcNow,
            CreatedByAppUserId = GetCurrentUserId()
        }, companyId);

        await dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Delivery run created.";
        return RedirectToAction(nameof(Index), new { slug, deliveryDate = form.ScheduledDate.ToString("yyyy-MM-dd") });
    }

    [HttpPost("/{slug}/delivery-logistics/tracking")]
    [Authorize(Policy = "CompanyEmployee")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveTracking(string slug, DeliveryLogisticsIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        var delivery = await dbContext.Deliveries
            .FirstOrDefaultAsync(d => d.Id == model.TrackingForm.DeliveryId
                                      && d.CompanyId == companyId
                                      && d.DeletedAt == null);
        if (delivery == null)
        {
            TempData["ErrorMessage"] = "Delivery is outside company scope.";
            return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
        }

        delivery.DeliveryStatusId = model.TrackingForm.DeliveryStatusId;
        delivery.DeliveredAt = model.TrackingForm.RescheduleTo.HasValue ? null : DateTime.UtcNow;
        delivery.FailureReason = string.IsNullOrWhiteSpace(model.TrackingForm.ProofReference)
            ? model.TrackingForm.Notes
            : $"{model.TrackingForm.Notes} | Proof: {model.TrackingForm.ProofReference}";
        delivery.UpdatedAt = DateTime.UtcNow;

        if (model.TrackingForm.RescheduleTo.HasValue)
        {
            delivery.ScheduledTime = DateTime.SpecifyKind(model.TrackingForm.RescheduleTo.Value, DateTimeKind.Utc);
        }

        await deliveryService.UpdateAsync(delivery, companyId);

        var attempts = await deliveryAttemptService.GetAllByDeliveryIdAsync(delivery.Id, companyId);
        var nextAttemptNo = attempts.Count + 1;
        await deliveryAttemptService.AddAsync(new DeliveryAttempt
        {
            DeliveryId = delivery.Id,
            DeliveryAttemptResultId = model.TrackingForm.DeliveryAttemptResultId,
            AttemptNo = nextAttemptNo,
            AttemptAt = DateTime.UtcNow,
            Notes = model.TrackingForm.Notes,
            CreatedAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Delivery tracking updated.";
        return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
    }

    [HttpPost("/{slug}/delivery-logistics/escalation")]
    [Authorize(Policy = "CompanyManager")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEscalationRules(string slug, DeliveryLogisticsIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        var settings = await companySettingsService.GetByCompanyIdAsync(companyId)
            ?? await companySettingsService.AddAsync(new CompanySettings
            {
                CompanyId = companyId,
                DefaultNoRepeatWeeks = 8,
                SelectionDeadlineDaysBeforeDelivery = 2,
                AllowAutoSelection = true,
                AllowPauseSubscription = true,
                AllowSkipWeek = true,
                MinimumSubscriptionWeeks = 1,
                MaxDeliveryAttempts = 3,
                AllowRedeliveryAfterFailure = true,
                ComplaintEscalationThreshold = model.EscalationForm.ComplaintEscalationThreshold,
                ComplaintEscalationDaysWindow = model.EscalationForm.ComplaintEscalationDaysWindow,
                AutoPrioritizeFreshestStock = model.EscalationForm.AutoPrioritizeFreshestStock,
                AutoAssignEarliestSlot = model.EscalationForm.AutoAssignEarliestSlot,
                UpdatedAt = DateTime.UtcNow,
                UpdatedByAppUserId = GetCurrentUserId()
            }, companyId);

        settings.ComplaintEscalationThreshold = model.EscalationForm.ComplaintEscalationThreshold;
        settings.ComplaintEscalationDaysWindow = model.EscalationForm.ComplaintEscalationDaysWindow;
        settings.AutoPrioritizeFreshestStock = model.EscalationForm.AutoPrioritizeFreshestStock;
        settings.AutoAssignEarliestSlot = model.EscalationForm.AutoAssignEarliestSlot;
        settings.UpdatedAt = DateTime.UtcNow;
        settings.UpdatedByAppUserId = GetCurrentUserId();

        await companySettingsService.UpdateAsync(settings, companyId);
        await dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Escalation rules updated.";
        return RedirectToAction(nameof(Index), new { slug, deliveryDate = model.DeliveryDate.ToString("yyyy-MM-dd") });
    }

    private async Task<DeliveryLogisticsIndexViewModel> BuildViewModelAsync(Guid companyId, string slug, DateTime targetDate)
    {
        var zones = await deliveryZoneService.GetAllByCompanyIdAsync(companyId);
        var activeZones = zones.Where(z => z.IsActive && z.DeletedAt == null).ToList();
        var schedules = await deliveryWindowService.GetAllByCompanyIdAsync(companyId);
        var deliveries = await deliveryService.GetAllByCompanyAndScheduledDateAsync(companyId, targetDate);
        var customers = await customerService.GetAllByCompanyIdAsync(companyId);
        var statuses = await operationalLookupService.GetDeliveryStatusesAsync();
        var attemptResults = await operationalLookupService.GetDeliveryAttemptResultsAsync();
        var settings = await companySettingsService.GetByCompanyIdAsync(companyId);

        var runs = deliveries.Select(d => new DeliveryRunOrderListItem
        {
            DeliveryId = d.Id,
            RunReference = d.Id.ToString("N")[..8].ToUpperInvariant(),
            CustomerName = BuildCustomerName(d.Customer),
            AddressSummary = $"{d.AddressLine}, {d.City}, {d.PostalCode}, {d.Country}",
            ZoneName = zones.FirstOrDefault(z => z.Id == d.DeliveryZoneId)?.Name ?? string.Empty,
            TimeWindow = FormatWindow(schedules.FirstOrDefault(w => w.Id == d.DeliveryWindowId)),
            StatusLabel = statuses.FirstOrDefault(s => s.Id == d.DeliveryStatusId)?.Label ?? "Unknown",
            ScheduledTime = d.ScheduledTime,
            DeliveredAt = d.DeliveredAt,
            FailureReason = d.FailureReason
        }).OrderBy(x => x.ScheduledTime).ToList();

        return new DeliveryLogisticsIndexViewModel
        {
            CompanyId = companyId,
            CompanySlug = slug,
            DeliveryDate = targetDate,
            Zones = zones.OrderBy(z => z.Name).ToList(),
            ZoneSchedules = schedules.OrderBy(w => w.DayOfWeek).ThenBy(w => w.StartTime).ToList(),
            RunsAndOrders = runs,
            DeliveryCreateForm = new DeliveryCreateFormViewModel
            {
                ScheduledDate = targetDate
            },
            CustomerOptions = customers
                .Where(c => c.IsActive && c.DeletedAt == null)
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .Select(c => new SelectListItem($"{BuildCustomerName(c)} ({c.Email})", c.Id.ToString()))
                .ToList(),
            ZoneOptions = activeZones.OrderBy(z => z.Name).Select(z => new SelectListItem(z.Name, z.Id.ToString())).ToList(),
            WindowOptions = schedules
                .Where(w => w.IsActive && w.DeletedAt == null && activeZones.Any(z => z.Id == w.DeliveryZoneId))
                .OrderBy(w => w.DayOfWeek)
                .ThenBy(w => w.StartTime)
                .Select(w => new SelectListItem($"Day {w.DayOfWeek}: {w.StartTime:hh\\:mm}-{w.EndTime:hh\\:mm}", w.Id.ToString()))
                .ToList(),
            DeliveryStatusOptions = statuses.OrderBy(s => s.Label).Select(s => new SelectListItem(s.Label, s.Id.ToString())).ToList(),
            AttemptResultOptions = attemptResults.OrderBy(s => s.Label).Select(s => new SelectListItem(s.Label, s.Id.ToString())).ToList(),
            EscalationForm = new DeliveryEscalationRulesFormViewModel
            {
                ComplaintEscalationThreshold = settings?.ComplaintEscalationThreshold ?? 2,
                ComplaintEscalationDaysWindow = settings?.ComplaintEscalationDaysWindow ?? 14,
                AutoPrioritizeFreshestStock = settings?.AutoPrioritizeFreshestStock ?? true,
                AutoAssignEarliestSlot = settings?.AutoAssignEarliestSlot ?? true
            }
        };
    }

    private static string BuildCustomerName(Customer? customer)
    {
        if (customer == null) return "Unknown";
        return (customer.FirstName + " " + customer.LastName).Trim();
    }

    private static string FormatWindow(DeliveryWindow? window)
    {
        return window == null
            ? "-"
            : $"{window.StartTime:hh\\:mm} - {window.EndTime:hh\\:mm}";
    }

    private async Task<App.Domain.Menu.MealSelection?> ResolveMealSelectionForWeekAsync(Guid mealSubscriptionId, Guid companyId, DateTime weekStart)
    {
        var selections = await mealSelectionService.GetAllByMealSubscriptionIdAsync(mealSubscriptionId);
        foreach (var selection in selections.Where(x => x.DeletedAt == null).OrderByDescending(x => x.SelectedAt))
        {
            var weeklyMenu = await weeklyMenuService.GetByIdAsync(selection.WeeklyMenuId, companyId);
            if (weeklyMenu != null && weeklyMenu.WeekStartDate.Date == weekStart.Date)
            {
                return selection;
            }
        }

        return null;
    }

    private static DateTime NormalizeWeekStart(DateTime value)
    {
        var day = value.Date;
        while (day.DayOfWeek != DayOfWeek.Monday)
        {
            day = day.AddDays(-1);
        }

        return day;
    }

    private static DateTime ResolveScheduledUtc(DateTime date, TimeSpan windowStart)
    {
        var local = date.Date.Add(windowStart);
        return DateTime.SpecifyKind(local, DateTimeKind.Utc);
    }

    private bool TryGetCompanyContext(string slug, out Guid companyId)
    {
        companyId = Guid.Empty;

        var companyIdRaw = User.FindFirstValue("company_id")
                           ?? User.FindFirstValue("tenant_id")
                           ?? User.FindFirstValue("companyId");
        var currentSlug = User.FindFirstValue("company_slug")
                          ?? User.FindFirstValue("tenant_slug");

        return Guid.TryParse(companyIdRaw, out companyId)
               && !string.IsNullOrWhiteSpace(currentSlug)
               && string.Equals(currentSlug, slug, StringComparison.OrdinalIgnoreCase);
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
}

