using App.Domain;
using App.Domain.Core;
using App.Domain.Delivery;
using App.Domain.Identity;
using App.Domain.Menu;
using App.Domain.Support;
using App.Domain.Subscription;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace App.DAL.EF;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser, AppRole, Guid>(options), IDataProtectionKeyContext
{

    public DbSet<AppRefreshToken> RefreshTokens { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    
    // Core tenant entities
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanySettings> CompanySettings { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CompanyAppUser> CompanyAppUsers { get; set; }
    public DbSet<CustomerAppUser> CustomerAppUsers { get; set; }
    public DbSet<CompanyRole> CompanyRoles { get; set; }
    
    // Recipe/menu entities
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
    public DbSet<NutritionalInfo> NutritionalInfos { get; set; }
    public DbSet<DietaryCategory> DietaryCategories { get; set; }
    public DbSet<RecipeDietaryCategory> RecipeDietaryCategories { get; set; }
    public DbSet<WeeklyMenu> WeeklyMenus { get; set; }
    public DbSet<WeeklyMenuRecipe> WeeklyMenuRecipes { get; set; }
    public DbSet<WeeklyMenuRuleConfig> WeeklyMenuRuleConfigs { get; set; }
    public DbSet<MealSelection> MealSelections { get; set; }
    public DbSet<CustomerPreference> CustomerPreferences { get; set; }
    public DbSet<CustomerExclusion> CustomerExclusions { get; set; }
    
    // Subscription/box entities
    public DbSet<Box> Boxes { get; set; }
    public DbSet<BoxPrice> BoxPrices { get; set; }
    public DbSet<MealSubscription> MealSubscriptions { get; set; }
    public DbSet<PlatformSubscription> PlatformSubscriptions { get; set; }
    public DbSet<PlatformSubscriptionTier> PlatformSubscriptionTiers { get; set; }
    public DbSet<PlatformSubscriptionStatus> PlatformSubscriptionStatuses { get; set; }
    
    // Delivery/quality entities
    public DbSet<DeliveryZone> DeliveryZones { get; set; }
    public DbSet<DeliveryWindow> DeliveryWindows { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<DeliveryAttempt> DeliveryAttempts { get; set; }
    public DbSet<DeliveryStatus> DeliveryStatuses { get; set; }
    public DbSet<DeliveryAttemptResult> DeliveryAttemptResults { get; set; }
    public DbSet<QualityComplaint> QualityComplaints { get; set; }
    public DbSet<QualityComplaintType> QualityComplaintTypes { get; set; }
    public DbSet<QualityComplaintStatus> QualityComplaintStatuses { get; set; }
    public DbSet<Rating> Ratings { get; set; }

    // System/support entities
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<TenantSupportAccess> TenantSupportAccesses { get; set; }
    public DbSet<SupportTicketStatus> SupportTicketStatuses { get; set; }
    public DbSet<SupportTicket> SupportTickets { get; set; }
    public DbSet<SupportImpersonationSession> SupportImpersonationSessions { get; set; }
    public DbSet<SystemAnalyticsSnapshot> SystemAnalyticsSnapshots { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure all DateTime properties to use UTC
        ConfigureDateTimeAsUtc(builder);

        // disable cascade delete
        foreach (var relationship in builder.Model
                     .GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.Restrict;
        }
        
        // Configure unique constraints
        builder.Entity<CompanySettings>(entity =>
        {
            entity.HasIndex(e => e.CompanyId).IsUnique();
        });

        builder.Entity<Ingredient>(entity =>
        {
            entity.HasIndex(e => new { e.CompanyId, e.Name }).IsUnique();
            entity.HasIndex(e => new { e.CompanyId, e.ExclusionKey });
        });

        builder.Entity<NutritionalInfo>(entity =>
        {
            entity.HasIndex(e => e.RecipeId).IsUnique();
        });

        builder.Entity<SystemSetting>(entity =>
        {
            entity.HasIndex(e => new { e.Category, e.Key }).IsUnique();
            entity.HasOne(e => e.UpdatedByAppUser)
                .WithMany(u => u.SystemSettingsUpdated)
                .HasForeignKey(e => e.UpdatedByAppUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<TenantSupportAccess>(entity =>
        {
            entity.HasIndex(e => new { e.CompanyId, e.SupportUserId, e.RevokedAt });
            entity.HasOne(e => e.SupportUser)
                .WithMany(u => u.TenantSupportAccesses)
                .HasForeignKey(e => e.SupportUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.GrantedByAppUser)
                .WithMany(u => u.TenantSupportAccessesGranted)
                .HasForeignKey(e => e.GrantedByAppUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<SupportTicketStatus>(entity =>
        {
            entity.HasIndex(e => e.Code).IsUnique();
        });

        builder.Entity<SupportTicket>(entity =>
        {
            entity.HasOne(e => e.CreatedByAppUser)
                .WithMany(u => u.SupportTicketsCreated)
                .HasForeignKey(e => e.CreatedByAppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.AssignedToAppUser)
                .WithMany(u => u.SupportTicketsAssigned)
                .HasForeignKey(e => e.AssignedToAppUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<SupportImpersonationSession>(entity =>
        {
            entity.HasOne(e => e.SupportUser)
                .WithMany(u => u.SupportImpersonationSessionsAsSupportUser)
                .HasForeignKey(e => e.SupportUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ImpersonatedAppUser)
                .WithMany(u => u.SupportImpersonationSessionsAsImpersonatedUser)
                .HasForeignKey(e => e.ImpersonatedAppUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Configure relationships with multiple FKs to same table
        // CompanyAppUser relationships
        builder.Entity<CompanyAppUser>(entity =>
        {
            entity.HasOne(e => e.AppUser)
                .WithMany(u => u.CompanyAppUsers)
                .HasForeignKey(e => e.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne(e => e.CreatedByAppUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByAppUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // CustomerAppUser relationships
        builder.Entity<CustomerAppUser>(entity =>
        {
            entity.HasOne(e => e.AppUser)
                .WithMany(u => u.CustomerAppUsers)
                .HasForeignKey(e => e.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // Company relationships
        builder.Entity<Company>(entity =>
        {
            entity.HasOne(e => e.CreatedByAppUser)
                .WithMany(u => u.CompaniesCreated)
                .HasForeignKey(e => e.CreatedByAppUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
        
        // CompanySettings relationships
        builder.Entity<CompanySettings>(entity =>
        {
            entity.HasOne(e => e.UpdatedByAppUser)
                .WithMany(u => u.CompanySettingsUpdated)
                .HasForeignKey(e => e.UpdatedByAppUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
    
    /// <summary>
    /// Configures all DateTime and DateTime? properties to convert to UTC when saving to PostgreSQL.
    /// PostgreSQL's 'timestamp with time zone' type requires UTC values.
    /// </summary>
    private static void ConfigureDateTimeAsUtc(ModelBuilder builder)
    {
        // Value converter for DateTime
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(v, DateTimeKind.Utc)
                : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        // Value converter for DateTime?
        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue
                ? (v.Value.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                    : v.Value.ToUniversalTime())
                : v,
            v => v.HasValue
                ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                : v);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }

}
