-- Sql/TenantBootstrap/03_Actions_Seed.sql
-- Idempotent. Seeds one Actions row per (Module x verb) combination that does
-- not already exist. Derived from the tenant's own Modules table, not
-- hardcoded, so this single script is correct for every tenant regardless of
-- its module set. Safe to re-run on any tenant database, new or existing.
--
-- Code format: PascalCase '{Module}{Verb}' (spaces stripped from the module
-- name), e.g. 'UsersView', 'InventoryCreate'.

SET XACT_ABORT ON;
BEGIN TRAN;

DECLARE @Verbs TABLE (Verb VARCHAR(20));
INSERT INTO @Verbs (Verb) VALUES ('View'), ('Create'), ('Edit'), ('Delete');

INSERT INTO dbo.Actions (ModuleId, Code, Name, IsActive)
SELECT
    m.ModuleId,
    REPLACE(m.Name, ' ', '') + v.Verb,
    m.Name + N' - ' + v.Verb,
    1
FROM dbo.Modules m
CROSS JOIN @Verbs v
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.Actions a
    WHERE a.Code = REPLACE(m.Name, ' ', '') + v.Verb
);

COMMIT;
