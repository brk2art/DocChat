using System.Text;
using System.Text.Json.Serialization;
using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;

const string IndexName = "docchat";
const int Dimensions = 1536; // what text-embedding-3-small produces

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

// 1. Read the documents and cut them into chunks.
var docsFolder = args.Length > 0 ? args[0] : "docs";
var files = Directory.GetFiles(docsFolder, "*.md").Order().ToList();
var chunks = files.SelectMany(Chunker.Split).ToList();
Console.WriteLine($"{chunks.Count} chunks from {files.Count} documents.");

// 2. Turn every chunk into a vector, in one call.
var azure = new AzureOpenAIClient(
    new Uri(config["AzureOpenAI:Endpoint"]!),
    new AzureKeyCredential(config["AzureOpenAI:Key"]!));
var embedder = azure.GetEmbeddingClient(config["AzureOpenAI:EmbeddingDeployment"]!).AsIEmbeddingGenerator();

var embeddings = await embedder.GenerateAsync(chunks.Select(c => c.EmbeddingText));
for (int i = 0; i < chunks.Count; i++)
    chunks[i] = chunks[i] with { ContentVector = embeddings[i].Vector.ToArray() };
Console.WriteLine($"Embedded {chunks.Count} chunks ({embeddings.Usage?.InputTokenCount} tokens).");

// 3. Create the index: which fields exist, which are keyword-searchable, and which one holds the vector.
var indexClient = new SearchIndexClient(
    new Uri(config["AzureSearch:Endpoint"]!),
    new AzureKeyCredential(config["AzureSearch:AdminKey"]!));

var index = new SearchIndex(IndexName)
{
    Fields =
    {
        new SimpleField("id", SearchFieldDataType.String) { IsKey = true },
        new SimpleField("source", SearchFieldDataType.String) { IsFilterable = true },
        new SearchableField("title"),
        new SearchableField("section"),
        new SearchableField("content"),
        new SearchField("contentVector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
        {
            IsSearchable = true,
            VectorSearchDimensions = Dimensions,
            VectorSearchProfileName = "vector-profile"
        }
    },
    VectorSearch = new VectorSearch
    {
        Algorithms = { new HnswAlgorithmConfiguration("hnsw") },
        Profiles = { new VectorSearchProfile("vector-profile", "hnsw") }
    }
};

// Start from an empty index every run, so chunks of changed or deleted documents don't linger.
try { await indexClient.DeleteIndexAsync(IndexName, CancellationToken.None); }
catch (RequestFailedException e) when (e.Status == 404) { }
await indexClient.CreateIndexAsync(index);

// 4. Upload the chunks.
var searchClient = indexClient.GetSearchClient(IndexName);
var upload = await searchClient.UploadDocumentsAsync(chunks);
Console.WriteLine($"Uploaded {upload.Value.Results.Count(r => r.Succeeded)} of {chunks.Count} chunks to '{IndexName}'.");

// 5. A first test: hybrid search (keywords + vector) for a question with words that aren't in the documents.
var question = "The box was smashed when it got to me"; 
var questionVector = (await embedder.GenerateAsync([question]))[0].Vector;

var options = new SearchOptions
{
    Size = 3,
    VectorSearch = new VectorSearchOptions
    {
        Queries = { new VectorizedQuery(questionVector) { KNearestNeighborsCount = 3, Fields = { "contentVector" } } }
    }
};

foreach (var field in new[] { "id", "source", "title", "section", "content" })
    options.Select.Add(field);

Console.WriteLine();
Console.WriteLine($"Question: {question}");
var results = await searchClient.SearchAsync<Chunk>(question, options);
await foreach (var result in results.Value.GetResultsAsync())
    Console.WriteLine($"  {result.Score:0.0000}  {result.Document.Source} > {result.Document.Section}");

// One chunk = one section of one document. The JSON names match the index field names.
public sealed record Chunk(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("section")] string Section,
    [property: JsonPropertyName("content")] string Content)
{
    [JsonPropertyName("contentVector")]
    public float[]? ContentVector { get; init; }

    // What gets embedded: the chunk plus where it comes from. "Compensation" alone could be about anything;
    // "Damaged parcels and claims - Compensation" can't.
    [JsonIgnore]
    public string EmbeddingText => $"{Title} - {Section}\n{Content}";
}

public static class Chunker
{
    private const int MaxChars = 1500;

    // Splits a Markdown document at its "## " headings: each section is about one thing, so it makes a good chunk.
    public static List<Chunk> Split(string path)
    {
        var source = Path.GetFileName(path);
        var baseId = Path.GetFileNameWithoutExtension(path);
        var lines = File.ReadAllLines(path);
        var title = lines.FirstOrDefault(l => l.StartsWith("# "))?[2..].Trim() ?? baseId;

        var sections = new List<(string Heading, string Text)>();
        var heading = "Introduction";
        var text = new StringBuilder();

        foreach (var line in lines)
        {
            if (line.StartsWith("# ") || IsNote(line))
                continue;

            if (line.StartsWith("## "))
            {
                AddSection();
                heading = line[3..].Trim();
                continue;
            }

            text.Append(line).Append('\n');
        }
        AddSection();

        var chunks = new List<Chunk>();
        foreach (var (sectionHeading, sectionText) in sections)
            foreach (var piece in Pieces(sectionText))
                chunks.Add(new Chunk($"{baseId}-{chunks.Count + 1}", source, title, sectionHeading, piece));
        return chunks;

        void AddSection()
        {
            var body = text.ToString().Trim();
            if (body.Length > 0)
                sections.Add((heading, body));
            text.Clear();
        }
    }

    // The italic line under each title ("*... fictional company ...*") answers no question, so it isn't indexed.
    private static bool IsNote(string line) =>
        line.StartsWith('*') && line.EndsWith('*') && !line.StartsWith("**");

    // A section longer than MaxChars is cut between paragraphs, never in the middle of a sentence.
    private static IEnumerable<string> Pieces(string text)
    {
        if (text.Length <= MaxChars)
        {
            yield return text;
            yield break;
        }

        var current = new StringBuilder();
        foreach (var paragraph in text.Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (current.Length > 0 && current.Length + paragraph.Length > MaxChars)
            {
                yield return current.ToString().Trim();
                current.Clear();
            }
            current.Append(paragraph).Append("\n\n");
        }
        if (current.Length > 0)
            yield return current.ToString().Trim();
    }
}