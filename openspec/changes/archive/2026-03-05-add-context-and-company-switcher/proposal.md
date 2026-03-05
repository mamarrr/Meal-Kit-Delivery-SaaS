## Why

Authenticated users need explicit control over operational perspective and tenant scope. The current shell supports limited context toggling and defaults to a single company claim resolution path, which is insufficient for users who:
- operate in both customer and company contexts,
- have system roles,
- belong to multiple companies.

Without a unified context and company selector in the topbar, routing and authorization intent are ambiguous and users cannot reliably choose the company they are acting within.

## What Changes

- Add a topbar context selector beside the greeting/logout controls with three contexts: `customer`, `company`, `system`.
- Add a company dropdown for authenticated users with multiple active company memberships.
- Persist active context and active company selection and use them during request lifecycle resolution.
- Redirect company-context selection to slug route `/{slug}/entry` so URL reflects selected company.
- Keep hybrid boundary: shared shell-level selector behavior remains in shared layout; existing role workspaces remain in current areas/routes.

## Capabilities

### Modified Capabilities
- `landing-page-entry-and-auth-routing`
- `slug-based-operations-routing`
- `standard-app-shell-layout`

### Potential New Capability
- `active-user-context-and-company-selection` for explicit selector behavior and authorization constraints.

## Impact

- Affected systems: `WebApp` topbar layout and login partial, entry routing/controller behavior, claims transformation/context resolution.
- Security impact: requires strict server-side validation so users can only select contexts and companies they are authorized for.
- UX impact: authenticated users gain explicit context and tenant switching controls in a single location.
