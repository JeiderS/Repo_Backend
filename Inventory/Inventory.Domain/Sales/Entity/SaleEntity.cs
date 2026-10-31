namespace Inventory.Domain.Sales.Entity
{
    public class SaleEntity
    {
        public const string OriginManual = "MANUAL";
        public const string OriginWeb = "WEB";

        // Para EF Core.
        private SaleEntity() { }

        public int Id { get; private set; }
        public string Number { get; private set; } = default!;
        public DateTime SaleDate { get; private set; }
        public decimal Total { get; private set; }
        public string Origin { get; private set; } = default!;

        /// <summary>
        /// Regla de negocio: una venta es un registro operativo inmutable, sin
        /// Update ni Delete (igual que InventoryMovementEntity). El Number se
        /// genera acá mismo, sin ida y vuelta a la base de datos.
        /// </summary>
        public static SaleEntity Create(string origin, decimal total)
        {
            if (origin != OriginManual && origin != OriginWeb)
                throw new ArgumentException("Origin must be MANUAL or WEB.", nameof(origin));

            return new()
            {
                Number = $"VTA-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
                SaleDate = DateTime.UtcNow,
                Total = total,
                Origin = origin,
            };
        }
    }
}
