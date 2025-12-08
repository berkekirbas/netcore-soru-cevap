using SoruCevapPortal.Data;

namespace SoruCevapPortal.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private ICategoryRepository? _categories;
    private IQuestionRepository? _questions;
    private IAnswerRepository? _answers;
    private IUserRepository? _users;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ApplicationDbContext Context => _context;

    public ICategoryRepository Categories
    {
        get
        {
            _categories ??= new CategoryRepository(_context);
            return _categories;
        }
    }

    public IQuestionRepository Questions
    {
        get
        {
            _questions ??= new QuestionRepository(_context);
            return _questions;
        }
    }

    public IAnswerRepository Answers
    {
        get
        {
            _answers ??= new AnswerRepository(_context);
            return _answers;
        }
    }

    public IUserRepository Users
    {
        get
        {
            _users ??= new UserRepository(_context);
            return _users;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

