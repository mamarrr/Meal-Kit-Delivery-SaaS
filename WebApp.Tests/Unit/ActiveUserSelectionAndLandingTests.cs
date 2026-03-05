using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using App.DAL.EF;
using App.Domain.Core;
using App.Domain.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using WebApp.Controllers;
using WebApp.Helpers;
using Xunit;

namespace WebApp.Tests.Unit;

public class ActiveUserSelectionAndLandingTests
{
    [Fact]
    public void GetAllowedContexts_WithSystemRole_IncludesSystem()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "SystemAdmin")
        ], "test"));

        var contexts = ActiveUserSelectionHelper.GetAllowedContexts(user, hasActiveCompanyMembership: false);

        Assert.Contains(ActiveUserSelectionHelper.SystemContext, contexts);
    }

    [Fact]
    public void ResolveValidContext_InvalidRequested_FallsBackToCompany()
    {
        var allowed = new List<string>
        {
            ActiveUserSelectionHelper.CompanyContext,
            ActiveUserSelectionHelper.CustomerContext
        };

        var resolved = ActiveUserSelectionHelper.ResolveValidContext("invalid", null, allowed);

        Assert.Equal(ActiveUserSelectionHelper.CompanyContext, resolved);
    }

    [Fact]
    public void CanRegisterNewCompanyFromSidebar_AuthenticatedWithoutSystemRole_ReturnsTrue()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "CompanyOwner")
        ], "test"));

        var canRegister = ActiveUserSelectionHelper.CanRegisterNewCompanyFromSidebar(user);

        Assert.True(canRegister);
    }

    [Fact]
    public void CanRegisterNewCompanyFromSidebar_SystemRole_ReturnsFalse()
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Role, "SystemSupport")
        ], "test"));

        var canRegister = ActiveUserSelectionHelper.CanRegisterNewCompanyFromSidebar(user);

        Assert.False(canRegister);
    }

    [Fact]
    public async Task Entry_CompanyContextWithoutSlug_RedirectsToSelectedCompanySlug()
    {
        await using var db = CreateDbContext();

        var userId = Guid.NewGuid();
        var companyId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();

        db.Companies.Add(new Company
        {
            Id = companyId,
            Name = "Acme",
            Slug = "acme",
            RegistrationNumber = "123",
            ContactEmail = "company@example.com",
            ContactPhone = "123",
            WebSiteUrl = "https://example.com",
            CreatedAt = DateTime.UtcNow,
            CreatedByAppUserId = creatorId
        });

        db.CompanyAppUsers.Add(new CompanyAppUser
        {
            Id = Guid.NewGuid(),
            AppUserId = userId,
            CompanyId = companyId,
            CompanyRoleId = Guid.NewGuid(),
            CreatedByAppUserId = creatorId,
            IsActive = true,
            IsOwner = true,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var controller = CreateController(db, userId, role: "CompanyOwner");

        var result = await controller.Entry(context: "company", slug: null);

        var redirect = Assert.IsType<RedirectResult>(result);
        Assert.Equal("/acme/entry", redirect.Url);
    }

    [Fact]
    public async Task SwitchCompany_UnauthorizedMembership_RedirectsToEntry()
    {
        await using var db = CreateDbContext();

        var userId = Guid.NewGuid();
        var creatorId = Guid.NewGuid();

        var controller = CreateController(db, userId, role: "CompanyOwner");

        var result = await controller.SwitchCompany(Guid.NewGuid());

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Entry", redirect.ActionName);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private static LandingController CreateController(AppDbContext dbContext, Guid userId, string role)
    {
        var userManager = CreateUserManagerMock();

        var controller = new LandingController(
            tenantOnboardingService: null!,
            dbContext,
            userManager.Object,
            NullLogger<LandingController>.Instance);

        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        ], "test"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        return controller;
    }

    private static Mock<UserManager<AppUser>> CreateUserManagerMock()
    {
        var store = new Mock<IUserStore<AppUser>>();
        return new Mock<UserManager<AppUser>>(
            store.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!);
    }
}
