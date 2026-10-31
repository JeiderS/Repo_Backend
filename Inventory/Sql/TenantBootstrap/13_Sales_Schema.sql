-- Sql/TenantBootstrap/13_Sales_Schema.sql
-- Idempotent. Creates the Sales table if it does not already exist.
-- Safe to re-run on any tenant database, new or existing.
--
-- Sales: header row for a registered sale (one or more SaleDetails lines).

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Sales' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Sales (
        SaleId   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Sales PRIMARY KEY,
        Number   NVARCHAR(30)  NOT NULL CONSTRAINT UQ_Sales_Number UNIQUE,
        SaleDate DATETIME2     NOT NULL CONSTRAINT DF_Sales_SaleDate DEFAULT(SYSUTCDATETIME()),
        Total    DECIMAL(18,2) NOT NULL,
        Origin   NVARCHAR(10)  NOT NULL
    );
END

COMMIT;
