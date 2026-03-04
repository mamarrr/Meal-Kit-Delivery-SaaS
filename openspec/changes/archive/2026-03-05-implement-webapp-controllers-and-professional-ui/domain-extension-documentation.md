# Domain Extension Documentation

This document describes newly introduced system/support entities for SYS-04..SYS-08 workflows.

## 1) SystemSetting

**Entity:** `SystemSetting`

| Attribute | Type | NULL/NOT NULL | UNIQUE/NOT UNIQUE |
|---|---|---|---|
| `Id` | `Guid` | NOT NULL | UNIQUE (PK) |
| `Category` | `string` | NOT NULL | NOT UNIQUE |
| `Key` | `string` | NOT NULL | NOT UNIQUE *(composite unique with `Category`)* |
| `Value` | `string` | NOT NULL | NOT UNIQUE |
| `ValueType` | `string` | NOT NULL | NOT UNIQUE |
| `IsSensitive` | `bool` | NOT NULL | NOT UNIQUE |
| `UpdatedAt` | `DateTime` | NOT NULL | NOT UNIQUE |
| `UpdatedByAppUserId` | `Guid` | NULL | NOT UNIQUE |

**Relations**
- Many `SystemSetting` → zero/one `AppUser` (`UpdatedByAppUserId`, restrictive delete).
- Unique index: (`Category`, `Key`).

## 2) TenantSupportAccess

**Entity:** `TenantSupportAccess`

| Attribute | Type | NULL/NOT NULL | UNIQUE/NOT UNIQUE |
|---|---|---|---|
| `Id` | `Guid` | NOT NULL | UNIQUE (PK) |
| `CompanyId` | `Guid` | NOT NULL | NOT UNIQUE |
| `SupportUserId` | `Guid` | NOT NULL | NOT UNIQUE |
| `IsReadOnly` | `bool` | NOT NULL | NOT UNIQUE |
| `Reason` | `string` | NOT NULL | NOT UNIQUE |
| `GrantedAt` | `DateTime` | NOT NULL | NOT UNIQUE |
| `RevokedAt` | `DateTime?` | NULL | NOT UNIQUE |
| `GrantedByAppUserId` | `Guid` | NOT NULL | NOT UNIQUE |

**Relations**
- Many `TenantSupportAccess` → one `Company`.
- Many `TenantSupportAccess` → one `AppUser` as `SupportUser` (`SupportUserId`, restrictive delete).
- Many `TenantSupportAccess` → one `AppUser` as `GrantedByAppUser` (`GrantedByAppUserId`, restrictive delete).
- Supporting index: (`CompanyId`, `SupportUserId`, `RevokedAt`).

## 3) SupportTicketStatus

**Entity:** `SupportTicketStatus`

| Attribute | Type | NULL/NOT NULL | UNIQUE/NOT UNIQUE |
|---|---|---|---|
| `Id` | `Guid` | NOT NULL | UNIQUE (PK) |
| `Code` | `string` | NOT NULL | UNIQUE |
| `Label` | `string` | NOT NULL | NOT UNIQUE |

**Relations**
- One `SupportTicketStatus` → many `SupportTicket`.

## 4) SupportTicket

**Entity:** `SupportTicket`

| Attribute | Type | NULL/NOT NULL | UNIQUE/NOT UNIQUE |
|---|---|---|---|
| `Id` | `Guid` | NOT NULL | UNIQUE (PK) |
| `Title` | `string` | NOT NULL | NOT UNIQUE |
| `Description` | `string` | NOT NULL | NOT UNIQUE |
| `Priority` | `string` | NOT NULL | NOT UNIQUE |
| `CreatedAt` | `DateTime` | NOT NULL | NOT UNIQUE |
| `UpdatedAt` | `DateTime?` | NULL | NOT UNIQUE |
| `ClosedAt` | `DateTime?` | NULL | NOT UNIQUE |
| `CompanyId` | `Guid?` | NULL | NOT UNIQUE |
| `CreatedByAppUserId` | `Guid` | NOT NULL | NOT UNIQUE |
| `AssignedToAppUserId` | `Guid?` | NULL | NOT UNIQUE |
| `SupportTicketStatusId` | `Guid` | NOT NULL | NOT UNIQUE |

**Relations**
- Many `SupportTicket` → zero/one `Company` (`CompanyId`).
- Many `SupportTicket` → one `AppUser` as `CreatedByAppUser` (`CreatedByAppUserId`, restrictive delete).
- Many `SupportTicket` → zero/one `AppUser` as `AssignedToAppUser` (`AssignedToAppUserId`, restrictive delete).
- Many `SupportTicket` → one `SupportTicketStatus` (`SupportTicketStatusId`).

## 5) SupportImpersonationSession

**Entity:** `SupportImpersonationSession`

| Attribute | Type | NULL/NOT NULL | UNIQUE/NOT UNIQUE |
|---|---|---|---|
| `Id` | `Guid` | NOT NULL | UNIQUE (PK) |
| `CompanyId` | `Guid` | NOT NULL | NOT UNIQUE |
| `SupportUserId` | `Guid` | NOT NULL | NOT UNIQUE |
| `ImpersonatedAppUserId` | `Guid` | NOT NULL | NOT UNIQUE |
| `Reason` | `string` | NOT NULL | NOT UNIQUE |
| `StartedAt` | `DateTime` | NOT NULL | NOT UNIQUE |
| `EndedAt` | `DateTime?` | NULL | NOT UNIQUE |

**Relations**
- Many `SupportImpersonationSession` → one `Company`.
- Many `SupportImpersonationSession` → one `AppUser` as `SupportUser` (`SupportUserId`, restrictive delete).
- Many `SupportImpersonationSession` → one `AppUser` as `ImpersonatedAppUser` (`ImpersonatedAppUserId`, restrictive delete).

## 6) SystemAnalyticsSnapshot

**Entity:** `SystemAnalyticsSnapshot`

| Attribute | Type | NULL/NOT NULL | UNIQUE/NOT UNIQUE |
|---|---|---|---|
| `Id` | `Guid` | NOT NULL | UNIQUE (PK) |
| `CapturedAt` | `DateTime` | NOT NULL | NOT UNIQUE |
| `ActiveCompanies` | `int` | NOT NULL | NOT UNIQUE |
| `ActiveSubscribers` | `int` | NOT NULL | NOT UNIQUE |
| `WeeklyDeliveries` | `int` | NOT NULL | NOT UNIQUE |
| `OpenSupportTickets` | `int` | NOT NULL | NOT UNIQUE |

**Relations**
- No foreign-key relations (snapshot table).

## Migration

Database migration added for these entities:
- `App.DAL.EF/Migrations/20260304155325_SupportSystemWorkflows.cs`

