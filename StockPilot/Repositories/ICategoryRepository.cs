using StockPilot.Models;

namespace StockPilot.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync();

        Task<Category> GetByIdAsync(int id);

        Task AddAsync(Category category);

        Task DeleteAsync(int id);
    }
}