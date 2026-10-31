-- Sql/TenantBootstrap/11_Inventory_Module_Actions_Seed.sql
-- Idempotent. Seeds the 'Inventario' Modules catalog row and its three
-- Actions (InventarioView, InventarioRegisterIn, InventarioRegisterOut), then
-- links them in ModuleActions. Safe to re-run on any tenant database, new
-- or existing.
--
-- Module name is in Spanish ('Inventario'), matching this tenant's existing
-- modules (Productos, Categorías) -- Action codes keep the English verb
-- suffix, same convention as ProductosView/CategoríasEdit. Placed as a
-- sibling of 'Productos' (same ParentId, next SortOrder) instead of a
-- hardcoded ParentId, since the real menu hierarchy is tenant-specific.
--
-- Unlike most modules, Inventario's operations are not the generic
-- View/Create/Edit/Delete verbs that 03_Actions_Seed.sql derives from
-- Modules x verbs -- they are domain-specific (register a stock entry /
-- exit), so this script seeds those exact Action codes explicitly instead
-- of relying on 03's generic derivation. If 03_Actions_Seed.sql also runs
-- against this Module afterwards, it will additionally create
-- InventarioCreate/InventarioEdit/InventarioDelete rows (its generic
-- derivation is Modules x verbs with no exclusion list) -- harmless, since
-- no controller requires them, but expected.
--
-- Column shapes (verified against the entity configs / existing seeds):
--   Modules       (ModuleId PK identity, Name, Icon NULL, Route, ParentId NULL,
--                   IsActive, SortOrder, RequiresSystemAdmin) -- see
--                   ModuleConfiguration.cs and 02c_Core_Modules_Seed.sql.
--   Actions       (ActionId PK identity, ModuleId, Code, Name, IsActive) --
--                   see 01_Actions_Schema.sql / 03_Actions_Seed.sql.
--   ModuleActions (ModuleId, ActionId) composite PK, no other columns --
--                   see ModuleActionConfiguration.cs.

SET XACT_ABORT ON;
BEGIN TRAN;

IF NOT EXISTS (SELECT 1 FROM dbo.Modules WHERE Name = 'Inventario')
BEGIN
    DECLARE @ParentId INT = (SELECT ParentId FROM dbo.Modules WHERE Name = 'Productos');
    DECLARE @SortOrder INT = (
        SELECT ISNULL(MAX(SortOrder), 0) + 1
        FROM dbo.Modules
        WHERE (ParentId = @ParentId) OR (@ParentId IS NULL AND ParentId IS NULL)
    );

    INSERT INTO dbo.Modules (Name, Route, ParentId, RequiresSystemAdmin, IsActive, SortOrder)
    VALUES ('Inventario', 'inventory', @ParentId, 0, 1, @SortOrder);
END

INSERT INTO dbo.Actions (ModuleId, Code, Name, IsActive)
SELECT m.ModuleId, v.Code, v.Name, 1
FROM dbo.Modules m
CROSS JOIN (VALUES
    ('InventarioView', N'Inventario - Ver'),
    ('InventarioRegisterIn', N'Inventario - Registrar entrada'),
    ('InventarioRegisterOut', N'Inventario - Registrar salida')
) AS v(Code, Name)
WHERE m.Name = 'Inventario'
  AND NOT EXISTS (SELECT 1 FROM dbo.Actions a WHERE a.Code = v.Code);

INSERT INTO dbo.ModuleActions (ModuleId, ActionId)
SELECT m.ModuleId, a.ActionId
FROM dbo.Modules m
INNER JOIN dbo.Actions a ON a.ModuleId = m.ModuleId
WHERE m.Name = 'Inventario'
  AND a.Code IN ('InventarioView', 'InventarioRegisterIn', 'InventarioRegisterOut')
  AND NOT EXISTS (
      SELECT 1 FROM dbo.ModuleActions ma
      WHERE ma.ModuleId = m.ModuleId AND ma.ActionId = a.ActionId
  );

COMMIT;
