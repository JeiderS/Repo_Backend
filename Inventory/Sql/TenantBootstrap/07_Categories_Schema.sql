-- Sql/TenantBootstrap/07_Categories_Schema.sql
-- Idempotent. Creates the Categories table if it does not already exist.
-- Safe to re-run on any tenant database, new or existing.
--
-- Categories: how products are grouped in the app. No ordering/hierarchy
--             column (unlike Modules) — a flat, name-unique catalog.

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Categories' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Categories (
        CategoryId  INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Categories PRIMARY KEY,
        Name        NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500) NULL,
        IsActive    BIT       NOT NULL CONSTRAINT DF_Categories_IsActive DEFAULT(1),
        CreatedAt   DATETIME2 NOT NULL CONSTRAINT DF_Categories_CreatedAt DEFAULT(SYSUTCDATETIME()),
        UpdatedAt   DATETIME2 NULL,
        CONSTRAINT UQ_Categories_Name UNIQUE (Name)
    );
END

COMMIT;
