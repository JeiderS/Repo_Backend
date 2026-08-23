-- Sql/Migrations/2026-08-user-management/03_Drop_UserRoles.sql
-- CHECKPOINT C ONLY. Deliberately separate from 01/02. Do NOT run this until:
--   1. Checkpoint B's code is deployed and verified working (login, sidebar
--      menu, create user, deactivate user) on EVERY tenant, and
--   2. at least one JWT lifetime (Jwt:ExpiresMinutes, default 120 minutes)
--      has elapsed since that deploy.
--
-- IRREVERSIBLE without the per-tenant DB snapshot taken before Checkpoint A.
-- Once this runs, the old N:M role mapping cannot be restored from this
-- database alone. See design.md "Migration / Rollout" and tasks.md Phase 4.

SET XACT_ABORT ON;
BEGIN TRAN;

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'UserRoles' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    DROP TABLE dbo.UserRoles;
END

COMMIT;
