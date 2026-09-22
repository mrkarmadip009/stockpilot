using Microsoft.AspNetCore.Mvc;
using StockPilot.Data;
using StockPilot.ViewModels;

namespace StockPilot.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel();

            model.TotalProducts = _context.Products.Count();

            model.TotalCategories = _context.Categories.Count();

            model.TotalStock = _context.Products
                .Sum(p => p.StockQuantity);

            model.LowStockProducts = _context.Products
                .Count(p => p.StockQuantity < 10);

            // Get products first, then calculate decimal value in C#
            var products = _context.Products.ToList();

            model.TotalInventoryValue = products
                .Sum(p => p.StockQuantity * p.BuyingPrice);

            return View(model);
        }
    }
}