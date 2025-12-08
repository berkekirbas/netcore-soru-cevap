using Microsoft.EntityFrameworkCore;
using SoruCevapPortal.Data;
using SoruCevapPortal.Models;

namespace SoruCevapPortal.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryWithQuestionsAsync(int id)
    {
        return await _dbSet
            .Include(c => c.Questions)
            .FirstOrDefaultAsync(c => c.Id == id);
    }
}

