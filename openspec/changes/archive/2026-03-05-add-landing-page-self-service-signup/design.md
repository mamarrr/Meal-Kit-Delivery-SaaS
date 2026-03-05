## Context

The change introduces a new top-level user entry point and a shared layout contract that affects routing, authentication flows, tenant onboarding, and reusable UI shell rendering. The current system already has multi-tenant and identity foundations, but lacks a unified landing surface that cleanly separates unauthenticated actions (register/login) from authenticated tenant navigation while preserving tenant isolation and role-aware access. This cross-cuts WebApp (routes/controllers/views/layout), Identity registration/login behavior, and BLL onboarding orchestration for creating a company plus initial `CompanyOwner`.

## Goals / Non-Goals

**Goals:**
- Provide a public landing page that is the default app entry and exposes two primary actions: self-service signup and existing-user login.
- Implement a deterministic onboarding flow that creates a tenant and initial `CompanyOwner` account atomically from the user perspective.
- Standardize the shell layout across pages with left sidebar navigation and top header sections (brand at top-left, auth-aware account actions at top-right).
- Ensure authenticated vs unauthenticated header actions are consistently rendered by one shared layout mechanism.
- Keep tenant boundaries explicit: signup creates a new company context; login routes user into permitted company/customer context without cross-tenant data leakage.

**Non-Goals:**
- Full redesign of all existing feature pages beyond adopting the new shared shell.
- Billing/subscription checkout expansion beyond selecting defaults required to create a new tenant.
- New authorization model; this change consumes existing Identity + role model.
- Deep customer-view feature additions unrelated to entry/onboarding/layout.

## Decisions

1. **Use a dedicated Landing controller/view as the root route**
   - **Decision:** Route `/` to a new landing page (e.g., `LandingController.Index`) that is intentionally unauthenticated and brand-forward.
   - **Rationale:** Keeps root entry predictable and prevents authenticated-only dashboard assumptions for first-time users.
   - **Alternative considered:** Reusing `Home/Index` with conditional blocks. Rejected because it tends to accumulate unrelated concerns and complicates future marketing/onboarding evolution.

2. **Separate onboarding orchestration into application service layer**
   - **Decision:** Add a dedicated onboarding use-case in BLL (e.g., `TenantOnboardingService`) called by a signup controller action.
   - **Rationale:** Tenant + owner creation spans identity user creation, company creation, role assignment, and initial settings; orchestration belongs outside MVC controller.
   - **Alternative considered:** Implementing directly in controller. Rejected due to testability and transactional consistency concerns.

3. **Use a tenant-onboarding transaction boundary with compensating failure handling**
   - **Decision:** Treat tenant creation + owner association as one onboarding unit; if later steps fail, rollback where possible or mark setup incomplete with safe retry guidance.
   - **Rationale:** Prevents orphan tenant/user states and improves operational support.
   - **Alternative considered:** Fire-and-forget sequential creation. Rejected because partial failures create difficult manual cleanup scenarios.

4. **Adopt a single shared layout with auth-aware top-right action partial**
   - **Decision:** Centralize shell in shared layout (`_Layout`) and render top-right account controls through one partial/view component that checks authentication state.
   - **Rationale:** Eliminates duplicated auth-action markup and guarantees consistent behavior across pages.
   - **Alternative considered:** Per-page header snippets. Rejected due to drift risk and higher maintenance cost.

5. **Implement left sidebar as role/context-aware navigation model**
   - **Decision:** Sidebar items are generated from a typed navigation model based on user auth state and selected context (company/customer).
   - **Rationale:** Maintains clarity as features scale and avoids hardcoded menu divergence.
   - **Alternative considered:** Static HTML sidebar. Rejected because it does not adapt safely to role/context constraints.

6. **Preserve existing Identity login; add explicit post-login return target resolution**
   - **Decision:** Keep current authentication mechanism and extend post-login routing so users land in an appropriate dashboard/context selector.
   - **Rationale:** Minimizes auth risk while improving user journey from landing page.
   - **Alternative considered:** Custom auth flow replacement. Rejected as unnecessary scope and security risk.

## Risks / Trade-offs

- **[Risk] Onboarding partial failure leaves inconsistent tenant/user data** → **Mitigation:** Introduce clear transaction/compensation strategy and idempotent retry semantics for failed onboarding attempts.
- **[Risk] New root route may break existing deep-link expectations** → **Mitigation:** Preserve existing route endpoints and use explicit redirects only from `/`.
- **[Risk] Shared layout migration can cause visual regressions on existing pages** → **Mitigation:** Incremental adoption with smoke checks on core authenticated and unauthenticated pages.
- **[Risk] Sidebar may expose links user cannot access if not role-filtered correctly** → **Mitigation:** Build navigation model from authorization-aware rules and keep server-side authorization as final enforcement.
- **[Trade-off] Faster delivery by reusing Identity UI patterns limits custom branding in auth screens initially** → **Mitigation:** Phase 2 styling pass after onboarding behavior is stabilized.

## Migration Plan

1. Add landing route/controller/view and keep all existing feature routes intact.
2. Introduce onboarding application service and wire signup endpoint to service.
3. Add/adjust shared layout + header account partial + sidebar rendering model.
4. Integrate login/register links from landing and unauthenticated header.
5. Validate authenticated header behavior (`Hello <email>` + logout) and unauthenticated behavior (register + login).
6. Rollback strategy: revert root routing to prior entry point and disable onboarding endpoint while preserving created tenant data.

## Open Questions

- Should tenant slug/path be chosen by user during signup or auto-generated then editable by `CompanyOwner`?
- Should onboarding immediately sign in the new `CompanyOwner`, or redirect to login with confirmation?
- What minimum company profile fields are mandatory at signup versus deferred to first-run setup?
- Should unauthenticated top-right action label be `Login` or `Sign in` for consistency with existing Identity pages?
