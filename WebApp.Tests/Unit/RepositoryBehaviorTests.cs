using App.DAL.EF;
using App.DAL.EF.Repositories.Core;
using App.DAL.EF.Repositories.Delivery;
using App.DAL.EF.Repositories.Identity;
using App.DAL.EF.Repositories.Subscription;
using App.BLL.Delivery;
using App.BLL.Subscription;
using App.Domain.Core;
using App.Domain.Delivery;
using App.Domain.Identity;
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
}
