using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SoruCevapPortal.Repositories;

namespace SoruCevapPortal.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        var questions = await _unitOfWork.Questions.GetAllAsync();
        var answers = await _unitOfWork.Answers.GetAllAsync();
        var users = await _unitOfWork.Users.GetAllAsync();

        ViewBag.TotalCategories = categories.Count();
        ViewBag.TotalQuestions = questions.Count();
        ViewBag.TotalAnswers = answers.Count();
        ViewBag.TotalUsers = users.Count();
        ViewBag.ActiveQuestions = questions.Count(q => q.IsActive);
        ViewBag.UnapprovedAnswers = answers.Count(a => !a.IsApproved && a.IsActive);

        return View();
    }
}

