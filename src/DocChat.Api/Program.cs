using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using DocChat.Api;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// One client for the Foundry resource; the deployment names choose the models.
var azure = new AzureOpenAIClient(
    new Uri(config["AzureOpenAI:Endpoint"]!),
    new AzureKeyCredential(config["AzureOpenAI:Key"]!));

builder.Services.AddChatClient(azure.GetChatClient(config["AzureOpenAI:ChatDeployment"]!).AsIChatClient());
builder.Services.AddEmbeddingGenerator(azure.GetEmbeddingClient(config["AzureOpenAI:EmbeddingDeployment"]!).AsIEmbeddingGenerator());

// The API only searches, so it gets the query key: it can read the index, never change it.
builder.Services.AddSingleton(new SearchClient(
    new Uri(config["AzureSearch:Endpoint"]!),
    "docchat",
    new AzureKeyCredential(config["AzureSearch:QueryKey"]!)));

builder.Services.AddSingleton<Retriever>();
builder.Services.AddSingleton<Answerer>();

var app = builder.Build();

app.MapPost("/ask", async (AskRequest request, Answerer answerer, CancellationToken ct) =>
{
    var question = request.Question?.Trim();
    if (string.IsNullOrEmpty(question) || question.Length > 500)
        return Results.BadRequest(new { error = "Ask a question of 1 to 500 characters." });

    return Results.Ok(await answerer.AskAsync(question, ct));
});

app.Run();
