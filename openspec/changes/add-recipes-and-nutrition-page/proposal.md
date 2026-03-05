## Why

Company workspaces currently lack a dedicated place to manage recipes together with nutritional and dietary metadata, which slows menu planning and increases inconsistency in allergen and exclusion handling. This change adds a unified Recipes & Nutrition experience so companies can maintain recipe quality, dietary fit, and nutrition transparency at scale.

## What Changes

- Add a new Company workspace area: **Recipes & Nutrition** (single page with sections or linked subpages).
- Add recipes list and recipe editor workflows covering ingredients, allergen/tags, and dietary categories.
- Add per-serving nutrition fields for recipes: calories, protein, carbs, fat, fiber, sodium.
- Add management for dietary categories and tags, including omnivore, vegetarian, pescatarian, and vegan.
- Add ingredients catalog management and exclusion mapping so ingredient tags (for example `cilantro`) can be used consistently in customer exclusions and recipe tagging.
- Add company navigation and role-appropriate access to Recipes & Nutrition workflows.

## Capabilities

### New Capabilities
- `company-recipes-and-nutrition-management`: Company users can browse, create, edit, and organize recipes with ingredient, allergen/tag, dietary category, and nutrition-per-serving data.
- `ingredients-catalog-and-exclusions-mapping`: Company users can manage ingredient/tag catalog entries and map them to exclusion semantics used by customer preference/exclusion workflows.

### Modified Capabilities
- `menu-and-recipe-model`: Extend requirements to include per-serving nutrition facts and richer recipe classification metadata used in company operations.
- `workflow-complete-web-ui`: Extend company UI workflow coverage to include Recipes & Nutrition navigation and end-to-end operational pages.

## Impact

- Affected systems: company-facing MVC UI (controllers/views/viewmodels), menu/recipe BLL services and contracts, DAL repositories and queries for recipe/ingredient/category data.
- Likely affected code areas: `WebApp/Controllers/`, `WebApp/Views/`, `WebApp/ViewModels/`, `App.Contracts.BLL/Menu/`, `App.BLL/Menu/`, `App.Contracts.DAL/Menu/`, `App.DAL.EF/Repositories/Menu/`, and `App.Domain/Menu/`.
- Data impact: additive domain and database changes for recipe nutrition fields, ingredient/tag taxonomy, and exclusion mapping support; tenant-scoped behavior must remain enforced.
- No external public API breaking change expected; internal service/repository contracts may be expanded to support new required workflows.
