using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockPilot.Data;
using StockPilot.Models;

namespace StockPilot.Controllers
{
    [Authorize(Roles = "User")]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p =>
                    p.IsActive &&
                    p.StockQuantity > 0)
                .ToListAsync();

            var cartItems =
                await _context.CartItems
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var cartQuantities =
                cartItems.ToDictionary(
                    x => x.ProductId,
                    x => x.Quantity);

            ViewBag.CartQuantities =
                cartQuantities;

            return View("Index", products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(
            int productId,
            int quantity)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var product =
                await _context.Products
                .FirstOrDefaultAsync(p =>
                    p.ProductId == productId &&
                    p.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            if (quantity < 1)
            {
                quantity = 1;
            }

            var cartItem =
                await _context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.UserId == userId &&
                    x.ProductId == productId);

            int alreadyInCart = 0;

            if (cartItem != null)
            {
                alreadyInCart =
                    cartItem.Quantity;
            }

            int availableStock =
                product.StockQuantity -
                alreadyInCart;

            if (availableStock <= 0)
            {
                TempData["Message"] =
                    "No more stock available.";

                return RedirectToAction("Index");
            }

            if (quantity > availableStock)
            {
                quantity = availableStock;
            }

            if (cartItem == null)
            {
                cartItem = new CartItem();

                cartItem.UserId = userId!;

                cartItem.ProductId =
                    productId;

                cartItem.Quantity =
                    quantity;

                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity =
                    cartItem.Quantity + quantity;
            }

            await _context.SaveChangesAsync();

            TempData["Message"] =
                product.Name +
                " added to cart.";

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Cart()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var cartItems =
                await _context.CartItems
                .Include(x => x.Product)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return View("Cart", cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Increase(int id)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var cartItem =
                await _context.CartItems
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x =>
                    x.CartItemId == id &&
                    x.UserId == userId);

            if (cartItem == null)
            {
                return NotFound();
            }

            if (cartItem.Product == null)
            {
                return NotFound();
            }

            if (cartItem.Quantity <
                cartItem.Product.StockQuantity)
            {
                cartItem.Quantity++;

                await _context.SaveChangesAsync();
            }
            else
            {
                TempData["Message"] =
                    "You cannot add more than available stock.";
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decrease(int id)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var cartItem =
                await _context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.CartItemId == id &&
                    x.UserId == userId);

            if (cartItem == null)
            {
                return NotFound();
            }

            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
            }
            else
            {
                _context.CartItems.Remove(cartItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var cartItem =
                await _context.CartItems
                .FirstOrDefaultAsync(x =>
                    x.CartItemId == id &&
                    x.UserId == userId);

            if (cartItem == null)
            {
                return NotFound();
            }

            _context.CartItems.Remove(cartItem);

            await _context.SaveChangesAsync();

            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var cartItems =
                await _context.CartItems
                .Where(x => x.UserId == userId)
                .ToListAsync();

            _context.CartItems.RemoveRange(
                cartItems);

            await _context.SaveChangesAsync();

            return RedirectToAction("Cart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(
            string deliveryAddress)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var user =
                await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId);

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            if (string.IsNullOrWhiteSpace(
                deliveryAddress))
            {
                TempData["Message"] =
                    "Please enter your delivery location.";

                return RedirectToAction("Cart");
            }

            var cartItems =
                await _context.CartItems
                .Include(x => x.Product)
                .Where(x => x.UserId == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
            {
                TempData["Message"] =
                    "Your cart is empty.";

                return RedirectToAction("Cart");
            }

            foreach (var item in cartItems)
            {
                if (item.Product == null)
                {
                    return RedirectToAction("Cart");
                }

                if (item.Quantity >
                    item.Product.StockQuantity)
                {
                    TempData["Message"] =
                        item.Product.Name +
                        " does not have enough stock.";

                    return RedirectToAction("Cart");
                }
            }

            decimal total = 0;

            foreach (var item in cartItems)
            {
                total +=
                    item.Quantity *
                    item.Product!.SellingPrice;
            }

            var sale = new Sale();

            sale.CustomerName =
                user.Name;

            sale.SaleDate =
                DateTime.Now;

            sale.TotalAmount =
                total;

            sale.UserName =
                user.Email;

            sale.DeliveryAddress =
                deliveryAddress;

            sale.Status =
                "Pending";

            _context.Sales.Add(sale);

            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var saleItem =
                    new SaleItem();

                saleItem.SaleId =
                    sale.SaleId;

                saleItem.ProductId =
                    item.ProductId;

                saleItem.Quantity =
                    item.Quantity;

                saleItem.PriceAtSale =
                    item.Product!.SellingPrice;

                _context.SaleItems.Add(
                    saleItem);

                item.Product.StockQuantity =
                    item.Product.StockQuantity -
                    item.Quantity;
            }

            _context.CartItems.RemoveRange(
                cartItems);

            await _context.SaveChangesAsync();

            return RedirectToAction(
                "OrderSuccess");
        }

        public async Task<IActionResult> OrderSuccess()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var user =
                await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId);

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var order =
                await _context.Sales
                .Where(x =>
                    x.UserName == user.Email &&
                    !x.DeletedByUser)
                .OrderByDescending(
                    x => x.SaleDate)
                .FirstOrDefaultAsync();

            return View(order);
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var user =
                await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId);

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var orders =
                await _context.Sales
                .Where(x =>
                    x.UserName == user.Email &&
                    !x.DeletedByUser)
                .OrderByDescending(
                    x => x.SaleDate)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(
            int id)
        {
            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            var user =
                await _context.Users
                .FirstOrDefaultAsync(x =>
                    x.Id == userId);

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var order =
                await _context.Sales
                .Include(x => x.Items)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x =>
                    x.SaleId == id &&
                    x.UserName == user.Email &&
                    !x.DeletedByUser);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}