using Heretic.InteractiveFiction.Grammars;

namespace Heretic.InteractiveFiction.Objects;

public sealed class AdventureEvent
{
    public Verb? Predicate { get; set; }
    public AHereticObject? ObjectOne => AllObjects.FirstOrDefault();
    public AHereticObject? ObjectTwo => AllObjects.Skip(1).FirstOrDefault();
    
    public List<AHereticObject> AllObjects { get; init; } = new();
    public List<string> UnidentifiedSentenceParts { get; init; } = new();
}