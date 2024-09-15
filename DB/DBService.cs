using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using QuizMaker.Models;

namespace QuizMaker.DB;
public class DBOps
{
    private readonly IMongoCollection<QuizSettings> _quizSettingsCollection;
    private readonly IMongoCollection<Quiz> _quizCollection;

    public DBOps(IOptions<MongoDBSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DbName);
        _quizSettingsCollection = database.GetCollection<QuizSettings>("QuizSettings");
        _quizCollection = database.GetCollection<Quiz>("Quiz");
    }

    public async Task AddQuiz(Quiz quiz)
    {
        await _quizCollection.InsertOneAsync(quiz);
    }

    public async Task<Quiz> GetQuiz(ObjectId quizSessionId)
    {
        return await _quizCollection.Find(q => q.QuizSessionId == quizSessionId).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateQuiz(Quiz updatedQuiz)
    {
        var filter = Builders<Quiz>.Filter.Eq(q => q.QuizSessionId, updatedQuiz.QuizSessionId);
        var result = await _quizCollection.ReplaceOneAsync(filter, updatedQuiz);
        return (result.MatchedCount > 0) ? true : false;
    }

    public async Task AddQuizSettings(QuizSettings quizSettings)
    {
        await _quizSettingsCollection.InsertOneAsync(quizSettings);
    }

    public async Task<QuizSettings> GetQuizSettings(string id)
    {
        if (!ObjectId.TryParse(id, out var quizId))
            throw new Exception("Invalid Quiz Id");
        return await _quizSettingsCollection.Find(q => q.QuizId == quizId).FirstOrDefaultAsync();
    }
}
