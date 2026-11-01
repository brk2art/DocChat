using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.AI;

namespace DocChat.Api;

// Finds the chunks most likely to answer a question: keyword and vector search together (hybrid).
public sealed class Retriever(SearchClient search, IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    private static readonly string[] Fields = ["id", "source", "title", "section", "content"];

    // The question must be embedded with the same model as the chunks, or the vectors aren't comparable.
    public async Task<List<Passage>> SearchAsync(string question, int top, CancellationToken ct)
    {
        // The question must be embedded with the same model as the chunks, or the vectors aren't comparable.
        var embeddings = await embedder.GenerateAsync([question], cancellationToken: ct);

        var options = new SearchOptions
        {
            Size = top,
            VectorSearch = new VectorSearchOptions
            {
                Queries =
                {
                    new VectorizedQuery(embeddings[0].Vector) { KNearestNeighborsCount = top, Fields = { "contentVector" } } 
                }
            }
        };
        foreach (var field in Fields)
            options.Select.Add(field); // never send the 1536 numbers back: nobody reads them

        var response = await search.SearchAsync<Passage>(question, options, ct);

        var passages = new List<Passage>();
        await foreach (var result in response.Value.GetResultsAsync())
            passages.Add(result.Document);
        return passages;
    }
}
