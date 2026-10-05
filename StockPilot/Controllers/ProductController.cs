
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StockPilot.Models;
using StockPilot.Repositories;

namespace StockPilot.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _environment;


        public ProductController(
            IProductRepository repository,
            ICategoryRepository categoryRepository,
            IWebHostEnvironment environment)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _environment = environment;
        }


        // =========================
        // PRODUCT LIST
        // =========================

        public async Task<IActionResult> Index()
        {
            var products = await _repository.GetAllAsync();

            return View(products);
        }


        // =========================
        // PRODUCT DETAILS
        // =========================

        public async Task<IActionResult> Details(int id)
        {
            var product = await _repository.GetByIdAsync(id);

            if (product == null)
                return NotFound();

            return View(product);
        }


        // =========================
        // CREATE PRODUCT PAGE
        // =========================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var categories =
                await _categoryRepository.GetAllAsync();

            ViewBag.Categories =
                new SelectList(
                    categories,
                    "CategoryId",
                    "Name"
                );

            return View();
        }


        // =========================
        // SEARCH EXISTING PRODUCTS
        // =========================

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchExistingProducts(
            string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>());
            }


            var products =
                await _repository.GetAllAsync();


            var result =
                products
                .Where(x =>
                    x.Name.Contains(
                        term.Trim(),
                        StringComparison.OrdinalIgnoreCase
                    )
                )
                .Take(10)
                .Select(x => new
                {
                    productId = x.ProductId,
                    name = x.Name,
                    stock = x.StockQuantity,
                    buyingPrice = x.BuyingPrice,
                    sellingPrice = x.SellingPrice
                })
                .ToList();


            return Json(result);
        }


        // =========================
        // CREATE NEW PRODUCT
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(
            Product product,
            IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                // Check duplicate product

                var allProducts =
                    await _repository.GetAllAsync();


                var existingProduct =
                    allProducts.FirstOrDefault(x =>
                        x.Name.Equals(
                            product.Name.Trim(),
                            StringComparison.OrdinalIgnoreCase
                        )
                    );


                if (existingProduct != null)
                {
                    ModelState.AddModelError(
                        "Name",
                        "This product already exists. Select it from the existing product search and add stock."
                    );


                    var existingCategories =
                        await _categoryRepository.GetAllAsync();


                    ViewBag.Categories =
                        new SelectList(
                            existingCategories,
                            "CategoryId",
                            "Name",
                            product.CategoryId
                        );


                    return View(product);
                }


                // =========================
                // IMAGE UPLOAD
                // =========================

                if (imageFile != null &&
                    imageFile.Length > 0)
                {
                    string folder =
                        Path.Combine(
                            _environment.WebRootPath,
                            "uploads",
                            "products"
                        );


                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }


                    string fileName =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(
                            imageFile.FileName
                        );


                    string filePath =
                        Path.Combine(
                            folder,
                            fileName
                        );


                    using (var stream =
                        new FileStream(
                            filePath,
                            FileMode.Create))
                    {
                        await imageFile.CopyToAsync(
                            stream
                        );
                    }


                    product.ImagePath =
                        "/uploads/products/"
                        + fileName;
                }


                await _repository.AddAsync(product);


                return RedirectToAction(
                    nameof(Index)
                );
            }


            var categories =
                await _categoryRepository.GetAllAsync();


            ViewBag.Categories =
                new SelectList(
                    categories,
                    "CategoryId",
                    "Name",
                    product.CategoryId
                );


            return View(product);
        }


        // =========================
        // ADD STOCK TO EXISTING PRODUCT
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddExistingStock(
            int productId,
            int quantity)
        {
            if (quantity <= 0)
            {
                TempData["Message"] =
                    "Please enter a valid stock quantity.";

                return RedirectToAction(
                    nameof(Create)
                );
            }


            var product =
                await _repository.GetByIdAsync(
                    productId
                );


            if (product == null)
            {
                return NotFound();
            }


            // Add only new stock

            product.StockQuantity =
                product.StockQuantity + quantity;


            await _repository.UpdateAsync(
                product
            );


            TempData["Message"] =
                quantity
                + " stock item(s) added to "
                + product.Name
                + ".";


            return RedirectToAction(
                nameof(Index)
            );
        }


        // =========================
        // EDIT PRODUCT
        // =========================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product =
                await _repository.GetByIdAsync(id);


            if (product == null)
                return NotFound();


            var categories =
                await _categoryRepository.GetAllAsync();


            ViewBag.Categories =
                new SelectList(
                    categories,
                    "CategoryId",
                    "Name",
                    product.CategoryId
                );


            return View(product);
        }


        // =========================
        // EDIT PRODUCT POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(
            Product product,
            IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                var oldProduct =
                    await _repository.GetByIdAsync(
                        product.ProductId
                    );


                if (oldProduct == null)
                    return NotFound();


                // Keep old image

                product.ImagePath =
                    oldProduct.ImagePath;


                // Upload new image if selected

                if (imageFile != null &&
                    imageFile.Length > 0)
                {
                    string folder =
                        Path.Combine(
                            _environment.WebRootPath,
                            "uploads",
                            "products"
                        );


                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }


                    string fileName =
                        Guid.NewGuid().ToString()
                        + Path.GetExtension(
                            imageFile.FileName
                        );


                    string filePath =
                        Path.Combine(
                            folder,
                            fileName
                        );


                    using (var stream =
                        new FileStream(
                            filePath,
                            FileMode.Create))
                    {
                        await imageFile.CopyToAsync(
                            stream
                        );
                    }


                    product.ImagePath =
                        "/uploads/products/"
                        + fileName;
                }


                await _repository.UpdateAsync(
                    product
                );


                return RedirectToAction(
                    nameof(Index)
                );
            }


            var categories =
                await _categoryRepository.GetAllAsync();


            ViewBag.Categories =
                new SelectList(
                    categories,
                    "CategoryId",
                    "Name",
                    product.CategoryId
                );


            return View(product);
        }


        // =========================
        // DELETE PRODUCT
        // =========================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var product =
                await _repository.GetByIdAsync(id);


            if (product == null)
                return NotFound();


            return View(product);
        }


        // =========================
        // DELETE CONFIRMED
        // =========================

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            await _repository.DeleteAsync(id);

            return RedirectToAction(
                nameof(Index)
            );
        }
    }
}