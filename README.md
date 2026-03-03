# webapp for 2025-2026 spring semester

**Meal Kit Delivery Platform SaaS.**

Backend and admin ui is implemented in AspNet Core MVC,
using standard libraries (EF Core, Identity, Postgres DB).  
Design patterns - N-Tier, Solid. Domain, DAL, BLL and UI layers.

# Assignment

## 5. Meal Kit Delivery Platform (SaaS)

"We power meal kit delivery companies globally. Each company manages their own recipes, menus, delivery zones, and subscribers through our platform.

A company configures box sizes (3/4/5 meals × 2/4 people), dietary categories (omnivore, vegetarian, pescatarian, vegan), and pricing. Customers set preferences and ingredient exclusions — no shellfish, no pork, no cilantro.

Recipes rotate weekly from the company's recipe pool. Weekly menus feature a configurable number of recipes across categories. Customers choose by a configurable deadline or get auto-selected based on ratings history — never repeating a meal from the past N weeks (configurable, default 8).

Nutritional information per recipe per serving: calories, protein, carbs, fat, fiber, sodium. Customers can filter by nutritional criteria for macro-tracking.

Delivery logistics: companies define zones with delivery days and time windows. Track delivery success, failed attempts, and quality complaints. Escalation rules for repeated complaints (e.g., prioritize freshest stock, earliest slot).

CompanyEmployees handle customer service and delivery tracking. CompanyManagers manage recipes, menus, and logistics. CompanyAdmins configure zones, pricing, and company settings. Free tier limited to 1 zone and 50 subscribers."

Entities: Company, CompanySettings, Customer, MealSubscription, DeliveryZone, Recipe, RecipeIngredient, Ingredient, NutritionalInfo, WeeklyMenu, MealSelection, Delivery, DeliveryAttempt, Rating, Exclusion, Box, Subscription, AppUser, AppUserRole
