## 1. Routing and entry-point setup

- [x] 1.1 Add a dedicated public landing route/controller/view and make it the default `/` entry point.
- [x] 1.2 Wire landing page CTAs to registration (self-service tenant onboarding) and existing-user login flows.
- [x] 1.3 Define post-login redirection behavior to route authenticated users into an allowed company/customer context entry page.

## 2. Self-service tenant onboarding workflow

- [x] 2.1 Create onboarding ViewModel(s) for company + owner input and server-side validation.
- [x] 2.2 Implement onboarding application service/use-case that creates tenant, owner identity user, role assignment, and initial tenant linkage.
- [x] 2.3 Add onboarding controller endpoints (GET/POST) that call the service and render success/error outcomes.
- [x] 2.4 Add failure-handling/compensation rules to avoid unsafe partial onboarding states and ensure no cross-tenant access leakage.

## 3. Standard application shell layout

- [x] 3.1 Update shared layout to include left sidebar region, top-left platform name area, and top-right account actions area.
- [x] 3.2 Implement auth-aware account action partial/component: authenticated shows `Hello <email>` + logout; unauthenticated shows register + login.
- [x] 3.3 Implement a typed sidebar navigation model filtered by authorization/context.
- [x] 3.4 Apply the shared shell to landing/authenticated pages that should use the standard layout without duplicating markup.

## 4. Authorization, tenant-safety, and UX consistency

- [x] 4.1 Ensure all new navigation links and routes preserve existing authorization checks and tenant isolation constraints.
- [x] 4.2 Verify unauthenticated vs authenticated header behaviors across key pages for consistency.
- [x] 4.3 Ensure onboarding cannot grant access to unauthorized tenant data in success or failure paths.

## 5. Validation and readiness

- [x] 5.1 Add/adjust tests for landing routing, onboarding success/failure behavior, and auth-aware layout rendering.
- [x] 5.2 Run solution build/tests and fix regressions introduced by the new layout and onboarding flow.
- [x] 5.3 Perform manual smoke checks for: landing page, signup, login, sidebar visibility, and top-right account actions.
