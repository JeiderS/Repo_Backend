-- Sql/TenantBootstrap/02d_Administration_Parent_Seed.sql
-- Idempotent. Adds the 'Administration' container module and reparents
-- 'Users', 'Roles', and 'Modules' underneath it, so the sidebar renders
-- all three as second-level modules instead of top-level. Must run AFTER
-- 02c_Core_Modules_Seed.sql (which guarantees 'Users'/'Roles' exist) and
-- after 'Modules' itself exists in the catalog. Reparenting does not touch
-- Action codes, which are derived from Module Name, not ParentId, so
-- re-running 03_Actions_Seed.sql is not required.
--
-- RequiresSystemAdmin stays 0 on 'Administration': it's a pure container
-- with no Actions of its own, so ModuleMenuService only shows it when at
-- least one child is visible (BuildVisibleLevel, "no ocultar contenedores").
-- Whoever has UsersView/Create/Edit/Delete granted sees 'Administration'
-- for free, with no per-role special case.
--
-- SortOrder = 0 on 'Administration' so it sorts before every other
-- top-level module. Under it, 'Users' = 0, 'Roles' = 1, 'Modules' = 2.
--
-- Safe to re-run: each reparent only fires while that module is still a
-- root module, so a tenant that has since moved it elsewhere via the
-- Module Management UI is left untouched.

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Name = 'Administration')
BEGIN
    INSERT INTO dbo.Modules (Name, Icon, Route, RequiresSystemAdmin, IsActive, SortOrder)
    VALUES ('Administration', 'fas fa-user-shield', NULL, 0, 1, 0);
END
ELSE
BEGIN
    UPDATE dbo.Modules SET Icon = 'fas fa-user-shield' WHERE Name = 'Administration' AND Icon IS NULL;
END

UPDATE u
SET u.ParentId = a.ModuleId,
    u.SortOrder = 0
FROM dbo.Modules u
INNER JOIN dbo.Modules a ON a.Name = 'Administration'
WHERE u.Name = 'Users' AND u.ParentId IS NULL;

UPDATE u
SET u.ParentId = a.ModuleId,
    u.SortOrder = 1
FROM dbo.Modules u
INNER JOIN dbo.Modules a ON a.Name = 'Administration'
WHERE u.Name = 'Roles' AND u.ParentId IS NULL;

UPDATE u
SET u.ParentId = a.ModuleId,
    u.SortOrder = 2
FROM dbo.Modules u
INNER JOIN dbo.Modules a ON a.Name = 'Administration'
WHERE u.Name = 'Modules' AND u.ParentId IS NULL;

COMMIT;
