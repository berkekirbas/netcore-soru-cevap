using Microsoft.AspNetCore.Mvc;
using SoruCevapPortal.Repositories;

namespace SoruCevapPortal.Controllers;

public class QuestionController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public QuestionController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
}
