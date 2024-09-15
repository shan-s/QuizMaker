using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
public class MongoDBSettings:IOptions<MongoDBSettings>
{
    public const string SectionName="MongoDBSettings";
    public string ConnectionString{get;set;}
    public string DbName {get;set;}
    public MongoDBSettings Value => throw new NotImplementedException();
}