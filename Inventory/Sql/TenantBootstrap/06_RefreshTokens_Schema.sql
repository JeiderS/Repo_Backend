-- Sql/TenantBootstrap/06_RefreshTokens_Schema.sql
-- Idempotent. Creates the RefreshTokens table (target-state schema for the
-- refresh-token flow: short-lived access token + httpOnly-cookie refresh
-- token with rotation). Safe to re-run on any tenant database, new or
-- existing.
--
-- RefreshTokens: one row per issued refresh token. Never stores the raw
--          value, only TokenHash (Base64 of SHA-256, 44 chars). A row with
--          RevokedAt set is dead even if ExpiresAt hasn't passed yet
--          (logout, or superseded by rotation on refresh).

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RefreshTokens' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.RefreshTokens (
        RefreshTokenId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_RefreshTokens PRIMARY KEY,
        UserId         INT NOT NULL CONSTRAINT FK_RefreshTokens_Users REFERENCES dbo.Users(UserId),
        TokenHash      NVARCHAR(100) NOT NULL CONSTRAINT UQ_RefreshTokens_TokenHash UNIQUE,
        CreatedAt      DATETIME2 NOT NULL,
        ExpiresAt      DATETIME2 NOT NULL,
        RevokedAt      DATETIME2 NULL
    );
END

COMMIT;
