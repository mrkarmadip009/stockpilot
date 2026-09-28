using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockPilot.Data;
using StockPilot.ViewModels;

namespace StockPilot.Controllers
{
[Authorize]
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

        model.Products = _context.Products
            .Include(p => p.Category)
            .Select(p => new ProductDashboardViewModel
            {
                ProductId = p.ProductId,
                ProductName = p.Name,

                CategoryName = p.Category != null
                    ? p.Category.Name
                    : "No Category",

                Quantity = p.StockQuantity,

                SellingPrice = p.SellingPrice,

                ImagePath = p.ImagePath
            })
            .ToList();

        return View(model);
    }
}


}
