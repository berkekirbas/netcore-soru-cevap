using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoruCevapPortal.Hubs;
using SoruCevapPortal.Models;
using SoruCevapPortal.Repositories;

namespace SoruCevapPortal.Controllers;

public class QuestionController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAdminNotificationService _notificationService;

    public QuestionController(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        IAdminNotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Details(int id)
    {
        var question = await _unitOfWork.Questions.GetQuestionWithDetailsAsync(id);

        if (question == null || !question.IsActive)
        {
            return NotFound();
        }

        // Görüntülenme sayısını artır
        question.ViewCount++;
        await _unitOfWork.Questions.UpdateAsync(question);
        await _unitOfWork.SaveChangesAsync();

        return View(question);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
        ViewBag.Categories = categories;
        return View();
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string title, string content, int categoryId)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content) || categoryId <= 0)
        {
            TempData["ErrorMessage"] = "Lütfen tüm alanları doldurun.";
            var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
            ViewBag.Categories = categories;
            return View();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);

        var question = new Question
        {
            Title = title,
            Content = content,
            UserId = user.Id,
            CategoryId = categoryId,
            IsActive = true,
            ViewCount = 0,
            CreatedAt = DateTime.Now
        };

        await _unitOfWork.Questions.AddAsync(question);
        await _unitOfWork.SaveChangesAsync();

        // Notify admins about new question
        await _notificationService.NotifyNewQuestionAsync(question.Id, question.Title, category?.Name ?? "Bilinmiyor");

        TempData["SuccessMessage"] = "Sorunuz başarıyla gönderildi.";
        return RedirectToAction("Details", new { id = question.Id });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Answer(int questionId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            TempData["ErrorMessage"] = "Cevap içeriği boş olamaz.";
            return RedirectToAction("Details", new { id = questionId });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var question = await _unitOfWork.Questions.GetByIdAsync(questionId);
        if (question == null)
        {
            return NotFound();
        }

        var answer = new Answer
        {
            Content = content,
            QuestionId = questionId,
            UserId = user.Id,
            IsApproved = false, // Admin onayı bekleyecek
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        await _unitOfWork.Answers.AddAsync(answer);
        await _unitOfWork.SaveChangesAsync();

        // Notify admins about new answer
        await _notificationService.NotifyNewAnswerAsync(answer.Id, questionId, question.Title);

        TempData["SuccessMessage"] = "Cevabınız gönderildi. Admin onayından sonra yayınlanacak.";
        return RedirectToAction("Details", new { id = questionId });
    }
}
