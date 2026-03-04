## Purpose

Define complete WebApp workflow coverage requirements across customer and company views, with clear authorization and usable feedback.

## Requirements

### Requirement: UI SHALL cover all required business workflows from configuration context
The WebApp SHALL expose customer-view and company-view workflows so each workflow described in the project configuration context is reachable through routed pages, controller actions, and views.

#### Scenario: Workflow has at least one UI entry point
- **WHEN** a required workflow is listed in the project configuration context
- **THEN** users can navigate to at least one corresponding UI flow that starts and completes the workflow

### Requirement: Role-based workflow visibility SHALL be enforced in UI navigation and actions
Workflow entry points and actions MUST be restricted by role and view context (system, company, customer) according to authorization requirements.

#### Scenario: Unauthorized role cannot access restricted workflow
- **WHEN** a user without required role attempts to open a restricted workflow route or action
- **THEN** access is denied and protected workflow data is not displayed

### Requirement: Customer and company view separation SHALL be explicit
The UI MUST present separate, coherent navigation surfaces for customer operations and company operations while preserving single-account identity behavior.

#### Scenario: User switches context between customer and company views
- **WHEN** a user with both customer and company roles navigates the application
- **THEN** the UI clearly distinguishes customer-view workflows from company-view workflows and preserves authorization boundaries

### Requirement: Workflow actions SHALL provide completion feedback
Each workflow form/action MUST return clear success, validation, and failure feedback in-page so users can complete workflows without ambiguity.

#### Scenario: Validation failure is actionable
- **WHEN** a user submits invalid workflow input
- **THEN** the UI displays field-level and summary validation guidance that allows correcting and resubmitting the workflow
