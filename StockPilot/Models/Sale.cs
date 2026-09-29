using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockPilot.Models
{
public class Sale
{
public int SaleId { get; set; }

    [Required]
    public string CustomerName { get; set; } = "";

    public DateTime SaleDate { get; set; } = DateTime.Now;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public string? UserName { get; set; }

    public bool DeletedByUser { get; set; } = false;

    public ICollection<SaleItem> Items { get; set; }
        = new List<SaleItem>();
}

}
