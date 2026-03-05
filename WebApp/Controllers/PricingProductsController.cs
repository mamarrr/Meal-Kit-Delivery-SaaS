using System.Security.Claims;
using App.Contracts.BLL.Subscription;
using App.DAL.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.ViewModels.Subscription;

namespace WebApp.Controllers;

[Authorize(Policy = "CompanyManager")]
public class PricingProductsController(
    IPricingProductsService pricingProductsService,
    AppDbContext dbContext,
    ILogger<PricingProductsController> logger) : Controller
{
    [HttpGet("/{slug}/pricing-products")]
    public async Task<IActionResult> Index(string slug)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        var page = await pricingProductsService.GetPageAsync(companyId);
        return View(MapToViewModel(page, companyId, slug));
    }

    [HttpPost("/{slug}/pricing-products/box")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveBox(string slug, PricingProductsIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        try
        {
            await pricingProductsService.UpsertBoxAsync(companyId, GetCurrentUserId(), new PricingBoxUpsertDto
            {
                BoxId = model.BoxForm.BoxId,
                Name = model.BoxForm.Name,
                MealsCount = model.BoxForm.MealsCount,
                PeopleCount = model.BoxForm.PeopleCount,
                AllowedDietaryCategoryIds = model.BoxForm.AllowedDietaryCategoryIds,
                IsActive = model.BoxForm.IsActive
            });
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Box configuration saved.";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "PricingProducts/SaveBox failed for slug {Slug}", slug);
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("/{slug}/pricing-products/price")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePrice(string slug, PricingProductsIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        try
        {
            await pricingProductsService.UpsertPriceAsync(companyId, GetCurrentUserId(), new PricingBoxPriceUpsertDto
            {
                BoxPriceId = model.PriceForm.BoxPriceId,
                BoxId = model.PriceForm.BoxId,
                PricingName = model.PriceForm.PricingName,
                PriceAmount = model.PriceForm.PriceAmount,
                IsActive = model.PriceForm.IsActive
            });
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Pricing configuration saved.";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "PricingProducts/SavePrice failed for slug {Slug}", slug);
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("/{slug}/pricing-products/adjustment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAdjustment(string slug, PricingProductsIndexViewModel model)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        try
        {
            await pricingProductsService.UpsertAdjustmentAsync(companyId, GetCurrentUserId(), new PricingAdjustmentUpsertDto
            {
                AdjustmentId = model.AdjustmentForm.AdjustmentId,
                AdjustmentType = model.AdjustmentForm.AdjustmentType,
                Label = model.AdjustmentForm.Label,
                Amount = model.AdjustmentForm.Amount,
                IsPercentage = model.AdjustmentForm.IsPercentage,
                IsActive = model.AdjustmentForm.IsActive
            });
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Pricing adjustment saved.";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "PricingProducts/SaveAdjustment failed for slug {Slug}", slug);
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("/{slug}/pricing-products/box/deactivate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateBox(string slug, Guid boxId)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        try
        {
            await pricingProductsService.DeactivateBoxAsync(companyId, boxId);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Box deactivated.";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "PricingProducts/DeactivateBox failed for slug {Slug}", slug);
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    [HttpPost("/{slug}/pricing-products/adjustment/deactivate")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeactivateAdjustment(string slug, Guid adjustmentId)
    {
        if (!TryGetCompanyContext(slug, out var companyId)) return Forbid();

        try
        {
            await pricingProductsService.DeactivateAdjustmentAsync(companyId, adjustmentId);
            await dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Adjustment deactivated.";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "PricingProducts/DeactivateAdjustment failed for slug {Slug}", slug);
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index), new { slug });
    }

    private static PricingProductsIndexViewModel MapToViewModel(PricingProductsPageDto dto, Guid companyId, string slug)
    {
        return new PricingProductsIndexViewModel
        {
            CompanyId = companyId,
            CompanySlug = slug,
            Boxes = dto.Boxes.ToList(),
            PricingConfigurations = dto.PricingConfigurations.ToList(),
            DietaryCategories = dto.DietaryCategories.ToList(),
            DeliveryFees = dto.DeliveryFees.ToList(),
            Discounts = dto.Discounts.ToList(),
            AdjustmentForm = new PricingAdjustmentUpsertInput
            {
                AdjustmentType = PricingConstants.DeliveryFeeType
            }
        };
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

