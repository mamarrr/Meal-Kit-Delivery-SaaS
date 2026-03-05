## Why

The current `WebApp` UI layer mixes production-critical onboarding/auth flows with legacy and soon-to-be-replaced pages, which creates high change risk and slows down a ground-up rework. A cleanup-first change is needed now to remove non-essential UI surface area while preserving registration, customer access, login, and slug-routing foundations required for continuity.

## What Changes

- Remove unused or non-rework-target MVC UI components from `WebApp` (controllers, view models, and related views) that are not part of:
  - registering companies,
  - customer registration/subscription entry flows,
  - authentication/login lifecycle.
- Preserve and explicitly protect slugging/routing logic so new pages in the reworked UI can continue to rely on stable slug-based tenant resolution.
- Define a minimal retained UI baseline and document deletion boundaries so implementation can safely remove old UI code without breaking required flows.
- Update OpenSpec capability requirements to reflect the reduced interim UI scope and preserved routing/auth entry points.
- **BREAKING**: Existing non-essential legacy UI routes and pages in `WebApp` will be removed as part of this cleanup phase.

## Capabilities

### New Capabilities
- `webapp-ui-cleanup-boundary`: Defines the required keep/remove contract for `WebApp` UI cleanup, including preserved auth/onboarding paths and deletion-safe boundaries.

### Modified Capabilities
- `workflow-complete-web-ui`: Narrows required interim UI coverage to only critical onboarding/auth continuity paths during rework preparation.
- `slug-based-operations-routing`: Clarifies slug-routing behavior as mandatory preserved infrastructure during UI cleanup and transition.
- `landing-page-entry-and-auth-routing`: Updates entry-point requirements to ensure login/register/customer/company routing remains functional after cleanup.

## Impact

- Affected systems: `WebApp` MVC layer (controllers, views, view models), tenant slug routing integration points, and auth entry routing.
- Affected specs: capability deltas for `workflow-complete-web-ui`, `slug-based-operations-routing`, and `landing-page-entry-and-auth-routing`, plus one new capability spec for cleanup boundaries.
- APIs/dependencies: no external API contract changes expected; internal route availability changes due to legacy UI removals.
- Delivery impact: establishes a cleaner baseline that de-risks subsequent ground-up UI implementation work.
