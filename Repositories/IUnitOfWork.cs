using SoruCevapPortal.Data;

namespace SoruCevapPortal.Repositories;

public interface IUnitOfWork : IDisposable
{
    ApplicationDbContext Context { get; }
    ICategoryRepository Categories { get; }
    IQuestionRepository Questions { get; }
    IAnswerRepository Answers { get; }
    IUserRepository Users { get; }
    Task<int> SaveChangesAsync();
}

