using DocChat.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocChat.Evals;

public sealed record GoldenCase(
    string Id,
    string Type,
    string Question,
    string[]? ExpectedSources,
    string[]? MustContain,
    string[]? MustNotContain,
    bool ExpectRefusal);

public sealed record CaseResult(AskResponse? Response, bool RetrievalOk, bool CitationOk, bool AnswerOk, List<string> Problems)
{
    public bool Passed => RetrievalOk && CitationOk && AnswerOk;
}

public static class Scoring
{
    private static readonly string[] RefusalPhrases = ["can't find", "cannot find", "can not find"];

    public static CaseResult Score(GoldenCase c, AskResponse? response, Exception? error)
    {
        if (response is null)
            return new CaseResult(null, false, false, false, [$"error: {error?.GetType().Name}: {error?.Message}"]);

        var problems = new List<string>();

        // Models write typographic apostrophes (can’t); the checks use plain ones (can't).
        var answer = response.Answer.Replace('’', '\'');
        var retrieved = response.Sources.Select(s => s.Document).ToHashSet();
        var cited = response.Sources.Where(s => s.Cited).Select(s => s.Document).ToHashSet();

        bool retrievalOk, citationOk;
        if (c.ExpectRefusal)
        {
            // Nothing in the documents answers this, so there is nothing to find, and nothing may be cited.
            retrievalOk = true;
            citationOk = cited.Count == 0;
            if (!citationOk)
                problems.Add($"cited {string.Join(", ", cited)} although nothing answers this");
        }
        else
        {
            var expected = c.ExpectedSources ?? [];

            var missed = expected.Where(e => !retrieved.Contains(e)).ToList();
            retrievalOk = missed.Count == 0;
            if (!retrievalOk)
                problems.Add($"search missed {string.Join(", ", missed)}");

            var uncited = expected.Where(e => !cited.Contains(e)).ToList();
            citationOk = uncited.Count == 0;
            if (!citationOk)
                problems.Add($"didn't cite {string.Join(", ", uncited)}");
        }

        var answerOk = true;
        if (c.ExpectRefusal && !RefusalPhrases.Any(p => answer.Contains(p, StringComparison.OrdinalIgnoreCase)))
        {
            answerOk = false;
            problems.Add("answered instead of saying it can't find it");
        }

        foreach (var must in c.MustContain ?? [])
        {
            if (!must.Split('|').Any(a => answer.Contains(a, StringComparison.OrdinalIgnoreCase)))
            {
                answerOk = false;
                problems.Add($"answer lacks \"{must}\"");
            }
        }

        foreach (var mustNot in c.MustNotContain ?? [])
        {
            if (mustNot.Split('|').Any(a => answer.Contains(a, StringComparison.OrdinalIgnoreCase)))
            {
                answerOk = false;
                problems.Add($"answer contains \"{mustNot}\"");
            }
        }

        return new CaseResult(response, retrievalOk, citationOk, answerOk, problems);
    }
}
