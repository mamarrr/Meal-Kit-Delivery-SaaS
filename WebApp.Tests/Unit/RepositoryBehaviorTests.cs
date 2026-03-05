using App.DAL.EF;
using App.DAL.EF.Repositories.Core;
using App.DAL.EF.Repositories.Delivery;
using App.DAL.EF.Repositories.Identity;
using App.DAL.EF.Repositories.Menu;
using App.DAL.EF.Repositories.Subscription;
using App.BLL.Delivery;
using App.BLL.Menu;
using App.BLL.Subscription;
using App.Contracts.BLL.Menu;
using App.Domain.Core;
using App.Domain.Delivery;
using App.Domain.Identity;
using App.Domain.Menu;
using App.Domain.Subscription;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Tests.Unit;

public class RepositoryBehaviorTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task DeliveryRepository_GetAllByCustomerIdAndCompanyIdAsync_FiltersByTenant()
    {
        await using var ctx = CreateContext();
        var repository = new DeliveryRepository(ctx);

        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();
        var customer = Guid.NewGuid();

        repository.Add(new Delivery
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            CustomerId = customer,
            ScheduledTime = DateTime.UtcNow,
            AddressLine = "Street 1",
            City = "Tallinn",
            PostalCode = "10111",
            Country = "EE"
        });

        repository.Add(new Delivery
        {
            Id = Guid.NewGuid(),
            CompanyId = companyB,
            CustomerId = customer,
            ScheduledTime = DateTime.UtcNow,
            AddressLine = "Street 2",
            City = "Tartu",
            PostalCode = "51004",
            Country = "EE"
        });

        await ctx.SaveChangesAsync();

        var result = await repository.GetAllByCustomerIdAsync(customer, companyA);

        Assert.Single(result);
        Assert.All(result, d => Assert.Equal(companyA, d.CompanyId));
    }

    [Fact]
    public async Task CustomerRepository_Update_PersistsDetachedEntityChanges()
    {
        await using var ctx = CreateContext();
        var repository = new CustomerRepository(ctx);

        var id = Guid.NewGuid();
        var companyId = Guid.NewGuid();

        repository.Add(new Customer
        {
            Id = id,
            CompanyId = companyId,
            Email = "old@example.com",
            FirstName = "Old",
            LastName = "Name",
            IsActive = true,
            AddressLine = "Old street",
            City = "Tallinn",
            PostalCode = "10111",
            Country = "EE"
        });
        await ctx.SaveChangesAsync();

        ctx.ChangeTracker.Clear();

        repository.Update(new Customer
        {
            Id = id,
            CompanyId = companyId,
            Email = "new@example.com",
            FirstName = "New",
            LastName = "Name",
            IsActive = true,
            AddressLine = "New street",
            City = "Tallinn",
            PostalCode = "10111",
            Country = "EE"
        });
        await ctx.SaveChangesAsync();

        var updated = await repository.GetByIdAsync(id);

        Assert.Equal("new@example.com", updated.Email);
        Assert.Equal("New street", updated.AddressLine);
    }

    [Fact]
    public async Task CompanyAppUserRepository_GetAllByAppUserIdAsync_FiltersByTenant()
    {
        await using var ctx = CreateContext();
        var repository = new CompanyAppUserRepository(ctx);

        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();
        var appUserId = Guid.NewGuid();

        repository.Add(new CompanyAppUser
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            AppUserId = appUserId,
            CompanyRoleId = Guid.NewGuid(),
            CreatedByAppUserId = Guid.NewGuid(),
            IsActive = true,
            IsOwner = false,
            CreatedAt = DateTime.UtcNow
        });

        repository.Add(new CompanyAppUser
        {
            Id = Guid.NewGuid(),
            CompanyId = companyB,
            AppUserId = appUserId,
            CompanyRoleId = Guid.NewGuid(),
            CreatedByAppUserId = Guid.NewGuid(),
            IsActive = true,
            IsOwner = false,
            CreatedAt = DateTime.UtcNow
        });

        await ctx.SaveChangesAsync();

        var result = await repository.GetAllByAppUserIdAsync(appUserId, companyA);

        Assert.Single(result);
        Assert.All(result, x => Assert.Equal(companyA, x.CompanyId));
    }

    [Fact]
    public async Task QualityComplaintRepository_GetAllByCustomerIdAsync_FiltersByTenant()
    {
        await using var ctx = CreateContext();
        var repository = new QualityComplaintRepository(ctx);

        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        repository.Add(new QualityComplaint
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            CustomerId = customerId,
            DeliveryId = Guid.NewGuid(),
            QualityComplaintTypeId = Guid.NewGuid(),
            QualityComplaintStatusId = Guid.NewGuid(),
            Severity = 1,
            Description = "Complaint A",
            CreatedAt = DateTime.UtcNow
        });

        repository.Add(new QualityComplaint
        {
            Id = Guid.NewGuid(),
            CompanyId = companyB,
            CustomerId = customerId,
            DeliveryId = Guid.NewGuid(),
            QualityComplaintTypeId = Guid.NewGuid(),
            QualityComplaintStatusId = Guid.NewGuid(),
            Severity = 2,
            Description = "Complaint B",
            CreatedAt = DateTime.UtcNow
        });

        await ctx.SaveChangesAsync();

        var result = await repository.GetAllByCustomerIdAsync(customerId, companyA);

        Assert.Single(result);
        Assert.All(result, x => Assert.Equal(companyA, x.CompanyId));
    }

    [Fact]
    public async Task AppRefreshTokenRepository_GetAllByUserIdAsync_RemainsUnchangedForNonTenantEntity()
    {
        await using var ctx = CreateContext();
        var repository = new AppRefreshTokenRepository(ctx);

        var userId = Guid.NewGuid();

        repository.Add(new AppRefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RefreshToken = "token-a",
            Expiration = DateTime.UtcNow.AddHours(1),
            PreviousExpiration = DateTime.UtcNow.AddHours(1)
        });

        repository.Add(new AppRefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RefreshToken = "token-b",
            Expiration = DateTime.UtcNow.AddHours(1),
            PreviousExpiration = DateTime.UtcNow.AddHours(1)
        });

        await ctx.SaveChangesAsync();

        var result = await repository.GetAllByUserIdAsync(userId);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task DeliveryService_UpdateAsync_PreservesTenantOwnership()
    {
        await using var ctx = CreateContext();
        var repository = new DeliveryRepository(ctx);
        var service = new DeliveryService(repository);

        var deliveryId = Guid.NewGuid();
        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();

        repository.Add(new Delivery
        {
            Id = deliveryId,
            CompanyId = companyA,
            CustomerId = Guid.NewGuid(),
            ScheduledTime = DateTime.UtcNow,
            AddressLine = "Before",
            City = "Tallinn",
            PostalCode = "10111",
            Country = "EE"
        });
        await ctx.SaveChangesAsync();

        var existing = await repository.GetByIdAsync(deliveryId);
        existing.AddressLine = "After";

        var updated = await service.UpdateAsync(existing, companyA);

        await ctx.SaveChangesAsync();

        Assert.Equal(companyA, updated.CompanyId);

        var fromDb = await repository.GetByIdAsync(deliveryId);
        Assert.Equal(companyA, fromDb.CompanyId);
        Assert.Equal("After", fromDb.AddressLine);
    }

    [Fact]
    public async Task DeliveryService_UpdateAsync_ThrowsForWrongTenantScope()
    {
        await using var ctx = CreateContext();
        var repository = new DeliveryRepository(ctx);
        var service = new DeliveryService(repository);

        var deliveryId = Guid.NewGuid();
        var ownerCompany = Guid.NewGuid();
        var otherCompany = Guid.NewGuid();

        repository.Add(new Delivery
        {
            Id = deliveryId,
            CompanyId = ownerCompany,
            CustomerId = Guid.NewGuid(),
            ScheduledTime = DateTime.UtcNow,
            AddressLine = "Before",
            City = "Tallinn",
            PostalCode = "10111",
            Country = "EE"
        });
        await ctx.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await service.UpdateAsync(new Delivery
            {
                Id = deliveryId,
                CompanyId = otherCompany,
                CustomerId = Guid.NewGuid(),
                ScheduledTime = DateTime.UtcNow,
                AddressLine = "After",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE"
            }, otherCompany)
        );
    }

    [Fact]
    public async Task MealSubscriptionRepository_GetAllByCustomerIdAsync_FiltersByTenant()
    {
        await using var ctx = CreateContext();
        var repository = new MealSubscriptionRepository(ctx);

        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();
        var customerId = Guid.NewGuid();

        repository.Add(new MealSubscription
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            CustomerId = customerId,
            BoxId = Guid.NewGuid(),
            IsActive = true,
            StartDate = DateTime.UtcNow,
            AutoSelectEnabled = true,
            CreatedAt = DateTime.UtcNow
        });

        repository.Add(new MealSubscription
        {
            Id = Guid.NewGuid(),
            CompanyId = companyB,
            CustomerId = customerId,
            BoxId = Guid.NewGuid(),
            IsActive = true,
            StartDate = DateTime.UtcNow,
            AutoSelectEnabled = true,
            CreatedAt = DateTime.UtcNow
        });

        await ctx.SaveChangesAsync();

        var result = await repository.GetAllByCustomerIdAsync(customerId, companyA);

        Assert.Single(result);
        Assert.All(result, x => Assert.Equal(companyA, x.CompanyId));
    }

    [Fact]
    public async Task PricingAdjustmentRepository_GetAllByTypeAsync_FiltersByTenantAndType()
    {
        await using var ctx = CreateContext();
        var repository = new PricingAdjustmentRepository(ctx);

        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();

        repository.Add(new PricingAdjustment
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            CreatedByAppUserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AdjustmentType = "delivery_fee",
            Label = "Primary delivery",
            Amount = 4.99m,
            IsPercentage = false,
            IsActive = true
        });

        repository.Add(new PricingAdjustment
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            CreatedByAppUserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AdjustmentType = "discount",
            Label = "Welcome discount",
            Amount = 10m,
            IsPercentage = true,
            IsActive = true
        });

        repository.Add(new PricingAdjustment
        {
            Id = Guid.NewGuid(),
            CompanyId = companyB,
            CreatedByAppUserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            AdjustmentType = "delivery_fee",
            Label = "Other tenant fee",
            Amount = 3.99m,
            IsPercentage = false,
            IsActive = true
        });

        await ctx.SaveChangesAsync();

        var result = await repository.GetAllByTypeAsync(companyA, "delivery_fee");

        Assert.Single(result);
        Assert.All(result, x =>
        {
            Assert.Equal(companyA, x.CompanyId);
            Assert.Equal("delivery_fee", x.AdjustmentType);
        });
    }

    [Fact]
    public async Task MealSubscriptionService_GetByIdAsync_RespectsTenantScope()
    {
        await using var ctx = CreateContext();
        var repository = new MealSubscriptionRepository(ctx);
        var service = new MealSubscriptionService(repository);

        var id = Guid.NewGuid();
        var ownerCompany = Guid.NewGuid();
        var otherCompany = Guid.NewGuid();

        repository.Add(new MealSubscription
        {
            Id = id,
            CompanyId = ownerCompany,
            CustomerId = Guid.NewGuid(),
            BoxId = Guid.NewGuid(),
            IsActive = true,
            StartDate = DateTime.UtcNow,
            AutoSelectEnabled = true,
            CreatedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        var visibleInOwnerScope = await service.GetByIdAsync(id, ownerCompany);
        var hiddenInOtherScope = await service.GetByIdAsync(id, otherCompany);

        Assert.NotNull(visibleInOwnerScope);
        Assert.Null(hiddenInOtherScope);
    }

    [Fact]
    public async Task CustomerRepository_GetSubscriberListAsync_FiltersByTenantAndStatus()
    {
        await using var ctx = CreateContext();
        var repository = new CustomerRepository(ctx);

        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();

        var customerA = new Customer
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            Email = "alpha@example.com",
            FirstName = "Alpha",
            LastName = "One",
            IsActive = true,
            AddressLine = "Street 1",
            City = "Tallinn",
            PostalCode = "10111",
            Country = "EE"
        };

        var customerB = new Customer
        {
            Id = Guid.NewGuid(),
            CompanyId = companyB,
            Email = "beta@example.com",
            FirstName = "Beta",
            LastName = "Two",
            IsActive = true,
            AddressLine = "Street 2",
            City = "Tartu",
            PostalCode = "51004",
            Country = "EE"
        };

        repository.Add(customerA);
        repository.Add(customerB);

        var boxA = new Box
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            CreatedByAppUserId = Guid.NewGuid(),
            MealsCount = 3,
            PeopleCount = 2,
            DisplayName = "Standard",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        ctx.Boxes.Add(boxA);

        var boxB = new Box
        {
            Id = Guid.NewGuid(),
            CompanyId = companyB,
            CreatedByAppUserId = Guid.NewGuid(),
            MealsCount = 4,
            PeopleCount = 2,
            DisplayName = "Premium",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        ctx.Boxes.Add(boxB);

        ctx.MealSubscriptions.Add(new MealSubscription
        {
            Id = Guid.NewGuid(),
            CompanyId = companyB,
            CustomerId = customerB.Id,
            BoxId = boxB.Id,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-2),
            AutoSelectEnabled = true,
            CreatedAt = DateTime.UtcNow
        });

        await ctx.SaveChangesAsync();

        var result = await repository.GetSubscriberListAsync(companyA, null, "inactive", null, null);

        Assert.Single(result);
        Assert.Equal(customerA.Id, result.First().CustomerId);
        Assert.Equal("Inactive", result.First().Status);
    }

    [Fact]
    public async Task WeeklyMenuRepository_GetRuleConfigByCompanyIdAsync_RespectsTenantScope()
    {
        await using var ctx = CreateContext();
        var repository = new WeeklyMenuRepository(ctx);

        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();

        repository.AddRuleConfig(new App.Domain.Menu.WeeklyMenuRuleConfig
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            RecipesPerCategory = 2,
            NoRepeatWeeks = 8,
            SelectionDeadlineDaysBeforeWeekStart = 2,
            CreatedAt = DateTime.UtcNow
        });

        repository.AddRuleConfig(new App.Domain.Menu.WeeklyMenuRuleConfig
        {
            Id = Guid.NewGuid(),
            CompanyId = companyB,
            RecipesPerCategory = 3,
            NoRepeatWeeks = 6,
            SelectionDeadlineDaysBeforeWeekStart = 1,
            CreatedAt = DateTime.UtcNow
        });

        await ctx.SaveChangesAsync();

        var configA = await repository.GetRuleConfigByCompanyIdAsync(companyA);

        Assert.NotNull(configA);
        Assert.Equal(companyA, configA!.CompanyId);
        Assert.Equal(2, configA.RecipesPerCategory);
    }

    [Fact]
    public async Task DeliveryRepository_GetAllByCompanyAndScheduledDateAsync_FiltersByDateAndTenant()
    {
        await using var ctx = CreateContext();
        var repository = new DeliveryRepository(ctx);

        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();
        var targetDate = DateTime.UtcNow.Date;

        repository.Add(new Delivery
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            CustomerId = Guid.NewGuid(),
            ScheduledTime = targetDate.AddHours(9),
            AddressLine = "A Street 1",
            City = "Tallinn",
            PostalCode = "10111",
            Country = "EE"
        });

        repository.Add(new Delivery
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            CustomerId = Guid.NewGuid(),
            ScheduledTime = targetDate.AddDays(1).AddHours(9),
            AddressLine = "A Street 2",
            City = "Tallinn",
            PostalCode = "10111",
            Country = "EE"
        });

        repository.Add(new Delivery
        {
            Id = Guid.NewGuid(),
            CompanyId = companyB,
            CustomerId = Guid.NewGuid(),
            ScheduledTime = targetDate.AddHours(10),
            AddressLine = "B Street 1",
            City = "Tartu",
            PostalCode = "51004",
            Country = "EE"
        });

        await ctx.SaveChangesAsync();

        var result = await repository.GetAllByCompanyAndScheduledDateAsync(companyA, targetDate);

        Assert.Single(result);
        Assert.All(result, x =>
        {
            Assert.Equal(companyA, x.CompanyId);
            Assert.Equal(targetDate, x.ScheduledTime.Date);
        });
    }

    [Fact]
    public async Task DeliveryZoneService_CanCreateZoneAsync_RespectsTierMaxZones()
    {
        await using var ctx = CreateContext();

        var zoneRepository = new DeliveryZoneRepository(ctx);
        var platformSubscriptionRepository = new PlatformSubscriptionRepository(ctx);
        var platformSubscriptionService = new PlatformSubscriptionService(platformSubscriptionRepository);
        var zoneService = new DeliveryZoneService(zoneRepository, platformSubscriptionService);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var tierId = Guid.NewGuid();
        var statusId = Guid.NewGuid();

        ctx.PlatformSubscriptionTiers.Add(new PlatformSubscriptionTier
        {
            Id = tierId,
            Code = "limited",
            Name = "Limited",
            MaxZones = 1,
            MaxSubscribers = 10,
            MaxEmployees = 1,
            MaxRecipes = 10,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        ctx.PlatformSubscriptionStatuses.Add(new PlatformSubscriptionStatus
        {
            Id = statusId,
            Code = "active",
            Label = "Active"
        });
        ctx.PlatformSubscriptions.Add(new PlatformSubscription
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            PlatformSubscriptionTierId = tierId,
            PlatformSubscriptionStatusId = statusId,
            CreatedByAppUserId = actorId,
            ValidFrom = DateTime.UtcNow.AddDays(-1),
            ValidTo = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        });

        zoneRepository.Add(new DeliveryZone
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            Name = "Central",
            Description = "Central zone",
            IsActive = true,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        var canCreate = await zoneService.CanCreateZoneAsync(companyId);

        Assert.False(canCreate);
    }

    [Fact]
    public async Task DeliveryZoneService_UpdateAsync_ThrowsForWrongTenantScope()
    {
        await using var ctx = CreateContext();

        var zoneRepository = new DeliveryZoneRepository(ctx);
        var platformSubscriptionRepository = new PlatformSubscriptionRepository(ctx);
        var platformSubscriptionService = new PlatformSubscriptionService(platformSubscriptionRepository);
        var zoneService = new DeliveryZoneService(zoneRepository, platformSubscriptionService);

        var ownerCompany = Guid.NewGuid();
        var otherCompany = Guid.NewGuid();
        var zoneId = Guid.NewGuid();

        zoneRepository.Add(new DeliveryZone
        {
            Id = zoneId,
            CompanyId = ownerCompany,
            Name = "Owner zone",
            IsActive = true,
            CreatedByAppUserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await zoneService.UpdateAsync(new DeliveryZone
            {
                Id = zoneId,
                CompanyId = otherCompany,
                Name = "Tampered",
                IsActive = true,
                CreatedByAppUserId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow
            }, otherCompany));
    }

    [Fact]
    public async Task DeliveryService_AddAsync_ThrowsWhenWindowIsOutsideSelectedZone()
    {
        await using var ctx = CreateContext();

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var deliveryStatusId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var weeklyMenuId = Guid.NewGuid();
        var validZoneId = Guid.NewGuid();
        var otherZoneId = Guid.NewGuid();
        var windowId = Guid.NewGuid();
        var boxId = Guid.NewGuid();
        var mealSubscriptionId = Guid.NewGuid();
        var mealSelectionId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();

        ctx.DeliveryStatuses.Add(new DeliveryStatus
        {
            Id = deliveryStatusId,
            Code = "scheduled",
            Label = "Scheduled"
        });
        ctx.Customers.Add(new Customer
        {
            Id = customerId,
            CompanyId = companyId,
            Email = "customer@example.com",
            FirstName = "Test",
            LastName = "Customer",
            IsActive = true,
            AddressLine = "Main 1",
            City = "Tallinn",
            PostalCode = "10111",
            Country = "EE",
            CreatedAt = DateTime.UtcNow
        });
        ctx.WeeklyMenus.Add(new WeeklyMenu
        {
            Id = weeklyMenuId,
            CompanyId = companyId,
            WeekStartDate = DateTime.UtcNow.Date,
            SelectionDeadlineAt = DateTime.UtcNow.Date.AddDays(-1),
            TotalRecipes = 1,
            IsPublished = true,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            CreatedByAppUserId = actorId
        });
        ctx.DeliveryZones.AddRange(
            new DeliveryZone
            {
                Id = validZoneId,
                CompanyId = companyId,
                Name = "Zone A",
                IsActive = true,
                CreatedByAppUserId = actorId,
                CreatedAt = DateTime.UtcNow
            },
            new DeliveryZone
            {
                Id = otherZoneId,
                CompanyId = companyId,
                Name = "Zone B",
                IsActive = true,
                CreatedByAppUserId = actorId,
                CreatedAt = DateTime.UtcNow
            });
        ctx.DeliveryWindows.Add(new DeliveryWindow
        {
            Id = windowId,
            DeliveryZoneId = otherZoneId,
            DayOfWeek = 1,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(10, 0, 0),
            Capacity = 10,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedByAppUserId = actorId
        });
        ctx.Boxes.Add(new Box
        {
            Id = boxId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            MealsCount = 3,
            PeopleCount = 2,
            DisplayName = "Family",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        ctx.MealSubscriptions.Add(new MealSubscription
        {
            Id = mealSubscriptionId,
            CompanyId = companyId,
            CustomerId = customerId,
            BoxId = boxId,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            AutoSelectEnabled = true,
            CreatedAt = DateTime.UtcNow
        });
        ctx.Recipes.Add(new Recipe
        {
            Id = recipeId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Name = "Recipe",
            DefaultServings = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        ctx.MealSelections.Add(new MealSelection
        {
            Id = mealSelectionId,
            MealSubscriptionId = mealSubscriptionId,
            WeeklyMenuId = weeklyMenuId,
            RecipeId = recipeId,
            SelectedAutomatically = false,
            SelectedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        var deliveryRepository = new DeliveryRepository(ctx);
        var customerRepository = new CustomerRepository(ctx);
        var weeklyMenuRepository = new WeeklyMenuRepository(ctx);
        var deliveryZoneRepository = new DeliveryZoneRepository(ctx);
        var boxRepository = new BoxRepository(ctx);
        var mealSelectionRepository = new MealSelectionRepository(ctx);
        var mealSubscriptionRepository = new MealSubscriptionRepository(ctx);
        var service = new DeliveryService(
            deliveryRepository,
            customerRepository,
            weeklyMenuRepository,
            deliveryZoneRepository,
            boxRepository,
            mealSelectionRepository,
            mealSubscriptionRepository,
            ctx);

        await Assert.ThrowsAsync<KeyNotFoundException>(async () =>
            await service.AddAsync(new Delivery
            {
                Id = Guid.NewGuid(),
                CompanyId = companyId,
                DeliveryStatusId = deliveryStatusId,
                CustomerId = customerId,
                WeeklyMenuId = weeklyMenuId,
                DeliveryZoneId = validZoneId,
                DeliveryWindowId = windowId,
                BoxId = boxId,
                MealSelectionId = mealSelectionId,
                MealSubscriptionId = mealSubscriptionId,
                ScheduledTime = DateTime.UtcNow,
                AddressLine = "Main 1",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE",
                CreatedAt = DateTime.UtcNow,
                CreatedByAppUserId = actorId
            }, companyId));
    }

    [Fact]
    public async Task WeeklyMenuService_AssignRecipeToWeekAsync_RejectsNoRepeatViolation()
    {
        await using var ctx = CreateContext();
        var weeklyMenuRepository = new WeeklyMenuRepository(ctx);
        var service = new WeeklyMenuService(weeklyMenuRepository);

        var companyId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var weekStart = new DateTime(2026, 03, 09, 0, 0, 0, DateTimeKind.Utc);
        var prevWeek = weekStart.AddDays(-7);

        ctx.Recipes.Add(new App.Domain.Menu.Recipe
        {
            Id = recipeId,
            CompanyId = companyId,
            CreatedByAppUserId = userId,
            Name = "Recipe A",
            DefaultServings = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        var oldMenu = new App.Domain.Menu.WeeklyMenu
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            CreatedByAppUserId = userId,
            WeekStartDate = prevWeek,
            SelectionDeadlineAt = prevWeek.AddDays(-2),
            TotalRecipes = 1,
            IsPublished = false,
            PublishedAt = prevWeek,
            CreatedAt = DateTime.UtcNow,
            WeeklyMenuRecipes = new List<App.Domain.Menu.WeeklyMenuRecipe>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    RecipeId = recipeId,
                    CreatedByAppUserId = userId,
                    CreatedAt = DateTime.UtcNow
                }
            }
        };

        ctx.WeeklyMenus.Add(oldMenu);

        weeklyMenuRepository.AddRuleConfig(new App.Domain.Menu.WeeklyMenuRuleConfig
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            RecipesPerCategory = 2,
            NoRepeatWeeks = 8,
            SelectionDeadlineDaysBeforeWeekStart = 2,
            CreatedAt = DateTime.UtcNow
        });

        await ctx.SaveChangesAsync();

        var result = await service.AssignRecipeToWeekAsync(companyId, new WeeklyMenuAssignmentCreateDto
        {
            WeekStartDate = weekStart,
            RecipeId = recipeId,
            CreatedByAppUserId = userId
        });

        Assert.False(result.Success);
        Assert.Contains("no-repeat", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task IngredientService_UpsertCatalogItemAsync_EnforcesUniqueNamePerTenant()
    {
        await using var ctx = CreateContext();
        var repository = new IngredientRepository(ctx);
        var service = new IngredientService(repository);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();

        await service.UpsertCatalogItemAsync(companyId, actorId, new IngredientCatalogUpsertDto
        {
            Name = "cilantro",
            IsExclusionTag = true,
            ExclusionKey = "cilantro"
        });
        await ctx.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await service.UpsertCatalogItemAsync(companyId, actorId, new IngredientCatalogUpsertDto
            {
                Name = "cilantro",
                IsExclusionTag = true,
                ExclusionKey = "cilantro"
            }));
    }

    [Fact]
    public async Task IngredientService_UpsertCatalogItemAsync_ReactivatesSoftDeletedIngredientWithSameName()
    {
        await using var ctx = CreateContext();
        var repository = new IngredientRepository(ctx);
        var service = new IngredientService(repository);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();

        var created = await service.UpsertCatalogItemAsync(companyId, actorId, new IngredientCatalogUpsertDto
        {
            Name = "Ingredient1",
            IsAllergen = false,
            IsExclusionTag = true
        });
        await ctx.SaveChangesAsync();

        await service.RemoveCatalogItemAsync(companyId, created.IngredientId);
        await ctx.SaveChangesAsync();

        var recreated = await service.UpsertCatalogItemAsync(companyId, actorId, new IngredientCatalogUpsertDto
        {
            Name = "Ingredient1",
            IsAllergen = true,
            IsExclusionTag = false
        });
        await ctx.SaveChangesAsync();

        Assert.Equal(created.IngredientId, recreated.IngredientId);
        Assert.True(recreated.IsAllergen);
        Assert.True(recreated.IsExclusionTag);
        Assert.Equal("ingredient1", recreated.ExclusionKey);

        var storedRows = await ctx.Ingredients
            .Where(x => x.CompanyId == companyId && x.NormalizedName == "ingredient1")
            .ToListAsync();

        Assert.Single(storedRows);
        Assert.Null(storedRows[0].DeletedAt);
    }

    [Fact]
    public async Task RecipeService_UpsertRecipeEditorAsync_RejectsOutOfScopeIngredientIds()
    {
        await using var ctx = CreateContext();
        var repository = new RecipeRepository(ctx);
        var service = new RecipeService(repository);

        var companyId = Guid.NewGuid();
        var otherCompany = Guid.NewGuid();
        var actorId = Guid.NewGuid();

        var outOfScopeIngredient = new Ingredient
        {
            Id = Guid.NewGuid(),
            CompanyId = otherCompany,
            CreatedByAppUserId = actorId,
            Name = "pork",
            CreatedAt = DateTime.UtcNow,
            IsExclusionTag = true
        };
        ctx.Ingredients.Add(outOfScopeIngredient);
        await ctx.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await service.UpsertRecipeEditorAsync(companyId, actorId, new RecipeEditorUpsertDto
            {
                Name = "Recipe X",
                DefaultServings = 2,
                IsActive = true,
                IngredientIds = [outOfScopeIngredient.Id],
                Nutrition = new RecipeNutritionDto
                {
                    CaloriesKcal = 450,
                    ProteinG = 30,
                    CarbsG = 40,
                    FatG = 15,
                    FiberG = 6,
                    SodiumMg = 500
                }
            }));
    }

    [Fact]
    public async Task DietaryCategoryService_RemoveCatalogItemAsync_SoftDeletesCategory()
    {
        await using var ctx = CreateContext();
        var repository = new DietaryCategoryRepository(ctx);
        var service = new DietaryCategoryService(repository);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();

        var created = await service.UpsertCatalogItemAsync(companyId, actorId, new DietaryCategoryCatalogUpsertDto
        {
            Code = "vegan",
            Name = "Vegan",
            IsActive = true
        });
        await ctx.SaveChangesAsync();

        await service.RemoveCatalogItemAsync(companyId, created.DietaryCategoryId);
        await ctx.SaveChangesAsync();

        var stored = await repository.GetByIdAsync(created.DietaryCategoryId);
        Assert.NotNull(stored);
        Assert.NotNull(stored!.DeletedAt);
    }

    [Fact]
    public async Task RecipeService_RemoveRecipeAsync_SoftDeletesRecipe()
    {
        await using var ctx = CreateContext();
        var repository = new RecipeRepository(ctx);
        var service = new RecipeService(repository);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();

        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Name = "Soft delete recipe",
            DefaultServings = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        ctx.Recipes.Add(recipe);
        await ctx.SaveChangesAsync();

        await service.RemoveRecipeAsync(companyId, recipe.Id);
        await ctx.SaveChangesAsync();

        var stored = await repository.GetByIdAsync(recipe.Id);
        Assert.NotNull(stored);
        Assert.NotNull(stored!.DeletedAt);
    }

    [Fact]
    public async Task RecipeRepository_UpdateNutritionalInfo_UsesTrackedInstanceWhenPresent()
    {
        await using var ctx = CreateContext();
        var repository = new RecipeRepository(ctx);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var nutritionId = Guid.NewGuid();

        ctx.Recipes.Add(new Recipe
        {
            Id = recipeId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Name = "Tracked nutrition recipe",
            DefaultServings = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        ctx.NutritionalInfos.Add(new NutritionalInfo
        {
            Id = nutritionId,
            RecipeId = recipeId,
            CaloriesKcal = 100,
            ProteinG = 10,
            CarbsG = 20,
            FatG = 5,
            FiberG = 3,
            SodiumMg = 200,
            CreatedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        var tracked = await ctx.NutritionalInfos.FirstAsync(x => x.Id == nutritionId);

        var detached = new NutritionalInfo
        {
            Id = nutritionId,
            RecipeId = recipeId,
            CaloriesKcal = 250,
            ProteinG = 18,
            CarbsG = 24,
            FatG = 8,
            FiberG = 6,
            SodiumMg = 320,
            CreatedAt = tracked.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        repository.UpdateNutritionalInfo(detached);
        await ctx.SaveChangesAsync();

        Assert.Equal(250, tracked.CaloriesKcal);
        Assert.Equal(18, tracked.ProteinG);
    }

    [Fact]
    public async Task WeeklyMenuService_AssignRecipeToWeekAsync_DerivesCategoryFromRecipe()
    {
        await using var ctx = CreateContext();
        var repository = new WeeklyMenuRepository(ctx);
        var service = new WeeklyMenuService(repository);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var expectedCategoryId = Guid.NewGuid();

        ctx.Recipes.Add(new Recipe
        {
            Id = recipeId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Name = "Assignment recipe",
            DefaultServings = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        ctx.DietaryCategories.Add(new DietaryCategory
        {
            Id = expectedCategoryId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Code = "vegan",
            Name = "Vegan",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        ctx.RecipeDietaryCategories.Add(new RecipeDietaryCategory
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            DietaryCategoryId = expectedCategoryId,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow
        });

        repository.AddRuleConfig(new WeeklyMenuRuleConfig
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            RecipesPerCategory = 2,
            NoRepeatWeeks = 0,
            SelectionDeadlineDaysBeforeWeekStart = 2,
            CreatedAt = DateTime.UtcNow
        });

        await ctx.SaveChangesAsync();

        var result = await service.AssignRecipeToWeekAsync(companyId, new WeeklyMenuAssignmentCreateDto
        {
            WeekStartDate = new DateTime(2026, 03, 09, 0, 0, 0, DateTimeKind.Utc),
            RecipeId = recipeId,
            DietaryCategoryId = Guid.NewGuid(),
            CreatedByAppUserId = actorId
        });
        await ctx.SaveChangesAsync();

        Assert.True(result.Success);
        Assert.Equal(expectedCategoryId, result.Assignment!.DietaryCategoryId);
    }

    [Fact]
    public async Task WeeklyMenuRepository_GetWeeklyAssignmentByIdAsync_RespectsTenantScope()
    {
        await using var ctx = CreateContext();
        var repository = new WeeklyMenuRepository(ctx);

        var companyA = Guid.NewGuid();
        var companyB = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var assignmentId = Guid.NewGuid();

        ctx.Recipes.Add(new Recipe
        {
            Id = recipeId,
            CompanyId = companyA,
            CreatedByAppUserId = actorId,
            Name = "Tenant scoped recipe",
            DefaultServings = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        var menu = new WeeklyMenu
        {
            Id = Guid.NewGuid(),
            CompanyId = companyA,
            CreatedByAppUserId = actorId,
            WeekStartDate = new DateTime(2026, 03, 09, 0, 0, 0, DateTimeKind.Utc),
            SelectionDeadlineAt = new DateTime(2026, 03, 07, 0, 0, 0, DateTimeKind.Utc),
            TotalRecipes = 1,
            IsPublished = false,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        ctx.WeeklyMenus.Add(menu);

        ctx.WeeklyMenuRecipes.Add(new WeeklyMenuRecipe
        {
            Id = assignmentId,
            WeeklyMenuId = menu.Id,
            RecipeId = recipeId,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow
        });

        await ctx.SaveChangesAsync();

        var inScope = await repository.GetWeeklyAssignmentByIdAsync(companyA, assignmentId);
        var outOfScope = await repository.GetWeeklyAssignmentByIdAsync(companyB, assignmentId);

        Assert.NotNull(inScope);
        Assert.Null(outOfScope);
    }

    [Fact]
    public async Task WeeklyMenuService_RemoveWeeklyAssignmentAsync_SoftDeletesAndDecrementsTotal()
    {
        await using var ctx = CreateContext();
        var repository = new WeeklyMenuRepository(ctx);
        var service = new WeeklyMenuService(repository);

        var companyId = Guid.NewGuid();
        var otherCompanyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();

        ctx.Recipes.Add(new Recipe
        {
            Id = recipeId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Name = "Remove assignment recipe",
            DefaultServings = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        var menu = new WeeklyMenu
        {
            Id = Guid.NewGuid(),
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            WeekStartDate = new DateTime(2026, 03, 09, 0, 0, 0, DateTimeKind.Utc),
            SelectionDeadlineAt = new DateTime(2026, 03, 07, 0, 0, 0, DateTimeKind.Utc),
            TotalRecipes = 1,
            IsPublished = false,
            PublishedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        ctx.WeeklyMenus.Add(menu);

        var assignment = new WeeklyMenuRecipe
        {
            Id = Guid.NewGuid(),
            WeeklyMenuId = menu.Id,
            RecipeId = recipeId,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow
        };
        ctx.WeeklyMenuRecipes.Add(assignment);
        await ctx.SaveChangesAsync();

        var result = await service.RemoveWeeklyAssignmentAsync(companyId, assignment.Id);
        await ctx.SaveChangesAsync();

        Assert.True(result.Success);

        var storedAssignment = await ctx.WeeklyMenuRecipes.FirstAsync(x => x.Id == assignment.Id);
        var storedMenu = await ctx.WeeklyMenus.FirstAsync(x => x.Id == menu.Id);
        Assert.NotNull(storedAssignment.DeletedAt);
        Assert.Equal(0, storedMenu.TotalRecipes);

        var outOfScopeResult = await service.RemoveWeeklyAssignmentAsync(otherCompanyId, assignment.Id);
        Assert.False(outOfScopeResult.Success);
    }

    [Fact]
    public async Task RecipeService_UpsertRecipeEditorAsync_RemovesDeselectedLinksWithoutRecreatingKeptOnes()
    {
        await using var ctx = CreateContext();
        var repository = new RecipeRepository(ctx);
        var service = new RecipeService(repository);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var keepIngredientId = Guid.NewGuid();
        var removeIngredientId = Guid.NewGuid();
        var keepCategoryId = Guid.NewGuid();
        var removeCategoryId = Guid.NewGuid();

        ctx.Ingredients.AddRange(
            new Ingredient
            {
                Id = keepIngredientId,
                CompanyId = companyId,
                CreatedByAppUserId = actorId,
                Name = "Keep ingredient",
                IsExclusionTag = true,
                CreatedAt = DateTime.UtcNow
            },
            new Ingredient
            {
                Id = removeIngredientId,
                CompanyId = companyId,
                CreatedByAppUserId = actorId,
                Name = "Remove ingredient",
                IsExclusionTag = true,
                CreatedAt = DateTime.UtcNow
            });

        ctx.DietaryCategories.AddRange(
            new DietaryCategory
            {
                Id = keepCategoryId,
                CompanyId = companyId,
                CreatedByAppUserId = actorId,
                Code = "keep",
                Name = "Keep",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new DietaryCategory
            {
                Id = removeCategoryId,
                CompanyId = companyId,
                CreatedByAppUserId = actorId,
                Code = "remove",
                Name = "Remove",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

        ctx.Recipes.Add(new Recipe
        {
            Id = recipeId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Name = "Link replacement recipe",
            DefaultServings = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
        ctx.NutritionalInfos.Add(new NutritionalInfo
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            CaloriesKcal = 300,
            ProteinG = 20,
            CarbsG = 30,
            FatG = 10,
            FiberG = 5,
            SodiumMg = 400,
            CreatedAt = DateTime.UtcNow
        });
        ctx.RecipeIngredients.AddRange(
            new RecipeIngredient
            {
                Id = Guid.NewGuid(),
                RecipeId = recipeId,
                IngredientId = keepIngredientId,
                CreatedByAppUserId = actorId,
                CreatedAt = DateTime.UtcNow
            },
            new RecipeIngredient
            {
                Id = Guid.NewGuid(),
                RecipeId = recipeId,
                IngredientId = removeIngredientId,
                CreatedByAppUserId = actorId,
                CreatedAt = DateTime.UtcNow
            });
        ctx.RecipeDietaryCategories.AddRange(
            new RecipeDietaryCategory
            {
                Id = Guid.NewGuid(),
                RecipeId = recipeId,
                DietaryCategoryId = keepCategoryId,
                CreatedByAppUserId = actorId,
                CreatedAt = DateTime.UtcNow
            },
            new RecipeDietaryCategory
            {
                Id = Guid.NewGuid(),
                RecipeId = recipeId,
                DietaryCategoryId = removeCategoryId,
                CreatedByAppUserId = actorId,
                CreatedAt = DateTime.UtcNow
            });
        await ctx.SaveChangesAsync();

        await service.UpsertRecipeEditorAsync(companyId, actorId, new RecipeEditorUpsertDto
        {
            RecipeId = recipeId,
            Name = "Link replacement recipe",
            DefaultServings = 2,
            IsActive = true,
            IngredientIds = [keepIngredientId],
            DietaryCategoryIds = [keepCategoryId],
            Nutrition = new RecipeNutritionDto
            {
                CaloriesKcal = 320,
                ProteinG = 25,
                CarbsG = 35,
                FatG = 9,
                FiberG = 6,
                SodiumMg = 410
            }
        });
        await ctx.SaveChangesAsync();

        var ingredientLinks = await ctx.RecipeIngredients
            .Where(x => x.RecipeId == recipeId)
            .ToListAsync();
        var categoryLinks = await ctx.RecipeDietaryCategories
            .Where(x => x.RecipeId == recipeId)
            .ToListAsync();

        Assert.Equal(2, ingredientLinks.Count);
        Assert.Equal(1, ingredientLinks.Count(x => x.IngredientId == keepIngredientId && x.DeletedAt == null));
        Assert.Equal(1, ingredientLinks.Count(x => x.IngredientId == removeIngredientId && x.DeletedAt != null));

        Assert.Equal(2, categoryLinks.Count);
        Assert.Equal(1, categoryLinks.Count(x => x.DietaryCategoryId == keepCategoryId && x.DeletedAt == null));
        Assert.Equal(1, categoryLinks.Count(x => x.DietaryCategoryId == removeCategoryId && x.DeletedAt != null));
    }

    [Fact]
    public async Task RecipeService_UpsertRecipeEditorAsync_RemovesAllLinksWhenSelectionsAreNull()
    {
        await using var ctx = CreateContext();
        var repository = new RecipeRepository(ctx);
        var service = new RecipeService(repository);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        ctx.Ingredients.Add(new Ingredient
        {
            Id = ingredientId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Name = "Ingredient",
            IsExclusionTag = true,
            CreatedAt = DateTime.UtcNow
        });

        ctx.DietaryCategories.Add(new DietaryCategory
        {
            Id = categoryId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Code = "cat",
            Name = "Category",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        ctx.Recipes.Add(new Recipe
        {
            Id = recipeId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Name = "Null selection recipe",
            DefaultServings = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        ctx.NutritionalInfos.Add(new NutritionalInfo
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            CaloriesKcal = 300,
            ProteinG = 20,
            CarbsG = 30,
            FatG = 10,
            FiberG = 5,
            SodiumMg = 400,
            CreatedAt = DateTime.UtcNow
        });

        ctx.RecipeIngredients.Add(new RecipeIngredient
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            IngredientId = ingredientId,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow
        });

        ctx.RecipeDietaryCategories.Add(new RecipeDietaryCategory
        {
            Id = Guid.NewGuid(),
            RecipeId = recipeId,
            DietaryCategoryId = categoryId,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        await service.UpsertRecipeEditorAsync(companyId, actorId, new RecipeEditorUpsertDto
        {
            RecipeId = recipeId,
            Name = "Null selection recipe",
            DefaultServings = 2,
            IsActive = true,
            IngredientIds = null!,
            DietaryCategoryIds = null!,
            Nutrition = new RecipeNutritionDto
            {
                CaloriesKcal = 310,
                ProteinG = 21,
                CarbsG = 31,
                FatG = 11,
                FiberG = 6,
                SodiumMg = 410
            }
        });
        await ctx.SaveChangesAsync();

        var ingredientLink = await ctx.RecipeIngredients.SingleAsync(x => x.RecipeId == recipeId && x.IngredientId == ingredientId);
        var categoryLink = await ctx.RecipeDietaryCategories.SingleAsync(x => x.RecipeId == recipeId && x.DietaryCategoryId == categoryId);

        Assert.NotNull(ingredientLink.DeletedAt);
        Assert.NotNull(categoryLink.DeletedAt);
    }

    [Fact]
    public async Task IngredientService_UpsertCatalogItemAsync_DerivesExclusionKeyFromName()
    {
        await using var ctx = CreateContext();
        var repository = new IngredientRepository(ctx);
        var service = new IngredientService(repository);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();

        var item = await service.UpsertCatalogItemAsync(companyId, actorId, new IngredientCatalogUpsertDto
        {
            Name = "Cilantro",
            IsExclusionTag = true,
            ExclusionKey = null
        });

        Assert.Equal("cilantro", item.ExclusionKey);
    }

    [Fact]
    public async Task IngredientService_UpsertCatalogItemAsync_AlwaysMarksIngredientAsExclusionTag()
    {
        await using var ctx = CreateContext();
        var repository = new IngredientRepository(ctx);
        var service = new IngredientService(repository);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();

        var item = await service.UpsertCatalogItemAsync(companyId, actorId, new IngredientCatalogUpsertDto
        {
            Name = "Nut",
            IsAllergen = true,
            IsExclusionTag = false
        });

        Assert.True(item.IsExclusionTag);
        Assert.Equal("nut", item.ExclusionKey);
    }

    [Fact]
    public async Task DietaryCategoryService_UpsertCatalogItemAsync_DerivesCodeFromName()
    {
        await using var ctx = CreateContext();
        var repository = new DietaryCategoryRepository(ctx);
        var service = new DietaryCategoryService(repository);

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();

        var item = await service.UpsertCatalogItemAsync(companyId, actorId, new DietaryCategoryCatalogUpsertDto
        {
            Name = "Vegetarian",
            Code = string.Empty,
            IsActive = true
        });

        Assert.Equal("vegetarian", item.Code);
    }
}
