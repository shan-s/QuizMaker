using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuizMaker.Models;
public class Quiz
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId QuizSessionId { get; set; }

    [BsonElement("QuizId")]
    public ObjectId QuizId { get; set; }

    [BsonElement("UserId")]
    public string UserId { get; set; }

    [BsonElement("DateTimeUTC")]
    public DateTime DateTimeUTC {get;set;} =DateTime.UtcNow;

    [BsonElement("QuizName")]
    public string QuizName { get; set; }

    [BsonElement("Questions")]
    [JsonPropertyName("questions")]
    public List<QuestionAnswer> Questions { get; set; }
}

public class QuestionAnswer
{
    [BsonElement("Question")]
    [JsonPropertyName("question")]
    public string QuestionText { get; set; }

    [BsonElement("OptionA")]
    [JsonPropertyName("option_A")]
    public string OptionA { get; set; }

    [BsonElement("OptionB")]
    [JsonPropertyName("option_B")]
    public string OptionB { get; set; }

    [BsonElement("OptionC")]
    [JsonPropertyName("option_C")]
    public string OptionC { get; set; }

    [BsonElement("OptionD")]
    [JsonPropertyName("option_D")]
    public string OptionD { get; set; }

    [BsonElement("OptionE")]
    [JsonPropertyName("option_E")]
    public string OptionE { get; set; }

    [BsonElement("Answer")]
    [JsonPropertyName("answer")]
    public string Answer { get; set; }

    [BsonElement("answerExplanation")]
    [JsonPropertyName("answerExplanation")]
    public string AnswerExplanation { get; set; }

    [BsonElement("SubmittedAnswer")]
    [JsonPropertyName("submittedAnswer")]
    public string SubmittedAnswer { get; set; }
}

