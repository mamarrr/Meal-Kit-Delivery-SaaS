## Why

Company workspaces currently lack a dedicated place to configure pricing and product structure, forcing critical subscription and box setup decisions to remain implicit or scattered across the system. Adding a focused Pricing & Products area now is necessary to support tenant-level monetization control, consistent box configuration, and clear operational pricing rules before deeper subscription workflows are expanded.

## What Changes

- Add a new company-facing **Pricing & Products** page (or structured subpages) for tenant configuration of product and pricing setup.
- Define tenant-manageable box size combinations for meal plans: 3/4/5 meals × 2/4 people.
- Define and display pricing tables aligned to plan levels (Free / Standard / Premium), including support for tenant-level plan configuration where applicable.
- Define configurable add-ons and fee components, including delivery fee and discount rules.
- Establish UI and behavior expectations so company admins/managers can view and maintain pricing/product settings in one cohesive workflow.

## Capabilities

### New Capabilities
- `company-pricing-and-products-management`: Tenant-level management of box sizes, pricing tables, and add-ons/fees from a dedicated company workspace page.
- `subscription-plan-pricing-configuration`: Definition and maintenance of Free/Standard/Premium pricing configuration and display rules at tenant scope.
- `pricing-fees-and-discounts-rules`: Management of delivery fees, discount entries, and how these pricing adjustments are represented in workspace configuration.

### Modified Capabilities
- `workflow-complete-web-ui`: Extend the company workflow surface so Pricing & Products is represented as a first-class page/section in the complete operational UI journey.

## Impact

- Affected UI: Company workspace navigation, new Pricing & Products page(s), related view models and form flows.
- Affected Web layer: New/updated MVC controllers, actions, and Razor views for pricing/product management.
- Affected BLL/DAL: Service and repository paths for box definitions, plan pricing, and fee/discount configuration retrieval/update.
- Affected Domain/DB: Potential additions or refinements to entities and relationships for tenant-scoped pricing structures, plan mappings, and fee/discount definitions.
- Authorization impact: Access scoped to tenant company roles (primarily CompanyAdmin/CompanyManager per policy).
