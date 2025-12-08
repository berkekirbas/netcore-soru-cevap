using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoruCevapPortal.Data;
using SoruCevapPortal.Models;
using SoruCevapPortal.Repositories;
using System.Security.Claims;

namespace SoruCevapPortal.Controllers;

public class AccountController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUnitOfWork unitOfWork, ApplicationDbContext context, ILogger<AccountController> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Kullanıcı adı ve şifre gereklidir.");
            return View();
        }

        // Try to find user by username or email
        var userByUsername = await _unitOfWork.Users.GetUserByUsernameAsync(username);
        var userByEmail = await _unitOfWork.Users.GetUserByEmailAsync(username);

        var foundUser = userByUsername ?? userByEmail;

        if (foundUser == null)
        {
            ModelState.AddModelError("", "Kullanıcı adı/email veya şifre hatalı.");
            return View();
        }

        // Validate password first (in production, use proper password hashing)
        if (foundUser.Password != password)
        {
            ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            return View();
        }

        if (!foundUser.IsActive)
        {
            ModelState.AddModelError("", "Kullanıcı hesabı aktif değil.");
            return View();
        }

        // Get user with roles
        User? user = null;
        try
        {
            user = await _unitOfWork.Users.GetUserWithRolesAsync(foundUser.Id);
            
            // Debug: Log user roles
            if (user != null)
            {
                _logger.LogInformation("User loaded: {UserId}, UserRoles count: {Count}", 
                    user.Id, user.UserRoles?.Count ?? 0);
                
                if (user.UserRoles != null && user.UserRoles.Any())
                {
                    foreach (var ur in user.UserRoles)
                    {
                        _logger.LogInformation("UserRole - UserId: {UserId}, RoleId: {RoleId}, RoleName: {RoleName}", 
                            ur.UserId, ur.RoleId, ur.Role?.Name ?? "NULL");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user roles for user {UserId}", foundUser.Id);
            ModelState.AddModelError("", "Kullanıcı bilgileri yüklenirken bir hata oluştu. Lütfen veritabanını kontrol edin.");
            return View();
        }

        if (user == null)
        {
            ModelState.AddModelError("", "Kullanıcı bilgileri alınamadı.");
            return View();
        }

        // Check if user has Admin role - DIRECT SQL QUERY (more reliable)
        bool isAdmin = false;
        
        try
        {
            // Direct query to UserRoles and Roles tables
            var hasAdminRole = await _context.Set<UserRole>()
                .Where(ur => ur.UserId == user.Id)
                .Join(_context.Set<Role>(),
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => r)
                .AnyAsync(r => r.Name == "Admin");
            
            isAdmin = hasAdminRole;
            
            _logger.LogInformation("Direct SQL check - User {UserId} has Admin role: {HasAdmin}", user.Id, isAdmin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking admin role for user {UserId}", user.Id);
            
            // Fallback: Check via navigation properties
            if (user.UserRoles != null && user.UserRoles.Any())
            {
                foreach (var userRole in user.UserRoles)
                {
                    if (userRole.Role != null && userRole.Role.Name == "Admin")
                    {
                        isAdmin = true;
                        break;
                    }
                }
            }
        }
        
        if (!isAdmin)
        {
            _logger.LogWarning("User {UserId} does not have Admin role. UserRoles count: {Count}", 
                user.Id, user.UserRoles?.Count ?? 0);
            ModelState.AddModelError("", "Bu sayfaya erişim yetkiniz yok. Admin rolüne sahip olmalısınız.");
            return View();
        }

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("Role", "Admin")
        };

        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth");
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
        };

        await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity), authProperties);

        _logger.LogInformation("User {Username} logged in", username);

        return RedirectToLocal(returnUrl);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        _logger.LogInformation("User logged out");
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Admin");
    }
}

