# Tech notes

Based on .NET. Everything is running in docker (db and webapp).


### Install or update tooling for .net

~~~bash
dotnet tool update -g dotnet-ef
dotnet tool update -g dotnet-aspnet-codegenerator
dotnet tool update -g Microsoft.Web.LibraryManager.Cli
~~~

## JS Libs

Add htmx and alpine to js libs.
~~~bash
libman install htmx.org --files dist/htmx.min.js 
libman install alpinejs --files dist/cdn.min.js 
~~~

### Generate database migration

Run from solution folder.

~~~bash
dotnet ef migrations --project App.DAL.EF --startup-project WebApp add Initial
dotnet ef database   --project App.DAL.EF --startup-project WebApp drop
dotnet ef migrations --project App.DAL.EF --startup-project WebApp remove
dotnet ef database   --project App.DAL.EF --startup-project WebApp update
~~~

## Generate identity UI

Install Microsoft.VisualStudio.Web.CodeGeneration.Design to WebApp.  
Run from inside the WebApp directory.

~~~bash
dotnet aspnet-codegenerator identity -dc DAL.App.EF.AppDbContext -f  
~~~

## Generate controllers

Run from inside the WebApp directory.    
Don't forget to add ***Microsoft.VisualStudio.Web.CodeGeneration.Design*** package to the WebApp project as a NuGet package reference.

MVC Web Controllers (disable global warnings as errors - otherwise only one controller will be generated, then compile starts to fail)

~~~bash
dotnet aspnet-codegenerator controller -name GpsLocationsController -m  GpsLocation -actions -dc AppDbContext -outDir Areas/Admin/Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f
~~~

API Controllers

~~~bash
dotnet aspnet-codegenerator controller -name GpsLocationsController     -m GpsLocation     -actions -dc AppDbContext -outDir ApiControllers -api --useAsyncActions  -f
~~~

## Tenant-scoped entity convention

- Implement `ITenantProvider` on every company-owned domain entity (tenant data must always expose `CompanyId`).
- Keep platform/global entities (for example identity/system lookup entities) outside `ITenantProvider`.
- Repository methods that query tenant-owned entities must accept company scope where needed and enforce `CompanyId` filtering in the database query.
- Service-level create/update paths must preserve tenant ownership by assigning and/or validating `CompanyId` in tenant scope operations.
- For detached/no-tracking update flows, continue to use explicit repository `Update()` calls before saving changes.

## Menus & Scheduling module notes

- Route: `/{slug}/menus-scheduling` for company-scoped operations.
- Navigation: available in company context sidebar for CompanyOwner, CompanyAdmin, CompanyManager, and CompanyEmployee.
- Core capabilities:
  - Weekly planning view by week start date.
  - Rule configuration (`recipes/category`, `no-repeat weeks`, `deadline days before week start`).
  - Recipe assignment with validation against category quota and no-repeat window.
  - Auto-selection simulation (dry-run) with exclusion diagnostics.
- Persistence updates:
  - New `WeeklyMenuRuleConfig` tenant entity.
  - `WeeklyMenuRecipe` now supports optional `DietaryCategoryId` to represent category-scoped assignments.
  - Migration: `20260305132703_MenusSchedulingRules`.

## Recipes & Nutrition module notes

- Route: `/{slug}/recipes-nutrition` for company-scoped operations.
- Navigation: available in company context sidebar for CompanyOwner, CompanyAdmin, CompanyManager, and CompanyEmployee.
- Core capabilities:
  - Recipes list with filters (search, dietary category, ingredient tag, active-only).
  - Recipe editor for ingredients, dietary categories, and nutrition per serving.
  - Nutrition fields: calories, protein, carbs, fat, fiber, sodium.
  - Ingredient catalog management with exclusion mapping metadata (for example `cilantro` as an exclusion tag).
- Persistence updates:
  - `Ingredient` includes `NormalizedName`, `IsAllergen`, `IsExclusionTag`, and `ExclusionKey`.
  - Uniqueness/index updates for ingredient name and exclusion key per company.
  - Migration: `20260305134656_RecipesNutritionCatalog`.

## Running postgre in docker

docker run --name meal-delivery-postgres --restart unless-stopped -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=meal-delivery-saas -p 5432:5432 -v meal-delivery-postgres-data:/var/lib/postgresql/data -d postgres:16