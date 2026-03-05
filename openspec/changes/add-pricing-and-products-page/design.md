## Context

Company workspaces need a coherent, tenant-scoped place to configure commercial offerings (box sizes, plan pricing, and pricing adjustments) that currently are either absent or fragmented. The platform already has subscription-related layers (`Box`, `BoxPrice`, platform subscription tiers) and multi-tenant role boundaries, so this change must connect existing domain/application building blocks into a dedicated Pricing & Products management workflow without breaking tenant isolation or existing operational pages.

This design supports introducing a new company workspace surface while keeping implementation aligned with current architecture (MVC → BLL → DAL → Domain), role-based access, and EF Core no-tracking update behavior.

## Goals / Non-Goals

**Goals:**
- Provide a dedicated Pricing & Products UI entry in company workspaces (single page with sections or subpages) for product/pricing administration.
- Enable tenant-scoped management of box combinations (3/4/5 meals × 2/4 people) and associated price configuration.
- Define how Free/Standard/Premium plan pricing tables are represented and configured at tenant level where applicable.
- Enable management of add-ons/fees and discounts (at minimum delivery fee and discount records/rules).
- Keep all reads/writes tenant-filtered and authorized for company operational roles.
- Prepare the model so later implementation tasks can be split cleanly across domain, services, repositories, and UI.

**Non-Goals:**
- Implementing customer checkout/cart pricing calculation engines.
- Building billing provider integrations or invoicing workflows.
- Redesigning global system-level subscription/billing admin flows.
- Defining advanced promotions logic beyond the core fee/discount configuration requirements.

## Decisions

1. **Introduce a dedicated company workspace capability boundary for Pricing & Products.**  
   - **Decision:** Add a new UI capability in company operations for pricing/product setup instead of embedding fields into unrelated pages.  
   - **Rationale:** Keeps business configuration discoverable and consistent, reduces operator confusion, and aligns with existing workflow-complete UI direction.  
   - **Alternatives considered:**
     - Extend existing settings pages only: rejected due to poor information architecture and mixing unrelated concerns.
     - Manage via back-office-only tools: rejected because tenant operators need direct self-service control.

2. **Model pricing configuration as tenant-scoped product definitions plus plan-aware pricing rows.**  
   - **Decision:** Treat box combinations as explicit tenant product definitions and connect them to plan-level pricing table entries (Free/Standard/Premium) using normalized records.  
   - **Rationale:** Supports clear CRUD, avoids hardcoded matrix logic, and allows future extension (regional pricing, temporal windows).  
   - **Alternatives considered:**
     - Hardcode a fixed matrix in view models/config files: rejected because it limits tenant flexibility and complicates future migrations.
     - Store opaque JSON blobs per tenant: rejected for weak queryability and validation.

3. **Use separate adjustment entities for add-ons/fees and discounts with explicit type and scope.**  
   - **Decision:** Capture delivery fee and discount configurations as first-class tenant records (with effective status and optional conditions) instead of embedding them in a single plan table column set.  
   - **Rationale:** Improves maintainability and enables independent validation, ordering, and evolution of fee/discount policies.  
   - **Alternatives considered:**
     - Single monolithic pricing table with many optional columns: rejected due to sparse schema and brittle evolution.

4. **Preserve layered architecture and explicit update semantics for no-tracking EF behavior.**  
   - **Decision:** Implement changes through repository/service contracts and ensure update operations call `Update()` on modified entities before save.  
   - **Rationale:** Required for correctness under `NoTrackingWithIdentityResolution` and keeps implementation consistent with existing patterns.  
   - **Alternatives considered:**
     - Controller-direct DbContext access for speed: rejected due to architecture violations and higher regression risk.

5. **Authorization scope: CompanyAdmin and CompanyManager writable; others read-only/no access per policy.**  
   - **Decision:** Gate management operations via role-based authorization in company view, matching operational responsibility boundaries.  
   - **Rationale:** Pricing/product setup is an operational management concern, not customer/self-service data entry.  
   - **Alternatives considered:**
     - CompanyEmployee write access: rejected to minimize accidental pricing changes.

## Risks / Trade-offs

- **[Risk] Existing subscription domain objects may not fully match required pricing granularity.** → **Mitigation:** Add minimal additive entities/relations and migration scripts; keep backward compatibility adapters in BLL where needed.
- **[Risk] UI complexity (matrix-like box/plan/fee combinations) may reduce usability.** → **Mitigation:** Structure into clear sections/subpages and provide validation/preview summaries before save.
- **[Risk] Tenant data leakage via insufficient filtering in new queries.** → **Mitigation:** Enforce tenant filters in repository methods and include authorization tests for cross-tenant access denial.
- **[Risk] Ambiguity between platform-level tiers and tenant-level pricing values.** → **Mitigation:** Define explicit semantics in specs: platform plan names are reference categories; tenant stores concrete values.
- **[Trade-off] Normalized entities increase implementation effort now.** → **Benefit:** Better extensibility and cleaner reporting compared with hardcoded or denormalized storage.

## Migration Plan

1. Add/adjust domain entities and EF mappings for tenant-scoped box product definitions, plan pricing rows, and fee/discount records.
2. Generate and apply EF migration for schema updates; ensure rollback migration path is valid.
3. Extend repository interfaces/implementations with tenant-filtered CRUD/query methods.
4. Extend BLL contracts/services and add validation rules for box combinations and pricing data integrity.
5. Implement MVC controllers, view models, and Razor views for Pricing & Products sections/subpages.
6. Wire navigation entry in company workspace UI and apply role-based authorization.
7. Add/update integration and UI-level tests for tenant isolation, authorization, and basic CRUD workflows.
8. Rollout strategy: deploy additive schema first, then UI/service changes; rollback by disabling new UI routes and reverting migration if needed.

## Open Questions

- Should pricing be stored as tax-inclusive, tax-exclusive, or both with jurisdiction-specific interpretation?
- Are discounts percentage-only, fixed-amount-only, or both with precedence rules?
- Should historical/effective-dated pricing versions be required immediately or deferred to a later change?
- Does Free tier allow configuring all box combinations with visibility limits, or enforce hard limits at entity level?
- Should delivery fees support zone-specific overrides in this change or remain tenant-wide defaults only?
