using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.KernelMemory;
using Azure.Search;
using Azure.Search.Documents;
using Microsoft.SemanticKernel.TemplateEngine;
using HandlebarsDotNet;
using Microsoft.Extensions.Options;
using System.Text.Json;
using QuizMaker.Models;
using Amazon.S3.Model;
using Microsoft.KernelMemory.Context;
using System.Text;
using Microsoft.AspNetCore.Identity;

public class KernelOps
{
    KernelSettings _settings;
    Kernel _kernel;

    IKernelMemory _longMemory;
    public KernelOps(IOptions<KernelSettings> options)
    {
        _settings = options.Value;

        var builder = Kernel.CreateBuilder();
        builder.Services.AddLogging(c => c.SetMinimumLevel(LogLevel.Information));
        builder.Services.AddAzureOpenAIChatCompletion(_settings.DeploymentName, _settings.EndpointUrl, _settings.ApiKey);
        _kernel = builder.Build();

        var txtGenConfig = new AzureOpenAIConfig()
        {
            Deployment = _settings.DeploymentName,
            Endpoint = _settings.EndpointUrl,
            APIKey = _settings.ApiKey,
            MaxTokenTotal = _settings.MaxTokens,
            Auth = AzureOpenAIConfig.AuthTypes.APIKey
        };

        var embeddingConfig = new AzureOpenAIConfig()
        {
            Deployment = "text-embedding-ada-002",
            Endpoint = _settings.EndpointUrl,
            APIKey = _settings.ApiKey,
            MaxTokenTotal = 20000,
            Auth = AzureOpenAIConfig.AuthTypes.APIKey
        };

        var aiSearchConfig = new AzureAISearchConfig()
        {
            Endpoint = _settings.SearchEndpointUrl,
            APIKey = _settings.SearchApiKey,
            Auth = AzureAISearchConfig.AuthTypes.APIKey,
            UseHybridSearch = false
        };
        _longMemory = new KernelMemoryBuilder()
                                .WithAzureOpenAITextGeneration(txtGenConfig)
                                .WithAzureOpenAITextEmbeddingGeneration(embeddingConfig)
                                .WithAzureAISearchMemoryDb(aiSearchConfig)
                                .Build();
        _kernel.ImportPluginFromObject(new MemoryPlugin(_longMemory, _settings.SearchIndex), "longmemory");
    }

    private KernelFunction GetKernelFunction(string prompt = "")
    {
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            MaxTokens = _settings.MaxTokens,
            Temperature = _settings.Temperature,
            TopP = _settings.TopP,
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        return _kernel.CreateFunctionFromPrompt(prompt != "" ? prompt : File.ReadAllText(_settings.PromptTemplatePath)
                , executionSettings);
    }

    public async Task<Quiz> GenerateQuiz(string topic, int numQuestions, string quizId)
    {
        var content = await GetContent(topic.Split("\n"),quizId);
        var tariffcodeSearchFn = GetKernelFunction();
        var result = await _kernel.InvokeAsync(tariffcodeSearchFn,
                new()
                {
                    ["topic"] = topic,
                    ["content"] = content,
                    ["questionsToGenerate"] = numQuestions.ToString()
                });

        string retval = result.GetValue<string>()?.ReplaceLineEndings("")!;
        Quiz? quiz = JsonSerializer.Deserialize<Quiz>(retval) ?? throw new Exception("Unable to generate quiz");
        return quiz;
    }

    public async Task<bool> UploadContent(string filePath, string quizId)
    {
        
        var tags = new TagCollection
        {
            { "QuizId", quizId }
        };
        await _longMemory.ImportDocumentAsync(filePath, documentId: quizId, tags:tags,index:_settings.SearchIndex);
        return true;
    }

    private async Task<string> GetContent(string[] topics, string quizId)
    {
        StringBuilder answerbuilder = new StringBuilder();
        var filter = new MemoryFilter
        {
            { "QuizId", quizId }
        };
        //Get relevant content for each topic
        foreach (var topic in topics)
        {
            var result = await _longMemory.AskAsync(topic, index:_settings.SearchIndex, filter: filter,minRelevance: 0.333);
            answerbuilder.Append($"{result.Result}{result.RelevantSources.Select(cite => string.Join(',', cite.SourceName))}");
        }
        return answerbuilder.ToString();
    }
}