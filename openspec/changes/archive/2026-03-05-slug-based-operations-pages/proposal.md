## Why

Operations pages are currently split across mixed route patterns, which creates inconsistent navigation, weak tenant context visibility, and higher risk of users operating in the wrong company scope. Standardizing operations under slug-based routes now is necessary to enforce a single predictable multi-tenant URL contract before more workflow surfaces are added.

## What Changes

- Standardize operations UI routing so all operational pages resolve under `/{slug}/...` paths.
- Define canonical route patterns for system, company, and customer operational pages, including examples such as `/{slug}/CompanySettings`.
- Introduce explicit behavior for slug validation and tenant scoping on operations endpoints.
- Define migration expectations from legacy non-slug operational URLs to slug-based URLs (redirect or canonical-link behavior).
- Align navigation generation requirements so links to operational areas always include slug context.

## Capabilities

### New Capabilities
- `slug-based-operations-routing`: Defines the canonical slug-prefixed routing contract for all operational pages (system, company, customer), including validation and URL generation behavior.

### Modified Capabilities
- `landing-page-entry-and-auth-routing`: Updates post-auth and entry routing requirements to land users into slug-scoped operational paths.
- `standard-app-shell-layout`: Updates navigation requirements so app-shell links for operational pages always preserve and emit slug-aware URLs.

## Impact

- Affected systems: MVC endpoint routing, controller route attributes/conventions, navigation/link builders, and tenant resolution middleware/services.
- Affected user flows: direct URL entry, post-login redirects, sidebar/top-nav navigation, and deep links/bookmarks.
- Compatibility impact: legacy non-slug operational routes may require redirects and phased deprecation handling.
- Testing impact: integration tests for route matching, authorization within slug scope, and navigation link correctness across system/company/customer contexts.
