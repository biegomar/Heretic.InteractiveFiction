using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Grammars;

public class GermanVerbHandler : IVerbHandler
{
    private readonly Universe universe;
    
    public IList<Verb> Verbs { get; }
    
    public GermanVerbHandler(Universe universe, IResourceProvider resourceProvider)
    {
        this.universe = universe;
        this.Verbs = resourceProvider.GetVerbsFromResources();
    }
    
    public List<Verb> ExtractPossibleVerbs(string word)
    {
        var verbList = this.universe.ActiveLocation.OptionalVerbs.SelectMany(x => x.Value).ToList();
        
        return ExtractPossibleVerbsFromList(word, this.Verbs.ToList()).Union(ExtractPossibleVerbsFromList(word, verbList)).ToList();
    }

    private static List<Verb> ExtractPossibleVerbsFromList(string word, List<Verb> verbList)
    {
        var optionalVerbs = verbList
            .Where(v => v.Variants.Select(v => v.Name).Contains(word, StringComparer.InvariantCultureIgnoreCase)).Select(
                v => new Verb()
                {
                    Key = v.Key,
                    ErrorMessage = v.ErrorMessage,
                    Prepositions = v.Prepositions,
                    Variants = v.Variants.Where(vi => vi.Name.Equals(word, StringComparison.InvariantCultureIgnoreCase))
                        .ToList()
                }).ToList();
        return optionalVerbs;
    }
}