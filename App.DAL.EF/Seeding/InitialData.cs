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

            // 2 users for every system role
            ("sysadmin1@platform.local", "Asd123!", SysAdmin1UserId, ["user", "SystemAdmin"]),
            ("sysadmin2@platform.local", "Asd123!", SysAdmin2UserId, ["user", "SystemAdmin"]),
            ("syssupport1@platform.local", "Asd123!", SysSupport1UserId, ["user", "SystemSupport"]),
            ("syssupport2@platform.local", "Asd123!", SysSupport2UserId, ["user", "SystemSupport"]),
            ("sysbilling1@platform.local", "Asd123!", SysBilling1UserId, ["user", "SystemBilling"]),
            ("sysbilling2@platform.local", "Asd123!", SysBilling2UserId, ["user", "SystemBilling"]),

            // Company 1 users: 1 owner, 2 admins, 2 managers, 2 employees
            ("owner@northbites.local", "Asd123!", NorthOwnerUserId, ["user", "CompanyOwner"]),
            ("admin1@northbites.local", "Asd123!", NorthAdmin1UserId, ["user", "CompanyAdmin"]),
            ("admin2@northbites.local", "Asd123!", NorthAdmin2UserId, ["user", "CompanyAdmin"]),
            ("manager1@northbites.local", "Asd123!", NorthManager1UserId, ["user", "CompanyManager"]),
            ("manager2@northbites.local", "Asd123!", NorthManager2UserId, ["user", "CompanyManager"]),
            ("employee1@northbites.local", "Asd123!", NorthEmployee1UserId, ["user", "CompanyEmployee"]),
            ("employee2@northbites.local", "Asd123!", NorthEmployee2UserId, ["user", "CompanyEmployee"]),

            // Customer-only users
            ("customer1@platform.local", "Asd123!", Customer1UserId, ["user", "Customer"]),
            ("customer2@platform.local", "Asd123!", Customer2UserId, ["user", "Customer"]),

            // Company 2 users: 1 owner, 2 admins, 2 managers, 2 employees
            ("owner@southspoon.local", "Asd123!", SouthOwnerUserId, ["user", "CompanyOwner"]),
            ("admin1@southspoon.local", "Asd123!", SouthAdmin1UserId, ["user", "CompanyAdmin"]),
            ("admin2@southspoon.local", "Asd123!", SouthAdmin2UserId, ["user", "CompanyAdmin"]),
            ("manager1@southspoon.local", "Asd123!", SouthManager1UserId, ["user", "CompanyManager"]),
            ("manager2@southspoon.local", "Asd123!", SouthManager2UserId, ["user", "CompanyManager"]),
            ("employee1@southspoon.local", "Asd123!", SouthEmployee1UserId, ["user", "CompanyEmployee"]),
            ("employee2@southspoon.local", "Asd123!", SouthEmployee2UserId, ["user", "CompanyEmployee"])
        ];

    // Company seed data
    public static readonly Company[] Companies =
    [
        new() { Id = NorthBitesCompanyId, Name = "North Bites", Slug = "north-bites", RegistrationNumber = "EE10000001", ContactEmail = "info@northbites.local", ContactPhone = "+3725000001", WebSiteUrl = "https://northbites.local", CreatedAt = SeededAt },
        new() { Id = SouthSpoonCompanyId, Name = "South Spoon", Slug = "south-spoon", RegistrationNumber = "EE10000002", ContactEmail = "info@southspoon.local", ContactPhone = "+3725000002", WebSiteUrl = "https://southspoon.local", CreatedAt = SeededAt }
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

    public static readonly CompanySettings[] CompanySettings =
    [
        new()
        {
            Id = Guid.Parse("51000000-0000-0000-0000-000000000001"),
            CompanyId = NorthBitesCompanyId,
            DefaultNoRepeatWeeks = 6,
            SelectionDeadlineDaysBeforeDelivery = 2,
            AllowAutoSelection = true,
            AllowPauseSubscription = true,
            AllowSkipWeek = true,
            MinimumSubscriptionWeeks = 4,
            MaxDeliveryAttempts = 2,
            AllowRedeliveryAfterFailure = true,
            ComplaintEscalationThreshold = 3,
            ComplaintEscalationDaysWindow = 7,
            AutoPrioritizeFreshestStock = true,
            AutoAssignEarliestSlot = true,
            UpdatedAt = SeededAt,
            UpdatedByAppUserId = NorthOwnerUserId
        },
        new()
        {
            Id = Guid.Parse("51000000-0000-0000-0000-000000000002"),
            CompanyId = SouthSpoonCompanyId,
            DefaultNoRepeatWeeks = 8,
            SelectionDeadlineDaysBeforeDelivery = 3,
            AllowAutoSelection = true,
            AllowPauseSubscription = true,
            AllowSkipWeek = false,
            MinimumSubscriptionWeeks = 6,
            MaxDeliveryAttempts = 3,
            AllowRedeliveryAfterFailure = false,
            ComplaintEscalationThreshold = 2,
            ComplaintEscalationDaysWindow = 5,
            AutoPrioritizeFreshestStock = true,
            AutoAssignEarliestSlot = false,
            UpdatedAt = SeededAt,
            UpdatedByAppUserId = SouthOwnerUserId
        }
    ];

    public static readonly CompanyAppUser[] CompanyAppUsers =
    [
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, AppUserId = NorthOwnerUserId, CompanyRoleId = CompanyRoleOwnerId, IsOwner = true, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthOwnerUserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000002"), CompanyId = NorthBitesCompanyId, AppUserId = NorthAdmin1UserId, CompanyRoleId = CompanyRoleAdminId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthOwnerUserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000003"), CompanyId = NorthBitesCompanyId, AppUserId = NorthAdmin2UserId, CompanyRoleId = CompanyRoleAdminId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthOwnerUserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000004"), CompanyId = NorthBitesCompanyId, AppUserId = NorthManager1UserId, CompanyRoleId = CompanyRoleManagerId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000005"), CompanyId = NorthBitesCompanyId, AppUserId = NorthManager2UserId, CompanyRoleId = CompanyRoleManagerId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000006"), CompanyId = NorthBitesCompanyId, AppUserId = NorthEmployee1UserId, CompanyRoleId = CompanyRoleEmployeeId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthManager1UserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000007"), CompanyId = NorthBitesCompanyId, AppUserId = NorthEmployee2UserId, CompanyRoleId = CompanyRoleEmployeeId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthManager1UserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000008"), CompanyId = SouthSpoonCompanyId, AppUserId = SouthOwnerUserId, CompanyRoleId = CompanyRoleOwnerId, IsOwner = true, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthOwnerUserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000009"), CompanyId = SouthSpoonCompanyId, AppUserId = SouthAdmin1UserId, CompanyRoleId = CompanyRoleAdminId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthOwnerUserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000010"), CompanyId = SouthSpoonCompanyId, AppUserId = SouthAdmin2UserId, CompanyRoleId = CompanyRoleAdminId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthOwnerUserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000011"), CompanyId = SouthSpoonCompanyId, AppUserId = SouthManager1UserId, CompanyRoleId = CompanyRoleManagerId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000012"), CompanyId = SouthSpoonCompanyId, AppUserId = SouthManager2UserId, CompanyRoleId = CompanyRoleManagerId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000013"), CompanyId = SouthSpoonCompanyId, AppUserId = SouthEmployee1UserId, CompanyRoleId = CompanyRoleEmployeeId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthManager1UserId },
        new() { Id = Guid.Parse("52000000-0000-0000-0000-000000000014"), CompanyId = SouthSpoonCompanyId, AppUserId = SouthEmployee2UserId, CompanyRoleId = CompanyRoleEmployeeId, IsOwner = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthManager1UserId }
    ];

    public static readonly Customer[] Customers =
    [
        new()
        {
            Id = Guid.Parse("53000000-0000-0000-0000-000000000001"),
            CompanyId = NorthBitesCompanyId,
            Email = "julia.north@client.local",
            FirstName = "Julia",
            LastName = "North",
            PhoneNumber = "+3725111001",
            IsActive = true,
            AddressLine = "Narva mnt 5",
            City = "Tallinn",
            PostalCode = "10117",
            Country = "EE",
            CreatedAt = SeededAt
        },
        new()
        {
            Id = Guid.Parse("53000000-0000-0000-0000-000000000002"),
            CompanyId = NorthBitesCompanyId,
            Email = "andres.north@client.local",
            FirstName = "Andres",
            LastName = "North",
            PhoneNumber = "+3725111002",
            IsActive = true,
            AddressLine = "Tartu mnt 55",
            City = "Tallinn",
            PostalCode = "10115",
            Country = "EE",
            CreatedAt = SeededAt
        },
        new()
        {
            Id = Guid.Parse("53000000-0000-0000-0000-000000000003"),
            CompanyId = SouthSpoonCompanyId,
            Email = "maria.south@client.local",
            FirstName = "Maria",
            LastName = "South",
            PhoneNumber = "+3725112001",
            IsActive = true,
            AddressLine = "Emajõe 9",
            City = "Tartu",
            PostalCode = "51007",
            Country = "EE",
            CreatedAt = SeededAt
        },
        new()
        {
            Id = Guid.Parse("53000000-0000-0000-0000-000000000004"),
            CompanyId = SouthSpoonCompanyId,
            Email = "jaan.south@client.local",
            FirstName = "Jaan",
            LastName = "South",
            PhoneNumber = "+3725112002",
            IsActive = true,
            AddressLine = "Rüütli 12",
            City = "Tartu",
            PostalCode = "51005",
            Country = "EE",
            CreatedAt = SeededAt
        }
    ];

    public static readonly CustomerAppUser[] CustomerAppUsers =
    [
        new() { Id = Guid.Parse("54000000-0000-0000-0000-000000000001"), CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000001"), AppUserId = Customer1UserId, CreatedAt = SeededAt },
        new() { Id = Guid.Parse("54000000-0000-0000-0000-000000000002"), CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000003"), AppUserId = Customer2UserId, CreatedAt = SeededAt }
    ];

    public static readonly DietaryCategory[] DietaryCategories =
    [
        new() { Id = Guid.Parse("55000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, Code = "vegan", Name = "Vegan", IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("55000000-0000-0000-0000-000000000002"), CompanyId = NorthBitesCompanyId, Code = "gluten_free", Name = "Gluten Free", IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("55000000-0000-0000-0000-000000000003"), CompanyId = NorthBitesCompanyId, Code = "high_protein", Name = "High Protein", IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin2UserId },
        new() { Id = Guid.Parse("55000000-0000-0000-0000-000000000004"), CompanyId = SouthSpoonCompanyId, Code = "vegetarian", Name = "Vegetarian", IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId },
        new() { Id = Guid.Parse("55000000-0000-0000-0000-000000000005"), CompanyId = SouthSpoonCompanyId, Code = "dairy_free", Name = "Dairy Free", IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId }
    ];

    public static readonly Ingredient[] Ingredients =
    [
        new() { Id = Guid.Parse("56000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, Name = "Chicken Breast", NormalizedName = "chicken breast", IsAllergen = false, IsExclusionTag = false, CreatedAt = SeededAt, CreatedByAppUserId = NorthEmployee1UserId },
        new() { Id = Guid.Parse("56000000-0000-0000-0000-000000000002"), CompanyId = NorthBitesCompanyId, Name = "Garlic", NormalizedName = "garlic", IsAllergen = false, IsExclusionTag = true, ExclusionKey = "garlic", CreatedAt = SeededAt, CreatedByAppUserId = NorthEmployee1UserId },
        new() { Id = Guid.Parse("56000000-0000-0000-0000-000000000003"), CompanyId = NorthBitesCompanyId, Name = "Peanuts", NormalizedName = "peanuts", IsAllergen = true, IsExclusionTag = true, ExclusionKey = "peanut", CreatedAt = SeededAt, CreatedByAppUserId = NorthEmployee2UserId },
        new() { Id = Guid.Parse("56000000-0000-0000-0000-000000000004"), CompanyId = NorthBitesCompanyId, Name = "Brown Rice", NormalizedName = "brown rice", IsAllergen = false, IsExclusionTag = false, CreatedAt = SeededAt, CreatedByAppUserId = NorthEmployee2UserId },
        new() { Id = Guid.Parse("56000000-0000-0000-0000-000000000005"), CompanyId = SouthSpoonCompanyId, Name = "Salmon", NormalizedName = "salmon", IsAllergen = false, IsExclusionTag = false, CreatedAt = SeededAt, CreatedByAppUserId = SouthEmployee1UserId },
        new() { Id = Guid.Parse("56000000-0000-0000-0000-000000000006"), CompanyId = SouthSpoonCompanyId, Name = "Milk", NormalizedName = "milk", IsAllergen = true, IsExclusionTag = true, ExclusionKey = "dairy", CreatedAt = SeededAt, CreatedByAppUserId = SouthEmployee1UserId },
        new() { Id = Guid.Parse("56000000-0000-0000-0000-000000000007"), CompanyId = SouthSpoonCompanyId, Name = "Spinach", NormalizedName = "spinach", IsAllergen = false, IsExclusionTag = false, CreatedAt = SeededAt, CreatedByAppUserId = SouthEmployee2UserId }
    ];

    public static readonly Recipe[] Recipes =
    [
        new() { Id = Guid.Parse("57000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, Name = "Garlic Chicken Bowl", Description = "Chicken, rice, garlic sauce", ImageUrl = "https://cdn.local/north/garlic-chicken.jpg", DefaultServings = 2, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthManager1UserId },
        new() { Id = Guid.Parse("57000000-0000-0000-0000-000000000002"), CompanyId = NorthBitesCompanyId, Name = "Vegan Peanut Stir Fry", Description = "Tofu, vegetables, peanut sauce", ImageUrl = "https://cdn.local/north/vegan-peanut.jpg", DefaultServings = 2, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthManager2UserId },
        new() { Id = Guid.Parse("57000000-0000-0000-0000-000000000003"), CompanyId = SouthSpoonCompanyId, Name = "Salmon Cream Pasta", Description = "Salmon and creamy sauce", ImageUrl = "https://cdn.local/south/salmon-pasta.jpg", DefaultServings = 2, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthManager1UserId },
        new() { Id = Guid.Parse("57000000-0000-0000-0000-000000000004"), CompanyId = SouthSpoonCompanyId, Name = "Spinach Ricotta Bake", Description = "Vegetarian baked pasta", ImageUrl = "https://cdn.local/south/spinach-bake.jpg", DefaultServings = 2, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthManager2UserId }
    ];

    public static readonly RecipeIngredient[] RecipeIngredients =
    [
        new() { Id = Guid.Parse("58000000-0000-0000-0000-000000000001"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000001"), IngredientId = Guid.Parse("56000000-0000-0000-0000-000000000001"), CreatedAt = SeededAt, CreatedByAppUserId = NorthManager1UserId },
        new() { Id = Guid.Parse("58000000-0000-0000-0000-000000000002"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000001"), IngredientId = Guid.Parse("56000000-0000-0000-0000-000000000002"), CreatedAt = SeededAt, CreatedByAppUserId = NorthManager1UserId },
        new() { Id = Guid.Parse("58000000-0000-0000-0000-000000000003"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000001"), IngredientId = Guid.Parse("56000000-0000-0000-0000-000000000004"), CreatedAt = SeededAt, CreatedByAppUserId = NorthManager1UserId },
        new() { Id = Guid.Parse("58000000-0000-0000-0000-000000000004"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000002"), IngredientId = Guid.Parse("56000000-0000-0000-0000-000000000003"), CreatedAt = SeededAt, CreatedByAppUserId = NorthManager2UserId },
        new() { Id = Guid.Parse("58000000-0000-0000-0000-000000000005"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000003"), IngredientId = Guid.Parse("56000000-0000-0000-0000-000000000005"), CreatedAt = SeededAt, CreatedByAppUserId = SouthManager1UserId },
        new() { Id = Guid.Parse("58000000-0000-0000-0000-000000000006"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000003"), IngredientId = Guid.Parse("56000000-0000-0000-0000-000000000006"), CreatedAt = SeededAt, CreatedByAppUserId = SouthManager1UserId },
        new() { Id = Guid.Parse("58000000-0000-0000-0000-000000000007"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000004"), IngredientId = Guid.Parse("56000000-0000-0000-0000-000000000007"), CreatedAt = SeededAt, CreatedByAppUserId = SouthManager2UserId },
        new() { Id = Guid.Parse("58000000-0000-0000-0000-000000000008"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000004"), IngredientId = Guid.Parse("56000000-0000-0000-0000-000000000006"), CreatedAt = SeededAt, CreatedByAppUserId = SouthManager2UserId }
    ];

    public static readonly RecipeDietaryCategory[] RecipeDietaryCategories =
    [
        new() { Id = Guid.Parse("59000000-0000-0000-0000-000000000001"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000001"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000003"), CreatedAt = SeededAt, CreatedByAppUserId = NorthManager1UserId },
        new() { Id = Guid.Parse("59000000-0000-0000-0000-000000000002"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000002"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000001"), CreatedAt = SeededAt, CreatedByAppUserId = NorthManager2UserId },
        new() { Id = Guid.Parse("59000000-0000-0000-0000-000000000003"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000003"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000005"), CreatedAt = SeededAt, CreatedByAppUserId = SouthManager1UserId },
        new() { Id = Guid.Parse("59000000-0000-0000-0000-000000000004"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000004"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000004"), CreatedAt = SeededAt, CreatedByAppUserId = SouthManager2UserId }
    ];

    public static readonly NutritionalInfo[] NutritionalInfos =
    [
        new() { Id = Guid.Parse("5a000000-0000-0000-0000-000000000001"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000001"), CaloriesKcal = 680, ProteinG = 45, CarbsG = 60, FatG = 22, FiberG = 6, SodiumMg = 780, SugarG = 6, SaturatedFatG = 4, CreatedAt = SeededAt },
        new() { Id = Guid.Parse("5a000000-0000-0000-0000-000000000002"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000002"), CaloriesKcal = 520, ProteinG = 28, CarbsG = 55, FatG = 18, FiberG = 10, SodiumMg = 650, SugarG = 8, SaturatedFatG = 3, CreatedAt = SeededAt },
        new() { Id = Guid.Parse("5a000000-0000-0000-0000-000000000003"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000003"), CaloriesKcal = 740, ProteinG = 40, CarbsG = 62, FatG = 28, FiberG = 5, SodiumMg = 820, SugarG = 7, SaturatedFatG = 9, CreatedAt = SeededAt },
        new() { Id = Guid.Parse("5a000000-0000-0000-0000-000000000004"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000004"), CaloriesKcal = 610, ProteinG = 30, CarbsG = 58, FatG = 20, FiberG = 7, SodiumMg = 700, SugarG = 6, SaturatedFatG = 6, CreatedAt = SeededAt }
    ];

    public static readonly WeeklyMenuRuleConfig[] WeeklyMenuRuleConfigs =
    [
        new() { Id = Guid.Parse("5b000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, RecipesPerCategory = 2, NoRepeatWeeks = 6, SelectionDeadlineDaysBeforeWeekStart = 2, CreatedAt = SeededAt },
        new() { Id = Guid.Parse("5b000000-0000-0000-0000-000000000002"), CompanyId = SouthSpoonCompanyId, RecipesPerCategory = 2, NoRepeatWeeks = 8, SelectionDeadlineDaysBeforeWeekStart = 3, CreatedAt = SeededAt }
    ];

    public static readonly WeeklyMenu[] WeeklyMenus =
    [
        new() { Id = Guid.Parse("5c000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, WeekStartDate = WeekStartNorth, SelectionDeadlineAt = WeekStartNorth.AddDays(-2), TotalRecipes = 2, IsPublished = true, PublishedAt = WeekStartNorth.AddDays(-3), CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("5c000000-0000-0000-0000-000000000002"), CompanyId = SouthSpoonCompanyId, WeekStartDate = WeekStartSouth, SelectionDeadlineAt = WeekStartSouth.AddDays(-3), TotalRecipes = 2, IsPublished = true, PublishedAt = WeekStartSouth.AddDays(-4), CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId }
    ];

    public static readonly WeeklyMenuRecipe[] WeeklyMenuRecipes =
    [
        new() { Id = Guid.Parse("5d000000-0000-0000-0000-000000000001"), WeeklyMenuId = Guid.Parse("5c000000-0000-0000-0000-000000000001"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000001"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000003"), DisplayOrder = 1, IsFeatured = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("5d000000-0000-0000-0000-000000000002"), WeeklyMenuId = Guid.Parse("5c000000-0000-0000-0000-000000000001"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000002"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000001"), DisplayOrder = 2, IsFeatured = false, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("5d000000-0000-0000-0000-000000000003"), WeeklyMenuId = Guid.Parse("5c000000-0000-0000-0000-000000000002"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000003"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000005"), DisplayOrder = 1, IsFeatured = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId },
        new() { Id = Guid.Parse("5d000000-0000-0000-0000-000000000004"), WeeklyMenuId = Guid.Parse("5c000000-0000-0000-0000-000000000002"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000004"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000004"), DisplayOrder = 2, IsFeatured = false, CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId }
    ];

    public static readonly Box[] Boxes =
    [
        new() { Id = Guid.Parse("5e000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, MealsCount = 3, PeopleCount = 2, DisplayName = "3 meals for 2", IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("5e000000-0000-0000-0000-000000000002"), CompanyId = NorthBitesCompanyId, MealsCount = 5, PeopleCount = 2, DisplayName = "5 meals for 2", IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("5e000000-0000-0000-0000-000000000003"), CompanyId = SouthSpoonCompanyId, MealsCount = 3, PeopleCount = 2, DisplayName = "3 meals for 2", IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId },
        new() { Id = Guid.Parse("5e000000-0000-0000-0000-000000000004"), CompanyId = SouthSpoonCompanyId, MealsCount = 4, PeopleCount = 4, DisplayName = "4 meals for 4", IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId }
    ];

    public static readonly BoxPrice[] BoxPrices =
    [
        new() { Id = Guid.Parse("5f000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000001"), PricingName = "Standard", PriceAmount = 59.90m, ValidFrom = SeededAt, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("5f000000-0000-0000-0000-000000000002"), CompanyId = NorthBitesCompanyId, BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000002"), PricingName = "Standard", PriceAmount = 89.90m, ValidFrom = SeededAt, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("5f000000-0000-0000-0000-000000000003"), CompanyId = SouthSpoonCompanyId, BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000003"), PricingName = "Standard", PriceAmount = 57.90m, ValidFrom = SeededAt, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId },
        new() { Id = Guid.Parse("5f000000-0000-0000-0000-000000000004"), CompanyId = SouthSpoonCompanyId, BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000004"), PricingName = "Family", PriceAmount = 109.90m, ValidFrom = SeededAt, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId }
    ];

    public static readonly BoxDietaryCategory[] BoxDietaryCategories =
    [
        new() { Id = Guid.Parse("60000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000001"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000003"), CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("60000000-0000-0000-0000-000000000002"), CompanyId = NorthBitesCompanyId, BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000002"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000001"), CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("60000000-0000-0000-0000-000000000003"), CompanyId = SouthSpoonCompanyId, BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000003"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000005"), CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId },
        new() { Id = Guid.Parse("60000000-0000-0000-0000-000000000004"), CompanyId = SouthSpoonCompanyId, BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000004"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000004"), CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId }
    ];

    public static readonly PlatformSubscription[] PlatformSubscriptions =
    [
        new()
        {
            Id = Guid.Parse("61000000-0000-0000-0000-000000000001"),
            CompanyId = NorthBitesCompanyId,
            PlatformSubscriptionTierId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            PlatformSubscriptionStatusId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            ValidFrom = SeededAt,
            ValidTo = null,
            CreatedAt = SeededAt,
            CreatedByAppUserId = RootUserId
        },
        new()
        {
            Id = Guid.Parse("61000000-0000-0000-0000-000000000002"),
            CompanyId = SouthSpoonCompanyId,
            PlatformSubscriptionTierId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            PlatformSubscriptionStatusId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            ValidFrom = SeededAt,
            ValidTo = null,
            CreatedAt = SeededAt,
            CreatedByAppUserId = RootUserId
        }
    ];

    public static readonly MealSubscription[] MealSubscriptions =
    [
        new() { Id = Guid.Parse("62000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000001"), BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000001"), IsActive = true, StartDate = WeekStartNorth, AutoSelectEnabled = true, NoRepeatWeeksOverride = 4, CreatedAt = SeededAt },
        new() { Id = Guid.Parse("62000000-0000-0000-0000-000000000002"), CompanyId = NorthBitesCompanyId, CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000002"), BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000002"), IsActive = true, StartDate = WeekStartNorth, AutoSelectEnabled = false, CreatedAt = SeededAt },
        new() { Id = Guid.Parse("62000000-0000-0000-0000-000000000003"), CompanyId = SouthSpoonCompanyId, CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000003"), BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000003"), IsActive = true, StartDate = WeekStartSouth, AutoSelectEnabled = true, NoRepeatWeeksOverride = 6, CreatedAt = SeededAt },
        new() { Id = Guid.Parse("62000000-0000-0000-0000-000000000004"), CompanyId = SouthSpoonCompanyId, CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000004"), BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000004"), IsActive = true, StartDate = WeekStartSouth, AutoSelectEnabled = false, CreatedAt = SeededAt }
    ];

    public static readonly MealSelection[] MealSelections =
    [
        new() { Id = Guid.Parse("63000000-0000-0000-0000-000000000001"), MealSubscriptionId = Guid.Parse("62000000-0000-0000-0000-000000000001"), WeeklyMenuId = Guid.Parse("5c000000-0000-0000-0000-000000000001"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000001"), SelectedAutomatically = true, SelectedAt = SeededAt.AddDays(6), AutoSelectionReason = "Default selection", AutoSelectionNotes = "Auto-picked due to no selection", CreatedAt = SeededAt.AddDays(6) },
        new() { Id = Guid.Parse("63000000-0000-0000-0000-000000000002"), MealSubscriptionId = Guid.Parse("62000000-0000-0000-0000-000000000002"), WeeklyMenuId = Guid.Parse("5c000000-0000-0000-0000-000000000001"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000002"), SelectedAutomatically = false, SelectedAt = SeededAt.AddDays(5), CreatedAt = SeededAt.AddDays(5) },
        new() { Id = Guid.Parse("63000000-0000-0000-0000-000000000003"), MealSubscriptionId = Guid.Parse("62000000-0000-0000-0000-000000000003"), WeeklyMenuId = Guid.Parse("5c000000-0000-0000-0000-000000000002"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000003"), SelectedAutomatically = true, SelectedAt = SeededAt.AddDays(6), AutoSelectionReason = "Auto-selection enabled", CreatedAt = SeededAt.AddDays(6) },
        new() { Id = Guid.Parse("63000000-0000-0000-0000-000000000004"), MealSubscriptionId = Guid.Parse("62000000-0000-0000-0000-000000000004"), WeeklyMenuId = Guid.Parse("5c000000-0000-0000-0000-000000000002"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000004"), SelectedAutomatically = false, SelectedAt = SeededAt.AddDays(5), CreatedAt = SeededAt.AddDays(5) }
    ];

    public static readonly CustomerPreference[] CustomerPreferences =
    [
        new() { Id = Guid.Parse("64000000-0000-0000-0000-000000000001"), CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000001"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000003"), CreatedAt = SeededAt },
        new() { Id = Guid.Parse("64000000-0000-0000-0000-000000000002"), CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000003"), DietaryCategoryId = Guid.Parse("55000000-0000-0000-0000-000000000005"), CreatedAt = SeededAt }
    ];

    public static readonly CustomerExclusion[] CustomerExclusions =
    [
        new() { Id = Guid.Parse("65000000-0000-0000-0000-000000000001"), CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000001"), IngredientId = Guid.Parse("56000000-0000-0000-0000-000000000003"), CreatedAt = SeededAt },
        new() { Id = Guid.Parse("65000000-0000-0000-0000-000000000002"), CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000003"), IngredientId = Guid.Parse("56000000-0000-0000-0000-000000000006"), CreatedAt = SeededAt }
    ];

    public static readonly DeliveryZone[] DeliveryZones =
    [
        new() { Id = Guid.Parse("66000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, Name = "Tallinn Center", Description = "Kesklinn and nearby", IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthManager1UserId },
        new() { Id = Guid.Parse("66000000-0000-0000-0000-000000000002"), CompanyId = SouthSpoonCompanyId, Name = "Tartu Center", Description = "Kesklinn and nearby", IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthManager1UserId }
    ];

    public static readonly DeliveryWindow[] DeliveryWindows =
    [
        new() { Id = Guid.Parse("67000000-0000-0000-0000-000000000001"), DeliveryZoneId = Guid.Parse("66000000-0000-0000-0000-000000000001"), DayOfWeek = 2, StartTime = new TimeSpan(16, 0, 0), EndTime = new TimeSpan(19, 0, 0), Capacity = 50, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthManager1UserId },
        new() { Id = Guid.Parse("67000000-0000-0000-0000-000000000002"), DeliveryZoneId = Guid.Parse("66000000-0000-0000-0000-000000000001"), DayOfWeek = 4, StartTime = new TimeSpan(16, 0, 0), EndTime = new TimeSpan(19, 0, 0), Capacity = 50, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthManager1UserId },
        new() { Id = Guid.Parse("67000000-0000-0000-0000-000000000003"), DeliveryZoneId = Guid.Parse("66000000-0000-0000-0000-000000000002"), DayOfWeek = 3, StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(18, 0, 0), Capacity = 40, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthManager1UserId },
        new() { Id = Guid.Parse("67000000-0000-0000-0000-000000000004"), DeliveryZoneId = Guid.Parse("66000000-0000-0000-0000-000000000002"), DayOfWeek = 5, StartTime = new TimeSpan(15, 0, 0), EndTime = new TimeSpan(18, 0, 0), Capacity = 40, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthManager1UserId }
    ];

    public static readonly PricingAdjustment[] PricingAdjustments =
    [
        new() { Id = Guid.Parse("68000000-0000-0000-0000-000000000001"), CompanyId = NorthBitesCompanyId, AdjustmentType = "delivery_fee", Label = "Zone Delivery Fee", Amount = 4.90m, IsPercentage = false, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = NorthAdmin1UserId },
        new() { Id = Guid.Parse("68000000-0000-0000-0000-000000000002"), CompanyId = SouthSpoonCompanyId, AdjustmentType = "discount", Label = "Welcome Discount", Amount = 10m, IsPercentage = true, IsActive = true, CreatedAt = SeededAt, CreatedByAppUserId = SouthAdmin1UserId }
    ];

    public static readonly Delivery[] Deliveries =
    [
        new()
        {
            Id = Guid.Parse("69000000-0000-0000-0000-000000000001"),
            CompanyId = NorthBitesCompanyId,
            CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000001"),
            WeeklyMenuId = Guid.Parse("5c000000-0000-0000-0000-000000000001"),
            DeliveryZoneId = Guid.Parse("66000000-0000-0000-0000-000000000001"),
            DeliveryWindowId = Guid.Parse("67000000-0000-0000-0000-000000000001"),
            BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000001"),
            MealSelectionId = Guid.Parse("63000000-0000-0000-0000-000000000001"),
            MealSubscriptionId = Guid.Parse("62000000-0000-0000-0000-000000000001"),
            DeliveryStatusId = Guid.Parse("10000000-0000-0000-0000-000000000003"),
            ScheduledTime = WeekStartNorth.AddDays(2).AddHours(17),
            DeliveredAt = WeekStartNorth.AddDays(2).AddHours(17).AddMinutes(20),
            AddressLine = "Narva mnt 5",
            City = "Tallinn",
            PostalCode = "10117",
            Country = "EE",
            CreatedAt = SeededAt.AddDays(6),
            CreatedByAppUserId = NorthEmployee1UserId
        },
        new()
        {
            Id = Guid.Parse("69000000-0000-0000-0000-000000000002"),
            CompanyId = SouthSpoonCompanyId,
            CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000003"),
            WeeklyMenuId = Guid.Parse("5c000000-0000-0000-0000-000000000002"),
            DeliveryZoneId = Guid.Parse("66000000-0000-0000-0000-000000000002"),
            DeliveryWindowId = Guid.Parse("67000000-0000-0000-0000-000000000003"),
            BoxId = Guid.Parse("5e000000-0000-0000-0000-000000000003"),
            MealSelectionId = Guid.Parse("63000000-0000-0000-0000-000000000003"),
            MealSubscriptionId = Guid.Parse("62000000-0000-0000-0000-000000000003"),
            DeliveryStatusId = Guid.Parse("10000000-0000-0000-0000-000000000004"),
            ScheduledTime = WeekStartSouth.AddDays(3).AddHours(16),
            DeliveredAt = null,
            FailureReason = "Customer not home",
            AddressLine = "Emajõe 9",
            City = "Tartu",
            PostalCode = "51007",
            Country = "EE",
            CreatedAt = SeededAt.AddDays(6),
            CreatedByAppUserId = SouthEmployee1UserId
        }
    ];

    public static readonly DeliveryAttempt[] DeliveryAttempts =
    [
        new() { Id = Guid.Parse("6a000000-0000-0000-0000-000000000001"), DeliveryId = Guid.Parse("69000000-0000-0000-0000-000000000001"), DeliveryAttemptResultId = Guid.Parse("20000000-0000-0000-0000-000000000001"), AttemptNo = 1, AttemptAt = WeekStartNorth.AddDays(2).AddHours(17), Notes = "Delivered successfully", CreatedAt = SeededAt.AddDays(6) },
        new() { Id = Guid.Parse("6a000000-0000-0000-0000-000000000002"), DeliveryId = Guid.Parse("69000000-0000-0000-0000-000000000002"), DeliveryAttemptResultId = Guid.Parse("20000000-0000-0000-0000-000000000002"), AttemptNo = 1, AttemptAt = WeekStartSouth.AddDays(3).AddHours(16), Notes = "Customer not home", CreatedAt = SeededAt.AddDays(6) }
    ];

    public static readonly QualityComplaint[] QualityComplaints =
    [
        new()
        {
            Id = Guid.Parse("6b000000-0000-0000-0000-000000000001"),
            CompanyId = NorthBitesCompanyId,
            CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000001"),
            DeliveryId = Guid.Parse("69000000-0000-0000-0000-000000000001"),
            QualityComplaintTypeId = Guid.Parse("30000000-0000-0000-0000-000000000004"),
            QualityComplaintStatusId = Guid.Parse("40000000-0000-0000-0000-000000000002"),
            Severity = 2,
            Description = "Packaging dented but contents intact",
            CreatedAt = SeededAt.AddDays(7)
        },
        new()
        {
            Id = Guid.Parse("6b000000-0000-0000-0000-000000000002"),
            CompanyId = SouthSpoonCompanyId,
            CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000003"),
            DeliveryId = Guid.Parse("69000000-0000-0000-0000-000000000002"),
            QualityComplaintTypeId = Guid.Parse("30000000-0000-0000-0000-000000000002"),
            QualityComplaintStatusId = Guid.Parse("40000000-0000-0000-0000-000000000001"),
            Severity = 3,
            Description = "Missing side salad",
            CreatedAt = SeededAt.AddDays(7)
        }
    ];

    public static readonly Rating[] Ratings =
    [
        new() { Id = Guid.Parse("6c000000-0000-0000-0000-000000000001"), CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000001"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000001"), Score = 5, Notes = "Loved the garlic sauce", RatedAt = SeededAt.AddDays(7) },
        new() { Id = Guid.Parse("6c000000-0000-0000-0000-000000000002"), CustomerId = Guid.Parse("53000000-0000-0000-0000-000000000003"), RecipeId = Guid.Parse("57000000-0000-0000-0000-000000000003"), Score = 3, Notes = "Good but could be warmer", RatedAt = SeededAt.AddDays(7) }
    ];
}
