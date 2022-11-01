using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Grammars;

public sealed class Verb
{
    public string Key { get; set; }
    public IEnumerable<VerbVariant> Variants { get; set; }
    public IEnumerable<PrepositionVariant> Prepositions { get; set; }
    public Description ErrorMessage { get; set; }
}