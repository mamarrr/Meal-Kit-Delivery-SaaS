## Context

Current flows are optimized for company onboarding and company-scoped operations, while the product vision requires one identity to support both customer and company participation. The requested change spans authentication entry points, post-login routing, customer subscription UX, and company user governance, making it a cross-cutting update across WebApp, BLL, DAL contracts, and authorization rules. Existing specs already cover tenant onboarding and shell/navigation behavior, so this design must preserve those capabilities while introducing an explicit customer-first path and a safer company user administration model.

## Goals / Non-Goals

**Goals:**
- Enable account registration without mandatory company creation from top-right auth entry.
- Route authenticated users into an explicit customer context where they can view their MealSubscriptions.
- Provide a customer browse-and-subscribe journey for discovering and joining MealSubscriptions.
- Replace company user management with invitation-based membership assignment by CompanyOwner/CompanyAdmin and role assignment to Admin/Manager/Employee.
- Add ownership transfer restricted to CompanyOwner with clear authorization and auditable state transitions.
- Keep strict tenant isolation and existing role boundaries intact while adding customer/company context switching.

**Non-Goals:**
- Redesigning pricing tiers, billing workflows, or system-level (SystemAdmin/SystemBilling/SystemSupport) behavior.
- Replacing ASP.NET Core Identity architecture or introducing external identity providers.
- Full UI visual redesign beyond screens and navigation required for the new flows.
- Introducing cross-tenant collaboration features.

## Decisions

1. **Dual-context principal model with explicit context selection**
   - Decision: Treat a signed-in account as potentially both customer and company member; determine default landing via context resolution (customer if no active company context, otherwise last/selected company context).
   - Rationale: Matches product requirement that one person can have both views and avoids forcing company onboarding.
   - Alternative considered: Separate customer-only and company-only accounts. Rejected because it conflicts with current vision and increases identity fragmentation.

2. **Top-right registration split into customer registration and company onboarding entry points**
   - Decision: Keep single auth access area but provide explicit actions: register as customer (no company), and company onboarding path for tenant creation.
   - Rationale: Minimal disruption to navigation while clarifying intent at entry.
   - Alternative considered: Hidden automatic company creation later. Rejected due to ambiguity and unnecessary tenant creation risk.

3. **Customer subscription self-service as dedicated customer pages/controllers**
   - Decision: Implement customer-facing endpoints for “My MealSubscriptions” and “Browse MealSubscriptions,” backed by customer-scoped query/command services.
   - Rationale: Clean separation from company operations and clearer authorization boundaries.
   - Alternative considered: Reusing company subscription pages with conditional rendering. Rejected due to complex mixed authorization and poor UX clarity.

4. **Company user management rework to invitation + role assignment workflow**
   - Decision: CompanyOwner and CompanyAdmin can invite by email and assign one of (Admin, Manager, Employee). Membership creation can occur on accept (or direct link if user exists), but role assignment remains tenant-scoped.
   - Rationale: Supports controlled user onboarding and explicit role governance.
   - Alternative considered: Immediate user creation by admins without invitation lifecycle. Rejected due to weaker verification and auditability.

5. **Ownership transfer as a protected workflow with validation and audit hooks**
   - Decision: Only CompanyOwner can initiate transfer; target must be an existing company member; transfer is atomic in service layer (demote old owner role, promote new owner role, persist audit metadata).
   - Rationale: Prevents orphaned ownership and enforces least privilege.
   - Alternative considered: Allow CompanyAdmin transfer. Rejected because ownership is a top-level tenant control.

6. **Authorization policy expansion with role+tenant guards**
   - Decision: Add/adjust policies for invitation management, role assignment, and ownership transfer; enforce tenant scoping in all repository/service queries.
   - Rationale: Required for security in multi-tenant architecture.
   - Alternative considered: Controller-only role checks. Rejected; policy + service checks provide defense in depth.

7. **No-tracking update discipline for mutable workflows**
   - Decision: For all update flows (invitation status, role changes, ownership transfer, subscription enrollment state), explicitly call repository/context update paths before save.
   - Rationale: DbContext no-tracking mode requires explicit updates to persist mutations.
   - Alternative considered: Relying on implicit tracking. Rejected as it will silently fail.

## Risks / Trade-offs

- **[Risk] Ambiguous post-login destination for users with both customer and company memberships** → Mitigation: define deterministic defaulting (customer-first fallback + remembered last company context) and add integration tests for each identity composition.
- **[Risk] Invitation abuse or incorrect role assignment** → Mitigation: restrict invitation/assignment endpoints to CompanyOwner/CompanyAdmin, validate allowed role set, and record actor/timestamp audit entries.
- **[Risk] Ownership transfer could lock out tenant control if target user is invalid/inactive** → Mitigation: precondition checks (active membership, valid account), transactional update, and rollback on any failure.
- **[Risk] Existing company user management pages may break during rework** → Mitigation: implement new endpoints/viewmodels behind feature flag or route replacement with parity tests before removing old flow.
- **[Trade-off] Additional complexity in app shell/navigation for context switching** → Mitigation: centralize navigation state composition in shared view model and keep routing rules explicit.
- **[Trade-off] More entities/state for invitations and transfer events** → Mitigation: keep model minimal and aligned with audit requirements, avoid over-modeling workflow states.

## Migration Plan

1. Introduce/adjust domain and contract support for invitation, role assignment, and ownership transfer; add EF migration(s) if new persistence structures are needed.
2. Implement BLL service operations and authorization policies for customer subscription self-service and company user governance workflows.
3. Add WebApp controllers/viewmodels/views for customer subscription pages and reworked company user management UI.
4. Update top-right auth UI and post-login routing/context selection logic.
5. Add/refresh integration tests covering registration without company, customer browse/subscribe, invitation/role assignment permissions, and ownership transfer constraints.
6. Deploy with migration and monitor auth/role-related logs; rollback by reverting route/UI toggles and migration if critical authorization issues are detected.

## Open Questions

- Should invitation acceptance require a signed token email flow, or is immediate linking allowed when invited email already maps to an existing account?
- Should CompanyAdmin be allowed to change another CompanyAdmin’s role, or only CompanyOwner can manage Admin-level assignments?
- Should ownership transfer require explicit acceptance by target user (two-step transfer) or be immediate after owner confirmation?
- For customers browsing MealSubscriptions, is the catalog limited to current path tenant/company context or platform-wide discovery with tenant filtering?
