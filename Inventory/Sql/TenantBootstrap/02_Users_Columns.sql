-- Sql/TenantBootstrap/02_Users_Columns.sql
-- Idempotent. Adds the single-role assignment column (Users.RoleId) and the
-- forced-password-change flag (Users.MustChangePassword) to an existing
-- Users table. Safe to re-run on any tenant database, new or existing.
--
-- RoleId is nullable: a user may exist with no assigned role (see spec
-- "Single Role Per User" -> "User has no role after creation without one").

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Users') AND name = 'RoleId')
BEGIN
    ALTER TABLE dbo.Users ADD RoleId INT NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Users_Roles')
BEGIN
    ALTER TABLE dbo.Users ADD CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles(RoleId);
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_RoleId' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    CREATE INDEX IX_Users_RoleId ON dbo.Users(RoleId);
END

IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Users') AND name = 'MustChangePassword')
BEGIN
    ALTER TABLE dbo.Users ADD MustChangePassword BIT NOT NULL CONSTRAINT DF_Users_MustChangePassword DEFAULT(0);
END

COMMIT;
