-- Sql/TenantBootstrap/05_Admin_User_Assign.sql
-- NOT purely idempotent-and-order-free: it VALIDATES the final state and
-- fails the script if nobody holds the Admin role. This is the documented
-- recovery path if an admin deactivates or demotes themselves (Phase 1 has
-- no self-protection guard, per the proposal).
--
-- ORDER-SENSITIVE. On a NEW tenant DB (no legacy data), run this as step 5,
-- right after 01-04. On an EXISTING tenant DB, this MUST run LAST -- after
-- Migrations/2026-08-user-management/01_Backfill_RoleId.sql and
-- 02_Backfill_RoleActions.sql -- so its check sees the true final state and
-- cannot be silently undone by the legacy-data backfill. See Sql/README.md.
--
-- Usage:
--   sqlcmd -S <server> -d <tenantDb> -b -i Sql\TenantBootstrap\05_Admin_User_Assign.sql -v AdminEmail="admin@<tenant>"

SET XACT_ABORT ON;
BEGIN TRAN;

DECLARE @AdminEmail NVARCHAR(100) = N'$(AdminEmail)';
DECLARE @AdminRoleId INT = (SELECT RoleId FROM dbo.Roles WHERE Name = 'Admin');

IF @AdminRoleId IS NULL
BEGIN
    ROLLBACK TRAN;
    RAISERROR('Admin role does not exist. Run TenantBootstrap/04_Admin_Role.sql first.', 16, 1);
    RETURN;
END

-- If the named admin email was supplied and currently holds no role, or
-- holds a role other than Admin, assign them Admin explicitly. This is a
-- best-effort repair step; it does not override an existing correct
-- assignment for a different user who already holds Admin.
IF @AdminEmail IS NOT NULL AND LEN(@AdminEmail) > 0
   AND NOT EXISTS (SELECT 1 FROM dbo.Users WHERE RoleId = @AdminRoleId)
BEGIN
    UPDATE dbo.Users
    SET RoleId = @AdminRoleId
    WHERE Email = @AdminEmail;
END

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE RoleId = @AdminRoleId)
BEGIN
    ROLLBACK TRAN;
    RAISERROR('No user holds the Admin role after this script ran. Provide -v AdminEmail="<existing user email>" pointing at a user who should be Admin, or update dbo.Users.RoleId manually before retrying.', 16, 1);
    RETURN;
END

COMMIT;
