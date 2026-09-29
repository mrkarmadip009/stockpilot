using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockPilot.Models
{
public class Product
{
public int ProductId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = "";

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public int StockQuantity { get; set; }

    [Range(0, 999999999)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal BuyingPrice { get; set; }

    [Range(0, 999999999)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SellingPrice { get; set; }

    public string Description { get; set; } = "";

    public string ImagePath { get; set; } = "";

    public bool IsActive { get; set; } = true;

    public DateTime AddedOn { get; set; } = DateTime.Now;

    public ICollection<SaleItem> SaleItems { get; set; }
        = new List<SaleItem>();
}

}
