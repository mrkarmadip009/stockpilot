using System.ComponentModel.DataAnnotations;

namespace StockPilot.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // One category can have many products
        public ICollection<Product> Products { get; set; }
    }
}