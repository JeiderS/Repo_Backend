namespace Inventory.Domain.SaleDetails.Entity
{
    public class SaleDetailEntity
    {
        // Para EF Core.
        private SaleDetailEntity() { }

        public int Id { get; private set; }
        public int SaleId { get; private set; }
        public int ProductId { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal Subtotal { get; private set; }

        /// <summary>Regla de negocio: una línea de venta es inmutable, sin Update ni Delete.</summary>
        public static SaleDetailEntity Create(int saleId, int productId, decimal quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            return new()
            {
                SaleId = saleId,
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Subtotal = unitPrice * quantity,
            };
        }
    }
}
