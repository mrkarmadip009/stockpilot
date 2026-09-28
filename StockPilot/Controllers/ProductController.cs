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

    public async Task<IActionResult> Index()
    {
        var products = await _repository.GetAllAsync();
        return View(products);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    // =========================
    // CREATE PRODUCT
    // =========================

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var categories = await _categoryRepository.GetAllAsync();

        ViewBag.Categories = new SelectList(
            categories,
            "CategoryId",
            "Name"
        );

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        Product product,
        IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                string folder = Path.Combine(
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
                    + Path.GetExtension(imageFile.FileName);

                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                product.ImagePath =
                    "/uploads/products/" + fileName;
            }

            await _repository.AddAsync(product);

            return RedirectToAction(nameof(Index));
        }

        var categories = await _categoryRepository.GetAllAsync();

        ViewBag.Categories = new SelectList(
            categories,
            "CategoryId",
            "Name",
            product.CategoryId
        );

        return View(product);
    }

    // =========================
    // EDIT PRODUCT / ADD STOCK
    // =========================

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        var categories = await _categoryRepository.GetAllAsync();

        ViewBag.Categories = new SelectList(
            categories,
            "CategoryId",
            "Name",
            product.CategoryId
        );

        return View(product);
    }

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
                await _repository.GetByIdAsync(product.ProductId);

            if (oldProduct == null)
                return NotFound();

            // Keep old image if no new image is selected
            product.ImagePath = oldProduct.ImagePath;

            // Save new image if selected
            if (imageFile != null && imageFile.Length > 0)
            {
                string folder = Path.Combine(
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
                    + Path.GetExtension(imageFile.FileName);

                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(
                    filePath,
                    FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                product.ImagePath =
                    "/uploads/products/" + fileName;
            }

            await _repository.UpdateAsync(product);

            return RedirectToAction(nameof(Index));
        }

        var categories = await _categoryRepository.GetAllAsync();

        ViewBag.Categories = new SelectList(
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
        var product = await _repository.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _repository.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }
}


}
