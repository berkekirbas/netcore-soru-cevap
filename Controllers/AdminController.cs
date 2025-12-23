using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoruCevapPortal.Models;
using SoruCevapPortal.Repositories;

namespace SoruCevapPortal.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        var questions = await _unitOfWork.Questions.GetAllAsync();
        var answers = await _unitOfWork.Answers.GetAllAsync();
        var users = _userManager.Users.ToList();

        ViewBag.TotalCategories = categories.Count();
        ViewBag.TotalQuestions = questions.Count();
        ViewBag.TotalAnswers = answers.Count();
        ViewBag.TotalUsers = users.Count;
        ViewBag.ActiveQuestions = questions.Count(q => q.IsActive);
        ViewBag.UnapprovedAnswers = answers.Count(a => !a.IsApproved && a.IsActive);

        // Son aktiviteler
        ViewBag.RecentQuestions = questions
            .OrderByDescending(q => q.CreatedAt)
            .Take(5)
            .ToList();

        ViewBag.RecentAnswers = answers
            .OrderByDescending(a => a.CreatedAt)
            .Take(5)
            .ToList();

        return View();
    }
}
