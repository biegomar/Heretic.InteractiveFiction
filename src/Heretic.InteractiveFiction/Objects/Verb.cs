namespace Heretic.InteractiveFiction.Objects;

public sealed class Verb
{
    public string Key { get; set; }
    public string PrimaryName { get; set; }
    public IEnumerable<string> Names { get; set; }
    public Description ErrorMessage { get; set; }
}