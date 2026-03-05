## ADDED Requirements

### Requirement: Company users SHALL access a dedicated Menus & Scheduling workspace
The system SHALL provide a dedicated company-view Menus & Scheduling page or subpage set where authorized company users can perform weekly menu planning and scheduling operations.

#### Scenario: Authorized user opens Menus & Scheduling
- **WHEN** a signed-in CompanyOwner, CompanyAdmin, or CompanyManager navigates company operations
- **THEN** the UI provides an entry to Menus & Scheduling
- **AND** the Menus & Scheduling route loads successfully for the active company context

#### Scenario: Unauthorized user is denied Menus & Scheduling operations
- **WHEN** a user without required company operational permissions attempts to access Menus & Scheduling routes
- **THEN** the UI and backend authorization deny access

### Requirement: Weekly menu planning SHALL support week-oriented visibility
The system SHALL provide week-based visibility of menu composition so company operators can plan recipes by week and category.

#### Scenario: Operator views a specific planning week
- **WHEN** an authorized operator selects a target week in Menus & Scheduling
- **THEN** the system displays the week context
- **AND** shows assigned recipes grouped by category for that week

### Requirement: Menu configuration SHALL support category quotas and scheduling rules
The system SHALL allow company operators to configure weekly menu rules, including number of recipes per category, customer selection deadline policy, and no-repeat window length in weeks.

#### Scenario: Operator configures category recipe counts
- **WHEN** an authorized operator saves recipe count configuration per category
- **THEN** the configuration is persisted for the active company
- **AND** subsequent weekly planning enforces those per-category counts

#### Scenario: Operator configures deadline and no-repeat policy
- **WHEN** an authorized operator saves a customer selection deadline rule and no-repeat N weeks value
- **THEN** the configuration is persisted for the active company
- **AND** validation logic applies these rules in recipe assignment and selection previews

### Requirement: Recipe assignment to weeks SHALL enforce configuration constraints
The system SHALL support assigning recipes to a target week while enforcing category quotas and no-repeat constraints derived from company configuration.

#### Scenario: Assignment succeeds when constraints are satisfied
- **WHEN** an authorized operator assigns a recipe to a week and category within configured limits and repeat-window rules
- **THEN** the assignment is persisted for that company and week

#### Scenario: Assignment fails when no-repeat or quota constraints are violated
- **WHEN** an authorized operator assigns a recipe that violates configured category limits or no-repeat N weeks constraints
- **THEN** the system rejects the assignment
- **AND** returns a clear validation message indicating the violated rule
