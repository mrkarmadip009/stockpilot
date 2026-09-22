namespace StockPilot.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }

        public int TotalCategories { get; set; }

        public int TotalStock { get; set; }

        public int LowStockProducts { get; set; }

        public decimal TotalInventoryValue { get; set; }
    }
}