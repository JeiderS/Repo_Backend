-- Sql/TenantBootstrap/16_Sales_AddCustomerId.sql
-- Idempotent. Adds the optional CustomerId column (+ FK) to the existing
-- Sales table created by 13_Sales_Schema.sql.
-- Safe to re-run on any tenant database, new or existing.
--
-- Must run after 15_Customers_Schema.sql: the FK target table must exist.

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.Sales') AND name = 'CustomerId'
)
BEGIN
    ALTER TABLE dbo.Sales ADD CustomerId INT NULL;
    ALTER TABLE dbo.Sales ADD CONSTRAINT FK_Sales_Customers FOREIGN KEY (CustomerId) REFERENCES dbo.Customers (CustomerId);
END

COMMIT;
