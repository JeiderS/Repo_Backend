-- Sql/TenantBootstrap/04_Admin_Role.sql
-- Idempotent. Ensures a single seeded 'Admin' role exists, holds every
-- Action currently in the catalog, and is flagged IsSystemAdmin. Safe to
-- re-run on any tenant database, new or existing -- re-running after
-- 03_Actions_Seed.sql adds new modules extends Admin's grants to cover
-- them.
--
-- Must run AFTER 02b_Claims_Columns.sql (needs Roles.IsSystemAdmin to
-- exist) and 03_Actions_Seed.sql (grants every Action that exists at the
-- time it runs), and BEFORE 05_Admin_User_Assign.sql on a new tenant. On an
-- existing tenant, 05 is deferred until after the Migrations/ scripts -- see
-- Sql/README.md for the full invocation order.

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM dbo.Roles WHERE Name = 'Admin')
BEGIN
    INSERT INTO dbo.Roles (Name) VALUES ('Admin');
END

UPDATE dbo.Roles SET IsSystemAdmin = 1 WHERE Name = 'Admin';

DECLARE @AdminRoleId INT = (SELECT RoleId FROM dbo.Roles WHERE Name = 'Admin');

INSERT INTO dbo.RoleActions (RoleId, ActionId)
SELECT @AdminRoleId, a.ActionId
FROM dbo.Actions a
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.RoleActions ra
    WHERE ra.RoleId = @AdminRoleId AND ra.ActionId = a.ActionId
);

COMMIT;
