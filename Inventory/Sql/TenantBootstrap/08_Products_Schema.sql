-- Sql/TenantBootstrap/08_Products_Schema.sql
-- Idempotent. Creates the Products table if it does not already exist.
-- Safe to re-run on any tenant database, new or existing.
--
-- Products: catalog items, each grouped under a Category. ImageUrl holds a
--           Cloudinary URL uploaded by the frontend — the backend never
--           handles file upload.

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Products' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Products (
        ProductId   INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Products PRIMARY KEY,
        ImageUrl    NVARCHAR(MAX) NULL,
        Name        NVARCHAR(150) NOT NULL,
        Description NVARCHAR(500) NULL,
        Price       DECIMAL(18,2) NOT NULL,
        CategoryId  INT       NOT NULL,
        IsActive    BIT       NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT(1),
        CreatedAt   DATETIME2 NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT(SYSUTCDATETIME()),
        UpdatedAt   DATETIME2 NULL,
        CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES dbo.Categories (CategoryId)
    );
END

COMMIT;
