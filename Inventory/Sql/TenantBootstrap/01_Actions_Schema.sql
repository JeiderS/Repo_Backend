-- Sql/TenantBootstrap/01_Actions_Schema.sql
-- Idempotent. Creates the Actions and RoleActions tables (target-state schema
-- for the action-based permission catalog) if they do not already exist.
-- Safe to re-run on any tenant database, new or existing.
--
-- Actions: one row per grantable action, tenant-derived from Modules x verbs
--          (see 03_Actions_Seed.sql). Code is PascalCase '{Module}{Verb}',
--          e.g. 'UsersView', 'InventoryCreate'.
-- RoleActions: composite-PK join table. A row's presence means "granted";
--          there is no boolean column, so "not granted" and "not configured"
--          are the same state by design (see design.md).

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Actions' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Actions (
        ActionId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Actions PRIMARY KEY,
        ModuleId INT           NOT NULL CONSTRAINT FK_Actions_Modules REFERENCES dbo.Modules(ModuleId),
        Code     VARCHAR(120)  NOT NULL CONSTRAINT UQ_Actions_Code UNIQUE,
        Name     NVARCHAR(100) NOT NULL,
        IsActive BIT           NOT NULL CONSTRAINT DF_Actions_IsActive DEFAULT(1)
    );
END

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RoleActions' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.RoleActions (
        RoleId   INT NOT NULL CONSTRAINT FK_RoleActions_Roles   REFERENCES dbo.Roles(RoleId)     ON DELETE CASCADE,
        ActionId INT NOT NULL CONSTRAINT FK_RoleActions_Actions REFERENCES dbo.Actions(ActionId),
        CONSTRAINT PK_RoleActions PRIMARY KEY (RoleId, ActionId)
    );
END

COMMIT;
