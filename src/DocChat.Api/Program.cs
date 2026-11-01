#pragma warning disable OPENAI001 // reasoning effort is marked experimental in the OpenAI SDK
using System.Threading.RateLimiting;
using Azure;
using Azure.AI.OpenAI;
using Azure.Core;
using Azure.Identity;
using Azure.Search.Documents;
using DocChat.Api;
using DocChat.Core;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// A key is used only where one is configured (user secrets on your PC). In Azure there is none: the app signs in
// with its managed identity, and Azure roles decide what it may do.
TokenCredential identity = new DefaultAzureCredential();

var openAiEndpoint = new Uri(config["AzureOpenAI:Endpoint"]!);
var openAiKey = config["AzureOpenAI:Key"];
var azure = string.IsNullOrEmpty(openAiKey)
    ? new AzureOpenAIClient(openAiEndpoint, identity)
    : new AzureOpenAIClient(openAiEndpoint, new AzureKeyCredential(openAiKey));

// Reasoning "low": measured 57/60 → 59/60 correct for +0.17 s and +29% cost (eval runs docs-v3 vs reasoning-low).
var reasoningEffort = new ChatReasoningEffortLevel(config["AzureOpenAI:ReasoningEffort"] ?? "low");
builder.Services.AddChatClient(azure.GetChatClient(config["AzureOpenAI:ChatDeployment"]!).AsIChatClient())
    .ConfigureOptions(o => o.RawRepresentationFactory = _ => new ChatCompletionOptions { ReasoningEffortLevel = reasoningEffort }); 
builder.Services.AddEmbeddingGenerator(azure.GetEmbeddingClient(config["AzureOpenAI:EmbeddingDeployment"]!).AsIEmbeddingGenerator());

// The API only searches: a query key locally, the "Search Index Data Reader" role in Azure. Neither can change the index.
var searchEndpoint = new Uri(config["AzureSearch:Endpoint"]!);
var searchKey = config["AzureSearch:QueryKey"];
builder.Services.AddSingleton(string.IsNullOrEmpty(searchKey)
    ? new SearchClient(searchEndpoint, "docchat", identity)
    : new SearchClient(searchEndpoint, "docchat", new AzureKeyCredential(searchKey)));

builder.Services.AddSingleton<Retriever>();
builder.Services.AddSingleton<Answerer>();

// A public demo: at most 100 questions per hour for everyone together, so nobody can run up the bill.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("ask", limiter =>
    {
        limiter.PermitLimit = 100;
        limiter.Window = TimeSpan.FromHours(1);
        limiter.QueueLimit = 0;
    });
});

var app = builder.Build();

// The built React page (wwwroot): index.html at "/", its scripts and styles as static files.
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRateLimiter();

app.MapPost("/ask", async (AskRequest request, Answerer answerer, CancellationToken ct) =>
{
    var question = request.Question?.Trim();
    if (string.IsNullOrEmpty(question) || question.Length > 500)
        return Results.BadRequest(new { error = "Ask a question of 1 to 500 characters." });

    return Results.Ok(await answerer.AskAsync(question, ct: ct));
})
.RequireRateLimiting("ask");

app.MapFallbackToFile("index.html");

app.Run();
