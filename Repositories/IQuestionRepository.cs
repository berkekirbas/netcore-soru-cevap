using SoruCevapPortal.Models;

namespace SoruCevapPortal.Repositories;

public interface IQuestionRepository : IGenericRepository<Question>
{
    Task<IEnumerable<Question>> GetActiveQuestionsAsync();
    Task<IEnumerable<Question>> GetQuestionsByCategoryAsync(int categoryId);
    Task<IEnumerable<Question>> GetQuestionsByUserAsync(int userId);
    Task<Question?> GetQuestionWithDetailsAsync(int id);
    Task<IEnumerable<Question>> GetAllWithDetailsAsync();
    Task ToggleActiveStatusAsync(int id);
}

