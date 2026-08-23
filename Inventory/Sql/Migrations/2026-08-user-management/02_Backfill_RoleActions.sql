-- Sql/Migrations/2026-08-user-management/02_Backfill_RoleActions.sql
-- ONE-SHOT. Skipped entirely on a brand-new tenant DB. Run only on an
-- EXISTING tenant DB, after 01_Backfill_RoleId.sql and before
-- TenantBootstrap/05_Admin_User_Assign.sql -- see Sql/README.md.
--
-- Expands every RoleModules grant (CanView/CanCreate/CanEdit/CanDelete = 1)
-- into the matching RoleActions row, using the same '{Module}{Verb}' PascalCase
-- Code convention as TenantBootstrap/03_Actions_Seed.sql.
--
-- WHERE NOT EXISTS guards against the composite RoleActions PK
-- (RoleId, ActionId): TenantBootstrap/04_Admin_Role.sql already grants Admin
-- every Action, and RoleModules is Admin's live working authorization today,
-- so without this guard backfilling Admin's own RoleModules grants would
-- insert a duplicate PK and abort the whole XACT_ABORT transaction.

SET XACT_ABORT ON;
BEGIN TRAN;

INSERT INTO dbo.RoleActions (RoleId, ActionId)
SELECT DISTINCT rm.RoleId, a.ActionId
FROM dbo.RoleModules rm
INNER JOIN dbo.Modules m ON m.ModuleId = rm.ModuleId
CROSS APPLY (
    SELECT REPLACE(m.Name, ' ', '') + 'View'   AS Code WHERE rm.CanView   = 1
    UNION ALL
    SELECT REPLACE(m.Name, ' ', '') + 'Create' AS Code WHERE rm.CanCreate = 1
    UNION ALL
    SELECT REPLACE(m.Name, ' ', '') + 'Edit'   AS Code WHERE rm.CanEdit   = 1
    UNION ALL
    SELECT REPLACE(m.Name, ' ', '') + 'Delete' AS Code WHERE rm.CanDelete = 1
) codes
INNER JOIN dbo.Actions a ON a.Code = codes.Code
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.RoleActions ra
    WHERE ra.RoleId = rm.RoleId AND ra.ActionId = a.ActionId
);

COMMIT;
