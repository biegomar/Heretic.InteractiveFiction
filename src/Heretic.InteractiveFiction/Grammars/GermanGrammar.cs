﻿using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Objects;

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