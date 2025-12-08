using SoruCevapPortal.Models;

namespace SoruCevapPortal.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    Task<Category?> GetCategoryWithQuestionsAsync(int id);
}

