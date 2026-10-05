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


        // =========================================
        // ALL SALES
        // =========================================

        public async Task<IActionResult> Index()
        {
            var sales = await _context.Sales
                .Where(x => !x.DeletedByUser)
                .OrderByDescending(x => x.SaleDate)
                .ToListAsync();

            return View(sales);
        }


        // =========================================
        // SALES DETAILS
        // =========================================

        public async Task<IActionResult> Details(int id)
        {
            var sale = await _context.Sales
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x =>
                    x.SaleId == id &&
                    !x.DeletedByUser);

            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }


        // =========================================
        // PENDING ORDERS
        // =========================================

        public async Task<IActionResult> PendingOrders()
        {
            var orders = await _context.Sales
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .Where(x =>
                    x.Status == "Pending" &&
                    !x.DeletedByUser)
                .OrderByDescending(x => x.SaleDate)
                .ToListAsync();

            return View(orders);
        }


        // =========================================
        // MARK ORDER AS COMPLETED
        // =========================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkCompleted(int id)
        {
            var order = await _context.Sales
                .FirstOrDefaultAsync(x =>
                    x.SaleId == id);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = "Completed";

            await _context.SaveChangesAsync();

            return RedirectToAction("PendingOrders");
        }
    }
}