using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Grammars;

public sealed class Request
{
    public Verb Predicate { get; set; }
    public AHereticObject ObjectOne { get; set; }
    public AHereticObject ObjectTwo { get; set; }
    public List<string> UnidentifiedSentenceParts { get; set; }

    public Request()
    {
        this.UnidentifiedSentenceParts = new List<string>();
    }
}