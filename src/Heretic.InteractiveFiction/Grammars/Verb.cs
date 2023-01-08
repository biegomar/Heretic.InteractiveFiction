using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Grammars;

public sealed class Verb
{
    public string Key { get; set; }
    public IEnumerable<VerbVariant> Variants { get; set; } = new List<VerbVariant>();
    public IEnumerable<PrepositionVariant> Prepositions { get; set; } = new List<PrepositionVariant>();
    public Description ErrorMessage { get; set; }
}