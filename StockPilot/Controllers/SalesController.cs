using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockPilot.Data;

namespace StockPilot.Controllers
{
[Authorize(Roles = "Admin")]
public class SalesController : Controller
{
private readonly ApplicationDbContext _context;


    public SalesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var sales = await _context.Sales
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();

        return View(sales);
    }

    public async Task<IActionResult> Details(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.SaleId == id);

        if (sale == null)
        {
            return NotFound();
        }

        return View(sale);
    }
}


}
