using SoruCevapPortal.Models;

namespace SoruCevapPortal.Repositories;

public interface IAnswerRepository : IGenericRepository<Answer>
{
    Task<IEnumerable<Answer>> GetAnswersByQuestionAsync(int questionId);
    Task<IEnumerable<Answer>> GetUnapprovedAnswersAsync();
    Task<IEnumerable<Answer>> GetAnswersByUserAsync(int userId);
    Task<IEnumerable<Answer>> GetAllWithDetailsAsync();
    Task ToggleApprovalStatusAsync(int id);
}

