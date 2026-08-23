-- Sql/TenantBootstrap/02b_Claims_Columns.sql
-- Idempotent. Adds the two columns the claims-authorization cutover reads at
-- runtime (Roles.IsSystemAdmin, Modules.RequiresSystemAdmin) and seeds the
-- system-admin-only 'Modules' catalog module. Purely additive -- nothing here
-- changes any currently-live behavior; the live authorization path still
-- reads RoleModules until Checkpoint B ships. Safe to re-run on any tenant
-- database, new or existing.
--
-- Numbered 02b (not a renumber): runs between 02_Users_Columns.sql and
-- 03_Actions_Seed.sql. See Sql/README.md for the full invocation order.

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Roles') AND name = 'IsSystemAdmin')
BEGIN
    ALTER TABLE dbo.Roles ADD IsSystemAdmin BIT NOT NULL CONSTRAINT DF_Roles_IsSystemAdmin DEFAULT(0);
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Modules') AND name = 'RequiresSystemAdmin')
BEGIN
    ALTER TABLE dbo.Modules ADD RequiresSystemAdmin BIT NOT NULL CONSTRAINT DF_Modules_RequiresSystemAdmin DEFAULT(0);
END
GO
-- GO forces a new batch: SQL Server compiles/binds an ad-hoc batch up front,
-- so a column ALTERed above is not recognized by a later statement in the
-- SAME batch ("Invalid column name"), and SET XACT_ABORT ON then rolls back
-- the whole transaction, including the ALTER, on that error. The transaction
-- opened above stays open across GO within one sqlcmd invocation/connection.

-- Seed (or flag) the 'Modules' catalog module (system-admin-only module
-- management UI). Upsert, not insert-only: a tenant may already have an
-- organically-created 'Modules' row (found in the default Inventory tenant,
-- ModuleId 15, pre-dating this change) with its own Route/ParentId/SortOrder
-- placement -- that placement is left untouched, but RequiresSystemAdmin
-- must be forced to 1 on it regardless, or the D5 menu-gating rule silently
-- does not apply to that tenant's existing row. IsActive=1 is required
-- explicitly on a fresh insert: ModuleMenuService.GetMenuForUserAsync filters
-- on m.IsActive, so an omitted/default-false value would leave a newly
-- inserted row permanently invisible in the menu regardless of any role's
-- Actions.
IF EXISTS (SELECT 1 FROM dbo.Modules WHERE Name = 'Modules')
BEGIN
    UPDATE dbo.Modules SET RequiresSystemAdmin = 1 WHERE Name = 'Modules';
END
ELSE
BEGIN
    INSERT INTO dbo.Modules (Name, Route, RequiresSystemAdmin, IsActive, SortOrder)
    VALUES ('Modules', 'modules', 1, 1, 0);
END

COMMIT;
