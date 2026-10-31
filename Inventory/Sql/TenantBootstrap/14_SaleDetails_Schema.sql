-- Sql/TenantBootstrap/14_SaleDetails_Schema.sql
-- Idempotent. Creates the SaleDetails table if it does not already exist.
-- Safe to re-run on any tenant database, new or existing.
--
-- SaleDetails: one row per product line within a Sale. Rows are never
--              updated or deleted (a sale is either fully registered or not
--              registered at all -- see RegisterSaleCommandHandler).

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SaleDetails' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.SaleDetails (
        SaleDetailId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_SaleDetails PRIMARY KEY,
        SaleId       INT           NOT NULL,
        ProductId    INT           NOT NULL,
        Quantity     DECIMAL(18,3) NOT NULL,
        UnitPrice    DECIMAL(18,2) NOT NULL,
        Subtotal     DECIMAL(18,2) NOT NULL,
        CONSTRAINT FK_SaleDetails_Sales FOREIGN KEY (SaleId) REFERENCES dbo.Sales (SaleId),
        CONSTRAINT FK_SaleDetails_Products FOREIGN KEY (ProductId) REFERENCES dbo.Products (ProductId)
    );
END

COMMIT;
