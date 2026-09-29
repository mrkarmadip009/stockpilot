using System.ComponentModel.DataAnnotations;

namespace StockPilot.ViewModels
{
public class BillingViewModel
{
public int ProductId { get; set; }


    public int Quantity { get; set; }

    [Required]
    public string CustomerName { get; set; } = "";

    public List<BillingCartItem> CartItems { get; set; }
        = new List<BillingCartItem>();

    public decimal TotalAmount
    {
        get
        {
            return CartItems.Sum(x => x.Total);
        }
    }
}

public class BillingCartItem
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = "";

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public decimal Total
    {
        get
        {
            return Quantity * Price;
        }
    }
}


}
