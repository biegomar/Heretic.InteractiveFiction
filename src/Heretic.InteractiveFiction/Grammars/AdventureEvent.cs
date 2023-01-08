using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Grammars;

public sealed class AdventureEvent
{
    public Verb Predicate { get; set; }
    public AHereticObject ObjectOne => AllObjects.FirstOrDefault();
    public AHereticObject ObjectTwo => AllObjects.Skip(1).FirstOrDefault();
    
    public List<AHereticObject> AllObjects { get; }
    public List<string> UnidentifiedSentenceParts { get; }

    public AdventureEvent()
    {
        this.UnidentifiedSentenceParts = new List<string>();
        this.AllObjects = new List<AHereticObject>();
    }
}