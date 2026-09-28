namespace StockPilot.ViewModels
{
public class DashboardViewModel
{
public List<ProductDashboardViewModel> Products { get; set; }
= new List<ProductDashboardViewModel>();
}


public class ProductDashboardViewModel
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = "";

    public string CategoryName { get; set; } = "";

    public int Quantity { get; set; }

    public decimal SellingPrice { get; set; }

    public string ImagePath { get; set; } = "";
}


}
