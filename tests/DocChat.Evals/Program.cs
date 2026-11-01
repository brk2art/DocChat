using System.Globalization;
using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using DocChat.Core;
using DocChat.Evals;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

#pragma warning disable OPENAI001 // reasoning effort is marked experimental in the OpenAI SDK

// dotnet run --project tests/DocChat.Evals -- --label baseline [--repeat 3] [--mode Hybrid|Keyword|Vector] [--case damage-1,norway-1]
string? Arg(string name)
{
    var i = Array.IndexOf(args, name);
    return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
}

var label = Arg("--label") ?? "run";
var repeat = int.Parse(Arg("--repeat") ?? "1", CultureInfo.InvariantCulture);
var mode = Enum.Parse<SearchMode>(Arg("--mode") ?? "Hybrid", ignoreCase: true);
var only = Arg("--case")?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

var json = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "golden.json"));
var cases = JsonSerializer.Deserialize<List<GoldenCase>>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
if (only is not null)
    cases = cases.Where(c => only.Contains(c.Id)).ToList();

// The same services as the API, built directly: the eval tests the RAG pipeline, not the HTTP endpoint.
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
var azure = new AzureOpenAIClient(
    new Uri(config["AzureOpenAI:Endpoint"]!),
    new AzureKeyCredential(config["AzureOpenAI:Key"]!));
// --reasoning low|medium|high: how much the model may think before answering. Off = the deployment's default.
var reasoning = Arg("--reasoning");
IChatClient chat = azure.GetChatClient(config["AzureOpenAI:ChatDeployment"]!).AsIChatClient();
if (reasoning is not null)
{
    var level = new ChatReasoningEffortLevel(reasoning.ToLowerInvariant());
    chat = chat.AsBuilder()
        .ConfigureOptions(o => o.RawRepresentationFactory = _ => new ChatCompletionOptions { ReasoningEffortLevel = level })
        .Build();
}
var embedder = azure.GetEmbeddingClient(config["AzureOpenAI:EmbeddingDeployment"]!).AsIEmbeddingGenerator();
var search = new SearchClient(
    new Uri(config["AzureSearch:Endpoint"]!),
    "docchat",
    new AzureKeyCredential(config["AzureSearch:QueryKey"]!));
var answerer = new Answerer(new Retriever(search, embedder), chat);

Console.WriteLine($"DocChat eval ({label}, {mode} search): {cases.Count} cases x {repeat}");

var results = new List<CaseRuns>();
foreach (var c in cases)
{
    var runs = new List<CaseResult>();
    for (var attempt = 0; attempt < repeat; attempt++)
    {
        try
        {
            runs.Add(Scoring.Score(c, await answerer.AskAsync(c.Question, mode), null));
        }
        catch (Exception ex)
        {
            runs.Add(Scoring.Score(c, null, ex));
        }
    }

    var caseRuns = new CaseRuns(c, runs);
    results.Add(caseRuns);
    var problems = string.Join("; ", runs.SelectMany(r => r.Problems).Distinct());
    Console.WriteLine($"{caseRuns.Verdict,-6} {caseRuns.Passes}/{runs.Count}  {c.Id,-11} {caseRuns.MedianMs,6} ms  {problems}");
}

var report = Report.Build(label, mode, repeat, results);
Console.WriteLine();
Console.WriteLine(report.Summary);

var folder = Path.Combine(Directory.GetCurrentDirectory(), "eval-results");
Directory.CreateDirectory(folder);
var file = Path.Combine(folder, $"{label}-{mode.ToString().ToLowerInvariant()}-{DateTime.Now:yyyyMMdd-HHmmss}.md");
await File.WriteAllTextAsync(file, report.Markdown);
Console.WriteLine($"Report: {file}");