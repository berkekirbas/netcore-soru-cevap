using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SoruCevapPortal.Models;
using SoruCevapPortal.Repositories;

namespace SoruCevapPortal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(int? categoryId)
    {
        var questions = await _unitOfWork.Questions.GetAllWithDetailsAsync();

        // Sadece aktif soruları göster
        questions = questions.Where(q => q.IsActive).ToList();

        // Kategoriye göre filtrele
        if (categoryId.HasValue && categoryId.Value > 0)
        {
            questions = questions.Where(q => q.CategoryId == categoryId.Value).ToList();
        }

        // Son soruları en üstte göster
        questions = questions.OrderByDescending(q => q.CreatedAt).ToList();

        // Kategorileri ViewBag'e ekle
        var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
        ViewBag.Categories = categories;
        ViewBag.SelectedCategoryId = categoryId;

        return View(questions);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
