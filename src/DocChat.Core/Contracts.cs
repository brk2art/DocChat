using System.Text.Json.Serialization;

namespace DocChat.Core;

// Hybrid = keywords and vectors together (the default). Keyword and Vector exist to measure each one alone.
public enum SearchMode { Hybrid, Keyword, Vector }

// Label is what the answer cites ("S2"); Cited says whether the answer actually used it.
public sealed record SourceRef(string Label, string Document, string Section, bool Cited);

public sealed record AskResponse(
    string Answer,
    List<SourceRef> Sources,
    long? InputTokens,
    long? OutputTokens,
    long DurationMs);

// One chunk as stored in the index. The JSON names match the index fields the indexer created.
public sealed record Passage(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("section")] string Section,
    [property: JsonPropertyName("content")] string Content);