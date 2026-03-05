using App.Domain.Core;
using App.Domain.Delivery;
using App.Domain.Identity;
using App.Domain.Subscription;

namespace App.DAL.EF.Seeding;

public static class InitialData
{
    public static readonly (string roleName, Guid? id)[]
        Roles =
        [
            ("admin", null),
            ("user", null),
            ("root", null),
            ("SystemAdmin", null),
            ("SystemSupport", null),
            ("SystemBilling", null),
            ("CompanyOwner", null),
            ("CompanyAdmin", null),
            ("CompanyManager", null),
            ("CompanyEmployee", null),
            ("Customer", null),
        ];

    public static readonly (string name, string password, Guid? id, string[] roles)[]
        Users =
        [
            ("root@root.com", "Kala.Maja.101", null, ["admin", "root", "user", "SystemAdmin"]),

            // 2 users for every system role
            ("sysadmin1@platform.local", "Kala.Maja.101", null, ["user", "SystemAdmin"]),
            ("sysadmin2@platform.local", "Kala.Maja.101", null, ["user", "SystemAdmin"]),
            ("syssupport1@platform.local", "Kala.Maja.101", null, ["user", "SystemSupport"]),
            ("syssupport2@platform.local", "Kala.Maja.101", null, ["user", "SystemSupport"]),
            ("sysbilling1@platform.local", "Kala.Maja.101", null, ["user", "SystemBilling"]),
            ("sysbilling2@platform.local", "Kala.Maja.101", null, ["user", "SystemBilling"]),

            // Company 1 users: 1 owner, 2 admins, 2 managers, 2 employees
            ("owner@northbites.local", "Kala.Maja.101", null, ["user", "CompanyOwner"]),
            ("admin1@northbites.local", "Kala.Maja.101", null, ["user", "CompanyAdmin"]),
            ("admin2@northbites.local", "Kala.Maja.101", null, ["user", "CompanyAdmin"]),
            ("manager1@northbites.local", "Kala.Maja.101", null, ["user", "CompanyManager"]),
            ("manager2@northbites.local", "Kala.Maja.101", null, ["user", "CompanyManager"]),
            ("employee1@northbites.local", "Kala.Maja.101", null, ["user", "CompanyEmployee"]),
            ("employee2@northbites.local", "Kala.Maja.101", null, ["user", "CompanyEmployee"]),

            // Customer-only users
            ("customer1@platform.local", "Kala.Maja.101", null, ["user", "Customer"]),
            ("customer2@platform.local", "Kala.Maja.101", null, ["user", "Customer"]),

            // Company 2 users: 1 owner, 2 admins, 2 managers, 2 employees
            ("owner@southspoon.local", "Kala.Maja.101", null, ["user", "CompanyOwner"]),
            ("admin1@southspoon.local", "Kala.Maja.101", null, ["user", "CompanyAdmin"]),
            ("admin2@southspoon.local", "Kala.Maja.101", null, ["user", "CompanyAdmin"]),
            ("manager1@southspoon.local", "Kala.Maja.101", null, ["user", "CompanyManager"]),
            ("manager2@southspoon.local", "Kala.Maja.101", null, ["user", "CompanyManager"]),
            ("employee1@southspoon.local", "Kala.Maja.101", null, ["user", "CompanyEmployee"]),
            ("employee2@southspoon.local", "Kala.Maja.101", null, ["user", "CompanyEmployee"])
        ];

    public static readonly (string name, string slug, string registrationNumber, string contactEmail, string contactPhone, string webSiteUrl)[]
        Companies =
        [
            ("North Bites", "north-bites", "EE10000001", "info@northbites.local", "+3725000001", "https://northbites.local"),
            ("South Spoon", "south-spoon", "EE10000002", "info@southspoon.local", "+3725000002", "https://southspoon.local")
        ];
    
    // Lookup table seed data
    public static readonly CompanyRole[] CompanyRoles =
    [
        new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Code = "owner", Label = "Owner" },
        new() { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Code = "admin", Label = "Admin" },
        new() { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Code = "manager", Label = "Manager" },
        new() { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Code = "employee", Label = "Employee" },
        new() { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Code = "support", Label = "Support Staff" },
    ];
    
    public static readonly PlatformSubscriptionTier[] PlatformSubscriptionTiers =
    [
        new() { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Code = "starter", Name = "Starter", MaxZones = 1, MaxSubscribers = 100, MaxEmployees = 5, MaxRecipes = 50, IsActive = true, CreatedAt = DateTime.UtcNow },
        new() { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Code = "growth", Name = "Growth", MaxZones = 3, MaxSubscribers = 500, MaxEmployees = 15, MaxRecipes = 200, IsActive = true, CreatedAt = DateTime.UtcNow },
        new() { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Code = "enterprise", Name = "Enterprise", MaxZones = 10, MaxSubscribers = 5000, MaxEmployees = 50, MaxRecipes = 1000, IsActive = true, CreatedAt = DateTime.UtcNow },
    ];
    
    public static readonly PlatformSubscriptionStatus[] PlatformSubscriptionStatuses =
    [
        new() { Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"), Code = "active", Label = "Active" },
        new() { Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), Code = "suspended", Label = "Suspended" },
        new() { Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff"), Code = "cancelled", Label = "Cancelled" },
        new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Code = "pending", Label = "Pending" },
    ];
    
    public static readonly DeliveryStatus[] DeliveryStatuses =
    [
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Code = "scheduled", Label = "Scheduled" },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Code = "in_transit", Label = "In Transit" },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Code = "delivered", Label = "Delivered" },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Code = "failed", Label = "Failed" },
        new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Code = "cancelled", Label = "Cancelled" },
    ];
    
    public static readonly DeliveryAttemptResult[] DeliveryAttemptResults =
    [
        new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Code = "success", Label = "Success" },
        new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Code = "customer_not_home", Label = "Customer Not Home" },
        new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Code = "address_not_found", Label = "Address Not Found" },
        new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Code = "access_denied", Label = "Access Denied" },
        new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), Code = "vehicle_issue", Label = "Vehicle Issue" },
        new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000006"), Code = "weather", Label = "Weather Conditions" },
    ];
    
    public static readonly QualityComplaintType[] QualityComplaintTypes =
    [
        new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), Code = "spoiled_food", Label = "Spoiled Food" },
        new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), Code = "missing_items", Label = "Missing Items" },
        new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000003"), Code = "wrong_recipe", Label = "Wrong Recipe" },
        new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000004"), Code = "packaging_issue", Label = "Packaging Issue" },
        new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000005"), Code = "temperature", Label = "Temperature Issue" },
        new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000006"), Code = "quality", Label = "General Quality" },
    ];
    
    public static readonly QualityComplaintStatus[] QualityComplaintStatuses =
    [
        new() { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), Code = "open", Label = "Open" },
        new() { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), Code = "in_review", Label = "In Review" },
        new() { Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), Code = "resolved", Label = "Resolved" },
        new() { Id = Guid.Parse("40000000-0000-0000-0000-000000000004"), Code = "escalated", Label = "Escalated" },
        new() { Id = Guid.Parse("40000000-0000-0000-0000-000000000005"), Code = "closed", Label = "Closed" },
    ];
}
