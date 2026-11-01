using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.AI;

namespace DocChat.Core;

// Finds the chunks most likely to answer a question.
public sealed class Retriever(SearchClient search, IEmbeddingGenerator<string, Embedding<float>> embedder)
{
    private static readonly string[] Fields = ["id", "source", "title", "section", "content"];

    public async Task<List<Passage>> SearchAsync(string question, int top, SearchMode mode, CancellationToken ct)
    {
        var options = new SearchOptions { Size = top };
        foreach (var field in Fields)
            options.Select.Add(field); // never send the 1536 numbers back: nobody reads them

        // Vector part: the question must be embedded with the same model as the chunks.
        if (mode != SearchMode.Keyword)
        {
            var embeddings = await embedder.GenerateAsync([question], cancellationToken: ct);
            options.VectorSearch = new VectorSearchOptions
            {
                Queries =
                {
                    new VectorizedQuery(embeddings[0].Vector) { KNearestNeighborsCount = top, Fields = { "contentVector" } },
                },
            };
        }

        // Keyword part: the question text. Without it (null), the search is vectors only.
        var searchText = mode == SearchMode.Vector ? null : question;
        var response = await search.SearchAsync<Passage>(searchText, options, ct);

        var passages = new List<Passage>();
        await foreach (var result in response.Value.GetResultsAsync())
            passages.Add(result.Document);
        return passages;
    }
}