## Context

The platform already distinguishes company-scoped roles, but it lacks a dedicated, cohesive flow for tenant user administration in day-to-day operations. CompanyOwner and CompanyAdmin need a single page to add existing users by email, manage roles, and remove memberships, while preserving strict tenant isolation and authorization constraints. Additionally, ownership continuity requires a safe ownership-transfer flow that cannot leave a company without an owner or accidentally elevate unauthorized users.

Current architecture is layered (`WebApp` → `App.BLL` → `App.DAL.EF` → `App.Domain`) with no-tracking EF behavior (`NoTrackingWithIdentityResolution`), which means all update flows must explicitly call update methods for persisted changes. Existing role model semantics and company-user linkage entities should be reused rather than introducing parallel identity constructs.

## Goals / Non-Goals

**Goals:**
- Provide a company-scoped **Manage Users and Roles** UI for CompanyOwner and CompanyAdmin.
- Enable adding users to company by existing account email with immediate membership creation (no invitation acceptance flow).
- Enable role changes among CompanyAdmin, CompanyManager, and CompanyEmployee for existing members within policy limits.
- Enable member removal by CompanyOwner/CompanyAdmin with guardrails preventing invalid company-admin state.
- Enable CompanyOwner-only ownership transfer to an existing company member with atomic role/state transition.
- Preserve tenant isolation, explicit authorization, and auditable state transitions.

**Non-Goals:**
- Creating new platform accounts from this flow when email does not exist.
- Implementing email invitation/acceptance pipelines.
- Changing system-level roles or billing/subscription permissions.
- Introducing cross-tenant user administration from this page.

## Decisions

1. **Introduce a dedicated company operations route and page for user/role administration**
   - **Decision:** Add a company-scoped MVC page (controller actions + view/viewmodels) under existing operations navigation patterns.
   - **Rationale:** Keeps user management discoverable and consistent with other company operations pages; avoids overloading unrelated settings pages.
   - **Alternatives considered:**
     - Embed into generic company settings page (rejected: mixed responsibilities and poorer UX for frequent admin operations).
     - Build as API-only endpoint first (rejected: project is MVC-first and needs integrated web workflow coverage).

2. **Use existing account lookup by normalized email and only allow add when account exists**
   - **Decision:** “Add user” operation resolves email to an existing `AppUser`; if not found, operation fails validation.
   - **Rationale:** Matches requested behavior and avoids invitation lifecycle complexity.
   - **Alternatives considered:**
     - Auto-create account skeletons (rejected: introduces identity lifecycle/security concerns).
     - Deferred invitations (rejected: explicitly out of scope for this change).

3. **Centralize business rules in BLL service methods instead of controller-level logic**
   - **Decision:** Controllers remain orchestration-only; membership/role/ownership invariants live in tenant-scoped service layer.
   - **Rationale:** Ensures consistent rule enforcement across any future endpoints and simplifies testing.
   - **Alternatives considered:**
     - Controller-heavy rule checks (rejected: duplicated logic, harder to test/maintain).

4. **Ownership transfer implemented as a guarded atomic role transition**
   - **Decision:** Service operation validates actor is current owner, target is current company member, and applies role updates so target becomes owner while prior owner is downgraded to a valid non-owner role in same transaction boundary.
   - **Rationale:** Prevents broken owner state and race-prone partial updates.
   - **Alternatives considered:**
     - Two independent role-change actions (rejected: can leave transient invalid ownership state).

5. **Explicit update semantics for EF no-tracking mode**
   - **Decision:** Any mutated existing entities are explicitly updated via repository/service update operations before save.
   - **Rationale:** Required by current DbContext no-tracking configuration to avoid silent no-op updates.
   - **Alternatives considered:**
     - Rely on tracked entity mutation (rejected: incompatible with current configuration).

6. **Authorization policy boundaries**
   - **Decision:**
     - CompanyOwner and CompanyAdmin: add user, change non-owner roles, remove users (with restrictions on owner account mutation).
     - CompanyOwner only: transfer ownership.
   - **Rationale:** Aligns with product role definitions while preserving a stricter boundary for ownership-sensitive operations.
   - **Alternatives considered:**
     - Allow CompanyAdmin ownership transfer (rejected: violates explicit owner-only control requirement).

## Risks / Trade-offs

- **[Risk] Concurrent admin operations could conflict (e.g., role update during ownership transfer)** → **Mitigation:** enforce service-level validation on current persisted state and use transactional update boundaries.
- **[Risk] Removing or downgrading privileged users could accidentally lock out management capability** → **Mitigation:** add guard rules preventing invalid owner state; require owner-specific checks for owner-affecting actions.
- **[Risk] Email matching inconsistencies across casing/normalization** → **Mitigation:** use normalized email lookup path in identity repository/service.
- **[Risk] Multi-tenant leakage through insufficient company scoping in queries** → **Mitigation:** require company-id-scoped repository methods and add tests for cross-tenant denial.
- **[Trade-off] No invitation flow means only existing users can be onboarded from this page** → **Mitigation:** acceptable for current scope; future change can layer invitations without breaking this contract.

## Migration Plan

1. Add/extend service contracts for company membership add, role update, removal, and ownership transfer.
2. Implement/extend BLL service logic with tenant-scoped validation and authorization checks.
3. Add/extend DAL query methods for user-by-email resolution and company membership retrieval/update patterns.
4. Add MVC controller actions, viewmodels, and Razor UI page for user management operations.
5. Add navigation entry visibility for CompanyOwner/CompanyAdmin in company operations shell.
6. Add automated tests (unit/integration) for authorization matrix and core invariants.
7. Deploy with no schema migration if current linkage model already supports role transitions; if schema adjustment becomes necessary during implementation, add explicit migration and rollback script in the same change.

Rollback strategy:
- Revert deployment package to prior version.
- Disable navigation entry if partial rollback needed while preserving existing membership data.
- If schema migration is introduced later, include reverse migration before full rollback.

## Open Questions

- Should CompanyAdmin be allowed to modify/remove other CompanyAdmin users, or only CompanyManager/CompanyEmployee?
- On ownership transfer, what is the exact fallback role for previous owner (CompanyAdmin vs preserving previous non-owner role is not applicable because owner is unique)?
- Should self-removal be allowed for CompanyAdmin and/or CompanyOwner, and under what safeguards?
