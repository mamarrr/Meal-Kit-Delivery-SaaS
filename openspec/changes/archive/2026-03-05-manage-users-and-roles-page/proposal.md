## Why

Company-level user administration is currently fragmented, making it hard for authorized staff to quickly onboard colleagues, assign correct roles, and keep access accurate as team responsibilities change. This change introduces a dedicated management flow now to support secure day-to-day tenant operations and ownership continuity in a multi-company environment.

## What Changes

- Add a new company-scoped **Manage Users and Roles** page where CompanyOwner and CompanyAdmin can manage company memberships.
- Add capability to invite/add an existing platform account to the current company by email with selected role, without acceptance workflow; if email exists, membership is created immediately.
- Add role management actions for existing company users to switch between CompanyAdmin, CompanyManager, and CompanyEmployee (within authorization boundaries).
- Add company user removal action for CompanyOwner and CompanyAdmin to revoke company membership.
- Add CompanyOwner-only ownership transfer action to promote an existing company member to CompanyOwner and demote prior owner according to role rules.
- Enforce tenant-isolated authorization and validation rules for all membership and role-changing operations.

## Capabilities

### New Capabilities
- `company-user-and-role-management`: Company-level membership lifecycle management (add existing user by email, change role, remove membership) with role-based authorization.
- `company-ownership-transfer`: Controlled ownership transfer within a tenant from current CompanyOwner to another existing company member.

### Modified Capabilities
- `workflow-complete-web-ui`: Extend operational web UI flows with a dedicated page and actions for company user/role administration.
- `service-implementations`: Extend company-scoped service behavior to support validated user membership assignment, role updates, removals, and owner transfer semantics.
- `service-contracts`: Extend service contracts for company membership and role administration use cases.

## Impact

- Affected layers: WebApp controllers/views/viewmodels, BLL service contracts and implementations, DAL queries for account lookup and membership updates.
- Authorization impact: CompanyOwner and CompanyAdmin management permissions; CompanyOwner-exclusive ownership transfer.
- Data integrity impact: role transition rules, prevention of invalid owner state, and tenant boundary enforcement on all mutations.
- Testing impact: add/extend unit and integration tests for authorization, validation (email exists), role transitions, removal, and owner transfer flows.
