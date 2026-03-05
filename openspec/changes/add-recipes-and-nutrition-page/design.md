## Context

The platform already includes menu/recipe domain primitives, but company workspaces do not provide a focused Recipes & Nutrition workflow for operational users. Recipe data quality is currently at risk because ingredients, exclusion tags, dietary categorization, and nutrition facts are not managed as a single coherent company-facing flow.

This change introduces a tenant-scoped company module for recipe and nutrition operations. It spans MVC UI, service orchestration, domain model enrichment, and repository/query updates. The design must preserve role-based access, strict CompanyId isolation, and EF Core no-tracking update discipline (`Update()` required for modified entities).

## Goals / Non-Goals

**Goals:**
- Add a company-facing Recipes & Nutrition area with discoverable navigation and role-appropriate authorization.
- Support recipe list and editor workflows with ingredient tagging, allergen/tags, and dietary categories.
- Store and validate nutrition-per-serving fields (calories, protein, carbs, fat, fiber, sodium) for recipes.
- Provide management for dietary categories/tags including omnivore, vegetarian, pescatarian, vegan.
- Provide an ingredient catalog and exclusion mapping model so ingredient tags (e.g., cilantro) can be reused across recipe metadata and customer exclusion workflows.
- Keep implementation aligned with layered architecture (`WebApp -> BLL -> DAL -> Domain`) and tenant boundaries.

**Non-Goals:**
- Customer-facing nutrition filtering UX enhancements outside existing customer workflow scope.
- Cross-tenant/global shared ingredient catalogs in this iteration.
- Reworking unrelated delivery/subscription modules.
- Introducing external nutrition APIs or third-party taxonomy providers.

## Decisions

1. **Introduce a dedicated Recipes & Nutrition MVC module in company view**
   - **Decision:** Add controller(s), viewmodels, and views under the existing company workspace conventions.
   - **Rationale:** Consolidates recipe, nutrition, and taxonomy operations into one operational workflow and reduces navigation friction.
   - **Alternative considered:** Extend existing recipe CRUD pages only. Rejected because it does not adequately expose nutrition and exclusion mapping lifecycle.

2. **Model nutrition as structured per-serving fields on recipe aggregate**
   - **Decision:** Persist numeric nutrition attributes on recipe entities/value model and expose them in service/viewmodel contracts.
   - **Rationale:** Matches product requirement for macro-tracking and enables consistent validation and querying.
   - **Alternative considered:** JSON/free-form nutrition blob. Rejected due to weak validation and poor queryability.

3. **Treat ingredients/tags as managed catalog entities with explicit exclusion mapping**
   - **Decision:** Keep a canonical ingredient/tag catalog and add mapping semantics for customer exclusion interpretation.
   - **Rationale:** Ensures consistent tag usage between recipe editing and customer preference/exclusion behavior.
   - **Alternative considered:** Free-text ingredients only. Rejected because synonyms and spelling variance break exclusion reliability.

4. **Enforce business rules in BLL services, not controllers**
   - **Decision:** Perform validation for required nutrition values, dietary/category consistency, and mapping integrity in service layer.
   - **Rationale:** Reusable, testable, and consistent across UI endpoints.
   - **Alternative considered:** Controller-only validation. Rejected for duplication and drift risk.

5. **Preserve tenant isolation and no-tracking persistence patterns by contract**
   - **Decision:** All DAL queries and write paths remain tenant-scoped; write flows explicitly call repository update methods.
   - **Rationale:** Prevents cross-tenant leaks and silent persistence failures in `NoTrackingWithIdentityResolution` mode.
   - **Alternative considered:** Implicit tracking behavior assumptions. Rejected as incompatible with current EF configuration.

6. **Modify existing model/UI capability specs instead of creating duplicate overlapping specs**
   - **Decision:** Extend `menu-and-recipe-model` and `workflow-complete-web-ui` while adding focused new capabilities for recipes+nutrition management and exclusion mapping.
   - **Rationale:** Keeps spec topology coherent and avoids fragmented requirements ownership.
   - **Alternative considered:** Entirely new umbrella capability replacing existing model spec. Rejected due to unnecessary churn.

## Risks / Trade-offs

- **[Risk] Taxonomy growth can create inconsistent tag governance** → **Mitigation:** Add uniqueness rules, normalization conventions, and admin-visible usage counts.
- **[Risk] Nutrition validation may block legacy recipes with incomplete data** → **Mitigation:** Use migration defaults and staged validation (warn then enforce on edit/save).
- **[Risk] Exclusion mapping semantics may be ambiguous for composite ingredients** → **Mitigation:** Introduce explicit mapping notes/rules and deterministic matching precedence in service logic.
- **[Risk] Query complexity increases for recipe list filters (dietary + tags + nutrition)** → **Mitigation:** Add tenant-scoped indexed predicates and projection-first repository methods.
- **[Trade-off] Centralized catalog management increases upfront setup burden** → **Benefit:** Higher long-term consistency for menu planning and customer trust.

## Migration Plan

1. Add/extend domain entities and EF mappings for recipe nutrition fields, ingredient/tag catalog metadata, and exclusion mapping structures.
2. Create migration with additive schema updates and safe defaults for existing recipe rows.
3. Extend DAL contracts/repositories for list filtering, recipe editor hydration, taxonomy CRUD, and mapping operations.
4. Extend BLL contracts/services with validation and orchestration for recipe+nature nutrition workflows.
5. Implement company MVC controllers/viewmodels/views and navigation integration for Recipes & Nutrition.
6. Add/extend tests for tenant isolation, validation behavior, and no-tracking update correctness.
7. Roll out by enabling navigation and routes; rollback by reverting route exposure and migration if required.

## Open Questions

- Should nutrition fields be mandatory for all recipes immediately, or only for newly created/edited recipes during transition?
- Should CompanyEmployee role have edit access to taxonomy mappings or read-only visibility in initial release?
- Do we need hierarchical ingredient relationships (e.g., coriander vs cilantro variants) in this iteration or simple canonical tags only?
