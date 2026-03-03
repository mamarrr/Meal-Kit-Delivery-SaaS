## Context

The current codebase lacks a complete domain model aligned to the PostgreSQL schema in `openspec/schema.sql`. The change must introduce all entities, relationships, and tenant-scoped constraints while honoring architectural patterns (BaseEntity inheritance, repository layering), soft-delete strategy, and EF Core global NoTracking behavior.

## Goals / Non-Goals

**Goals:**
- Establish a domain entity model that maps 1:1 to the schema tables, including audit and soft-delete columns.
- Define tenant-scoped relationships centered on Company and explicitly model join tables.
- Provide clear entity relationships and configuration points for EF Core (FKs, required fields, delete behaviors).

**Non-Goals:**
- Implement application workflows (menus generation, delivery routing, etc.).
- Build APIs, UI, or business services beyond the domain model and mappings.
- Data migrations or production data backfills (schema is authoritative).

## Decisions

- **Entity-per-table mapping**: Model every table in `openspec/schema.sql` as a domain entity to avoid drift and keep EF Core mappings explicit.
  - *Alternative*: Use implicit many-to-many mappings. Rejected because schema already defines join tables and needs audit fields.
- **Explicit join entities**: Keep entities such as RecipeIngredient, RecipeDietaryCategory, WeeklyMenuRecipe, CustomerPreference, CustomerAppUser, CompanyAppUser, etc.
  - *Alternative*: EF Core skip navigations. Rejected due to audit/soft-delete columns and required CreatedBy fields.
- **Tenant boundary enforced via CompanyId**: All tenant-scoped entities require a CompanyId FK and navigation to Company, matching schema.
  - *Alternative*: Rely on query filters only. Rejected because the schema requires explicit FK constraints and auditing.
- **Soft delete represented with nullable DeletedAt**: Use nullable timestamps to reflect deleted records across entities.
  - *Alternative*: Boolean IsDeleted on every entity. Rejected because schema uses DeletedAt consistently.
- **No cascade delete**: Configure relationships with `DeleteBehavior.Restrict` to match global constraint and avoid accidental multi-tenant data loss.
  - *Alternative*: Cascade deletes. Rejected due to explicit requirement and auditability.
- **Identity linkage through AppUser**: AppUser is the domain-level identity anchor, linked to IdentityUser via FK and to Company/Customer via join tables.
  - *Alternative*: Directly reference IdentityUser in business entities. Rejected to keep domain independent of identity implementation.

## Risks / Trade-offs

- **High entity count increases maintenance** → Mitigation: generate consistent base types, apply shared configuration patterns.
- **Complex relationships may cause circular navigation issues** → Mitigation: carefully choose navigation properties and consider required/optional ends.
- **No-tracking default can lead to missed updates** → Mitigation: enforce repository update patterns and ensure Update calls are used.

## Migration Plan

- Implement domain entities and EF Core configurations in App.Domain/App.DAL.EF.
- Register entities in DbContext and verify model creation against schema.
- Update seed data and tests that assume missing entities.
- Validate by running existing migrations or generating a model snapshot for comparison (no schema changes intended).

## Open Questions

- Should any lookup tables (e.g., DeliveryStatus, QualityComplaintStatus) be seeded with defaults now or deferred to a later change?
- Are any nullable fields in the schema intended to be required by business rules and thus annotated as required in the domain model?
