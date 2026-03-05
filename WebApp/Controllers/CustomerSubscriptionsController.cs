using System.Security.Claims;
using App.Contracts.BLL.Menu;
using App.Contracts.BLL.Subscription;
using App.DAL.EF;
using App.Domain.Core;
using App.Domain.Subscription;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebApp.ViewModels.Subscription;

namespace WebApp.Controllers;

[Authorize(Policy = "CustomerAccess")]
public class CustomerSubscriptionsController(
    IBoxService boxService,
    IMealSubscriptionService mealSubscriptionService,
    IMealSelectionService mealSelectionService,
    AppDbContext dbContext,
    ILogger<CustomerSubscriptionsController> logger) : Controller
{
    [HttpGet("/customer/subscriptions/discover")]
    public async Task<IActionResult> Discover(
        [FromQuery] List<Guid>? companyIds,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] List<Guid>? dietaryCategoryIds)
    {
        var model = await BuildDiscoverViewModelAsync(
            companyIds ?? [],
            minPrice,
            maxPrice,
            dietaryCategoryIds ?? []);

        return View(model);
    }

    [HttpPost("/customer/subscriptions/subscribe")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Subscribe(DiscoverSubscriptionsViewModel model)
    {
        try
        {
            var selectedBox = (await boxService.GetDiscoverableBoxesAsync(new CustomerBoxDiscoveryFilterDto()))
                .FirstOrDefault(x => x.BoxId == model.SubscribeBoxId);

            if (selectedBox == null)
            {
                TempData["ErrorMessage"] = "Selected box is not available.";
                return RedirectToAction(nameof(Discover), new
                {
                    companyIds = model.SelectedCompanyIds,
                    minPrice = model.MinPrice,
                    maxPrice = model.MaxPrice,
                    dietaryCategoryIds = model.SelectedDietaryCategoryIds
                });
            }

            var companyId = selectedBox.CompanyId;
            var customer = await EnsureCustomerForCompanyAsync(companyId);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer profile could not be resolved for current user in company scope.";
                return RedirectToAction(nameof(Discover), new
                {
                    companyIds = model.SelectedCompanyIds,
                    minPrice = model.MinPrice,
                    maxPrice = model.MaxPrice,
                    dietaryCategoryIds = model.SelectedDietaryCategoryIds
                });
            }

            await using var tx = await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var hasActiveDuplicate = await dbContext.MealSubscriptions
                .AnyAsync(x =>
                    x.CompanyId == companyId &&
                    x.CustomerId == customer.Id &&
                    x.BoxId == selectedBox.BoxId &&
                    x.DeletedAt == null &&
                    x.IsActive);

            if (hasActiveDuplicate)
            {
                TempData["ErrorMessage"] = "You already have an active subscription for this box.";
                return RedirectToAction(nameof(Discover), new
                {
                    companyIds = model.SelectedCompanyIds,
                    minPrice = model.MinPrice,
                    maxPrice = model.MaxPrice,
                    dietaryCategoryIds = model.SelectedDietaryCategoryIds
                });
            }

            var now = DateTime.UtcNow;
            await mealSubscriptionService.AddAsync(new MealSubscription
            {
                CompanyId = companyId,
                CustomerId = customer.Id,
                BoxId = selectedBox.BoxId,
                IsActive = true,
                StartDate = now,
                AutoSelectEnabled = true,
                CreatedAt = now,
                UpdatedAt = now,
                DeletedAt = null
            }, companyId);

            await dbContext.SaveChangesAsync();
            await tx.CommitAsync();
            TempData["SuccessMessage"] = "Subscription created.";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "CustomerSubscriptions/Subscribe failed");
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Discover), new
        {
            companyIds = model.SelectedCompanyIds,
            minPrice = model.MinPrice,
            maxPrice = model.MaxPrice,
            dietaryCategoryIds = model.SelectedDietaryCategoryIds
        });
    }

    [HttpGet("/customer/subscriptions/manage")]
    public async Task<IActionResult> Manage(Guid? selectedSubscriptionId)
    {
        var customerIds = await ResolveAccessibleCustomerIdsForUserAsync();
        if (customerIds.Count == 0)
        {
            return Forbid();
        }

        var subscriptions = (await mealSubscriptionService.GetAllByCustomerIdsAsync(customerIds))
            .Where(x => x.DeletedAt == null)
            .OrderByDescending(x => x.IsActive)
            .ThenByDescending(x => x.StartDate)
            .ToList();

        var activeSubscriptions = subscriptions
            .Where(x => x.IsActive)
            .ToList();

        var subscriptionHistory = subscriptions
            .Where(x => !x.IsActive)
            .ToList();

        var selected = selectedSubscriptionId.HasValue
            ? subscriptions.FirstOrDefault(x => x.Id == selectedSubscriptionId.Value)
            : subscriptions.FirstOrDefault();

        return View(new ManageSubscriptionsViewModel
        {
            Subscriptions = subscriptions,
            ActiveSubscriptions = activeSubscriptions,
            SubscriptionHistory = subscriptionHistory,
            SelectedSubscriptionId = selected?.Id,
            SelectedSubscription = selected
        });
    }

    [HttpPost("/customer/subscriptions/unsubscribe")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unsubscribe(Guid subscriptionId)
    {
        try
        {
            var customerIds = await ResolveAccessibleCustomerIdsForUserAsync();
            if (customerIds.Count == 0)
            {
                return Forbid();
            }

            var subscription = await dbContext.MealSubscriptions
                .FirstOrDefaultAsync(x => x.Id == subscriptionId && x.DeletedAt == null);

            if (subscription == null || !customerIds.Contains(subscription.CustomerId))
            {
                TempData["ErrorMessage"] = "Subscription is outside customer ownership scope.";
                return RedirectToAction(nameof(Manage));
            }

            if (!subscription.IsActive)
            {
                TempData["SuccessMessage"] = "Subscription is already inactive.";
                return RedirectToAction(nameof(Manage));
            }

            subscription.IsActive = false;
            subscription.EndDate = DateTime.UtcNow.Date;
            subscription.UpdatedAt = DateTime.UtcNow;

            await mealSubscriptionService.UpdateAsync(subscription, subscription.CompanyId);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Subscription cancelled.";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "CustomerSubscriptions/Unsubscribe failed");
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Manage));
    }

    [HttpGet("/customer/meal-selection")]
    public async Task<IActionResult> MealSelection(Guid? selectedSubscriptionId, DateTime? weekStartDate)
    {
        var customerIds = await ResolveAccessibleCustomerIdsForUserAsync();
        if (customerIds.Count == 0)
        {
            return Forbid();
        }

        var activeSubscriptions = (await mealSubscriptionService.GetAllByCustomerIdsAsync(customerIds))
            .Where(x => x.DeletedAt == null && x.IsActive && customerIds.Contains(x.CustomerId))
            .OrderByDescending(x => x.StartDate)
            .ToList();

        var selectedSubscription = selectedSubscriptionId.HasValue
            ? activeSubscriptions.FirstOrDefault(x => x.Id == selectedSubscriptionId.Value)
            : activeSubscriptions.FirstOrDefault();

        var weekStart = NormalizeWeekStart(weekStartDate ?? DateTime.UtcNow.Date);
        var model = await BuildMealSelectionViewModelAsync(activeSubscriptions, selectedSubscription, weekStart);
        return View(model);
    }

    [HttpPost("/customer/meal-selection")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MealSelection(CustomerMealSelectionSaveRequestViewModel model)
    {
        var weekStart = NormalizeWeekStart(model.WeekStartDate);

        try
        {
            var customerIds = await ResolveAccessibleCustomerIdsForUserAsync();
            if (customerIds.Count == 0)
            {
                return Forbid();
            }

            var activeSubscriptions = (await mealSubscriptionService.GetAllByCustomerIdsAsync(customerIds))
                .Where(x => x.DeletedAt == null && x.IsActive && customerIds.Contains(x.CustomerId))
                .ToList();

            var subscription = activeSubscriptions.FirstOrDefault(x => x.Id == model.SubscriptionId);
            if (subscription == null)
            {
                TempData["ErrorMessage"] = "Selected subscription is outside your active customer scope.";
                return RedirectToAction(nameof(MealSelection), new
                {
                    selectedSubscriptionId = model.SubscriptionId,
                    weekStartDate = weekStart.ToString("yyyy-MM-dd")
                });
            }

            var weeklyMenu = await dbContext.WeeklyMenus
                .FirstOrDefaultAsync(x =>
                    x.CompanyId == subscription.CompanyId &&
                    x.WeekStartDate.Date == weekStart.Date &&
                    x.DeletedAt == null);

            if (weeklyMenu == null)
            {
                TempData["ErrorMessage"] = "No weekly menu is published for the selected subscription week.";
                return RedirectToAction(nameof(MealSelection), new
                {
                    selectedSubscriptionId = model.SubscriptionId,
                    weekStartDate = weekStart.ToString("yyyy-MM-dd")
                });
            }

            var eligibleRecipeIds = await dbContext.WeeklyMenuRecipes
                .Include(x => x.Recipe)
                .Where(x =>
                    x.WeeklyMenuId == weeklyMenu.Id &&
                    x.DeletedAt == null &&
                    x.Recipe != null &&
                    x.Recipe.CompanyId == subscription.CompanyId &&
                    x.Recipe.DeletedAt == null &&
                    x.Recipe.IsActive)
                .Select(x => x.RecipeId)
                .Distinct()
                .ToListAsync();

            var eligibleRecipeSet = eligibleRecipeIds.ToHashSet();
            var selectedRecipeIds = (model.SelectedRecipeIds ?? [])
                .Distinct()
                .ToList();

            if (selectedRecipeIds.Any(id => !eligibleRecipeSet.Contains(id)))
            {
                TempData["ErrorMessage"] = "One or more selected recipes are outside your subscription menu scope.";
                return RedirectToAction(nameof(MealSelection), new
                {
                    selectedSubscriptionId = model.SubscriptionId,
                    weekStartDate = weekStart.ToString("yyyy-MM-dd")
                });
            }

            var existingSelections = await dbContext.MealSelections
                .Where(x =>
                    x.MealSubscriptionId == subscription.Id &&
                    x.WeeklyMenuId == weeklyMenu.Id &&
                    x.DeletedAt == null)
                .ToListAsync();

            foreach (var existingSelection in existingSelections)
            {
                await mealSelectionService.RemoveAsync(existingSelection.Id, subscription.CompanyId);
            }

            var now = DateTime.UtcNow;
            foreach (var recipeId in selectedRecipeIds)
            {
                await mealSelectionService.AddAsync(new App.Domain.Menu.MealSelection
                {
                    MealSubscriptionId = subscription.Id,
                    WeeklyMenuId = weeklyMenu.Id,
                    RecipeId = recipeId,
                    SelectedAutomatically = false,
                    SelectedAt = now,
                    LockedAt = null,
                    AutoSelectionReason = null,
                    AutoSelectionNotes = null,
                    CreatedAt = now,
                    UpdatedAt = now,
                    DeletedAt = null
                }, subscription.CompanyId);
            }

            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = selectedRecipeIds.Count == 0
                ? "Meal selections cleared for the selected week."
                : $"Saved {selectedRecipeIds.Count} meal selection(s) for the selected week.";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "CustomerSubscriptions/MealSelection save failed");
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(MealSelection), new
        {
            selectedSubscriptionId = model.SubscriptionId,
            weekStartDate = weekStart.ToString("yyyy-MM-dd")
        });
    }

    private async Task<DiscoverSubscriptionsViewModel> BuildDiscoverViewModelAsync(
        List<Guid> selectedCompanyIds,
        decimal? minPrice,
        decimal? maxPrice,
        List<Guid> selectedDietaryCategoryIds)
    {
        var boxes = await boxService.GetDiscoverableBoxesAsync(new CustomerBoxDiscoveryFilterDto
        {
            CompanyIds = selectedCompanyIds,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            DietaryCategoryIds = selectedDietaryCategoryIds
        });

        var companyOptions = boxes
            .GroupBy(x => new { x.CompanyId, x.CompanyName })
            .OrderBy(x => x.Key.CompanyName)
            .Select(x => new CompanyOptionItem
            {
                CompanyId = x.Key.CompanyId,
                CompanyName = x.Key.CompanyName
            })
            .ToList();

        return new DiscoverSubscriptionsViewModel
        {
            Boxes = boxes.OrderBy(x => x.CompanyName).ThenBy(x => x.DisplayName).ToList(),
            CompanyOptions = companyOptions,
            SelectedCompanyIds = selectedCompanyIds,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SelectedDietaryCategoryIds = selectedDietaryCategoryIds
        };
    }

    private async Task<CustomerMealSelectionPageViewModel> BuildMealSelectionViewModelAsync(
        List<MealSubscription> activeSubscriptions,
        MealSubscription? selectedSubscription,
        DateTime weekStartDate)
    {
        var model = new CustomerMealSelectionPageViewModel
        {
            WeekStartDate = weekStartDate,
            ActiveSubscriptions = activeSubscriptions
                .Select(x => new CustomerMealSelectionSubscriptionOptionViewModel
                {
                    SubscriptionId = x.Id,
                    CompanyId = x.CompanyId,
                    BoxDisplayName = x.Box?.DisplayName ?? x.BoxId.ToString(),
                    StartDate = x.StartDate
                })
                .ToList(),
            SelectedSubscriptionId = selectedSubscription?.Id
        };

        if (selectedSubscription == null)
        {
            return model;
        }

        var weeklyMenu = await dbContext.WeeklyMenus
            .FirstOrDefaultAsync(x =>
                x.CompanyId == selectedSubscription.CompanyId &&
                x.WeekStartDate.Date == weekStartDate.Date &&
                x.DeletedAt == null);

        if (weeklyMenu == null)
        {
            return model;
        }

        model.WeeklyMenuId = weeklyMenu.Id;
        model.SelectionDeadlineAt = weeklyMenu.SelectionDeadlineAt;

        var availableRecipes = await dbContext.WeeklyMenuRecipes
            .Include(x => x.Recipe)
            .Where(x =>
                x.WeeklyMenuId == weeklyMenu.Id &&
                x.DeletedAt == null &&
                x.Recipe != null &&
                x.Recipe.CompanyId == selectedSubscription.CompanyId &&
                x.Recipe.DeletedAt == null &&
                x.Recipe.IsActive)
            .Select(x => new
            {
                RecipeId = x.RecipeId,
                RecipeName = x.Recipe!.Name
            })
            .Distinct()
            .OrderBy(x => x.RecipeName)
            .Select(x => new CustomerMealSelectionRecipeOptionViewModel
            {
                RecipeId = x.RecipeId,
                RecipeName = x.RecipeName
            })
            .ToListAsync();

        model.AvailableRecipes = availableRecipes;
        var availableRecipeIds = availableRecipes.Select(x => x.RecipeId).ToHashSet();

        model.SelectedRecipeIds = await dbContext.MealSelections
            .Where(x =>
                x.MealSubscriptionId == selectedSubscription.Id &&
                x.WeeklyMenuId == weeklyMenu.Id &&
                x.DeletedAt == null)
            .Select(x => x.RecipeId)
            .Where(x => availableRecipeIds.Contains(x))
            .Distinct()
            .ToListAsync();

        return model;
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

    private async Task<HashSet<Guid>> ResolveAccessibleCustomerIdsForUserAsync()
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
            "CustomerSubscriptions.ResolveAccessibleCustomerIds: userId={UserId}, mappings=[{Mappings}]",
            userId,
            string.Join(",", mappedCustomers.Select(x => $"{x.CustomerId}@{x.CompanyId}:deleted={x.CustomerDeletedAt != null}")));

        var ids = await dbContext.CustomerAppUsers
            .Where(link => link.AppUserId == userId && link.Customer != null && link.Customer.DeletedAt == null)
            .Select(link => link.CustomerId)
            .ToListAsync();

        logger.LogInformation(
            "CustomerSubscriptions.ResolveAccessibleCustomerIds result: userId={UserId}, customerIds=[{CustomerIds}]",
            userId,
            string.Join(",", ids));

        return ids.ToHashSet();
    }

    private async Task<Customer?> ResolveCustomerAsync(Guid companyId)
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
            "CustomerSubscriptions.ResolveCustomer start: userId={UserId}, targetCompanyId={TargetCompanyId}, claimCompanyId={ClaimCompanyId}, claimCompanySlug={ClaimCompanySlug}, mappings=[{Mappings}]",
            userId,
            companyId,
            User.FindFirstValue("company_id") ?? "<null>",
            User.FindFirstValue("company_slug") ?? "<null>",
            string.Join(",", mappedCustomers.Select(x => $"{x.CustomerId}@{x.CompanyId}:deleted={x.CustomerDeletedAt != null}")));

        var customerId = await dbContext.CustomerAppUsers
            .Where(link => link.AppUserId == userId && link.Customer != null && link.Customer.CompanyId == companyId)
            .Select(link => link.CustomerId)
            .FirstOrDefaultAsync();

        if (customerId == Guid.Empty)
        {
            logger.LogWarning(
                "CustomerSubscriptions.ResolveCustomer unresolved mapping: userId={UserId}, targetCompanyId={TargetCompanyId}",
                userId,
                companyId);
            return null;
        }

        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId && c.CompanyId == companyId && c.DeletedAt == null);

        logger.LogInformation(
            "CustomerSubscriptions.ResolveCustomer result: userId={UserId}, targetCompanyId={TargetCompanyId}, customerId={CustomerId}, resolved={Resolved}",
            userId,
            companyId,
            customerId,
            customer != null);

        return customer;
    }

    private async Task<Customer?> EnsureCustomerForCompanyAsync(Guid companyId)
    {
        var existing = await ResolveCustomerAsync(companyId);
        if (existing != null)
        {
            return existing;
        }

        var userId = GetCurrentUserId();
        var appUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        if (appUser == null)
        {
            logger.LogWarning(
                "CustomerSubscriptions.EnsureCustomerForCompany app user not found: userId={UserId}, targetCompanyId={TargetCompanyId}",
                userId,
                companyId);
            return null;
        }

        var now = DateTime.UtcNow;
        var firstName = string.IsNullOrWhiteSpace(appUser.FirstName) ? "Customer" : appUser.FirstName;
        var lastName = string.IsNullOrWhiteSpace(appUser.LastName) ? "User" : appUser.LastName;
        var email = string.IsNullOrWhiteSpace(appUser.Email) ? $"{userId}@customer.local" : appUser.Email;

        var customer = new Customer
        {
            CompanyId = companyId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = appUser.PhoneNumber,
            IsActive = true,
            AddressLine = "Not provided",
            City = "Not provided",
            PostalCode = "00000",
            Country = "EE",
            CreatedAt = now,
            UpdatedAt = now,
            DeletedAt = null
        };

        await dbContext.Customers.AddAsync(customer);
        await dbContext.CustomerAppUsers.AddAsync(new CustomerAppUser
        {
            CustomerId = customer.Id,
            AppUserId = userId,
            CreatedAt = now,
            DeletedAt = null
        });

        logger.LogInformation(
            "CustomerSubscriptions.EnsureCustomerForCompany creating customer mapping: userId={UserId}, targetCompanyId={TargetCompanyId}, customerId={CustomerId}",
            userId,
            companyId,
            customer.Id);

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
}

