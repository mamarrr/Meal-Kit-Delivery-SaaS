# Missing Controller Inventory

This inventory enumerates missing MVC controllers required to expose the workflows in `workflow-inventory.md`.

## Customer-facing controllers

| Controller | Primary service contract | Workflow IDs |
|---|---|---|
| `MealSubscriptionsController` | `App.Contracts.BLL.Subscription.IMealSubscriptionService` | CUST-01, CUST-02, CUST-03 |
| `CustomerPreferencesController` | `App.Contracts.BLL.Menu.IDietaryCategoryService` (+ preference-specific handling) | CUST-04 |
| `CustomerExclusionsController` | `App.Contracts.BLL.Menu.IIngredientService` (+ exclusion-specific handling) | CUST-05 |
| `WeeklyMenusController` | `App.Contracts.BLL.Menu.IWeeklyMenuService` | CUST-06, CUST-10 |
| `MealSelectionsController` | `App.Contracts.BLL.Menu.IMealSelectionService` | CUST-07, CUST-08, CUST-09 |
| `DeliveriesController` | `App.Contracts.BLL.Delivery.IDeliveryService` | CUST-11 |
| `QualityComplaintsController` | `App.Contracts.BLL.Delivery.IQualityComplaintService` | CUST-12 |

## Company-facing controllers

| Controller | Primary service contract | Workflow IDs |
|---|---|---|
| `CompanySettingsController` | `App.Contracts.BLL.Core.ICompanySettingsService` | COMP-01 |
| `BoxesController` | `App.Contracts.BLL.Subscription.IBoxService` | COMP-02 |
| `PlatformSubscriptionsController` | `App.Contracts.BLL.Subscription.IPlatformSubscriptionService` | COMP-03, COMP-15 |
| `DietaryCategoriesController` | `App.Contracts.BLL.Menu.IDietaryCategoryService` | COMP-04 |
| `RecipesController` | `App.Contracts.BLL.Menu.IRecipeService` | COMP-05, COMP-06 |
| `WeeklyMenusController` | `App.Contracts.BLL.Menu.IWeeklyMenuService` | COMP-07, COMP-08 |
| `DeliveryZonesController` | `App.Contracts.BLL.Delivery.IDeliveryZoneService` | COMP-09, COMP-10 |
| `DeliveriesController` | `App.Contracts.BLL.Delivery.IDeliveryService` | COMP-11 |
| `QualityComplaintsController` | `App.Contracts.BLL.Delivery.IQualityComplaintService` | COMP-12, COMP-13 |
| `CompanyUsersController` | `App.Contracts.BLL.Core.ICompanyAppUserService` | COMP-14 |

## System/support-facing controllers

| Controller | Primary service contract | Workflow IDs |
|---|---|---|
| `CompaniesController` | `App.Contracts.BLL.Core.ICompanyService` | SYS-01 |
| `PlatformSubscriptionTiersController` | `App.Contracts.BLL.Subscription.IPlatformSubscriptionTierService` | SYS-02 |
| `PlatformSubscriptionsController` | `App.Contracts.BLL.Subscription.IPlatformSubscriptionService` | SYS-03 |
| `SystemSettingsController` | (to be added in later slice) | SYS-04 |
| `AnalyticsController` | (to be added in later slice) | SYS-05 |
| `SupportImpersonationController` | (to be added in later slice) | SYS-06 |
| `SupportAccessController` | (to be added in later slice) | SYS-07 |
| `SupportTicketsController` | (to be added in later slice) | SYS-08 |

## Notes

- Controllers marked “to be added in later slice” require dedicated service contracts that are not yet present in `App.Contracts.BLL`.
- The immediate scaffolding/hardening scope (tasks 3.x/4.x) focuses on controllers that map to existing service contracts.
