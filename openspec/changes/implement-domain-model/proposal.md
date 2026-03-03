## Why

The project needs a concrete domain model that matches the PostgreSQL schema so EF Core can persist and enforce tenant-scoped relationships correctly. Aligning the code with the schema now avoids drift and unblocks application features that depend on these entities.

## What Changes

- Define domain entities for all tables in `openspec/schema.sql`, including required fields, audit fields, and soft-delete timestamps.
- Model tenant-scoped relationships (Company-scoped data) across subscriptions, menus, deliveries, complaints, and preferences.
- Represent join tables (e.g., recipe ingredients, dietary categories, weekly menu recipes, customer preferences) as explicit domain entities.
- Add identity linkage entities (AppUser, CompanyAppUser, CustomerAppUser) to support multi-tenant roles and customer/company views.
- Include reference data entities (delivery status, complaint type/status, subscription tier/status) as lookup tables.

## Capabilities

### New Capabilities
- `domain-entities`: Domain entity set aligned 1:1 with the database tables, including required fields and audit/soft-delete columns.
- `tenant-scoped-relationships`: Tenant-aware relationships and constraints between Company and all dependent entities (subscriptions, menus, deliveries, complaints, prices).
- `menu-and-recipe-model`: Weekly menu, recipe, ingredient, dietary category, and selection relationships modeled in the domain.
- `delivery-and-quality-model`: Delivery, delivery attempts, delivery windows/zones, and quality complaint relationships modeled in the domain.
- `identity-linkage-model`: AppUser linkage to IdentityUser plus company and customer membership entities to support multi-tenant roles.

### Modified Capabilities
<!-- None; no existing specs yet. -->

## Impact

- `App.Domain` entity definitions, navigation properties, and base entity usage.
- `App.DAL.EF` entity configuration (FKs, required/optional fields, delete behavior) and DbContext registrations.
- Potential changes to BLL services and controllers to use new domain entities.
- Test data seeding and integration/unit tests that assume domain entity availability.
