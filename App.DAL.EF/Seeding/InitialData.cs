using App.Domain.Core;
using App.Domain.Delivery;
using App.Domain.Identity;
using App.Domain.Menu;
using App.Domain.Subscription;

namespace App.DAL.EF.Seeding;

public static class InitialData
{
    public static readonly DateTime SeededAt = new(2026, 3, 1, 8, 0, 0, DateTimeKind.Utc);
    public static readonly DateTime WeekStartNorth = new(2026, 3, 9, 0, 0, 0, DateTimeKind.Utc);
    public static readonly DateTime WeekStartSouth = new(2026, 3, 9, 0, 0, 0, DateTimeKind.Utc);

    public static readonly Guid NorthBitesCompanyId = Guid.Parse("c0000000-0000-0000-0000-000000000001");
    public static readonly Guid SouthSpoonCompanyId = Guid.Parse("c0000000-0000-0000-0000-000000000002");

    public static readonly Guid CompanyRoleOwnerId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid CompanyRoleAdminId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid CompanyRoleManagerId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid CompanyRoleEmployeeId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid CompanyRoleSupportId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public static readonly Guid RootUserId = Guid.Parse("90000000-0000-0000-0000-000000000001");
    public static readonly Guid SysAdmin1UserId = Guid.Parse("90000000-0000-0000-0000-000000000011");
    public static readonly Guid SysAdmin2UserId = Guid.Parse("90000000-0000-0000-0000-000000000012");
    public static readonly Guid SysSupport1UserId = Guid.Parse("90000000-0000-0000-0000-000000000021");
    public static readonly Guid SysSupport2UserId = Guid.Parse("90000000-0000-0000-0000-000000000022");
    public static readonly Guid SysBilling1UserId = Guid.Parse("90000000-0000-0000-0000-000000000031");
    public static readonly Guid SysBilling2UserId = Guid.Parse("90000000-0000-0000-0000-000000000032");
    public static readonly Guid NorthOwnerUserId = Guid.Parse("90000000-0000-0000-0000-000000000101");
    public static readonly Guid NorthAdmin1UserId = Guid.Parse("90000000-0000-0000-0000-000000000111");
    public static readonly Guid NorthAdmin2UserId = Guid.Parse("90000000-0000-0000-0000-000000000112");
    public static readonly Guid NorthManager1UserId = Guid.Parse("90000000-0000-0000-0000-000000000121");
    public static readonly Guid NorthManager2UserId = Guid.Parse("90000000-0000-0000-0000-000000000122");
    public static readonly Guid NorthEmployee1UserId = Guid.Parse("90000000-0000-0000-0000-000000000131");
    public static readonly Guid NorthEmployee2UserId = Guid.Parse("90000000-0000-0000-0000-000000000132");
    public static readonly Guid Customer1UserId = Guid.Parse("90000000-0000-0000-0000-000000000201");
    public static readonly Guid Customer2UserId = Guid.Parse("90000000-0000-0000-0000-000000000202");
    public static readonly Guid SouthOwnerUserId = Guid.Parse("90000000-0000-0000-0000-000000000301");
    public static readonly Guid SouthAdmin1UserId = Guid.Parse("90000000-0000-0000-0000-000000000311");
    public static readonly Guid SouthAdmin2UserId = Guid.Parse("90000000-0000-0000-0000-000000000312");
    public static readonly Guid SouthManager1UserId = Guid.Parse("90000000-0000-0000-0000-000000000321");
    public static readonly Guid SouthManager2UserId = Guid.Parse("90000000-0000-0000-0000-000000000322");
    public static readonly Guid SouthEmployee1UserId = Guid.Parse("90000000-0000-0000-0000-000000000331");
    public static readonly Guid SouthEmployee2UserId = Guid.Parse("90000000-0000-0000-0000-000000000332");

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
            ("root@root.com", "Asd123!", RootUserId, ["admin", "root", "user", "SystemAdmin"]),
        ];
    
    // Lookup table seed data
    public static readonly CompanyRole[] CompanyRoles =
    [
        new() { Id = CompanyRoleOwnerId, Code = "owner", Label = "Owner" },
        new() { Id = CompanyRoleAdminId, Code = "admin", Label = "Admin" },
        new() { Id = CompanyRoleManagerId, Code = "manager", Label = "Manager" },
        new() { Id = CompanyRoleEmployeeId, Code = "employee", Label = "Employee" },
        new() { Id = CompanyRoleSupportId, Code = "support", Label = "Support Staff" },
    ];
    
    public static readonly PlatformSubscriptionTier[] PlatformSubscriptionTiers =
    [
        new() { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Code = "starter", Name = "Starter", MaxZones = 1, MaxSubscribers = 100, MaxEmployees = 5, MaxRecipes = 50, IsActive = true, CreatedAt = SeededAt },
        new() { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Code = "growth", Name = "Growth", MaxZones = 3, MaxSubscribers = 500, MaxEmployees = 15, MaxRecipes = 200, IsActive = true, CreatedAt = SeededAt },
        new() { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Code = "enterprise", Name = "Enterprise", MaxZones = 10, MaxSubscribers = 5000, MaxEmployees = 50, MaxRecipes = 1000, IsActive = true, CreatedAt = SeededAt },
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
