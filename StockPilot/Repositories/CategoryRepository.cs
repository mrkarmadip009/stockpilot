using Microsoft.EntityFrameworkCore;
using StockPilot.Data;
using StockPilot.Models;

namespace StockPilot.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category != null)
            {
                _context.Categories.Remove(category);

                await _context.SaveChangesAsync();
            }
        }
    }
}