## Context

The project’s architecture mandates repository interfaces and EF Core implementations per domain aggregate, with shared base abstractions in `Base.Contracts.DAL` and `Base.DAL.EF`. Currently the codebase has base repository types but lacks a consistent set of concrete repositories/interfaces across the domain, which blocks predictable data access patterns and tenant-safe querying. The proposal establishes two capabilities: repository contracts and repository implementations for domain aggregates.

## Goals / Non-Goals

**Goals:**
- Define a consistent repository contract pattern for domain aggregates, aligned with existing base abstractions.
- Implement EF Core repositories that inherit from the base EF repository and honor no-tracking update requirements.
- Ensure repositories are tenant-aware where applicable (CompanyId filtering) and are discoverable via DI registration patterns.

**Non-Goals:**
- Introducing new domain entities or changing business requirements.
- Modifying database schema or migrations.
- Implementing higher-level BLL logic or UI features beyond repository wiring.

## Decisions

- **Repository interface per aggregate in App.DAL.Contracts** → Provides explicit contracts and prevents direct DbContext usage across the solution, improving testability and enforcing architecture rules.
- **EF Core repositories in App.DAL.EF inheriting Base.DAL.EF.BaseRepository** → Reuses shared CRUD behavior and ensures consistent handling of no-tracking update semantics.
- **Tenant-aware querying at repository level** → Centralizes CompanyId filtering to reduce cross-tenant leakage risk; aligns with multi-tenancy requirements.
- **Registration via DAL/BLL composition root (existing setup extension points)** → Keeps construction in one place and avoids scattering DbContext usage.

Alternatives considered:
- **Generic repository only** → Rejected because domain-specific query needs and tenant filters require explicit interfaces for clarity and safety.
- **Direct DbContext access in BLL** → Rejected due to architectural constraints and weaker testability.

## Risks / Trade-offs

- **Risk:** Repository proliferation increases maintenance overhead → **Mitigation:** Use shared base repository and consistent conventions to reduce boilerplate.
- **Risk:** Incorrect tenant filters could leak data → **Mitigation:** Include tenant filtering rules in repository contracts/specs and validate in tests.
- **Trade-off:** Slight upfront design effort → **Benefit:** Long-term consistency across DAL/BLL and safer multi-tenant boundaries.
