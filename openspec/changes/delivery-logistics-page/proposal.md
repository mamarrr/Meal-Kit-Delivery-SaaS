## Why

Company operators currently do not have a unified place to manage delivery logistics workflows end-to-end (zones, schedules, runs/orders, delivery outcomes, and complaint-driven escalation). This change is needed now to complete the company operations surface so day-to-day delivery execution and quality handling can be managed within the SaaS app instead of external spreadsheets/tools.

## What Changes

- Add a new company-facing "Delivery Logistics" page that consolidates operational logistics data and actions in one workspace.
- Add delivery zones listing for the active company tenant.
- Add zone schedule management visibility for delivery days, delivery time windows, and per-window capacity limits.
- Add deliveries/runs and orders-by-date list views for dispatch and operational review.
- Add delivery tracking fields and workflows for success/failure attempts, notes, proof capture metadata, and reschedule actions.
- Add escalation rules management based on complaints and operational priority policies (including freshest stock and earliest slot prioritization).
- Integrate the page into company operations navigation and authorization boundaries for company roles.

## Capabilities

### New Capabilities
- `company-delivery-logistics-management`: Company workspace capability for a unified Delivery Logistics page with sections for zones, schedules, runs/orders, tracking, and escalation rules.
- `delivery-zones-schedules-and-capacity`: Operational configuration capability for tenant-scoped zones, delivery days, time windows, and capacity limits.
- `delivery-runs-orders-and-tracking`: Execution capability for viewing deliveries/orders by date and recording delivery outcomes, notes, proof, and reschedule state.
- `complaint-based-delivery-escalation-rules`: Rules capability for complaint-triggered prioritization behavior (freshest stock, earliest slot) used by logistics operations.

### Modified Capabilities
- `workflow-complete-web-ui`: Extend workflow-complete UI requirements to include Delivery Logistics in the company operations surface.

## Impact

- Affected systems: company MVC controllers, view models, Razor views, navigation/routing, BLL service orchestration, and DAL query paths for delivery entities.
- Affected data scope: tenant-isolated delivery zone/window/run/attempt/complaint records and related scheduling fields.
- Affected authorization: company-role gated access for logistics management and tracking actions.
- Dependencies: existing delivery domain entities/services/repositories are reused and extended where needed; no external third-party dependency is required.
