## Why

The WebApp currently lacks a complete MVC controller layer and a cohesive, production-quality UI that exposes all required business workflows. Defining this change now enables structured, tenant-safe delivery of missing web functionality and gives a clear contract for validating UI completeness against platform workflows.

## What Changes

- Scaffold missing MVC controllers in WebApp using `dotnet aspnet-codegenerator` conventions documented in `README-TECH.md`, then refine generated controllers for project standards (tenant scoping, service usage, no-tracking-safe updates, role authorization, and view model usage).
- Build and align UI screens and navigation for both Customer and Company views so all workflows described in `openspec/config.yaml` are available through the web interface.
- Introduce a professional UI baseline (layout consistency, information hierarchy, workflow-oriented navigation, reusable components/partials, and clear status/error feedback) across generated and existing screens.
- Define acceptance criteria for “UI completion” based on workflow coverage from `openspec/config.yaml`, including evidence mapping from workflow requirements to implemented pages/actions.
- Keep architecture boundaries intact (WebApp → App.BLL → App.DAL.EF → App.Domain) while expanding controller/view integration.

## Capabilities

### New Capabilities
- `mvc-controller-scaffolding-and-hardening`: Standardized generation and hardening of MVC controllers for domain workflows using project-specific conventions and constraints.
- `workflow-complete-web-ui`: End-to-end UI coverage for required customer/company/system workflows, including routing, actions, and views.
- `professional-ui-foundation`: Consistent, clean, professional visual and interaction system for the WebApp.
- `workflow-coverage-validation`: Traceable verification that all workflows defined in `openspec/config.yaml` are represented in UI and controller surface.

### Modified Capabilities
- `service-contracts`: Service usage requirements are extended at the web layer to guarantee controller actions rely on contract-based BLL operations for newly surfaced workflows.
- `service-implementations`: Existing service behavior and query/update paths may require requirement-level additions to support complete controller/UI workflows under tenant and no-tracking constraints.

## Impact

- Affected code: `WebApp` controllers, views, view models, layout/navigation, and supporting composition code.
- Affected architecture boundaries: stronger integration points between WebApp and BLL service contracts.
- Affected specs: new capability specs for controller generation/hardening, UI workflow coverage, UI quality baseline, and workflow completeness validation; plus modifications to service capability specs where requirements change.
- Dependencies/tooling: continued use of ASP.NET scaffolding tools (`dotnet-aspnet-codegenerator`) and project conventions from `README-TECH.md`.
