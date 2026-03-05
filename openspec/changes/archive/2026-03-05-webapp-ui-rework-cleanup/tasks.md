## 1. Scope Baseline and Retention Map

- [x] 1.1 Inventory `WebApp` controllers, views, and view models and classify each as retained continuity flow or removable legacy surface.
- [x] 1.2 Produce an explicit retention whitelist for company registration, customer entry/registration continuity, login/auth lifecycle, and slug dependencies.
- [x] 1.3 Identify shared layout/navigation references that point to removable endpoints.

## 2. Remove Legacy WebApp UI Surface

- [x] 2.1 Delete non-whitelisted controllers from `WebApp` and remove associated route/action references.
- [x] 2.2 Delete non-whitelisted view models and clean up compile-time references/usings.
- [x] 2.3 Delete non-whitelisted views and partials tied to removed workflows.
- [x] 2.4 Update shared layout/navigation to remove links to deleted routes.

## 3. Preserve and Validate Critical Continuity Flows

- [x] 3.1 Verify public landing page continues to provide register/login entry actions.
- [x] 3.2 Verify company registration flow remains reachable and functional.
- [x] 3.3 Verify customer continuity entry/registration flow remains reachable and functional.
- [x] 3.4 Verify authentication/login and post-login redirect behavior still resolves to authorized contexts.
- [x] 3.5 Verify slug-based routing, slug validation, and authorization checks still execute on retained operational routes.

## 4. Align Specifications and Change Artifacts

- [x] 4.1 Add new capability spec for cleanup boundary (`webapp-ui-cleanup-boundary`).
- [x] 4.2 Apply spec deltas for `workflow-complete-web-ui` to constrain interim required UI scope.
- [x] 4.3 Apply spec deltas for `landing-page-entry-and-auth-routing` to preserve cleanup-phase entry/auth behavior.
- [x] 4.4 Apply spec deltas for `slug-based-operations-routing` to lock slug infrastructure continuity.

## 5. Verification and Handoff

- [x] 5.1 Run build/tests needed to confirm no compile/runtime breakage after removals.
- [x] 5.2 Perform route smoke-checks for retained flows and confirm removed legacy routes are unavailable.
- [x] 5.3 Document cleanup outcome and known temporary scope reductions in change summary/handoff notes.
