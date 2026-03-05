using App.DAL.EF;
using App.Domain.Core;
using App.Domain.Identity;
using App.Domain.Subscription;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels.Landing;

namespace WebApp.Services;

public class TenantOnboardingService(
    AppDbContext dbContext,
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    ILogger<TenantOnboardingService> logger)
{
    public async Task<(bool Success, string? Error)> RegisterTenantWithOwnerAsync(TenantSignupViewModel model)
    {
        var normalizedEmail = model.Email.Trim();
        var existingUser = await userManager.FindByEmailAsync(normalizedEmail);
        if (existingUser != null)
        {
            return (false, "A user with this email already exists.");
        }

        if (await dbContext.Companies.AnyAsync(c => c.RegistrationNumber == model.RegistrationNumber.Trim()))
        {
            return (false, "A company with this registration number already exists.");
        }

        var user = new AppUser
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            UserName = normalizedEmail,
            Email = normalizedEmail,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow,
        };

        var createUserResult = await userManager.CreateAsync(user, model.Password);
        if (!createUserResult.Succeeded)
        {
            return (false, createUserResult.Errors.FirstOrDefault()?.Description ?? "Failed to create owner user.");
        }

        var addCompanyOwnerRoleResult = await userManager.AddToRoleAsync(user, "CompanyOwner");
        if (!addCompanyOwnerRoleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return (false, addCompanyOwnerRoleResult.Errors.FirstOrDefault()?.Description ?? "Failed to assign CompanyOwner role.");
        }

        var addCustomerRoleResult = await userManager.AddToRoleAsync(user, "Customer");
        if (!addCustomerRoleResult.Succeeded)
        {
            await userManager.RemoveFromRoleAsync(user, "CompanyOwner");
            await userManager.DeleteAsync(user);
            return (false, addCustomerRoleResult.Errors.FirstOrDefault()?.Description ?? "Failed to assign Customer role.");
        }

        var slug = CreateSlug(model.CompanyName);
        slug = await EnsureUniqueSlugAsync(slug);

        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var now = DateTime.UtcNow;

            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = model.CompanyName.Trim(),
                Slug = slug,
                RegistrationNumber = model.RegistrationNumber.Trim(),
                ContactEmail = model.CompanyContactEmail.Trim(),
                ContactPhone = model.CompanyContactPhone.Trim(),
                WebSiteUrl = model.CompanyWebsiteUrl.Trim(),
                CreatedAt = now,
                CreatedByAppUserId = user.Id,
            };
            dbContext.Companies.Add(company);

            var ownerRole = await dbContext.CompanyRoles.FirstOrDefaultAsync(r => r.Code == "owner");
            if (ownerRole == null)
            {
                await tx.RollbackAsync();
                await userManager.RemoveFromRoleAsync(user, "CompanyOwner");
                await userManager.RemoveFromRoleAsync(user, "Customer");
                await userManager.DeleteAsync(user);
                return (false, "Company owner role mapping is missing.");
            }

            var companyUser = new CompanyAppUser
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                AppUserId = user.Id,
                CompanyRoleId = ownerRole.Id,
                IsOwner = true,
                IsActive = true,
                CreatedAt = now,
                CreatedByAppUserId = user.Id,
            };
            dbContext.CompanyAppUsers.Add(companyUser);

            var settings = new CompanySettings
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                DefaultNoRepeatWeeks = 8,
                SelectionDeadlineDaysBeforeDelivery = 2,
                AllowAutoSelection = true,
                AllowPauseSubscription = true,
                AllowSkipWeek = true,
                MinimumSubscriptionWeeks = 1,
                MaxDeliveryAttempts = 2,
                AllowRedeliveryAfterFailure = true,
                ComplaintEscalationThreshold = 3,
                ComplaintEscalationDaysWindow = 30,
                AutoPrioritizeFreshestStock = true,
                AutoAssignEarliestSlot = true,
                UpdatedAt = now,
                UpdatedByAppUserId = user.Id,
            };
            dbContext.CompanySettings.Add(settings);

            var starterTier = await dbContext.PlatformSubscriptionTiers.FirstOrDefaultAsync(t => t.Code == "starter");
            var pendingStatus = await dbContext.PlatformSubscriptionStatuses.FirstOrDefaultAsync(s => s.Code == "pending");
            if (starterTier != null && pendingStatus != null)
            {
                dbContext.PlatformSubscriptions.Add(new PlatformSubscription
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company.Id,
                    PlatformSubscriptionTierId = starterTier.Id,
                    PlatformSubscriptionStatusId = pendingStatus.Id,
                    ValidFrom = now,
                    ValidTo = null,
                    CreatedAt = now,
                    CreatedByAppUserId = user.Id,
                });
            }

            await dbContext.SaveChangesAsync();
            await tx.CommitAsync();

            await signInManager.SignInAsync(user, isPersistent: false);
            return (true, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Tenant onboarding failed for email {Email}", model.Email);
            await tx.RollbackAsync();
            await userManager.RemoveFromRoleAsync(user, "CompanyOwner");
            await userManager.RemoveFromRoleAsync(user, "Customer");
            await userManager.DeleteAsync(user);
            return (false, "Onboarding failed. Please try again.");
        }
    }

    public async Task<(bool Success, string? Error)> RegisterTenantForExistingUserAsync(Guid appUserId, ExistingUserTenantSignupViewModel model)
    {
        var existingUser = await userManager.FindByIdAsync(appUserId.ToString());
        if (existingUser == null)
        {
            return (false, "Authenticated user profile was not found.");
        }

        if (await dbContext.Companies.AnyAsync(c => c.RegistrationNumber == model.RegistrationNumber.Trim()))
        {
            return (false, "A company with this registration number already exists.");
        }

        var slug = CreateSlug(model.CompanyName);
        slug = await EnsureUniqueSlugAsync(slug);

        await using var tx = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var now = DateTime.UtcNow;

            var company = new Company
            {
                Id = Guid.NewGuid(),
                Name = model.CompanyName.Trim(),
                Slug = slug,
                RegistrationNumber = model.RegistrationNumber.Trim(),
                ContactEmail = model.CompanyContactEmail.Trim(),
                ContactPhone = model.CompanyContactPhone.Trim(),
                WebSiteUrl = model.CompanyWebsiteUrl.Trim(),
                CreatedAt = now,
                CreatedByAppUserId = existingUser.Id,
            };
            dbContext.Companies.Add(company);

            var ownerRole = await dbContext.CompanyRoles.FirstOrDefaultAsync(r => r.Code == "owner");
            if (ownerRole == null)
            {
                await tx.RollbackAsync();
                return (false, "Company owner role mapping is missing.");
            }

            var companyUser = new CompanyAppUser
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                AppUserId = existingUser.Id,
                CompanyRoleId = ownerRole.Id,
                IsOwner = true,
                IsActive = true,
                CreatedAt = now,
                CreatedByAppUserId = existingUser.Id,
            };
            dbContext.CompanyAppUsers.Add(companyUser);

            var settings = new CompanySettings
            {
                Id = Guid.NewGuid(),
                CompanyId = company.Id,
                DefaultNoRepeatWeeks = 8,
                SelectionDeadlineDaysBeforeDelivery = 2,
                AllowAutoSelection = true,
                AllowPauseSubscription = true,
                AllowSkipWeek = true,
                MinimumSubscriptionWeeks = 1,
                MaxDeliveryAttempts = 2,
                AllowRedeliveryAfterFailure = true,
                ComplaintEscalationThreshold = 3,
                ComplaintEscalationDaysWindow = 30,
                AutoPrioritizeFreshestStock = true,
                AutoAssignEarliestSlot = true,
                UpdatedAt = now,
                UpdatedByAppUserId = existingUser.Id,
            };
            dbContext.CompanySettings.Add(settings);

            var starterTier = await dbContext.PlatformSubscriptionTiers.FirstOrDefaultAsync(t => t.Code == "starter");
            var pendingStatus = await dbContext.PlatformSubscriptionStatuses.FirstOrDefaultAsync(s => s.Code == "pending");
            if (starterTier != null && pendingStatus != null)
            {
                dbContext.PlatformSubscriptions.Add(new PlatformSubscription
                {
                    Id = Guid.NewGuid(),
                    CompanyId = company.Id,
                    PlatformSubscriptionTierId = starterTier.Id,
                    PlatformSubscriptionStatusId = pendingStatus.Id,
                    ValidFrom = now,
                    ValidTo = null,
                    CreatedAt = now,
                    CreatedByAppUserId = existingUser.Id,
                });
            }

            await dbContext.SaveChangesAsync();
            await tx.CommitAsync();
            return (true, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Tenant onboarding for existing user failed for user {UserId}", appUserId);
            await tx.RollbackAsync();
            return (false, "Onboarding failed. Please try again.");
        }
    }

    private static string CreateSlug(string companyName)
    {
        var cleaned = new string(companyName
            .Trim()
            .ToLowerInvariant()
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '-')
            .ToArray());

        while (cleaned.Contains("--"))
        {
            cleaned = cleaned.Replace("--", "-");
        }

        return cleaned.Trim('-');
    }

    private async Task<string> EnsureUniqueSlugAsync(string slug)
    {
        if (!await dbContext.Companies.AnyAsync(c => c.Slug == slug))
        {
            return slug;
        }

        var suffix = 2;
        while (await dbContext.Companies.AnyAsync(c => c.Slug == $"{slug}-{suffix}"))
        {
            suffix++;
        }

        return $"{slug}-{suffix}";
    }
}
