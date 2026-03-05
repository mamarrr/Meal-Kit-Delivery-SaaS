## ADDED Requirements

### Requirement: Cleanup SHALL preserve only critical continuity flows in WebApp
The cleanup implementation SHALL retain only UI components required for company registration, customer registration/entry continuity, authentication/login lifecycle, and slug infrastructure dependencies needed by those flows.

#### Scenario: Retained inventory is enforced
- **WHEN** cleanup is executed against `WebApp`
- **THEN** only components mapped to the approved continuity-flow inventory remain
- **AND** non-inventory UI components are removed from active code paths

### Requirement: Cleanup SHALL remove non-essential legacy WebApp UI surface
The system SHALL remove legacy `WebApp` controllers, view models, views, and navigation endpoints that are not required by preserved continuity flows.

#### Scenario: Non-essential UI route is requested after cleanup
- **WHEN** a user requests a legacy route outside the retained continuity boundary
- **THEN** the route is not available as an operational UI endpoint

### Requirement: Shared navigation SHALL not expose removed legacy destinations
After cleanup, shared layout and navigation components SHALL expose only retained continuity-flow destinations.

#### Scenario: User views shared navigation after cleanup
- **WHEN** an authenticated or anonymous user loads a page with shared navigation
- **THEN** navigation does not include links to removed legacy workflow pages

