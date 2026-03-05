## Why

The platform currently lacks a clear unauthenticated entry experience that supports both new company onboarding and returning user access. Adding a dedicated landing page now is necessary to enable self-service tenant growth while standardizing authenticated and unauthenticated navigation behavior across the app.

## What Changes

- Add a new public landing page as the default entry point, presenting clear primary actions for self-service company signup and existing-user login.
- Introduce a self-service signup flow that creates a new tenant (company) and provisions the initial `CompanyOwner` user in one guided experience.
- Integrate existing-user login access from the landing experience and route authenticated users into tenant-aware/company views.
- Establish a standard application layout with:
  - Left sidebar navigation
  - Platform name in the top-left area
  - User area in the top-right showing:
    - `Hello <user email>` and logout when authenticated
    - Register and login actions when unauthenticated
- Align UI shell behavior so authenticated and unauthenticated states render consistent, role-appropriate top navigation controls.

## Capabilities

### New Capabilities
- `landing-page-entry-and-auth-routing`: Public landing page and top-level routing for signup/login entry points and post-auth navigation.
- `self-service-tenant-onboarding`: Tenant creation workflow that registers a company and creates an initial `CompanyOwner` account.
- `standard-app-shell-layout`: Shared layout contract for sidebar + header regions with auth-aware user action rendering.

### Modified Capabilities
- `professional-ui-foundation`: Extend existing UI foundation requirements to include the new standardized shell placement rules (left sidebar, top-left platform brand, top-right auth-dependent actions) and landing-page integration.

## Impact

- Affected systems: `WebApp` routing, MVC/Razor views/layouts, Identity registration/login surfaces, and tenant onboarding orchestration in BLL/DAL boundaries.
- Affected user journeys: first-time company onboarding, returning-user sign-in, and global navigation consistency.
- Potential API/domain impact: tenant provisioning and owner-user bootstrap flow may require explicit application-service endpoints/use-cases and validation rules.
- Dependencies: ASP.NET Core Identity flows, tenant context resolution, and existing UI styling/layout assets.
