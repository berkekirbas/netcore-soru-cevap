using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoruCevapPortal.Models;
using SoruCevapPortal.Repositories;

namespace SoruCevapPortal.Controllers;

[Authorize(Roles = "Admin")]
public class AdminQuestionController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminQuestionController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
    {
        var questions = await _unitOfWork.Questions.GetAllWithDetailsAsync();
        var pagedQuestions = PagedList<Question>.Create(questions, page, pageSize);
        return View(pagedQuestions);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var question = await _unitOfWork.Questions.GetQuestionWithDetailsAsync(id);
        if (question == null)
        {
            return NotFound();
        }
        return View(question);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
        var users = await _userManager.Users.Where(u => u.IsActive).ToListAsync();
        
        ViewBag.Categories = new SelectList(categories, "Id", "Name");
        ViewBag.Users = new SelectList(users, "Id", "UserName");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Title,Content,UserId,CategoryId,IsActive")] Question question)
    {
        // Navigation properties'i ModelState'den çıkar
        ModelState.Remove("User");
        ModelState.Remove("Category");

        if (ModelState.IsValid)
        {
            try
            {
                question.CreatedAt = DateTime.Now;
                await _unitOfWork.Questions.AddAsync(question);
                await _unitOfWork.SaveChangesAsync();
                TempData["SuccessMessage"] = "Soru başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Soru oluşturulurken bir hata oluştu.";
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
            }
        }
        var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
        var users = await _userManager.Users.Where(u => u.IsActive).ToListAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", question.CategoryId);
        ViewBag.Users = new SelectList(users, "Id", "UserName", question.UserId);
        return View(question);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var question = await _unitOfWork.Questions.GetQuestionWithDetailsAsync(id);
        if (question == null)
        {
            return NotFound();
        }
        var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
        var users = await _userManager.Users.Where(u => u.IsActive).ToListAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", question.CategoryId);
        ViewBag.Users = new SelectList(users, "Id", "UserName", question.UserId);
        return View(question);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,UserId,CategoryId,IsActive")] Question question)
    {
        if (id != question.Id)
        {
            return NotFound();
        }

        // Navigation properties'i ModelState'den çıkar
        ModelState.Remove("User");
        ModelState.Remove("Category");

        if (ModelState.IsValid)
        {
            try
            {
                question.UpdatedAt = DateTime.Now;
                await _unitOfWork.Questions.UpdateAsync(question);
                await _unitOfWork.SaveChangesAsync();
                TempData["SuccessMessage"] = "Soru başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Soru güncellenirken bir hata oluştu.";
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
            }
        }
        var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
        var users = await _userManager.Users.Where(u => u.IsActive).ToListAsync();
        ViewBag.Categories = new SelectList(categories, "Id", "Name", question.CategoryId);
        ViewBag.Users = new SelectList(users, "Id", "UserName", question.UserId);
        return View(question);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null)
            {
                return Json(new { success = false, message = "Soru bulunamadı." });
            }

            await _unitOfWork.Questions.DeleteAsync(question);
            await _unitOfWork.SaveChangesAsync();
            return Json(new { success = true, message = "Soru başarıyla silindi." });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "Soru silinirken bir hata oluştu." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleActive(int id)
    {
        try
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(id);
            if (question == null)
            {
                return Json(new { success = false, message = "Soru bulunamadı." });
            }

            await _unitOfWork.Questions.ToggleActiveStatusAsync(id);
            await _unitOfWork.SaveChangesAsync();

            question = await _unitOfWork.Questions.GetByIdAsync(id);
            return Json(new { success = true, isActive = question?.IsActive ?? false });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "Durum değiştirilirken bir hata oluştu." });
        }
    }
}
