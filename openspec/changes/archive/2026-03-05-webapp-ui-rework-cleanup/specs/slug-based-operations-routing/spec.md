## MODIFIED Requirements

### Requirement: Operational pages SHALL use canonical slug-prefixed URLs
All retained operational pages after cleanup SHALL continue to use canonical slug-prefixed routes so tenant context remains explicit. Removal of legacy UI pages MUST NOT alter canonical slug route shape for preserved flows.

#### Scenario: User opens a retained operational page after cleanup
- **WHEN** an authenticated user navigates to a retained company or customer operational destination
- **THEN** the page is served from a canonical slug-prefixed URL

### Requirement: Slug validation and access checks SHALL execute before operations data is returned
For every retained slug-scoped operational request, the system SHALL continue validating slug resolution and enforcing authorization before returning content or data, even when upstream UI surface has been reduced.

#### Scenario: Unknown slug requested after cleanup
- **WHEN** a request targets a retained operational URL with an unresolved slug
- **THEN** the system returns not-found and does not render operations content

#### Scenario: Unauthorized slug requested after cleanup
- **WHEN** an authenticated user requests a retained operational URL for a slug they are not authorized to access
- **THEN** the system denies access and does not disclose tenant-scoped data

