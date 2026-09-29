
namespace StockPilot.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }

        public string UserId { get; set; } = "";

        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public int Quantity { get; set; }
    }
}

