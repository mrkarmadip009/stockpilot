namespace StockPilot.Models
{
    public class SaleItem
    {
        public int SaleItemId { get; set; }

        public int SaleId { get; set; }

        public Sale? Sale { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int Quantity { get; set; }

        public decimal PriceAtSale { get; set; }

        public decimal Total
        {
            get
            {
                return Quantity * PriceAtSale;
            }
        }
    }
}