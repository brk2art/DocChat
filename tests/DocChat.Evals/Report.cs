using System.Text;
using DocChat.Core;

namespace DocChat.Evals;

// All runs of one case. PASS: every run passed; FAIL: none did; FLAKY: some did.
public sealed record CaseRuns(GoldenCase Case, List<CaseResult> Runs)
{
    public int Passes => Runs.Count(r => r.Passed);

    public string Verdict => Passes == Runs.Count ? "PASS" : Passes == 0 ? "FAIL" : "FLAKY";

    public long MedianMs
    {
        get
        {
            var ms = Runs.Where(r => r.Response is not null).Select(r => r.Response!.DurationMs).Order().ToList();
            return ms.Count == 0 ? 0 : ms[ms.Count / 2];
        }
    }
}

public static class Report
{
    // gpt-5.4-mini, Data Zone Standard, USD per 1 million tokens (Azure price list, October 2026).
    // Embedding costs are too small to matter here.
    private const double InputPrice = 0.83;
    private const double OutputPrice = 4.95;

    public static (string Summary, string Markdown) Build(string label, SearchMode mode, int repeat, List<CaseRuns> results)
    {
        static string Rate(int ok, int all) => all == 0 ? "-" : $"{ok}/{all} ({100.0 * ok / all:0}%)";

        var runs = results.SelectMany(r => r.Runs).ToList();
        var answered = runs.Where(r => r.Response is not null).Select(r => r.Response!).ToList();

        var ms = answered.Select(a => a.DurationMs).Order().ToList();
        long Percentile(double p) => ms.Count == 0 ? 0 : ms[Math.Max(0, (int)Math.Ceiling(p * ms.Count) - 1)];

        var avgIn = answered.Count == 0 ? 0 : answered.Average(a => a.InputTokens ?? 0);
        var avgOut = answered.Count == 0 ? 0 : answered.Average(a => a.OutputTokens ?? 0);
        var costPerQuestion = (avgIn * InputPrice + avgOut * OutputPrice) / 1_000_000;
        var runCost = answered.Sum(a => ((a.InputTokens ?? 0) * InputPrice + (a.OutputTokens ?? 0) * OutputPrice) / 1_000_000);

        var s = new StringBuilder();
        s.AppendLine($"DocChat eval: {label}, {mode} search, each case {repeat}x");
        s.AppendLine($"Passed: {Rate(runs.Count(r => r.Passed), runs.Count)} of all runs");
        s.AppendLine($"Cases: {results.Count(r => r.Verdict == "PASS")} pass, " +
                     $"{results.Count(r => r.Verdict == "FLAKY")} flaky, {results.Count(r => r.Verdict == "FAIL")} fail");
        s.AppendLine($"Search found the right documents: {Rate(runs.Count(r => r.RetrievalOk), runs.Count)}   " +
                     $"Cited them: {Rate(runs.Count(r => r.CitationOk), runs.Count)}   " +
                     $"Answer correct: {Rate(runs.Count(r => r.AnswerOk), runs.Count)}");
        s.AppendLine($"Time: P50 {Percentile(0.5)} ms, P95 {Percentile(0.95)} ms   Tokens per question: {avgIn:0} in, {avgOut:0} out");
        s.AppendLine($"Cost: ${costPerQuestion:0.00000} per question, ${runCost:0.0000} for this run");
        s.AppendLine("By type:");
        foreach (var group in results.GroupBy(r => r.Case.Type))
        {
            var groupRuns = group.SelectMany(r => r.Runs).ToList();
            s.AppendLine($"  {group.Key,-10} {Rate(groupRuns.Count(r => r.Passed), groupRuns.Count)}");
        }

        var md = new StringBuilder();
        md.AppendLine($"# DocChat evaluation: {label}");
        md.AppendLine();
        md.AppendLine($"Run {DateTime.Now:yyyy-MM-dd HH:mm}, {results.Count} cases x {repeat}, {mode} search.");
        md.AppendLine();
        md.AppendLine("```");
        md.Append(s);
        md.AppendLine("```");
        md.AppendLine();
        md.AppendLine("| Case | Type | Verdict | Passed | Median ms | Answer (last run) | Problems |");
        md.AppendLine("|---|---|---|---|---|---|---|");
        foreach (var r in results)
        {
            var answer = (r.Runs[^1].Response?.Answer ?? "").Replace("|", "\\|").Replace("\n", " ");
            var problems = string.Join("; ", r.Runs.SelectMany(x => x.Problems).Distinct()).Replace("|", "\\|");
            md.AppendLine($"| {r.Case.Id} | {r.Case.Type} | {r.Verdict} | {r.Passes}/{r.Runs.Count} | {r.MedianMs} | {answer} | {problems} |");
        }

        md.AppendLine();
        md.AppendLine("## Failing answers");
        foreach (var r in results)
        {
            foreach (var (run, i) in r.Runs.Select((run, i) => (run, i + 1)).Where(x => !x.run.Passed))
            {
                md.AppendLine();
                md.AppendLine($"**{r.Case.Id}, run {i}:** {string.Join("; ", run.Problems)}");
                md.AppendLine();
                md.AppendLine($"> {(run.Response?.Answer ?? "(no answer)").Replace("\n", " ")}");
            }
        }

        return (s.ToString(), md.ToString());
    }
}