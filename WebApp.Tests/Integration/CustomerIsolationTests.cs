using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using App.DAL.EF;
using App.Domain.Core;
using App.Domain.Delivery;
using App.Domain.Menu;
using App.Domain.Subscription;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApp.Tests.Helpers;
using Xunit;

namespace WebApp.Tests.Integration;

[Collection("Database tests")]
public class CustomerIsolationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CustomerIsolationTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        CustomWebApplicationFactory<Program>.UseTestAuth = true;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task ManageSubscriptions_ShowsOnlyOwnedSubscriptions()
    {
        var userId = Guid.NewGuid();
        CustomWebApplicationFactory<Program>.TestUserId = userId;
        CustomWebApplicationFactory<Program>.TestUserEmail = "customer-a@example.com";
        CustomWebApplicationFactory<Program>.TestUserRole = "Customer";

        var (ownSubscriptionId, otherSubscriptionId) = await SeedCustomerSubscriptionAsync(userId);

        var response = await _client.GetAsync("/customer/subscriptions/manage");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains(ownSubscriptionId.ToString(), html);
        Assert.DoesNotContain(otherSubscriptionId.ToString(), html);
    }

    [Fact]
    public async Task Deliveries_Index_ShowsOnlyOwnedDeliveries()
    {
        var userId = Guid.NewGuid();
        CustomWebApplicationFactory<Program>.TestUserId = userId;
        CustomWebApplicationFactory<Program>.TestUserEmail = "customer-b@example.com";
        CustomWebApplicationFactory<Program>.TestUserRole = "Customer";

        var (ownDeliveryId, otherDeliveryId) = await SeedCustomerDeliveryAsync(userId);

        var response = await _client.GetAsync("/customer/deliveries");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains(ownDeliveryId.ToString()[..8], html);
        Assert.DoesNotContain(otherDeliveryId.ToString()[..8], html);
    }

    [Fact]
    public async Task Complaints_Index_ShowsOnlyOwnedComplaints()
    {
        var userId = Guid.NewGuid();
        CustomWebApplicationFactory<Program>.TestUserId = userId;
        CustomWebApplicationFactory<Program>.TestUserEmail = "customer-c@example.com";
        CustomWebApplicationFactory<Program>.TestUserRole = "Customer";

        var (ownComplaintId, otherComplaintId) = await SeedCustomerComplaintAsync(userId);

        var response = await _client.GetAsync("/customer/complaints");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains(ownComplaintId.ToString()[..8], html);
        Assert.DoesNotContain(otherComplaintId.ToString()[..8], html);
    }

    private async Task<(Guid ownSubscriptionId, Guid otherSubscriptionId)> SeedCustomerSubscriptionAsync(Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var boxId = Guid.NewGuid();

        var customerId = Guid.NewGuid();
        var otherCustomerId = Guid.NewGuid();

        db.Customers.AddRange(
            new Customer
            {
                Id = customerId,
                CompanyId = companyId,
                Email = "customer-a@example.com",
                FirstName = "Customer",
                LastName = "A",
                IsActive = true,
                AddressLine = "Main",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE",
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                Id = otherCustomerId,
                CompanyId = companyId,
                Email = "customer-other@example.com",
                FirstName = "Customer",
                LastName = "Other",
                IsActive = true,
                AddressLine = "Main",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE",
                CreatedAt = DateTime.UtcNow
            });

        db.CustomerAppUsers.Add(new CustomerAppUser
        {
            Id = Guid.NewGuid(),
            AppUserId = userId,
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow
        });

        db.Boxes.Add(new Box
        {
            Id = boxId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            MealsCount = 3,
            PeopleCount = 2,
            DisplayName = "Standard",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        var ownSubscriptionId = Guid.NewGuid();
        var otherSubscriptionId = Guid.NewGuid();

        db.MealSubscriptions.AddRange(
            new MealSubscription
            {
                Id = ownSubscriptionId,
                CompanyId = companyId,
                CustomerId = customerId,
                BoxId = boxId,
                IsActive = true,
                StartDate = DateTime.UtcNow.AddDays(-10),
                AutoSelectEnabled = true,
                CreatedAt = DateTime.UtcNow
            },
            new MealSubscription
            {
                Id = otherSubscriptionId,
                CompanyId = companyId,
                CustomerId = otherCustomerId,
                BoxId = boxId,
                IsActive = true,
                StartDate = DateTime.UtcNow.AddDays(-5),
                AutoSelectEnabled = true,
                CreatedAt = DateTime.UtcNow
            });

        await db.SaveChangesAsync();
        return (ownSubscriptionId, otherSubscriptionId);
    }

    private async Task<(Guid ownDeliveryId, Guid otherDeliveryId)> SeedCustomerDeliveryAsync(Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var boxId = Guid.NewGuid();
        var deliveryStatusId = Guid.NewGuid();
        var weeklyMenuId = Guid.NewGuid();
        var deliveryZoneId = Guid.NewGuid();
        var deliveryWindowId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();

        var customerId = Guid.NewGuid();
        var otherCustomerId = Guid.NewGuid();

        db.DeliveryStatuses.Add(new DeliveryStatus
        {
            Id = deliveryStatusId,
            Code = "scheduled",
            Label = "Scheduled"
        });

        db.Customers.AddRange(
            new Customer
            {
                Id = customerId,
                CompanyId = companyId,
                Email = "customer-b@example.com",
                FirstName = "Customer",
                LastName = "B",
                IsActive = true,
                AddressLine = "Main",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE",
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                Id = otherCustomerId,
                CompanyId = companyId,
                Email = "customer-other@example.com",
                FirstName = "Customer",
                LastName = "Other",
                IsActive = true,
                AddressLine = "Main",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE",
                CreatedAt = DateTime.UtcNow
            });

        db.CustomerAppUsers.Add(new CustomerAppUser
        {
            Id = Guid.NewGuid(),
            AppUserId = userId,
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow
        });

        db.Boxes.Add(new Box
        {
            Id = boxId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            MealsCount = 3,
            PeopleCount = 2,
            DisplayName = "Standard",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        db.WeeklyMenus.Add(new WeeklyMenu
        {
            Id = weeklyMenuId,
            CompanyId = companyId,
            WeekStartDate = DateTime.UtcNow.Date,
            SelectionDeadlineAt = DateTime.UtcNow.Date.AddDays(-1),
            TotalRecipes = 1,
            IsPublished = true,
            PublishedAt = DateTime.UtcNow.AddDays(-2),
            CreatedAt = DateTime.UtcNow,
            CreatedByAppUserId = actorId
        });

        db.DeliveryZones.Add(new DeliveryZone
        {
            Id = deliveryZoneId,
            CompanyId = companyId,
            Name = "Zone A",
            IsActive = true,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow
        });

        db.DeliveryWindows.Add(new DeliveryWindow
        {
            Id = deliveryWindowId,
            DeliveryZoneId = deliveryZoneId,
            DayOfWeek = (int)DayOfWeek.Monday,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(10, 0, 0),
            Capacity = 10,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedByAppUserId = actorId
        });

        db.Recipes.Add(new Recipe
        {
            Id = recipeId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Name = "Recipe",
            DefaultServings = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        var ownSubscriptionId = Guid.NewGuid();
        var otherSubscriptionId = Guid.NewGuid();

        db.MealSubscriptions.AddRange(
            new MealSubscription
            {
                Id = ownSubscriptionId,
                CompanyId = companyId,
                CustomerId = customerId,
                BoxId = boxId,
                IsActive = true,
                StartDate = DateTime.UtcNow.AddDays(-10),
                AutoSelectEnabled = true,
                CreatedAt = DateTime.UtcNow
            },
            new MealSubscription
            {
                Id = otherSubscriptionId,
                CompanyId = companyId,
                CustomerId = otherCustomerId,
                BoxId = boxId,
                IsActive = true,
                StartDate = DateTime.UtcNow.AddDays(-10),
                AutoSelectEnabled = true,
                CreatedAt = DateTime.UtcNow
            });

        var ownSelectionId = Guid.NewGuid();
        var otherSelectionId = Guid.NewGuid();

        db.MealSelections.AddRange(
            new MealSelection
            {
                Id = ownSelectionId,
                MealSubscriptionId = ownSubscriptionId,
                WeeklyMenuId = weeklyMenuId,
                RecipeId = recipeId,
                SelectedAutomatically = true,
                SelectedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            },
            new MealSelection
            {
                Id = otherSelectionId,
                MealSubscriptionId = otherSubscriptionId,
                WeeklyMenuId = weeklyMenuId,
                RecipeId = recipeId,
                SelectedAutomatically = true,
                SelectedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });

        var ownDeliveryId = Guid.NewGuid();
        var otherDeliveryId = Guid.NewGuid();

        db.Deliveries.AddRange(
            new Delivery
            {
                Id = ownDeliveryId,
                CompanyId = companyId,
                DeliveryStatusId = deliveryStatusId,
                CustomerId = customerId,
                WeeklyMenuId = weeklyMenuId,
                DeliveryZoneId = deliveryZoneId,
                DeliveryWindowId = deliveryWindowId,
                BoxId = boxId,
                MealSelectionId = ownSelectionId,
                MealSubscriptionId = ownSubscriptionId,
                ScheduledTime = DateTime.UtcNow.Date.AddDays(1),
                AddressLine = "Main",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE",
                CreatedAt = DateTime.UtcNow,
                CreatedByAppUserId = actorId
            },
            new Delivery
            {
                Id = otherDeliveryId,
                CompanyId = companyId,
                DeliveryStatusId = deliveryStatusId,
                CustomerId = otherCustomerId,
                WeeklyMenuId = weeklyMenuId,
                DeliveryZoneId = deliveryZoneId,
                DeliveryWindowId = deliveryWindowId,
                BoxId = boxId,
                MealSelectionId = otherSelectionId,
                MealSubscriptionId = otherSubscriptionId,
                ScheduledTime = DateTime.UtcNow.Date.AddDays(1),
                AddressLine = "Main",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE",
                CreatedAt = DateTime.UtcNow,
                CreatedByAppUserId = actorId
            });

        await db.SaveChangesAsync();
        return (ownDeliveryId, otherDeliveryId);
    }

    private async Task<(Guid ownComplaintId, Guid otherComplaintId)> SeedCustomerComplaintAsync(Guid userId)
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var companyId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var boxId = Guid.NewGuid();
        var deliveryStatusId = Guid.NewGuid();
        var complaintTypeId = Guid.NewGuid();
        var complaintStatusId = Guid.NewGuid();
        var weeklyMenuId = Guid.NewGuid();
        var deliveryZoneId = Guid.NewGuid();
        var deliveryWindowId = Guid.NewGuid();
        var recipeId = Guid.NewGuid();

        var customerId = Guid.NewGuid();
        var otherCustomerId = Guid.NewGuid();

        db.DeliveryStatuses.Add(new DeliveryStatus
        {
            Id = deliveryStatusId,
            Code = "delivered",
            Label = "Delivered"
        });
        db.QualityComplaintTypes.Add(new QualityComplaintType
        {
            Id = complaintTypeId,
            Code = "quality",
            Label = "Quality"
        });
        db.QualityComplaintStatuses.Add(new QualityComplaintStatus
        {
            Id = complaintStatusId,
            Code = "open",
            Label = "Open"
        });

        db.Customers.AddRange(
            new Customer
            {
                Id = customerId,
                CompanyId = companyId,
                Email = "customer-c@example.com",
                FirstName = "Customer",
                LastName = "C",
                IsActive = true,
                AddressLine = "Main",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE",
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                Id = otherCustomerId,
                CompanyId = companyId,
                Email = "customer-other@example.com",
                FirstName = "Customer",
                LastName = "Other",
                IsActive = true,
                AddressLine = "Main",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE",
                CreatedAt = DateTime.UtcNow
            });

        db.CustomerAppUsers.Add(new CustomerAppUser
        {
            Id = Guid.NewGuid(),
            AppUserId = userId,
            CustomerId = customerId,
            CreatedAt = DateTime.UtcNow
        });

        db.Boxes.Add(new Box
        {
            Id = boxId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            MealsCount = 3,
            PeopleCount = 2,
            DisplayName = "Standard",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        db.WeeklyMenus.Add(new WeeklyMenu
        {
            Id = weeklyMenuId,
            CompanyId = companyId,
            WeekStartDate = DateTime.UtcNow.Date,
            SelectionDeadlineAt = DateTime.UtcNow.Date.AddDays(-1),
            TotalRecipes = 1,
            IsPublished = true,
            PublishedAt = DateTime.UtcNow.AddDays(-2),
            CreatedAt = DateTime.UtcNow,
            CreatedByAppUserId = actorId
        });

        db.DeliveryZones.Add(new DeliveryZone
        {
            Id = deliveryZoneId,
            CompanyId = companyId,
            Name = "Zone A",
            IsActive = true,
            CreatedByAppUserId = actorId,
            CreatedAt = DateTime.UtcNow
        });

        db.DeliveryWindows.Add(new DeliveryWindow
        {
            Id = deliveryWindowId,
            DeliveryZoneId = deliveryZoneId,
            DayOfWeek = (int)DayOfWeek.Monday,
            StartTime = new TimeSpan(8, 0, 0),
            EndTime = new TimeSpan(10, 0, 0),
            Capacity = 10,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            CreatedByAppUserId = actorId
        });

        db.Recipes.Add(new Recipe
        {
            Id = recipeId,
            CompanyId = companyId,
            CreatedByAppUserId = actorId,
            Name = "Recipe",
            DefaultServings = 2,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        var ownSubscriptionId = Guid.NewGuid();
        var otherSubscriptionId = Guid.NewGuid();

        db.MealSubscriptions.AddRange(
            new MealSubscription
            {
                Id = ownSubscriptionId,
                CompanyId = companyId,
                CustomerId = customerId,
                BoxId = boxId,
                IsActive = true,
                StartDate = DateTime.UtcNow.AddDays(-10),
                AutoSelectEnabled = true,
                CreatedAt = DateTime.UtcNow
            },
            new MealSubscription
            {
                Id = otherSubscriptionId,
                CompanyId = companyId,
                CustomerId = otherCustomerId,
                BoxId = boxId,
                IsActive = true,
                StartDate = DateTime.UtcNow.AddDays(-10),
                AutoSelectEnabled = true,
                CreatedAt = DateTime.UtcNow
            });

        var ownSelectionId = Guid.NewGuid();
        var otherSelectionId = Guid.NewGuid();

        db.MealSelections.AddRange(
            new MealSelection
            {
                Id = ownSelectionId,
                MealSubscriptionId = ownSubscriptionId,
                WeeklyMenuId = weeklyMenuId,
                RecipeId = recipeId,
                SelectedAutomatically = true,
                SelectedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            },
            new MealSelection
            {
                Id = otherSelectionId,
                MealSubscriptionId = otherSubscriptionId,
                WeeklyMenuId = weeklyMenuId,
                RecipeId = recipeId,
                SelectedAutomatically = true,
                SelectedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            });

        var ownDeliveryId = Guid.NewGuid();
        var otherDeliveryId = Guid.NewGuid();

        db.Deliveries.AddRange(
            new Delivery
            {
                Id = ownDeliveryId,
                CompanyId = companyId,
                DeliveryStatusId = deliveryStatusId,
                CustomerId = customerId,
                WeeklyMenuId = weeklyMenuId,
                DeliveryZoneId = deliveryZoneId,
                DeliveryWindowId = deliveryWindowId,
                BoxId = boxId,
                MealSelectionId = ownSelectionId,
                MealSubscriptionId = ownSubscriptionId,
                ScheduledTime = DateTime.UtcNow.Date.AddDays(1),
                AddressLine = "Main",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE",
                CreatedAt = DateTime.UtcNow,
                CreatedByAppUserId = actorId
            },
            new Delivery
            {
                Id = otherDeliveryId,
                CompanyId = companyId,
                DeliveryStatusId = deliveryStatusId,
                CustomerId = otherCustomerId,
                WeeklyMenuId = weeklyMenuId,
                DeliveryZoneId = deliveryZoneId,
                DeliveryWindowId = deliveryWindowId,
                BoxId = boxId,
                MealSelectionId = otherSelectionId,
                MealSubscriptionId = otherSubscriptionId,
                ScheduledTime = DateTime.UtcNow.Date.AddDays(1),
                AddressLine = "Main",
                City = "Tallinn",
                PostalCode = "10111",
                Country = "EE",
                CreatedAt = DateTime.UtcNow,
                CreatedByAppUserId = actorId
            });

        var ownComplaintId = Guid.NewGuid();
        var otherComplaintId = Guid.NewGuid();

        db.QualityComplaints.AddRange(
            new QualityComplaint
            {
                Id = ownComplaintId,
                CompanyId = companyId,
                CustomerId = customerId,
                DeliveryId = ownDeliveryId,
                QualityComplaintTypeId = complaintTypeId,
                QualityComplaintStatusId = complaintStatusId,
                Severity = 2,
                Description = "Issue",
                CreatedAt = DateTime.UtcNow
            },
            new QualityComplaint
            {
                Id = otherComplaintId,
                CompanyId = companyId,
                CustomerId = otherCustomerId,
                DeliveryId = otherDeliveryId,
                QualityComplaintTypeId = complaintTypeId,
                QualityComplaintStatusId = complaintStatusId,
                Severity = 3,
                Description = "Other Issue",
                CreatedAt = DateTime.UtcNow
            });

        await db.SaveChangesAsync();
        return (ownComplaintId, otherComplaintId);
    }
}
