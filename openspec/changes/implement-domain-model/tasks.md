## 1. Domain Entities

- [ ] 1.1 Inventory schema tables and map to planned domain entity classes
- [ ] 1.2 Add entity classes for core tenant data (Company, CompanySettings, Customer, AppUser, CompanyAppUser, CustomerAppUser)
- [ ] 1.3 Add entity classes for recipe/menu data (Recipe, Ingredient, RecipeIngredient, NutritionalInfo, DietaryCategory, RecipeDietaryCategory, WeeklyMenu, WeeklyMenuRecipe, MealSelection)
- [ ] 1.4 Add entity classes for subscription/box data (Box, BoxPrice, MealSubscription, PlatformSubscription, PlatformSubscriptionTier, PlatformSubscriptionStatus)
- [ ] 1.5 Add entity classes for delivery/quality data (DeliveryZone, DeliveryWindow, Delivery, DeliveryAttempt, DeliveryStatus, DeliveryAttemptResult, QualityComplaint, QualityComplaintType, QualityComplaintStatus, Rating, CustomerPreference, CustomerExclusion)

## 2. Relationships and Navigation

- [ ] 2.1 Define navigation properties and FK properties matching schema for all entities
- [ ] 2.2 Explicitly model join entities and required relationships (RecipeIngredient, RecipeDietaryCategory, WeeklyMenuRecipe, CustomerPreference, CustomerAppUser, CompanyAppUser)
- [ ] 2.3 Ensure tenant-scoped entities include CompanyId and Company navigation

## 3. EF Core Configuration

- [ ] 3.1 Register all entities in AppDbContext and verify key properties
- [ ] 3.2 Configure relationship delete behaviors to Restrict (no cascade)
- [ ] 3.3 Configure required/optional properties and unique constraints (e.g., CompanySettings.CompanyId, AppUser.IdentityUserId)

## 4. Validation and Consistency

- [ ] 4.1 Update seeding to cover lookup tables and required references
- [ ] 4.2 Validate model snapshot matches schema (no missing entities/relationships)
- [ ] 4.3 Run tests or add minimal checks to ensure DbContext builds successfully
