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
}
