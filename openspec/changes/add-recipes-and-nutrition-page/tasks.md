## 1. Domain and persistence for recipes, nutrition, and taxonomy

- [x] 1.1 Extend recipe domain and EF mappings to include per-serving nutrition fields (calories, protein, carbs, fat, fiber, sodium).
- [x] 1.2 Add or extend ingredient/tag catalog entities and persistence mappings for tenant-scoped canonical entries.
- [x] 1.3 Implement exclusion mapping persistence linking ingredient catalog entries to customer exclusion semantics.
- [x] 1.4 Create and apply EF migration(s) for additive schema updates with safe defaults for existing recipe data.

## 2. Repository and service layer implementation

- [x] 2.1 Extend DAL contracts/repositories for recipe list filtering, editor hydration, ingredient catalog CRUD, and exclusion mapping queries.
- [x] 2.2 Extend BLL contracts/services for recipe create/edit workflows covering ingredients, allergen/tags, dietary categories, and nutrition fields.
- [x] 2.3 Implement validation rules for required nutrition fields, tenant-scoped uniqueness, and catalog-backed ingredient/tag usage.
- [x] 2.4 Ensure all update paths use explicit update operations compatible with EF no-tracking behavior.

## 3. Company UI and authorization workflows

- [x] 3.1 Implement Recipes & Nutrition controller/actions and strongly typed viewmodels for list, editor, taxonomy management, and mapping pages.
- [x] 3.2 Build Razor views for recipe list, recipe editor, nutrition fields, dietary/tag management, and ingredient catalog + exclusion mapping workflows.
- [x] 3.3 Add company navigation entry and enforce role-based access for CompanyOwner, CompanyAdmin, and CompanyManager.

## 4. Validation, integration, and readiness

- [x] 4.1 Add or extend automated tests for repository/service/controller behavior, including validation failures and tenant isolation.
- [x] 4.2 Add tests ensuring exclusion mappings are consistently applied in recipe and customer-exclusion-related flows.
- [x] 4.3 Validate end-to-end company workflow from navigation to recipe editing and mapping management in the active company context.
- [x] 4.4 Update technical documentation/changelog with Recipes & Nutrition capabilities and operational rules.
