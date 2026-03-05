## Why

Company workspaces currently lack a unified place to manage subscriber operations, forcing staff to piece together subscriber state from multiple screens or data sources. Adding a dedicated Subscribers & Accounts page now closes a core operational gap needed for support, fulfillment planning, and subscription lifecycle handling at tenant level.

## What Changes

- Add a new **Subscribers & Accounts** page in company workspaces with on-page sub-sections for:
  - Subscribers list with search and filters (status, tier, delivery zone)
  - Subscriber detail view including addresses, delivery zone, plan/box, preference profile, exclusion profile, and historical activity
  - Preferences and exclusions management (e.g., shellfish, pork, cilantro)
  - Ratings history visibility to support auto-selection behavior audits
  - Subscription lifecycle states and actions visibility (pauses, skips, cancellations)
- Define tenant-scoped behavior and visibility rules so company users only access subscribers in their own company context.
- Introduce requirements for list/detail data composition so operational teams can manage account context without cross-page navigation for essential subscriber operations.

## Capabilities

### New Capabilities
- `company-subscribers-and-accounts-management`: Company workspace subscriber operations surface covering list filtering, subscriber details, preferences/exclusions, ratings history, and pause/skip/cancellation state visibility.

### Modified Capabilities
- `workflow-complete-web-ui`: Extend required company workflow coverage to include navigation entry and end-to-end page behavior for Subscribers & Accounts management.

## Impact

- Affected systems: Company MVC UI (controllers, views, viewmodels), tenant-scoped BLL services, and repository query surfaces used for subscriber list/detail aggregation.
- Affected APIs/contracts: Company-facing service/repository methods for subscriber search/filtering and detail retrieval may require additions.
- Data/access impact: Additional tenant-filtered reads for subscriber profile, preferences/exclusions, ratings, and subscription lifecycle history.
- No external dependency changes required; implementation remains within existing ASP.NET Core MVC + EF Core architecture.
