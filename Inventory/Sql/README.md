# Sql/ — Per-Tenant Runbook

Hand-written SQL for the Inventory backend. This project's hard rule: **never use
`dotnet ef migrations add/remove/database update`**. Every schema change here is a
tracked `.sql` script, applied by hand (or by an operator-driven pipeline) per
tenant database, and mapped in EF Core with `ExcludeFromMigrations()`.

This folder is **not referenced by any `.csproj`** — these are operator artifacts,
not application content.

```
Sql/
├── README.md                                  <- this file
├── TenantBootstrap/                           <- idempotent; defines target state;
│                                                  run on EVERY tenant DB, new or existing
│   ├── 01_Actions_Schema.sql
│   ├── 02_Users_Columns.sql
│   ├── 02b_Claims_Columns.sql                 <- Roles.IsSystemAdmin, Modules.RequiresSystemAdmin
│   ├── 02c_Core_Modules_Seed.sql              <- Users/Roles Modules rows, CRITICAL, see runbook
│   ├── 03_Actions_Seed.sql
│   ├── 04_Admin_Role.sql
│   └── 05_Admin_User_Assign.sql               <- order-sensitive, see below
├── Migrations/2026-08-user-management/        <- one-shot; a brand-new tenant DB
│   │                                              skips this folder entirely
│   ├── 01_Backfill_RoleId.sql
│   ├── 02_Backfill_RoleActions.sql
│   └── 03_Drop_UserRoles.sql                  <- Checkpoint C only, deferred, gated
└── Migrations/2026-08-claims-authorization-cutover/  <- one-shot; claims cutover
    ├── 01_Report_RoleModules_Drift.sql        <- STRICTLY READ-ONLY, D6 pre-cutover gate
    └── 02_Drop_RoleModules.sql                <- Checkpoint C only, deferred, gated
```

## Claims Authorization Cutover — Checkpoint A -> B -> C runbook

This runbook covers the `claims-authorization-cutover` change layered on top of
the `user-management` change above. `RoleActions` becomes the enforced
authorization source; `RoleModules` is decommissioned only at the very end.

**Checkpoint A — additive schema, zero behavior change.** Run
`TenantBootstrap/02b_Claims_Columns.sql` (between `02` and `03`, see the
updated invocation orders below) and
`Migrations/2026-08-claims-authorization-cutover/01_Report_RoleModules_Drift.sql`
on every tenant. `01_Report_RoleModules_Drift.sql` is **strictly read-only** —
it never inserts or deletes anything. It has two sections:
- **Section A (possible gaps)**: a non-empty result means a human must
  manually grant the missing Action via the existing
  `PUT api/v1/roles/{id}/actions` endpoint after reviewing each row. Do
  **not** re-run `Migrations/2026-08-user-management/02_Backfill_RoleActions.sql`
  to "fix" this — that script is `INSERT ... WHERE NOT EXISTS` and cannot
  tell a genuine gap apart from an Action a tenant admin has already and
  legitimately revoked via that same endpoint since Phase 1 shipped;
  re-running it risks silently resurrecting a revoked grant.
- **Section B (RoleActions with no RoleModules backing)**: informational
  only, expected to be non-empty in the normal case (legitimate post-Phase-1
  admin changes). No action required unless something looks obviously wrong.

Cutover to Checkpoint B proceeds once a human has reviewed both sections —
not once the report is merely empty.

**CRITICAL, discovered during Phase 4 test execution against real tenant
databases**: run `TenantBootstrap/02c_Core_Modules_Seed.sql` (between `02b`
and `03`, see the updated invocation orders below) on every tenant **before**
the Checkpoint B code deploy. Neither this change nor the prior
`user-management` change ever seeded `Users` or `Roles` as `Modules` catalog
rows — invisible until now because `UsersController`/`RolesController` have
always gated on the literal role name `[Authorize(Roles = "Admin")]`,
independent of the `Modules`/`Actions`/`RoleActions` catalog. Checkpoint B's
per-action re-gate (design.md D4) makes those catalog rows load-bearing for
the first time: without `02c`, `03_Actions_Seed.sql` can never derive the
`UsersView`/`UsersCreate`/`RolesEdit`/etc. Action codes, so `RoleActions` can
never grant them to anyone — Checkpoint B would lock **every** role,
including System Admin, out of user and role management on any tenant this
was not run against.

**Checkpoint B — code deploy** (claims middleware, controller re-gate,
`RoleModules` read paths migrated to `RoleActions`). See `design.md` in
`openspec/changes/claims-authorization-cutover/` for the full rollout plan.

**Checkpoint C — decommission, gated.** Only after B is verified green on
every tenant, run
`Migrations/2026-08-claims-authorization-cutover/02_Drop_RoleModules.sql`
per tenant.

## Before running anything: confirm the tenant list

`Tenants:Registry` in `appsettings.json` is the source of truth for which tenant
databases exist, but **deployed environments override it via environment
variables**. Confirm the actual, currently-deployed tenant list operationally
(check the live environment configuration, not just the checked-in
`appsettings.json`) before fanning any script out across tenants.

## Checkpoint A — take a snapshot first

Take a full backup/snapshot of each tenant database **before** running any script
in this folder. Checkpoint A is additive and reversible by dropping the new
objects (see "Rollback" below), but `Migrations/03_Drop_UserRoles.sql`
(Checkpoint C, run much later) is not reversible without this snapshot.

## Invocation order — differs by tenant state

### New tenant DB (no legacy `RoleModules`/`UserRoles` data)

Run `TenantBootstrap/` straight through, 01 → 02c → 05:

```
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\01_Actions_Schema.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\02_Users_Columns.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\02b_Claims_Columns.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\02c_Core_Modules_Seed.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\03_Actions_Seed.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\04_Admin_Role.sql

sqlcmd -S <server> -d <tenantDb> -b -i "Sql\Migrations\2026-08-claims-authorization-cutover\01_Report_RoleModules_Drift.sql"
:: read-only; review both report sections (see the runbook above) before continuing

sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\05_Admin_User_Assign.sql -v AdminEmail="admin@<tenant>"
```

### Existing tenant DB (has legacy `RoleModules`/`UserRoles` data)

`05_Admin_User_Assign.sql` MUST run **last**, after both `Migrations/` scripts —
not as step 5 of bootstrap. `Migrations/01_Backfill_RoleId.sql` independently
re-derives every user's `RoleId` from legacy data; running `05` before `01` lets
`01` silently overwrite the admin `05` just validated, reproducing the exact
lockout `05`'s `RAISERROR` exists to prevent.

```
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\01_Actions_Schema.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\02_Users_Columns.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\02b_Claims_Columns.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\02c_Core_Modules_Seed.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\03_Actions_Seed.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\04_Admin_Role.sql

sqlcmd -S <server> -d <tenantDb> -b -i "Sql\Migrations\2026-08-user-management\01_Backfill_RoleId.sql"
sqlcmd -S <server> -d <tenantDb> -b -i "Sql\Migrations\2026-08-user-management\02_Backfill_RoleActions.sql"

sqlcmd -S <server> -d <tenantDb> -b -i "Sql\Migrations\2026-08-claims-authorization-cutover\01_Report_RoleModules_Drift.sql"
:: read-only; review both report sections (see the runbook above) before continuing.
:: do NOT re-run 02_Backfill_RoleActions.sql above to "fix" Section A findings.

sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\05_Admin_User_Assign.sql -v AdminEmail="admin@<tenant>"
```

Before running `01_Backfill_RoleId.sql`'s `UPDATE`, run its report `SELECT`
first (it is the first statement in the file — run it standalone, or read the
output before the script's own `UPDATE` commits) and keep the output as an
audit record: it lists every multi-role user, the role the backfill will keep,
and the roles it will drop.

## Idempotency check before fan-out

`TenantBootstrap/*` is designed to be safe to re-run. Before fanning any script
out across every tenant, re-run the full `TenantBootstrap/01-02c-05` sequence
on one already-bootstrapped test database and confirm:
- no error
- no duplicate rows in `Actions` or `RoleActions`
- `dbo.Users`/`dbo.Roles`/`dbo.Modules`/`dbo.RoleModules` data is unchanged
- login and sidebar-menu behavior is byte-identical to before the re-run

## Checkpoint C — deferred, gated, per tenant

Do **not** run `Migrations/2026-08-user-management/03_Drop_UserRoles.sql` until:

1. The Checkpoint B code deploy is live and verified (login, sidebar menu,
   create user, deactivate user) on **every** tenant, and
2. at least one JWT lifetime (`Jwt:ExpiresMinutes`, currently 120 minutes) has
   elapsed since that deploy.

```
sqlcmd -S <server> -d <tenantDb> -b -i "Sql\Migrations\2026-08-user-management\03_Drop_UserRoles.sql"
```

This is irreversible without the Checkpoint A snapshot. Confirm
`DROP TABLE UserRoles` succeeds with no consumer errors before moving to the
next tenant.

## Rollback (Checkpoint A only)

```sql
DROP TABLE dbo.RoleActions, dbo.Actions;
ALTER TABLE dbo.Users DROP CONSTRAINT FK_Users_Roles;
DROP INDEX IX_Users_RoleId ON dbo.Users;
ALTER TABLE dbo.Users DROP COLUMN RoleId;
ALTER TABLE dbo.Users DROP CONSTRAINT DF_Users_MustChangePassword;
ALTER TABLE dbo.Users DROP COLUMN MustChangePassword;
```

Rollback for the claims-authorization-cutover Checkpoint A additions
(`02b_Claims_Columns.sql`): drop the two new columns and the seeded `Modules`
catalog row.

```sql
DELETE FROM dbo.Modules WHERE Name = 'Modules' AND RequiresSystemAdmin = 1;
ALTER TABLE dbo.Modules DROP CONSTRAINT DF_Modules_RequiresSystemAdmin;
ALTER TABLE dbo.Modules DROP COLUMN RequiresSystemAdmin;
ALTER TABLE dbo.Roles DROP CONSTRAINT DF_Roles_IsSystemAdmin;
ALTER TABLE dbo.Roles DROP COLUMN IsSystemAdmin;
```

Rollback for `02c_Core_Modules_Seed.sql`: only delete the rows if nothing
else came to depend on them in the interim (a fresh Actions/RoleActions
backfill from `03`/`04` would already reference their `ModuleId`s).

```sql
DELETE FROM dbo.RoleActions WHERE ActionId IN (SELECT ActionId FROM dbo.Actions WHERE ModuleId IN (SELECT ModuleId FROM dbo.Modules WHERE Name IN ('Users', 'Roles')));
DELETE FROM dbo.Actions WHERE ModuleId IN (SELECT ModuleId FROM dbo.Modules WHERE Name IN ('Users', 'Roles'));
DELETE FROM dbo.Modules WHERE Name IN ('Users', 'Roles');
```

Checkpoint B rollback is a code revert only — `UserRoles` is untouched by
Checkpoint A or B, so the old N:M mapping keeps working immediately after a
revert. See `design.md` "Migration / Rollout" for the full rollback narrative.
