using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace QuizMaker.Models;
public class QuizSettings
{
   
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId QuizId{get;set;}

    [BsonElement("UserId")]
    public string UserId {get;set;}
    [Required]
    [BsonElement("QuizName")]
    public string QuizName {get;set;}
    [Required]

    [BsonElement("Topics")]
    public string Topics { get; set; }

    [BsonElement("NumofQuestions")]
    [Required]
    [Range(2, 10, ErrorMessage = "Number of questions must be between 2 and 10.")]
    public int NumOfQuestions { get; set; }

    [BsonIgnore]
    [Required]
    public IFormFile UploadFile {get;set;}
}
