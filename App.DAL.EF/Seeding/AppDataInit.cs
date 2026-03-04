using App.Domain;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Seeding;

public static class AppDataInit
{
    public static void SeedAppData(AppDbContext context)
    {
        SeedLookupTables(context);
    }
    
    public static void SeedLookupTables(AppDbContext context)
    {
        // Seed CompanyRoles
        foreach (var role in InitialData.CompanyRoles)
        {
            if (!context.CompanyRoles.Any(r => r.Code == role.Code))
            {
                context.CompanyRoles.Add(role);
            }
        }
        
        // Seed PlatformSubscriptionTiers
        foreach (var tier in InitialData.PlatformSubscriptionTiers)
        {
            if (!context.PlatformSubscriptionTiers.Any(t => t.Code == tier.Code))
            {
                context.PlatformSubscriptionTiers.Add(tier);
            }
        }
        
        // Seed PlatformSubscriptionStatuses
        foreach (var status in InitialData.PlatformSubscriptionStatuses)
        {
            if (!context.PlatformSubscriptionStatuses.Any(s => s.Code == status.Code))
            {
                context.PlatformSubscriptionStatuses.Add(status);
            }
        }
        
        // Seed DeliveryStatuses
        foreach (var status in InitialData.DeliveryStatuses)
        {
            if (!context.DeliveryStatuses.Any(s => s.Code == status.Code))
            {
                context.DeliveryStatuses.Add(status);
            }
        }
        
        // Seed DeliveryAttemptResults
        foreach (var result in InitialData.DeliveryAttemptResults)
        {
            if (!context.DeliveryAttemptResults.Any(r => r.Code == result.Code))
            {
                context.DeliveryAttemptResults.Add(result);
            }
        }
        
        // Seed QualityComplaintTypes
        foreach (var type in InitialData.QualityComplaintTypes)
        {
            if (!context.QualityComplaintTypes.Any(t => t.Code == type.Code))
            {
                context.QualityComplaintTypes.Add(type);
            }
        }
        
        // Seed QualityComplaintStatuses
        foreach (var status in InitialData.QualityComplaintStatuses)
        {
            if (!context.QualityComplaintStatuses.Any(s => s.Code == status.Code))
            {
                context.QualityComplaintStatuses.Add(status);
            }
        }
        
        context.SaveChanges();
    }


    public static void MigrateDatabase(AppDbContext context)
    {
        context.Database.Migrate();
    }

    public static void DeleteDatabase(AppDbContext context)
    {
        context.Database.EnsureDeleted();
    }

    public static void SeedIdentity(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        foreach (var (roleName, id) in InitialData.Roles)
        {
            var role = roleManager.FindByNameAsync(roleName).Result;

            if (role != null) continue;

            role = new AppRole()
            {
                Name = roleName,
            };

            var result = roleManager.CreateAsync(role).Result;
            if (!result.Succeeded)
            {
                throw new ApplicationException("Role creation failed!");
            }
        }


        foreach (var userInfo in InitialData.Users)
        {
            var user = userManager.FindByEmailAsync(userInfo.name).Result;
            if (user == null)
            {
                user = new AppUser()
                {
                    Email = userInfo.name,
                    UserName = userInfo.name,
                    EmailConfirmed = true,
                    FirstName = userInfo.name,
                    LastName = "User"
                };
                var result = userManager.CreateAsync(user, userInfo.password).Result;
                if (!result.Succeeded)
                {
                    throw new ApplicationException("User creation failed!");
                }
            }

            foreach (var role in userInfo.roles)
            {
                if (userManager.IsInRoleAsync(user, role).Result)
                {
                    Console.WriteLine($"User {user.UserName} already in role {role}");
                    continue;
                }

                var roleResult = userManager.AddToRoleAsync(user, role).Result;
                if (!roleResult.Succeeded)
                {
                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
                else
                {
                    Console.WriteLine($"User {user.UserName} added to role {role}");
                }
            }
        }
    }
}