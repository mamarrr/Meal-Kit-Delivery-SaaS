## 1. Context and Company Selection Domain

- [x] 1.1 Define allowed context matrix (`customer`, `company`, `system`) against roles and claims.
- [x] 1.2 Define active selection persistence contract for `active_context` and `active_company` cookies.
- [x] 1.3 Define fallback rules when persisted selection is invalid or unauthorized.

## 2. Shared Topbar UX and Interaction

- [x] 2.1 Add context selector UI next to hello/logout in shared shell.
- [x] 2.2 Add company dropdown for users with multiple active company memberships.
- [x] 2.3 Ensure selector visibility and defaults are role-aware and accessible.

## 3. Switching Endpoints and Routing

- [x] 3.1 Add server endpoints/actions for context switch and company switch.
- [x] 3.2 Enforce server-side authorization for requested context and company membership.
- [x] 3.3 Redirect company switches to `/{slug}/entry`; redirect non-company context switches to `/entry`.

## 4. Claims and Request Lifecycle Integration

- [x] 4.1 Update claims/context transformation to respect active company selection.
- [x] 4.2 Keep slug resolution consistent with selected company in company context.
- [x] 4.3 Ensure invalid company selections are cleared and recovered safely.

## 5. Hybrid Area Boundary Confirmation

- [x] 5.1 Keep selector orchestration in shared layout and shared controllers.
- [x] 5.2 Keep system workspace in existing root area; do not add new customer/company areas in this change.
- [x] 5.3 Document deferred extraction path for future area split if workflows grow.

## 6. Verification

- [x] 6.1 Add automated tests for role/context matrix and unauthorized switch attempts.
- [x] 6.2 Add tests for multi-company selection and slug redirect correctness.
- [x] 6.3 Run manual smoke tests for topbar behavior and persistence across requests.
