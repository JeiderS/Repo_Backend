-- Sql/TenantBootstrap/10_InventoryMovements_Schema.sql
-- Idempotent. Creates the InventoryMovements table if it does not already exist.
-- Safe to re-run on any tenant database, new or existing.
--
-- InventoryMovements: append-only ledger of stock entries (IN) and exits (OUT)
--                     for an Inventory record. Rows are never updated or deleted.

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'InventoryMovements' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.InventoryMovements (
        InventoryMovementId BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_InventoryMovements PRIMARY KEY,
        InventoryId          INT           NOT NULL,
        MovementType         NVARCHAR(3)   NOT NULL,
        Quantity             DECIMAL(18,3) NOT NULL,
        Description          NVARCHAR(250) NULL,
        CreatedAt            DATETIME2     NOT NULL CONSTRAINT DF_InventoryMovements_CreatedAt DEFAULT(SYSUTCDATETIME()),
        CONSTRAINT FK_InventoryMovements_Inventories FOREIGN KEY (InventoryId) REFERENCES dbo.Inventories (InventoryId)
    );
END

COMMIT;
