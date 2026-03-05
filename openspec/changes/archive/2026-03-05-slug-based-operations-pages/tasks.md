## 1. Route Inventory and Canonical Contract Setup

- [x] 1.1 Audit existing system, company, and customer operations endpoints and map current non-slug URLs to target canonical slug-prefixed URLs.
- [x] 1.2 Define and apply slug-first route templates/conventions for operations controllers/pages to enforce canonical `/{slug}/...` routing.
- [x] 1.3 Add reserved/validated handling for invalid or unknown slugs at operations entry points.

## 2. Tenant Resolution, Authorization, and Redirect Migration

- [x] 2.1 Ensure operations requests validate slug-to-tenant resolution before executing business logic.
- [x] 2.2 Enforce authorization checks for slug scope so users cannot access unauthorized tenant operations pages.
- [x] 2.3 Implement legacy non-slug operational route redirects to canonical slug URLs with loop-safe redirect behavior.

## 3. Navigation and Authentication Flow Updates

- [x] 3.1 Update shared app-shell navigation models/partials so operational links always include active slug context.
- [x] 3.2 Update post-authentication routing to land users on authorized slug-scoped operations entry routes.
- [x] 3.3 Handle multi-company user default slug selection consistently when no explicit tenant context is provided.

## 4. Validation and Rollout Safety

- [x] 4.1 Add integration tests for canonical slug route matching across system/company/customer operations pages.
- [x] 4.2 Add tests for unknown slug handling, unauthorized slug access denial, and legacy-to-canonical redirect behavior.
- [x] 4.3 Add navigation/link-generation tests to verify sidebar and entry links emit slug-aware operational URLs.
- [x] 4.4 Validate rollout with telemetry/monitoring for route errors and redirect volume, with documented rollback steps.
