## 1. Company subscriber query and contract foundation

- [x] 1.1 Add/extend company-scoped repository interfaces for subscriber list filtering (search, status, tier, zone) and detail retrieval (addresses, zone, preferences, exclusions, plan/box, ratings, lifecycle history)
- [x] 1.2 Implement repository query projections with strict `CompanyId` filtering for both list and detail paths
- [x] 1.3 Add/extend BLL service contracts and implementations to expose the new subscriber list/detail operations for company workspace use

## 2. Company WebApp page and view models

- [x] 2.1 Create/extend controller actions for Subscribers & Accounts page entry, filtered list retrieval, and subscriber detail loading
- [x] 2.2 Create dedicated ViewModels for filter state, list rows, and detail sections without using `ViewBag`/`ViewData`
- [x] 2.3 Implement Razor UI for Subscribers & Accounts page with on-page sections for list, details, preferences/exclusions, ratings history, and pauses/skips/cancellations

## 3. Navigation, authorization, and UX behavior

- [x] 3.1 Add company workspace navigation entry to Subscribers & Accounts for authorized company roles
- [x] 3.2 Enforce role authorization and tenant-scoped access checks on all related routes/actions
- [x] 3.3 Add empty-state and error-state UI handling for missing ratings history, exclusions, and lifecycle events

## 4. Verification and hardening

- [x] 4.1 Add/update tests for tenant isolation and filter correctness (status, tier, zone, search)
- [x] 4.2 Add/update tests for authorization behavior and workflow reachability in company UI
- [x] 4.3 Validate workflow-complete-web-ui and new capability acceptance scenarios end-to-end
