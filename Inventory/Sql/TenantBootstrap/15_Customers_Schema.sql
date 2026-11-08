-- Sql/TenantBootstrap/15_Customers_Schema.sql
-- Idempotent. Creates the Customers table if it does not already exist.
-- Safe to re-run on any tenant database, new or existing.
--
-- Customers: who a sale can optionally be linked to. No name-unique
--            constraint (unlike Categories) -- customers can share a name.

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Customers' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Customers (
        CustomerId INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Customers PRIMARY KEY,
        Name       NVARCHAR(150) NOT NULL,
        Phone      NVARCHAR(30)  NOT NULL,
        Email      NVARCHAR(150) NULL,
        Address    NVARCHAR(250) NULL,
        IsActive   BIT       NOT NULL CONSTRAINT DF_Customers_IsActive DEFAULT(1),
        CreatedAt  DATETIME2 NOT NULL CONSTRAINT DF_Customers_CreatedAt DEFAULT(SYSUTCDATETIME()),
        UpdatedAt  DATETIME2 NULL
    );
END

COMMIT;
