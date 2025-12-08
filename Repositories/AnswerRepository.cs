using Microsoft.EntityFrameworkCore;
using SoruCevapPortal.Data;
using SoruCevapPortal.Models;

namespace SoruCevapPortal.Repositories;

public class AnswerRepository : GenericRepository<Answer>, IAnswerRepository
{
    public AnswerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Answer>> GetAnswersByQuestionAsync(int questionId)
    {
        return await _dbSet
            .Where(a => a.QuestionId == questionId && a.IsActive)
            .Include(a => a.User)
            .OrderByDescending(a => a.IsApproved)
            .ThenByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Answer>> GetUnapprovedAnswersAsync()
    {
        return await _dbSet
            .Where(a => !a.IsApproved && a.IsActive)
            .Include(a => a.Question)
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Answer>> GetAnswersByUserAsync(int userId)
    {
        return await _dbSet
            .Where(a => a.UserId == userId)
            .Include(a => a.Question)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Answer>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(a => a.Question)
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task ToggleApprovalStatusAsync(int id)
    {
        var answer = await _dbSet.FindAsync(id);
        if (answer != null)
        {
            answer.IsApproved = !answer.IsApproved;
            answer.UpdatedAt = DateTime.Now;
            _dbSet.Update(answer);
        }
    }
}

