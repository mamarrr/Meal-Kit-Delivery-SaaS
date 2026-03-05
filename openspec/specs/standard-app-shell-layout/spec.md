## Purpose

Define a standard application shell layout with consistent sidebar, branding, and account-action regions.

## Requirements

### Requirement: Application shell SHALL provide standard structural regions
The system SHALL render a shared application shell that includes a left sidebar navigation area, a top-left platform branding area, and a top-right account action area.

#### Scenario: User opens a page that uses the shared shell
- **WHEN** a page is rendered through the standard layout
- **THEN** the page displays left sidebar navigation and header regions for brand and account actions

### Requirement: Top-right account area SHALL be authentication-aware
The top-right account region MUST render `Hello <user email>` and logout when authenticated, and MUST render register/login actions when unauthenticated.

#### Scenario: Authenticated user loads shell-based page
- **WHEN** an authenticated user requests a page using the standard shell
- **THEN** the top-right area shows a greeting with the user email and a logout action

#### Scenario: Anonymous user loads shell-based page
- **WHEN** an unauthenticated user requests a page using the standard shell
- **THEN** the top-right area shows register and login actions

### Requirement: Sidebar navigation SHALL honor authorization boundaries
Sidebar navigation entries MUST be filtered to the current user’s allowed context and SHALL generate slug-aware links for operational destinations so users remain within authorized tenant scope.

#### Scenario: User lacks access to an administrative section
- **WHEN** a non-authorized user renders the shared layout
- **THEN** sidebar items for unauthorized sections are not shown

#### Scenario: User follows an operations link from the sidebar
- **WHEN** an authorized user selects a sidebar navigation item targeting an operational page
- **THEN** the generated destination URL includes the active authorized slug segment
- **AND** the destination resolves to the canonical slug-scoped operational route
