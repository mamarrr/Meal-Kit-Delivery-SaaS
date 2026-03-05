## Why

Customer-facing workflows are currently incomplete compared to company operations, which blocks end-to-end subscription lifecycle use in the platform. This change is needed now to deliver a production-ready customer experience for discovery, subscription management, preferences, deliveries, complaints, and nutritionally informed meal selection.

## What Changes

- Add customer workflow pages for:
  - Discover Subscriptions: browse public meal boxes across companies with filtering by company, price range, and dietary categories, and subscribe from discovery.
  - Manage Subscriptions: list current customer meal subscriptions, view subscription details, and unsubscribe.
  - Preferences/Exclusions: manage dietary preferences and ingredient exclusions, using company-specific categories and ingredients from currently subscribed companies.
  - Deliveries: view customer delivery schedule/history and delivery outcome states.
  - Complaints: submit and track quality complaints related to delivered meals.
- Define customer meal choice behavior with configurable selection deadline and automatic fallback selection when deadline is missed.
- Define non-repetition rules for auto-selection using rating history and a configurable lookback window (default 8 weeks).
- Add nutrition-driven meal filtering criteria for customer meal selection (calories, protein, carbs, fat, fiber, sodium per serving).
- Align customer-side UI and contracts with existing multi-tenant customer/company split and role-based access.

## Capabilities

### New Capabilities
- `customer-subscription-discovery-and-management`: Customer browse/subscribe/unsubscribe flows, including filterable discovery and subscription detail management.
- `customer-preferences-exclusions-and-nutrition-filtering`: Customer preference/exclusion management and nutrition-based filtering for meal selection.
- `customer-deliveries-and-complaints-workflow`: Customer delivery visibility and complaint submission/tracking workflow.
- `customer-meal-selection-deadline-and-auto-selection`: Configurable deadline-driven selection behavior with rating-based auto-selection and non-repetition window.

### Modified Capabilities
- `workflow-complete-web-ui`: Extend workflow coverage requirements to include complete customer-facing pages (discover/manage/preferences/deliveries/complaints).
- `menu-and-recipe-model`: Extend requirements to cover customer auto-selection constraints, lookback non-repetition policy, and nutritional filter usage in customer meal selection.

## Impact

- Affected areas: WebApp customer controllers/views/viewmodels, BLL service contracts and implementations, DAL repository query surfaces, and tenant-safe filtering across customer workflows.
- Data/model impact: usage and potentially extension of meal selection, subscription, nutrition, delivery, and complaint-related entities to satisfy customer UX requirements.
- Authorization impact: customer-view route and permission checks for customer-owned subscriptions, deliveries, and complaints.
- Cross-capability impact: integrates with existing specs for workflow completeness and menu/selection behavior while adding dedicated customer workflow capabilities.
