## ADDED Requirements

### Requirement: Operational pages SHALL use canonical slug-prefixed URLs
The system SHALL expose operational pages using canonical slug-prefixed paths so that tenant context is always explicit in the URL. Canonical operational URLs MUST follow `/{slug}/{OperationPage}` or an equivalent slug-first route form in MVC route templates.

#### Scenario: User opens a company operations page
- **WHEN** an authenticated user navigates to a company operations destination such as company settings
- **THEN** the page is served from a canonical slug-prefixed URL (for example, `/{slug}/CompanySettings`)

### Requirement: System, company, and customer operations SHALL be represented under slug scope
The system MUST route system operations, company operations, and customer operations using slug-scoped URLs and SHALL NOT require non-slug operational entry points as canonical addresses.

#### Scenario: User navigates to operations in different contexts
- **WHEN** the user navigates to system, company, or customer operational pages
- **THEN** each resolved route includes slug scope in the URL

### Requirement: Slug validation and access checks SHALL execute before operations data is returned
For every slug-scoped operational request, the system SHALL validate that the slug resolves to a known tenant context and MUST enforce authorization before returning operation content or data.

#### Scenario: Unknown slug requested
- **WHEN** a request targets an operational URL with a slug that does not resolve
- **THEN** the system returns a not-found result and does not render operations content

#### Scenario: Unauthorized slug requested
- **WHEN** an authenticated user requests an operational URL for a slug they are not authorized to access
- **THEN** the system denies access and does not disclose tenant-scoped operation data

### Requirement: Legacy non-slug operational URLs SHALL redirect to canonical slug URLs during migration
The system SHALL provide compatibility behavior for known legacy non-slug operational routes by redirecting to canonical slug-prefixed routes when tenant context can be safely resolved.

#### Scenario: Legacy operations bookmark is used
- **WHEN** a user requests a supported legacy non-slug operational URL
- **THEN** the system redirects the request to the corresponding canonical slug-prefixed URL
