using SoruCevapPortal.Models;

namespace SoruCevapPortal.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserWithRolesAsync(int id);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<IEnumerable<User>> GetUsersWithRolesAsync();
    Task<bool> ValidateUserAsync(string username, string password);
}

