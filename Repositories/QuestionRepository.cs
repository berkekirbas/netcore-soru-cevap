using Microsoft.EntityFrameworkCore;
using SoruCevapPortal.Data;
using SoruCevapPortal.Models;

namespace SoruCevapPortal.Repositories;

public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
{
    public QuestionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Question>> GetActiveQuestionsAsync()
    {
        return await _dbSet
            .Where(q => q.IsActive)
            .Include(q => q.User)
            .Include(q => q.Category)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetQuestionsByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Where(q => q.CategoryId == categoryId && q.IsActive)
            .Include(q => q.User)
            .Include(q => q.Category)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Question>> GetQuestionsByUserAsync(int userId)
    {
        return await _dbSet
            .Where(q => q.UserId == userId)
            .Include(q => q.Category)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task<Question?> GetQuestionWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(q => q.User)
            .Include(q => q.Category)
            .Include(q => q.Answers)
                .ThenInclude(a => a.User)
            .Include(q => q.QuestionVotes)
            .FirstOrDefaultAsync(q => q.Id == id);
    }

    public async Task<IEnumerable<Question>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(q => q.User)
            .Include(q => q.Category)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();
    }

    public async Task ToggleActiveStatusAsync(int id)
    {
        var question = await _dbSet.FindAsync(id);
        if (question != null)
        {
            question.IsActive = !question.IsActive;
            question.UpdatedAt = DateTime.Now;
            _dbSet.Update(question);
        }
    }
}

