namespace Heretic.InteractiveFiction.Grammars;

public interface IVerbHandler
{
    public IList<Verb> Verbs { get; }

    public List<Verb> ExtractPossibleVerbs(string word);
}