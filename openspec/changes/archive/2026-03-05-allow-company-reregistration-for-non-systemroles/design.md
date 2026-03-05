## Context

Authenticated users without system-level roles need a reliable way to start company onboarding directly from the in-app shell, even when they already belong to a company. The current navigation and entry logic appears oriented around first-company onboarding and may not expose a repeatable CTA for additional tenant creation. This change touches UI composition, role/eligibility checks, and the handoff to existing onboarding endpoints, so it should be designed as a small cross-cutting update rather than a single view tweak.

## Goals / Non-Goals

**Goals:**
- Add a bottom-anchored left-sidebar action for eligible authenticated users to register a new company.
- Preserve existing onboarding flow endpoints and behavior, while enabling repeated entry by the same non-system user.
- Make visibility deterministic: hidden for system roles, shown for non-system users regardless of existing company memberships.
- Keep the change testable through focused UI visibility and navigation-path tests.

**Non-Goals:**
- Redesigning the full sidebar information architecture.
- Changing database tenancy model or user-company relationship schema.
- Altering onboarding business rules beyond allowing entry for already-associated eligible users.
- Introducing new role types or changing system-role definitions.

## Decisions

1. **Render the CTA in the shared app shell/sidebar partial at the bottom section.**  
   - **Why:** The requirement is explicitly placement-driven and applies across authenticated app contexts. Centralizing in the shared layout avoids drift across pages.
   - **Alternative considered:** Injecting the action only on landing/dashboard pages. Rejected because it is less discoverable and violates the persistent-sidebar requirement.

2. **Use a single eligibility helper/policy for CTA visibility, based on “authenticated AND not in any system role.”**  
   - **Why:** Visibility logic should be reusable and testable; policy centralization avoids duplicated role checks in Razor markup.
   - **Alternative considered:** Inline role checks in the view (`User.IsInRole(...)`). Rejected because it becomes brittle and hard to unit test when role sets evolve.

3. **Reuse existing tenant onboarding route/service for action target instead of creating a parallel flow.**  
   - **Why:** Keeps onboarding behavior consistent and minimizes risk by reusing validated controller/service code.
   - **Alternative considered:** New dedicated “re-register company” endpoint. Rejected as unnecessary duplication unless onboarding semantics diverge later.

4. **Allow entry independent of “already has company” state; enforce only role-based gating.**  
   - **Why:** This is the core requirement and supports multi-company membership scenarios.
   - **Alternative considered:** Showing CTA only when user has exactly one company (or none). Rejected because it conflicts with requested behavior.

5. **Add/adjust tests around sidebar visibility matrix and CTA routing.**  
   - **Why:** Regression risk is mostly in role/context branching; tests should lock expected behavior for system-role and non-system-role users.
   - **Alternative considered:** Relying on manual verification only. Rejected due to high chance of future UI regressions.

## Risks / Trade-offs

- **[Risk]** Existing middleware/claims transformation may classify mixed-role users unexpectedly, causing CTA to appear or disappear incorrectly.  
  **→ Mitigation:** Define precedence clearly (any system role suppresses CTA) and add tests for mixed-role identity claims.

- **[Risk]** Sidebar placement differences between customer/company views may produce inconsistent bottom positioning.  
  **→ Mitigation:** Implement CTA in shared shell component used by both contexts, with consistent container styling.

- **[Risk]** Reusing onboarding route could expose users to assumptions in onboarding service about first-company state.  
  **→ Mitigation:** Validate onboarding service/controller preconditions for repeat use and adjust only minimal guard logic needed.

- **[Trade-off]** Centralized eligibility helper adds a small abstraction layer.  
  **→ Mitigation:** Keep helper focused and documented; use it only for this class of navigation visibility decisions.
