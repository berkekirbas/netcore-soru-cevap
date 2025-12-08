using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoruCevapPortal.Models;
using SoruCevapPortal.Repositories;

namespace SoruCevapPortal.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminAnswerController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminAnswerController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var answers = await _unitOfWork.Answers.GetAllWithDetailsAsync();
        return View(answers);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var answers = await _unitOfWork.Answers.GetAllWithDetailsAsync();
        var answer = answers.FirstOrDefault(a => a.Id == id);
        if (answer == null)
        {
            return NotFound();
        }
        return View(answer);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var questions = await _unitOfWork.Questions.GetAllWithDetailsAsync();
        var users = await _unitOfWork.Users.GetActiveUsersAsync();
        
        ViewBag.Questions = new SelectList(questions, "Id", "Title");
        ViewBag.Users = new SelectList(users, "Id", "Username");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Content,QuestionId,UserId,IsApproved,IsActive")] Answer answer)
    {
        // Navigation properties'i ModelState'den çıkar
        ModelState.Remove("Question");
        ModelState.Remove("User");

        if (ModelState.IsValid)
        {
            try
            {
                answer.CreatedAt = DateTime.Now;
                await _unitOfWork.Answers.AddAsync(answer);
                await _unitOfWork.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cevap başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Cevap oluşturulurken bir hata oluştu.";
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
            }
        }
        var questions = await _unitOfWork.Questions.GetAllWithDetailsAsync();
        var users = await _unitOfWork.Users.GetActiveUsersAsync();
        ViewBag.Questions = new SelectList(questions, "Id", "Title", answer.QuestionId);
        ViewBag.Users = new SelectList(users, "Id", "Username", answer.UserId);
        return View(answer);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var answer = await _unitOfWork.Answers.GetByIdAsync(id);
        if (answer == null)
        {
            return NotFound();
        }
        var questions = await _unitOfWork.Questions.GetAllWithDetailsAsync();
        var users = await _unitOfWork.Users.GetActiveUsersAsync();
        ViewBag.Questions = new SelectList(questions, "Id", "Title", answer.QuestionId);
        ViewBag.Users = new SelectList(users, "Id", "Username", answer.UserId);
        return View(answer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Content,QuestionId,UserId,IsApproved,IsActive")] Answer answer)
    {
        if (id != answer.Id)
        {
            return NotFound();
        }

        // Navigation properties'i ModelState'den çıkar
        ModelState.Remove("Question");
        ModelState.Remove("User");

        if (ModelState.IsValid)
        {
            try
            {
                answer.UpdatedAt = DateTime.Now;
                await _unitOfWork.Answers.UpdateAsync(answer);
                await _unitOfWork.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cevap başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Cevap güncellenirken bir hata oluştu.";
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
            }
        }
        var questions = await _unitOfWork.Questions.GetAllWithDetailsAsync();
        var users = await _unitOfWork.Users.GetActiveUsersAsync();
        ViewBag.Questions = new SelectList(questions, "Id", "Title", answer.QuestionId);
        ViewBag.Users = new SelectList(users, "Id", "Username", answer.UserId);
        return View(answer);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var answer = await _unitOfWork.Answers.GetByIdAsync(id);
            if (answer == null)
            {
                return Json(new { success = false, message = "Cevap bulunamadı." });
            }

            await _unitOfWork.Answers.DeleteAsync(answer);
            await _unitOfWork.SaveChangesAsync();
            return Json(new { success = true, message = "Cevap başarıyla silindi." });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "Cevap silinirken bir hata oluştu." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleApproval(int id)
    {
        try
        {
            var answer = await _unitOfWork.Answers.GetByIdAsync(id);
            if (answer == null)
            {
                return Json(new { success = false, message = "Cevap bulunamadı." });
            }

            await _unitOfWork.Answers.ToggleApprovalStatusAsync(id);
            await _unitOfWork.SaveChangesAsync();

            answer = await _unitOfWork.Answers.GetByIdAsync(id);
            return Json(new { success = true, isApproved = answer?.IsApproved ?? false });
        }
        catch (Exception)
        {
            return Json(new { success = false, message = "Durum değiştirilirken bir hata oluştu." });
        }
    }
}

