using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public class KernelSettings:IOptions<KernelSettings>
{
    public const string SectionName = "KernelSettings";

    public string DeploymentName { get; set; } = string.Empty;
    public string EndpointUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 0;
    public double Temperature { get; set; } = 0;
    public double TopP { get; set; } = 0;
    public string PromptTemplatePath { get; set; } = string.Empty;
    public string SearchEndpointUrl {get;set;}=string.Empty;
    public string SearchApiKey {get;set;}=string.Empty;
    public string SearchIndex {get;set;}=string.Empty;
    public KernelSettings Value => this;
}
