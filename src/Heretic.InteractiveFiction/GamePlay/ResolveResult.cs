using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

internal abstract record ResolveResult<T> where T : AHereticObject
{
    internal sealed record Found(T Object) : ResolveResult<T>;
    internal sealed record NotFound : ResolveResult<T>;
    internal sealed record Ambiguous(IReadOnlyList<T> Candidates) : ResolveResult<T>;
}
