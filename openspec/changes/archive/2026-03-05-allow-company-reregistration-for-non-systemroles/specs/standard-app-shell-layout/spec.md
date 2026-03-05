## MODIFIED Requirements

### Requirement: Sidebar navigation SHALL honor authorization boundaries
Sidebar navigation entries MUST be filtered to the current user’s allowed context and SHALL generate slug-aware links for operational destinations so users remain within authorized tenant scope. The sidebar MUST also include a bottom-positioned "Register new company" action for authenticated users who are not assigned any system-level role, and MUST hide this action for users with any system-level role.

#### Scenario: User lacks access to an administrative section
- **WHEN** a non-authorized user renders the shared layout
- **THEN** sidebar items for unauthorized sections are not shown

#### Scenario: User follows an operations link from the sidebar
- **WHEN** an authorized user selects a sidebar navigation item targeting an operational page
- **THEN** the generated destination URL includes the active authorized slug segment
- **AND** the destination resolves to the canonical slug-scoped operational route

#### Scenario: Eligible authenticated user sees register new company action
- **WHEN** an authenticated user without any system-level role renders the shared layout
- **THEN** the bottom section of the sidebar shows a "Register new company" action

#### Scenario: System-level user does not see register new company action
- **WHEN** an authenticated user with at least one system-level role renders the shared layout
- **THEN** the sidebar does not show the "Register new company" action

