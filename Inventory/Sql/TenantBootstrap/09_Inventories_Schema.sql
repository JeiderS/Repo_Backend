-- Sql/TenantBootstrap/09_Inventories_Schema.sql
-- Idempotent. Creates the Inventories table if it does not already exist.
-- Safe to re-run on any tenant database, new or existing.
--
-- Inventories: current stock quantity per Product. One row per Product.

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Inventories' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Inventories (
        InventoryId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Inventories PRIMARY KEY,
        ProductId   INT           NOT NULL,
        Quantity    DECIMAL(18,3) NOT NULL,
        UpdatedAt   DATETIME2     NOT NULL CONSTRAINT DF_Inventories_UpdatedAt DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_Inventories_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products (ProductId),
        CONSTRAINT UQ_Inventories_ProductId UNIQUE (ProductId)
    );
END

COMMIT;
