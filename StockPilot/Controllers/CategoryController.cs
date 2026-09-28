using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StockPilot.Models;
using StockPilot.Repositories;

namespace StockPilot.Controllers
{
[Authorize]
public class CategoryController : Controller
{
private readonly ICategoryRepository _repository;


    public CategoryController(ICategoryRepository repository)
    {
        _repository = repository;
    }


    // Everyone who is logged in can view categories

    public async Task<IActionResult> Index()
    {
        var categories = await _repository.GetAllAsync();

        return View(categories);
    }


    // Everyone who is logged in can view category details

    public async Task<IActionResult> Details(int id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category == null)
            return NotFound();

        return View(category);
    }


    // Only Admin can create

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(Category category)
    {
        if (ModelState.IsValid)
        {
            await _repository.AddAsync(category);

            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }


    // Only Admin can edit

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category == null)
            return NotFound();

        return View(category);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            await _repository.UpdateAsync(category);

            return RedirectToAction(nameof(Index));
        }

        return View(category);
    }


    // Only Admin can delete

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _repository.GetByIdAsync(id);

        if (category == null)
            return NotFound();

        return View(category);
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
