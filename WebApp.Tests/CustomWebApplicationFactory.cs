using System;
using System.Linq;
using App.DAL.EF;
using App.Domain.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using WebApp.Tests.Helpers;

namespace WebApp.Tests;

public class CustomWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup: class
{
    public static Guid? TestUserId { get; set; }
    public static string? TestUserEmail { get; set; }
    public static string? TestUserRole { get; set; }
    public static bool UseTestAuth { get; set; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // find DbContextOptions
            var descriptorDbContextOptions = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<AppDbContext>));

            // if found - remove
            if (descriptorDbContextOptions != null)
            {
                services.Remove(descriptorDbContextOptions);
            }
            // TODO: Use postgres test db in docker, inmemory flacky
            // add new DbContextOptions
            var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("InMemoryDbForTesting");
            services.AddScoped<DbContextOptions<AppDbContext>>(_ => contextOptions.Options);

            
            // create db and seed data
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();
            var logger = scopedServices
                .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

            db.Database.EnsureCreated();

            try
            { 
                DataSeeder.SeedData(db);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred seeding the " +
                                    "database with test data. Error: {Message}", ex.Message);
            }

            if (TestUserId.HasValue && !string.IsNullOrWhiteSpace(TestUserEmail))
            {
                SeedTestUser(db, TestUserId.Value, TestUserEmail!, TestUserRole ?? "Customer");
            }
        });


        builder.ConfigureServices(services =>
        {
            if (!UseTestAuth)
            {
                return;
            }

            services.TryAddSingleton<IAuthenticationSchemeProvider, TestSchemeProvider>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                    options.DefaultChallengeScheme = TestAuthHandler.Scheme;
                    options.DefaultScheme = TestAuthHandler.Scheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CustomerAccess", policy =>
                    policy.RequireRole("Customer"));
                options.AddPolicy("CompanyOwner", policy =>
                    policy.RequireRole("CompanyOwner"));
                options.AddPolicy("CompanyAdmin", policy =>
                    policy.RequireRole("CompanyOwner", "CompanyAdmin"));
                options.AddPolicy("CompanyManager", policy =>
                    policy.RequireRole("CompanyOwner", "CompanyAdmin", "CompanyManager"));
                options.AddPolicy("CompanyEmployee", policy =>
                    policy.RequireRole("CompanyOwner", "CompanyAdmin", "CompanyManager", "CompanyEmployee"));
                options.AddPolicy("SystemAdmin", policy =>
                    policy.RequireRole("SystemAdmin"));
                options.AddPolicy("SystemBilling", policy =>
                    policy.RequireRole("SystemAdmin", "SystemBilling"));
                options.AddPolicy("SystemSupport", policy =>
                    policy.RequireRole("SystemAdmin", "SystemSupport"));
            });
        });
    }

    private static void SeedTestUser(AppDbContext db, Guid userId, string email, string role)
    {
        if (!db.Users.Any(u => u.Id == userId))
        {
            db.Users.Add(new AppUser
            {
                Id = userId,
                Email = email,
                UserName = email,
                NormalizedEmail = email.ToUpperInvariant(),
                NormalizedUserName = email.ToUpperInvariant(),
                FirstName = "Test",
                LastName = "User",
                EmailConfirmed = true
            });
        }

        var roleEntity = db.Roles.FirstOrDefault(r => r.Name == role);
        if (roleEntity == null)
        {
            roleEntity = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = role,
                NormalizedName = role.ToUpperInvariant()
            };
            db.Roles.Add(roleEntity);
        }

        if (!db.UserRoles.Any(ur => ur.UserId == userId && ur.RoleId == roleEntity.Id))
        {
            db.UserRoles.Add(new IdentityUserRole<Guid>
            {
                UserId = userId,
                RoleId = roleEntity.Id
            });
        }

        db.SaveChanges();
    }
}
