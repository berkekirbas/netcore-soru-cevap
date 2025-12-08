using Microsoft.EntityFrameworkCore;
using SoruCevapPortal.Data;
using SoruCevapPortal.Models;

namespace SoruCevapPortal.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserWithRolesAsync(int id)
    {
        try
        {
            var user = await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
            
            // Eğer UserRoles yüklenmediyse, manuel olarak yükle
            if (user != null && (user.UserRoles == null || !user.UserRoles.Any()))
            {
                var userRoles = await _context.Set<UserRole>()
                    .Where(ur => ur.UserId == id)
                    .Include(ur => ur.Role)
                    .ToListAsync();
                
                user.UserRoles = userRoles;
            }
            
            return user;
        }
            catch
        {
            // If Include fails, try manual loading
            var user = await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                try
                {
                    var userRoles = await _context.Set<UserRole>()
                        .Where(ur => ur.UserId == id)
                        .Include(ur => ur.Role)
                        .ToListAsync();
                    user.UserRoles = userRoles;
                }
                catch
                {
                    // Ignore if still fails
                }
            }
            return user;
        }
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersWithRolesAsync()
    {
        return await _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<bool> ValidateUserAsync(string username, string password)
    {
        var user = await GetUserByUsernameAsync(username);
        if (user == null || !user.IsActive)
            return false;

        // In production, use proper password hashing (BCrypt, Argon2, etc.)
        return user.Password == password;
    }
}

