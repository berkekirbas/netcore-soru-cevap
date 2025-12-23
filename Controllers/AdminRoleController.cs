using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoruCevapPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace SoruCevapPortal.Controllers;

[Authorize(Roles = "Admin")]
public class AdminRoleController : Controller
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminRoleController(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var roleViewModels = new List<RoleViewModel>();

        foreach (var role in roles)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            roleViewModels.Add(new RoleViewModel
            {
                Id = role.Id,
                Name = role.Name!,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                UserCount = usersInRole.Count
            });
        }

        return View(roleViewModels);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateRoleViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            var roleExists = await _roleManager.RoleExistsAsync(model.Name);
            if (roleExists)
            {
                ModelState.AddModelError("Name", "Bu rol adı zaten kullanılıyor.");
                return View(model);
            }

            var role = new ApplicationRole
            {
                Name = model.Name,
                Description = model.Description,
                CreatedAt = DateTime.Now
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Rol başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            return NotFound();
        }

        var model = new EditRoleViewModel
        {
            Id = role.Id,
            Name = role.Name!,
            Description = role.Description
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditRoleViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return NotFound();
            }

            // Don't allow editing system roles name
            if (IsSystemRole(role.Name!) && role.Name != model.Name)
            {
                ModelState.AddModelError("Name", "Sistem rollerinin adı değiştirilemez.");
                return View(model);
            }

            role.Name = model.Name;
            role.Description = model.Description;

            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Rol başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
            {
                return Json(new { success = false, message = "Rol bulunamadı." });
            }

            // Don't allow deleting system roles
            if (IsSystemRole(role.Name!))
            {
                return Json(new { success = false, message = "Sistem rolleri silinemez." });
            }

            // Check if role has users
            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
            if (usersInRole.Any())
            {
                return Json(new { success = false, message = $"Bu rolde {usersInRole.Count} kullanıcı var. Önce kullanıcıları başka role atayın." });
            }

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                return Json(new { success = true, message = "Rol başarıyla silindi." });
            }

            return Json(new { success = false, message = "Rol silinirken bir hata oluştu." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Rol silinirken bir hata oluştu: " + ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Users(int id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());
        if (role == null)
        {
            return NotFound();
        }

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);

        ViewBag.RoleName = role.Name;
        ViewBag.RoleId = role.Id;

        return View(usersInRole);
    }

    private bool IsSystemRole(string roleName)
    {
        var systemRoles = new[] { "Admin", "User", "Moderator" };
        return systemRoles.Contains(roleName);
    }
}

// ViewModels for AdminRole
public class RoleViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserCount { get; set; }
}

public class CreateRoleViewModel
{
    [Required(ErrorMessage = "Rol adı zorunludur.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Rol adı 2-50 karakter arasında olmalıdır.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }
}

public class EditRoleViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Rol adı zorunludur.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Rol adı 2-50 karakter arasında olmalıdır.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Description { get; set; }
}

