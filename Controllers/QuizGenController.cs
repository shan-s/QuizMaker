using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using QuizMaker.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.SemanticKernel;
using System.Security.Cryptography;
using MongoDB.Bson;
using QuizMaker.DB;

namespace QuizMaker.Controllers;
public class QuizGenController : Controller
{
    
    KernelOps _kernelOps;
    DBOps _dbOps;
    public QuizGenController(KernelOps kernelOps, DBOps dbOps)
    {
        _kernelOps = kernelOps;
        _dbOps=dbOps;
    }
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Submit(QuizSettings settings)
    {
        settings.QuizId=ObjectId.GenerateNewId();
        await _dbOps.AddQuizSettings(settings);
        var localPath = $"wwwroot/data/uploads/{settings.QuizId}-{settings.UploadFile.FileName}";
        using (var localFileStream =new FileStream(localPath,FileMode.CreateNew))
        await settings.UploadFile.CopyToAsync(localFileStream);
        var result = await _kernelOps.UploadContent(localPath,$"{settings.QuizId}");
        var quizLink = Url.Action("Index", "Quiz", new { id = settings.QuizId }, Request.Scheme);
        // Redirect to Success action
        return RedirectToAction("Result", new { quizLink });
    }

    public IActionResult Result(string quizLink)
    {
        ViewData["QuizLink"] = quizLink;
        return View();
    }

}
