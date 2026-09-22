using System.ComponentModel.DataAnnotations;

namespace StockPilot.Models
{
    public class Sale
    {
        public int SaleId { get; set; }

        [Required]
        public string CustomerName { get; set; } = "";

        public DateTime SaleDate { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public string? UserName { get; set; }

        public bool DeletedByUser { get; set; } = false;

        public ICollection<SaleItem> Items { get; set; }
            = new List<SaleItem>();
    }
}