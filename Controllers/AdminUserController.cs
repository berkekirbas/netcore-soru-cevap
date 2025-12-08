using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoruCevapPortal.Models;
using SoruCevapPortal.Repositories;

namespace SoruCevapPortal.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminUserController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminUserController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _unitOfWork.Users.GetUsersWithRolesAsync();
        return View(users);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(User user)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();
                TempData["SuccessMessage"] = "Kullanıcı başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kullanıcı oluşturulurken bir hata oluştu.";
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
            }
        }
        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, User user)
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                user.UpdatedAt = DateTime.Now;
                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                TempData["SuccessMessage"] = "Kullanıcı başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Kullanıcı güncellenirken bir hata oluştu.";
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
            }
        }
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _unitOfWork.Users.GetUserWithRolesAsync(id);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı." });
            }

            // Kullanıcının sorularını kontrol et
            var questionCount = user.Questions?.Count ?? 0;
            var answerCount = user.Answers?.Count ?? 0;

            if (questionCount > 0 || answerCount > 0)
            {
                return Json(new { success = false, message = $"Bu kullanıcıya ait {questionCount} soru ve {answerCount} cevap bulunuyor. Önce bunları silmelisiniz." });
            }

            // UserRoles ilişkilerini sil
            if (user.UserRoles != null && user.UserRoles.Any())
            {
                foreach (var userRole in user.UserRoles.ToList())
                {
                    _unitOfWork.Context.Set<UserRole>().Remove(userRole);
                }
            }

            await _unitOfWork.Users.DeleteAsync(user);
            await _unitOfWork.SaveChangesAsync();
            return Json(new { success = true, message = "Kullanıcı başarıyla silindi." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Kullanıcı silinirken bir hata oluştu: " + ex.Message });
        }
    }
}

