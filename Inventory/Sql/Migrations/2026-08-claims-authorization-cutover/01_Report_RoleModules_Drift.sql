-- Sql/Migrations/2026-08-claims-authorization-cutover/01_Report_RoleModules_Drift.sql
-- STRICTLY READ-ONLY. This script never INSERTs, UPDATEs, or DELETEs
-- anything -- it only SELECTs. Run it per tenant before Checkpoint B
-- (design.md D6) and have a human review both sections below. Do NOT
-- follow this with a re-run of Migrations/2026-08-user-management/
-- 02_Backfill_RoleActions.sql -- that INSERT-only script cannot distinguish
-- a genuine gap from a legitimate post-Phase-1 revocation made via the
-- already-shipped PUT api/v1/roles/{id}/actions endpoint, and re-running it
-- risks silently resurrecting a grant an admin deliberately removed.
--
-- Granularity: per verb (CanView/CanCreate/CanEdit/CanDelete), matching the
-- real backfill logic and the Actions.Code convention from
-- TenantBootstrap/03_Actions_Seed.sql ('{Module}{Verb}', spaces stripped).

------------------------------------------------------------------------------
-- SECTION A -- possible gaps.
-- Every (RoleId, Module, Verb) where RoleModules grants the verb but no
-- matching RoleActions row exists for the corresponding Action code.
-- Phase 1's own backfill run should have already caught all of these; a
-- non-empty result here after Phase 1 is itself worth investigating, not
-- silently trusted. A human reviews each row and, if it is a genuine gap,
-- grants it manually via the existing PUT api/v1/roles/{id}/actions
-- endpoint -- the same tool an admin would use normally, not a new SQL
-- insert path.
------------------------------------------------------------------------------
SELECT
    rm.RoleId,
    r.Name                                  AS RoleName,
    rm.ModuleId,
    m.Name                                  AS ModuleName,
    v.Verb,
    REPLACE(m.Name, ' ', '') + v.Verb       AS ExpectedActionCode
FROM dbo.RoleModules rm
JOIN dbo.Roles r   ON r.RoleId = rm.RoleId
JOIN dbo.Modules m ON m.ModuleId = rm.ModuleId
CROSS APPLY (VALUES
    ('View',   rm.CanView),
    ('Create', rm.CanCreate),
    ('Edit',   rm.CanEdit),
    ('Delete', rm.CanDelete)
) AS v(Verb, Granted)
WHERE v.Granted = 1
  AND NOT EXISTS (
        SELECT 1
        FROM dbo.RoleActions ra
        JOIN dbo.Actions a ON a.ActionId = ra.ActionId
        WHERE ra.RoleId = rm.RoleId
          AND a.Code = REPLACE(m.Name, ' ', '') + v.Verb
  )
ORDER BY rm.RoleId, rm.ModuleId, v.Verb;

------------------------------------------------------------------------------
-- SECTION B -- RoleActions grants with no RoleModules backing.
-- Purely informational. This is the expected shape of any legitimate
-- post-Phase-1 admin change (a new grant made via the Roles UI) or an
-- intentional revocation-adjacent state -- not a defect to fix. An operator
-- reviews it only to build confidence nothing looks obviously wrong (e.g. a
-- role that should clearly still hold UsersView is missing it). No
-- automated action follows from this section; Section B is expected to be
-- non-empty in the normal case and that is fine.
------------------------------------------------------------------------------
SELECT
    ra.RoleId,
    r.Name    AS RoleName,
    a.ActionId,
    a.Code    AS ActionCode,
    a.ModuleId,
    m.Name    AS ModuleName
FROM dbo.RoleActions ra
JOIN dbo.Roles r    ON r.RoleId = ra.RoleId
JOIN dbo.Actions a  ON a.ActionId = ra.ActionId
JOIN dbo.Modules m  ON m.ModuleId = a.ModuleId
WHERE NOT EXISTS (
        SELECT 1
        FROM dbo.RoleModules rm
        WHERE rm.RoleId = ra.RoleId
          AND rm.ModuleId = a.ModuleId
          AND (
                (a.Code = REPLACE(m.Name, ' ', '') + 'View'   AND rm.CanView   = 1) OR
                (a.Code = REPLACE(m.Name, ' ', '') + 'Create' AND rm.CanCreate = 1) OR
                (a.Code = REPLACE(m.Name, ' ', '') + 'Edit'   AND rm.CanEdit   = 1) OR
                (a.Code = REPLACE(m.Name, ' ', '') + 'Delete' AND rm.CanDelete = 1)
          )
  )
ORDER BY ra.RoleId, a.ModuleId, a.Code;
