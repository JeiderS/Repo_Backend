-- Sql/TenantBootstrap/12_Inventories_Backfill.sql
-- Idempotent. Creates a zero-quantity Inventories row for every Product that
-- does not already have one. Covers products created before the Inventory
-- module existed (CreateProductCommandHandler now creates this row itself
-- for every NEW product, in the same transaction -- see
-- CreateProductCommandHandler.cs). Safe to re-run on any tenant database,
-- new or existing.

SET XACT_ABORT ON;
BEGIN TRAN;

INSERT INTO dbo.Inventories (ProductId, Quantity, UpdatedAt)
SELECT p.ProductId, 0, SYSUTCDATETIME()
FROM dbo.Products p
WHERE NOT EXISTS (SELECT 1 FROM dbo.Inventories i WHERE i.ProductId = p.ProductId);

COMMIT;
