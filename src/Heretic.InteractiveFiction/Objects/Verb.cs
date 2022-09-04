namespace Heretic.InteractiveFiction.Objects;

public sealed class Verb
{
    public string Key { get; set; }
    public IEnumerable<VerbVariant> Variants { get; set; }
    public Description ErrorMessage { get; set; }
}