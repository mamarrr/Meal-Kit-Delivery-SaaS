## 1. Customer discovery and subscription management

- [x] 1.1 Add/extend customer-facing BLL and DAL contracts for public box discovery with company, price range, and dietary-category filters
- [x] 1.2 Implement tenant-safe discovery repository queries and service methods for filterable public subscription offerings
- [x] 1.3 Implement subscribe flow from discovery results (create customer-owned meal subscription)
- [x] 1.4 Implement manage subscriptions list/detail/unsubscribe service endpoints and MVC actions with ownership authorization checks
- [x] 1.5 Create Razor views and ViewModels for Discover Subscriptions and Manage Subscriptions pages (including filter state and validation)

## 2. Preferences, exclusions, and nutrition filtering

- [x] 2.1 Implement retrieval of dietary categories and ingredients constrained to currently subscribed companies
- [x] 2.2 Implement update workflows for customer dietary preferences and ingredient exclusions with explicit entity `Update()` usage on modifications
- [x] 2.3 Add nutrition filter DTO/contracts (calories, protein, carbs, fat, fiber, sodium min/max) across controller → service → repository layers
- [x] 2.4 Apply nutrition filters in meal candidate/discovery queries and ensure composition with preference/exclusion constraints
- [x] 2.5 Create Razor views and ViewModels for Preferences/Exclusions page with persisted form state and validation messages

## 3. Deliveries and complaints customer pages

- [x] 3.1 Implement customer-scoped delivery query surfaces for delivery list/history with outcome states
- [x] 3.2 Implement customer complaint create/list/status workflows with strict customer-ownership constraints
- [x] 3.3 Build MVC controllers/actions and ViewModels for Deliveries and Complaints pages
- [x] 3.4 Create Razor views for Deliveries and Complaints including submission UX and status timeline display

## 4. Deadline and auto-selection policy

- [x] 4.1 Add configuration model and lookup logic for selection deadline and non-repetition lookback window (default 8 weeks)
- [x] 4.2 Implement auto-selection policy pipeline: missed-deadline detection, candidate generation, recent-repeat exclusion, rating-based ranking, deterministic tiebreak
- [x] 4.3 Persist fallback auto-selections as explicit meal selection records with traceable metadata/reason fields
- [x] 4.4 Implement fallback behavior for insufficient candidate sets and expose user-visible outcome messaging

## 5. UI workflow completeness and navigation integration

- [x] 5.1 Add customer navigation entries for Discover, Manage Subscriptions, Preferences/Exclusions, Deliveries, and Complaints in customer context
- [x] 5.2 Ensure company/customer view separation remains explicit and route authorization boundaries are enforced
- [x] 5.3 Validate end-to-end route reachability for all required customer workflow pages

## 6. Verification and quality gates

- [x] 6.1 Add/extend unit tests for filter logic, preference/exclusion application, nutrition criteria validation, and auto-selection ranking
- [x] 6.2 Add integration tests for tenant isolation and customer ownership on subscriptions, deliveries, and complaints
- [x] 6.3 Add scenario tests for deadline handling and non-repetition lookback behavior
- [x] 6.4 Run build/tests and resolve regressions prior to implementation handoff
