## 1. Domain and persistence foundation

- [x] 1.1 Add or refine tenant-scoped domain entities/relationships for box product definitions (3/4/5 meals × 2/4 people), plan-tier pricing rows, and fee/discount records.
- [x] 1.2 Update EF Core mappings and create migration(s) for new/updated pricing-product schema with soft-deactivation support for adjustments.
- [x] 1.3 Verify rollback path and migration integrity for existing tenant data.

## 2. Repository and service layer implementation

- [x] 2.1 Extend repository contracts and implementations with tenant-filtered CRUD/query operations for box products, plan pricing, fees, and discounts.
- [x] 2.2 Extend BLL contracts/services with validation rules (allowed box combinations, monetary bounds, required plan pricing completeness).
- [x] 2.3 Ensure all update flows use explicit EF `Update()` semantics for modified entities under no-tracking behavior.

## 3. Company workspace UI and authorization

- [x] 3.1 Add Pricing & Products navigation entry in company workspace and map controller routes/actions.
- [x] 3.2 Implement Razor views + view models for box configuration, plan pricing tables (Free/Standard/Premium), and fee/discount management.
- [x] 3.3 Apply role-based authorization so CompanyAdmin/CompanyManager can manage pricing/product data and unauthorized roles are denied.

## 4. Validation, auditability, and tenant isolation safeguards

- [x] 4.1 Add server-side validation/error presentation for invalid combinations, invalid monetary values, and incomplete required pricing entries.
- [x] 4.2 Capture change metadata (actor and timestamp) for pricing updates to satisfy audit expectations.
- [x] 4.3 Add tenant-isolation guard checks on read/write paths and deny cross-tenant record access attempts.

## 5. Verification and rollout readiness

- [x] 5.1 Add/update automated tests for authorized/unauthorized access, tenant isolation, and core CRUD workflows for products/pricing/adjustments.
- [x] 5.2 Add/update UI workflow coverage so Pricing & Products is included in continuity-critical company operations.
- [x] 5.3 Validate end-to-end behavior in company workspace and document any follow-up work items discovered during implementation.

## 6. Correction pass (user feedback)

- [x] 6.1 Remove platform plan-tier concept from Pricing & Products, replace with company-defined pricing configurations using required pricing name.
- [x] 6.2 Require box name input and add optional box-to-dietary-category associations.
- [x] 6.3 Keep Pricing & Products success/error messaging on Pricing & Products page.
- [x] 6.4 Remove auto-selection simulation UI/action from Menus & Scheduling page workflow.
- [x] 6.5 Enforce soft-delete consistency for ingredient-linked recipe filtering and add soft-delete actions for dietary categories and recipes.
- [x] 6.6 Add migration for revised Pricing & Products schema updates (pricing name + box dietary associations).
- [x] 6.7 Update automated tests for newly added soft-delete behaviors and run full build/test verification.

## 7. Regression correction pass (post-release feedback)

- [x] 7.1 Fix recipe nutrition update tracking conflict by making nutritional info updates safe when another instance with same key is already tracked.
- [x] 7.2 Remove manual category override in Menus & Scheduling assignment flow and derive assignment category from selected recipe.
- [x] 7.3 Fix recipe update replacement logic so deselected ingredient and dietary category links are soft-deactivated without duplicating kept links.
- [x] 7.4 Simplify Recipes & Nutrition forms by removing user-entered ingredient exclusion key and dietary category code inputs; derive values internally.
- [x] 7.5 Add regression-focused automated tests for nutrition tracking updates, category derivation on assignment, link replacement behavior, and internal code/key derivation; run full build/test verification.

## 8. Incremental correction pass (weekly assignment removal)

- [x] 8.1 Add backend support to remove a weekly recipe assignment using tenant-scoped soft-delete behavior.
- [x] 8.2 Add Menus & Scheduling remove assignment action and wire a remove button in weekly assignments table.
- [x] 8.3 Add/update tests for weekly assignment removal tenant-scoping + soft-delete behavior and run build/test verification.

## 9. Incremental correction pass (post-release regressions)

- [x] 9.1 Fix ingredient recreate-after-soft-delete flow so same-name create reactivates tenant-scoped soft-deleted ingredient without unique-constraint failure.
- [x] 9.2 Simplify Ingredients catalog UI by removing "Participates in exclusions" input and enforce excludable-by-default behavior in backend upsert handling.
- [x] 9.3 Ensure Menus & Scheduling success/error flash messages render on Menus & Scheduling page.
- [x] 9.4 Add regression tests for ingredient reactivation and default exclusion behavior; run full solution tests.

## 10. Incremental correction pass (recipe link removal regression)

- [x] 10.1 Fix recipe update handling so null ingredient/dietary selections from UI submissions are treated as empty selections and existing links are soft-deactivated.
- [x] 10.2 Add regression test coverage for null-selection recipe updates to verify both ingredient and dietary-category links are removed via soft-delete.
- [x] 10.3 Run build/test verification after correction.
