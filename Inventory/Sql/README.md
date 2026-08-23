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
│   ├── 03_Actions_Seed.sql
│   ├── 04_Admin_Role.sql
│   └── 05_Admin_User_Assign.sql               <- order-sensitive, see below
└── Migrations/2026-08-user-management/        <- one-shot; a brand-new tenant DB
    │                                              skips this folder entirely
    ├── 01_Backfill_RoleId.sql
    ├── 02_Backfill_RoleActions.sql
    └── 03_Drop_UserRoles.sql                  <- Checkpoint C only, deferred, gated
```

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

Run `TenantBootstrap/` straight through, 01 → 05:

```
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\01_Actions_Schema.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\02_Users_Columns.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\03_Actions_Seed.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\04_Admin_Role.sql
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
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\03_Actions_Seed.sql
sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\04_Admin_Role.sql

sqlcmd -S <server> -d <tenantDb> -b -i "Sql\Migrations\2026-08-user-management\01_Backfill_RoleId.sql"
sqlcmd -S <server> -d <tenantDb> -b -i "Sql\Migrations\2026-08-user-management\02_Backfill_RoleActions.sql"

sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\05_Admin_User_Assign.sql -v AdminEmail="admin@<tenant>"
```

Before running `01_Backfill_RoleId.sql`'s `UPDATE`, run its report `SELECT`
first (it is the first statement in the file — run it standalone, or read the
output before the script's own `UPDATE` commits) and keep the output as an
audit record: it lists every multi-role user, the role the backfill will keep,
and the roles it will drop.

## Idempotency check before fan-out

`TenantBootstrap/*` is designed to be safe to re-run. Before fanning any script
out across every tenant, re-run the full `TenantBootstrap/01-05` sequence on one
already-bootstrapped test database and confirm:
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

Checkpoint B rollback is a code revert only — `UserRoles` is untouched by
Checkpoint A or B, so the old N:M mapping keeps working immediately after a
revert. See `design.md` "Migration / Rollout" for the full rollback narrative.
