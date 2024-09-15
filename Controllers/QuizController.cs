using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text.Json;
using QuizMaker.Models;
using QuizMaker.DB;
using MongoDB.Bson;

namespace QuizMaker.Controllers;
public class QuizController : Controller
{
    KernelOps _kernelOps;
    DBOps _dbOps;
    public QuizController(KernelOps kernelOps, DBOps dbOps)
    {
        _kernelOps = kernelOps;
        _dbOps = dbOps;
    }

    public async Task<Quiz> GetQuiz(string quizId)
    {
        if (string.IsNullOrEmpty(quizId)) return new Quiz(); ;
        var quizSettings = await _dbOps.GetQuizSettings(quizId);
        if (quizSettings?.QuizId == null) return new Quiz();
        var quiz = await _kernelOps.GenerateQuiz(quizSettings.Topics, quizSettings.NumOfQuestions, quizId);
        quiz.QuizId = quizSettings.QuizId;
        quiz.QuizName = quizSettings.QuizName;
        return quiz;
    }
    public IActionResult Index(string id)
    {
        TempData["QuizId"] = id;
        return View();
    }

    public async Task<IActionResult> LoadQuiz(string quizId)
    {
        var quiz = await GetQuiz(quizId ?? "");
        if (quiz?.QuizId == null) return NotFound();
        quiz.QuizSessionId = ObjectId.GenerateNewId();
        await _dbOps.AddQuiz(quiz);
        return View("DisplayQuiz", quiz);
    }

    // This action displays the quiz
    public IActionResult DisplayQuiz(Quiz quiz)
    {
        // Render the quiz view
        return View(quiz);
    }

    // This action handles the form submission
    [HttpPost]
    public async Task<IActionResult> Submit(Quiz submitted)
    {
        // Process the quiz answers here
        Quiz _quiz = await _dbOps.GetQuiz(submitted.QuizSessionId);
        submitted.QuizId = _quiz.QuizId;
        submitted.QuizSessionId = _quiz.QuizSessionId;
        submitted.QuizName = _quiz.QuizName;
        submitted.UserId = _quiz.UserId;
        int correctAnswers = 0;
        // You can check the user's answers against the correct answers
        for (var i = 0; i < submitted.Questions.Count; i++)
        {
            submitted.Questions[i].QuestionText = _quiz.Questions[i].QuestionText;
            submitted.Questions[i].OptionA = _quiz.Questions[i].OptionA;
            submitted.Questions[i].OptionB = _quiz.Questions[i].OptionB;
            submitted.Questions[i].OptionC = _quiz.Questions[i].OptionC;
            submitted.Questions[i].OptionD = _quiz.Questions[i].OptionD;
            submitted.Questions[i].OptionE = _quiz.Questions[i].OptionE;
            submitted.Questions[i].Answer = _quiz.Questions[i].Answer;
            if (submitted.Questions[i].SubmittedAnswer == _quiz.Questions[i].Answer)
                correctAnswers++;
        }

        TempData["QuizResult"] = $"{double.Round((((double)correctAnswers / _quiz.Questions.Count) * 100),2)}";
        await _dbOps.UpdateQuiz(submitted);
        return View("Result", submitted);
    }

    // This action displays the results
    public IActionResult Result(Quiz quiz)
    {
        // Render the result view
        return View(quiz);
    }
}
