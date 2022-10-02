using System.Globalization;
using System.Resources;
using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Grammars;

public class GermanGrammar: IGrammar
{
    private IDictionary<string, (string, string)> CombinedPrepositionsAndArticles { get; }

    public IList<Verb> Verbs { get; }
    
    public IDictionary<string, IEnumerable<string>> Prepositions { get; }

    public GermanGrammar(IResourceProvider resourceProvider)
    {
        this.Verbs = resourceProvider.GetVerbsFromResources();
        this.Prepositions = resourceProvider.GetPrepositionsFromResources();
        this.CombinedPrepositionsAndArticles = resourceProvider.PreparePrepositionsAndArticlesFromResource();
    }

    public bool IsVerb(string verbToCheck, Location location)
    {
        var verbOverrides = location.OptionalVerbs.Values.SelectMany(x => x);
        
        return verbOverrides.Any()
            ? this.Verbs.Select(v => v.Key).Contains(verbToCheck, StringComparer.InvariantCultureIgnoreCase)
              || verbOverrides.Select(v => v.Key).Contains(verbToCheck, StringComparer.InvariantCultureIgnoreCase)
            : this.Verbs.Select(v => v.Key).Contains(verbToCheck, StringComparer.InvariantCultureIgnoreCase);
    }
    
    public Verb GetVerb(string verbToCheck, Location location)
    {
        var result = this.Verbs.FirstOrDefault(v => v.Key.Equals(verbToCheck, StringComparison.InvariantCultureIgnoreCase));
        if (result != default)
        {
            return result;
        }
        
        var verbOverrides = location.OptionalVerbs.SelectMany(x => x.Value).ToList();
        return verbOverrides.SingleOrDefault(v => v.Key.Equals(verbToCheck, StringComparison.InvariantCultureIgnoreCase));
    }

    public (string preposition, string article) GetPrepositionAndArticleFromCombinedWord(string word)
    {
        return this.CombinedPrepositionsAndArticles.ContainsKey(word) ? this.CombinedPrepositionsAndArticles[word] : default;
    }

    public bool HasPrepositionOrPrefix(IEnumerable<string> sentence)
    {
        var allPrefixes = this.Verbs.SelectMany(v => v.Variants).Where(p => !string.IsNullOrEmpty(p.Prefix)).Select(p => p.Prefix).Distinct();
        var allPrepositions = this.Prepositions.Values.SelectMany(x => x);
        
        return sentence.Intersect(allPrefixes.Union(allPrepositions)).Any();
    }
}