using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

internal abstract record AnalysisResult
{
    internal sealed record Success(AdventureEvent Event) : AnalysisResult;
    internal sealed record Ambiguous(IReadOnlyList<AHereticObject> Candidates) : AnalysisResult;
}
