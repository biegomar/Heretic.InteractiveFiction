using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Grammars;

public sealed class Verb
{
    public VerbKey Key { get; init; }
    public IEnumerable<VerbVariant> Variants { get; set; } = new List<VerbVariant>();
    public IEnumerable<PrepositionVariant> Prepositions { get; init; } = new List<PrepositionVariant>();
    public Description ErrorMessage { get; set; } = string.Empty;
}