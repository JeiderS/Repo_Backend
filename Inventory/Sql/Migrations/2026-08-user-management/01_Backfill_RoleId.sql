-- Sql/Migrations/2026-08-user-management/01_Backfill_RoleId.sql
-- ONE-SHOT. Skipped entirely on a brand-new tenant DB (there is no legacy
-- UserRoles data to migrate). Run only on an EXISTING tenant DB, after
-- TenantBootstrap/01-04 and before TenantBootstrap/05 -- see Sql/README.md.
--
-- Picks, for every user, the single RoleId that becomes authoritative:
--   - Admin, if the user currently holds it (via legacy UserRoles) -- this
--     prevents the migration itself from silently demoting a multi-role
--     admin and creating an admin-less tenant.
--   - Otherwise MIN(RoleId) among the user's currently-assigned roles
--     (Roles.RoleId is IDENTITY, so this is "the oldest role defined").
--
-- Step 1: report query. Every UserId with more than one legacy role, the
-- role this script will keep, and the roles it will drop. Run this and keep
-- the output as an audit record BEFORE the UPDATE below executes.

SELECT
    ur.UserId,
    u.Email,
    (
        SELECT TOP 1 ur2.RoleId
        FROM dbo.UserRoles ur2
        INNER JOIN dbo.Roles r2 ON r2.RoleId = ur2.RoleId
        WHERE ur2.UserId = ur.UserId
        ORDER BY CASE WHEN r2.Name = 'Admin' THEN 0 ELSE 1 END, ur2.RoleId ASC
    ) AS RoleIdToKeep,
    STRING_AGG(CAST(ur.RoleId AS VARCHAR(10)), ',') AS AllLegacyRoleIds
FROM dbo.UserRoles ur
INNER JOIN dbo.Users u ON u.UserId = ur.UserId
GROUP BY ur.UserId, u.Email
HAVING COUNT(*) > 1;

-- Step 2: the backfill itself. WHERE NOT EXISTS guards against re-running
-- this script after Users.RoleId has already been populated for a user.

SET XACT_ABORT ON;
BEGIN TRAN;

UPDATE u
SET u.RoleId = (
    SELECT TOP 1 ur.RoleId
    FROM dbo.UserRoles ur
    INNER JOIN dbo.Roles r ON r.RoleId = ur.RoleId
    WHERE ur.UserId = u.UserId
    ORDER BY CASE WHEN r.Name = 'Admin' THEN 0 ELSE 1 END, ur.RoleId ASC
)
FROM dbo.Users u
WHERE u.RoleId IS NULL
  AND EXISTS (SELECT 1 FROM dbo.UserRoles ur WHERE ur.UserId = u.UserId);

COMMIT;
