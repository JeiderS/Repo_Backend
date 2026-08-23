-- Sql/TenantBootstrap/02c_Core_Modules_Seed.sql
-- Idempotent. Seeds the 'Users' and 'Roles' Modules catalog rows.
--
-- CRITICAL, discovered during Phase 4 (claims-authorization-cutover) test
-- execution against real tenant databases: no TenantBootstrap or Migrations
-- script, in this change or the prior 'user-management' change, ever seeded
-- 'Users' or 'Roles' as Modules catalog rows. That was invisible before this
-- change because UsersController/RolesController have always gated on the
-- literal role name [Authorize(Roles = "Admin")], entirely independent of
-- the Modules/Actions/RoleActions catalog. Checkpoint B's per-action re-gate
-- (design.md D4: [Authorize(Roles = "UsersView")], "RolesEdit", etc.) makes
-- these catalog rows load-bearing for the FIRST time: 03_Actions_Seed.sql
-- can only derive 'UsersView'/'UsersCreate'/'RolesEdit'/etc. Action codes
-- from a Modules row named 'Users'/'Roles'. Without this script, Checkpoint
-- B locks EVERY role -- including System Admin -- out of user and role
-- management, because the Action codes those controllers require can never
-- exist in dbo.Actions, so RoleActions can never grant them to anyone.
--
-- Must run BEFORE 03_Actions_Seed.sql on every tenant, new or existing, that
-- is going to receive the Checkpoint B code deploy. Safe to re-run.
--
-- Upsert-safe like 02b_Claims_Columns.sql's 'Modules' row: if a tenant
-- already has an organically-created 'Users' or 'Roles' row (e.g. created
-- manually via the Module Management UI) its own Route/ParentId/SortOrder
-- placement is left untouched -- this only guarantees the row exists at all.
-- RequiresSystemAdmin is left at its default 0: ordinary admin-managed
-- modules, not gated behind Roles.IsSystemAdmin.

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Name = 'Users')
BEGIN
    INSERT INTO dbo.Modules (Name, Route, RequiresSystemAdmin, IsActive, SortOrder)
    VALUES ('Users', 'users', 0, 1, 1);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Name = 'Roles')
BEGIN
    INSERT INTO dbo.Modules (Name, Route, RequiresSystemAdmin, IsActive, SortOrder)
    VALUES ('Roles', 'roles', 0, 1, 2);
END

COMMIT;
