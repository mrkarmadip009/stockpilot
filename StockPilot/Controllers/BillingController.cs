using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockPilot.Data;
using StockPilot.Models;
using StockPilot.ViewModels;
using System.Text.Json;

namespace StockPilot.Controllers
{
[Authorize(Roles = "Admin")]
public class BillingController : Controller
{
private readonly ApplicationDbContext _context;


    public BillingController(ApplicationDbContext context)
    {
        _context = context;
    }


    // ==============================
    // BILLING PAGE
    // ==============================

    public async Task<IActionResult> Index()
    {
        var products = await _context.Products
            .Where(p => p.IsActive && p.StockQuantity > 0)
            .OrderBy(p => p.Name)
            .ToListAsync();

        ViewBag.Products = products;

        var cart = GetCart();

        var model = new BillingViewModel
        {
            CartItems = cart
        };

        return View(model);
    }


    // ==============================
    // ADD PRODUCT TO CART
    // ==============================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(
        int productId,
        int quantity)
    {
        if (quantity <= 0)
        {
            TempData["BillingError"] =
                "Please enter a valid quantity.";

            return RedirectToAction(nameof(Index));
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(p =>
                p.ProductId == productId &&
                p.IsActive);

        if (product == null)
        {
            TempData["BillingError"] =
                "Product not found.";

            return RedirectToAction(nameof(Index));
        }

        if (product.StockQuantity < quantity)
        {
            TempData["BillingError"] =
                "Not enough stock available.";

            return RedirectToAction(nameof(Index));
        }

        var cart = GetCart();

        var existingItem = cart.FirstOrDefault(
            x => x.ProductId == productId);


        if (existingItem != null)
        {
            int newQuantity =
                existingItem.Quantity + quantity;

            if (newQuantity > product.StockQuantity)
            {
                TempData["BillingError"] =
                    "Requested quantity is greater than available stock.";

                return RedirectToAction(nameof(Index));
            }

            existingItem.Quantity = newQuantity;
        }
        else
        {
            cart.Add(new BillingCartItem
            {
                ProductId = product.ProductId,
                ProductName = product.Name,
                Quantity = quantity,
                Price = product.SellingPrice
            });
        }

        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }


    // ==============================
    // REMOVE FROM CART
    // ==============================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveFromCart(int productId)
    {
        var cart = GetCart();

        var item = cart.FirstOrDefault(
            x => x.ProductId == productId);

        if (item != null)
        {
            cart.Remove(item);
        }

        SaveCart(cart);

        return RedirectToAction(nameof(Index));
    }


    // ==============================
    // COMPLETE SALE
    // ==============================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteSale(
        string customerName)
    {
        var cart = GetCart();

        if (cart.Count == 0)
        {
            TempData["BillingError"] =
                "Cart is empty.";

            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrWhiteSpace(customerName))
        {
            TempData["BillingError"] =
                "Please enter customer name.";

            return RedirectToAction(nameof(Index));
        }


        using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            decimal totalAmount = 0;

            var sale = new Sale
            {
                CustomerName = customerName,
                SaleDate = DateTime.Now,
                UserName = User.Identity?.Name,
                TotalAmount = 0
            };

            _context.Sales.Add(sale);

            await _context.SaveChangesAsync();


            foreach (var cartItem in cart)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(
                        p => p.ProductId == cartItem.ProductId);

                if (product == null)
                {
                    throw new Exception(
                        "Product not found.");
                }

                if (product.StockQuantity <
                    cartItem.Quantity)
                {
                    throw new Exception(
                        "Not enough stock for "
                        + product.Name);
                }


                var saleItem = new SaleItem
                {
                    SaleId = sale.SaleId,
                    ProductId = product.ProductId,
                    Quantity = cartItem.Quantity,
                    PriceAtSale = product.SellingPrice
                };

                _context.SaleItems.Add(saleItem);


                product.StockQuantity -=
                    cartItem.Quantity;


                totalAmount +=
                    cartItem.Quantity *
                    product.SellingPrice;
            }


            sale.TotalAmount = totalAmount;

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            HttpContext.Session.Remove("BillingCart");

            TempData["BillingSuccess"] =
                "Sale completed successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            TempData["BillingError"] =
                ex.Message;

            return RedirectToAction(nameof(Index));
        }
    }


    // ==============================
    // GET CART FROM SESSION
    // ==============================

    private List<BillingCartItem> GetCart()
    {
        var cartJson =
            HttpContext.Session.GetString("BillingCart");

        if (string.IsNullOrEmpty(cartJson))
        {
            return new List<BillingCartItem>();
        }

        return JsonSerializer.Deserialize<
            List<BillingCartItem>>(cartJson)
            ?? new List<BillingCartItem>();
    }


    // ==============================
    // SAVE CART TO SESSION
    // ==============================

    private void SaveCart(
        List<BillingCartItem> cart)
    {
        var cartJson =
            JsonSerializer.Serialize(cart);

        HttpContext.Session.SetString(
            "BillingCart",
            cartJson);
    }
}


}
