## MODIFIED Requirements

### Requirement: Sidebar navigation SHALL honor authorization boundaries
Sidebar navigation entries MUST be filtered to the current user’s allowed context and SHALL generate slug-aware links for operational destinations so users remain within authorized tenant scope.

#### Scenario: User lacks access to an administrative section
- **WHEN** a non-authorized user renders the shared layout
- **THEN** sidebar items for unauthorized sections are not shown

#### Scenario: User follows an operations link from the sidebar
- **WHEN** an authorized user selects a sidebar navigation item targeting an operational page
- **THEN** the generated destination URL includes the active authorized slug segment
- **AND** the destination resolves to the canonical slug-scoped operational route
