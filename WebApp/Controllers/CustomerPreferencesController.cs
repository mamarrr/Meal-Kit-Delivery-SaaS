using System.Security.Claims;
using App.Contracts.BLL.Menu;
using App.Contracts.BLL.Subscription;
using App.DAL.EF;
using App.Domain.Core;
using App.Domain.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels.Subscription;

namespace WebApp.Controllers;

[Authorize(Policy = "CustomerAccess")]
public class CustomerPreferencesController(
    IMealSubscriptionService mealSubscriptionService,
    IDietaryCategoryService dietaryCategoryService,
    IIngredientService ingredientService,
    ICustomerPreferenceService customerPreferenceService,
    ICustomerExclusionService customerExclusionService,
    AppDbContext dbContext,
    ILogger<CustomerPreferencesController> logger) : Controller
{
    [HttpGet("/customer/preferences")]
    public async Task<IActionResult> Preferences()
    {
        var customer = await ResolveCustomerAsync();
        if (customer == null)
        {
            return Forbid();
        }

        var model = await BuildViewModelAsync(customer, null);
        return View(model);
    }

    [HttpPost("/customer/preferences")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Preferences(CustomerPreferencesViewModel model)
    {
        var customer = await ResolveCustomerAsync();
        if (customer == null)
        {
            return Forbid();
        }

        ValidateNutritionRanges(model);

        if (!ModelState.IsValid)
        {
            var invalidModel = await BuildViewModelAsync(customer, model);
            return View(invalidModel);
        }

        try
        {
            await SyncPreferencesAsync(customer, model);
            await SyncExclusionsAsync(customer, model);
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Preferences updated.";
            return RedirectToAction(nameof(Preferences));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "CustomerPreferences/Preferences save failed");
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        var errorModel = await BuildViewModelAsync(customer, model);
        return View(errorModel);
    }

    private async Task<CustomerPreferencesViewModel> BuildViewModelAsync(Customer customer, CustomerPreferencesViewModel? input)
    {
        var companyIds = await mealSubscriptionService.GetDistinctCompanyIdsByCustomerIdAsync(customer.Id);
        var categories = (await dietaryCategoryService.GetAllByCompanyIdsAsync(companyIds))
            .Where(x => x.DeletedAt == null && x.IsActive)
            .OrderBy(x => x.Name)
            .ToList();
        var ingredients = (await ingredientService.GetAllByCompanyIdsAsync(companyIds))
            .Where(x => x.DeletedAt == null)
            .OrderBy(x => x.Name)
            .ToList();

        var preferences = await customerPreferenceService.GetAllByCustomerIdAsync(customer.Id);
        var exclusions = await customerExclusionService.GetAllByCustomerIdAsync(customer.Id);

        return new CustomerPreferencesViewModel
        {
            AvailableDietaryCategories = categories,
            AvailableIngredients = ingredients,
            SelectedPreferenceIds = input?.SelectedPreferenceIds ?? preferences.Select(x => x.DietaryCategoryId).Distinct().ToList(),
            SelectedExclusionIds = input?.SelectedExclusionIds ?? exclusions.Select(x => x.IngredientId).Distinct().ToList(),
            MinCaloriesKcal = input?.MinCaloriesKcal,
            MaxCaloriesKcal = input?.MaxCaloriesKcal,
            MinProteinG = input?.MinProteinG,
            MaxProteinG = input?.MaxProteinG,
            MinCarbsG = input?.MinCarbsG,
            MaxCarbsG = input?.MaxCarbsG,
            MinFatG = input?.MinFatG,
            MaxFatG = input?.MaxFatG,
            MinFiberG = input?.MinFiberG,
            MaxFiberG = input?.MaxFiberG,
            MinSodiumMg = input?.MinSodiumMg,
            MaxSodiumMg = input?.MaxSodiumMg
        };
    }

    private async Task SyncPreferencesAsync(Customer customer, CustomerPreferencesViewModel model)
    {
        var companyIds = await mealSubscriptionService.GetDistinctCompanyIdsByCustomerIdAsync(customer.Id);
        var categories = (await dietaryCategoryService.GetAllByCompanyIdsAsync(companyIds)).ToList();
        var categoryCompanyLookup = categories.ToDictionary(x => x.Id, x => x.CompanyId);

        var selectedIds = model.SelectedPreferenceIds
            .Where(id => categoryCompanyLookup.ContainsKey(id))
            .Distinct()
            .ToHashSet();

        var existing = await customerPreferenceService.GetAllByCustomerIdAsync(customer.Id);
        var existingIds = existing.Select(x => x.DietaryCategoryId).ToHashSet();

        var toRemove = existing.Where(x => !selectedIds.Contains(x.DietaryCategoryId)).ToList();
        foreach (var item in toRemove)
        {
            var companyId = item.DietaryCategory?.CompanyId
                            ?? (categoryCompanyLookup.TryGetValue(item.DietaryCategoryId, out var fallbackId)
                                ? fallbackId
                                : customer.CompanyId);
            await customerPreferenceService.RemoveAsync(item.Id, companyId);
        }

        var toAdd = selectedIds.Except(existingIds).ToList();
        foreach (var dietaryCategoryId in toAdd)
        {
            var companyId = categoryCompanyLookup[dietaryCategoryId];
            await customerPreferenceService.AddAsync(new CustomerPreference
            {
                CustomerId = customer.Id,
                DietaryCategoryId = dietaryCategoryId,
                CreatedAt = DateTime.UtcNow,
                DeletedAt = null
            }, companyId);
        }
    }

    private async Task SyncExclusionsAsync(Customer customer, CustomerPreferencesViewModel model)
    {
        var companyIds = await mealSubscriptionService.GetDistinctCompanyIdsByCustomerIdAsync(customer.Id);
        var ingredients = (await ingredientService.GetAllByCompanyIdsAsync(companyIds)).ToList();
        var ingredientCompanyLookup = ingredients.ToDictionary(x => x.Id, x => x.CompanyId);

        var selectedIds = model.SelectedExclusionIds
            .Where(id => ingredientCompanyLookup.ContainsKey(id))
            .Distinct()
            .ToHashSet();

        var existing = await customerExclusionService.GetAllByCustomerIdAsync(customer.Id);
        var existingIds = existing.Select(x => x.IngredientId).ToHashSet();

        var toRemove = existing.Where(x => !selectedIds.Contains(x.IngredientId)).ToList();
        foreach (var item in toRemove)
        {
            var companyId = item.Ingredient?.CompanyId
                            ?? (ingredientCompanyLookup.TryGetValue(item.IngredientId, out var fallbackId)
                                ? fallbackId
                                : customer.CompanyId);
            await customerExclusionService.RemoveAsync(item.Id, companyId);
        }

        var toAdd = selectedIds.Except(existingIds).ToList();
        foreach (var ingredientId in toAdd)
        {
            var companyId = ingredientCompanyLookup[ingredientId];
            await customerExclusionService.AddAsync(new CustomerExclusion
            {
                CustomerId = customer.Id,
                IngredientId = ingredientId,
                CreatedAt = DateTime.UtcNow,
                DeletedAt = null
            }, companyId);
        }
    }

    private void ValidateNutritionRanges(CustomerPreferencesViewModel model)
    {
        ValidateRange(model.MinCaloriesKcal, model.MaxCaloriesKcal, nameof(model.MinCaloriesKcal), "Calories");
        ValidateRange(model.MinProteinG, model.MaxProteinG, nameof(model.MinProteinG), "Protein");
        ValidateRange(model.MinCarbsG, model.MaxCarbsG, nameof(model.MinCarbsG), "Carbs");
        ValidateRange(model.MinFatG, model.MaxFatG, nameof(model.MinFatG), "Fat");
        ValidateRange(model.MinFiberG, model.MaxFiberG, nameof(model.MinFiberG), "Fiber");
        ValidateRange(model.MinSodiumMg, model.MaxSodiumMg, nameof(model.MinSodiumMg), "Sodium");
    }

    private void ValidateRange(decimal? min, decimal? max, string fieldName, string label)
    {
        if (min.HasValue && max.HasValue && min.Value > max.Value)
        {
            ModelState.AddModelError(fieldName, $"{label} min must be less than or equal to max.");
        }
    }

    private async Task<Customer?> ResolveCustomerAsync()
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
            "CustomerPreferences.ResolveCustomer start: userId={UserId}, claimCompanyId={ClaimCompanyId}, claimCompanySlug={ClaimCompanySlug}, mappings=[{Mappings}]",
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
            logger.LogWarning("CustomerPreferences.ResolveCustomer unresolved mapping: userId={UserId}", userId);
            return null;
        }

        var customer = await dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId && c.DeletedAt == null);

        logger.LogInformation(
            "CustomerPreferences.ResolveCustomer result: userId={UserId}, customerId={CustomerId}, resolved={Resolved}",
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
}
