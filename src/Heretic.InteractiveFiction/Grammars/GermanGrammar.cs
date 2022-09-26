using System.Globalization;
using System.Resources;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Grammars;

public class GermanGrammar: IGrammar
{
    private const string NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR = "";
    private const string ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR = "";
    private const string DATIV_INDEFINITEARTICLE_NEUTRUM_SINGULAR = "";

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
    
    public bool IsPronounActiveObject(AHereticObject activeObject, string pronoun)
    {
        var upperItemName = pronoun.ToUpperInvariant();
        
        if (activeObject != null && (upperItemName == activeObject.Grammar.GetAccusativePronoun().ToUpperInvariant() 
                                     || upperItemName == activeObject.Grammar.GetDativePronoun().ToUpperInvariant()  
                                     || upperItemName == activeObject.Grammar.GetNominativePronoun().ToUpperInvariant()))
        {
            return true;
        }

        return false;
    }

    public string GetArticleForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.ARTICLE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.ARTICLE_MALE_SINGULAR,
                Genders.Neutrum => BaseGrammar.ARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.ARTICLE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.ARTICLE_NEUTRUM_SINGULAR
            };

            return result;
        }
        
        return string.Empty;
    }

    public string GetNominativeIndefiniteArticleForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => processingObject.Grammar.IsAbstract ? NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR : BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR
            };
        
            return result;
        }
        
        return string.Empty;
    }

    public string GetDativeIndefiniteArticleForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.DATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.DATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => processingObject.Grammar.IsAbstract ? DATIV_INDEFINITEARTICLE_NEUTRUM_SINGULAR : BaseGrammar.DATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.DATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.DATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR
            };
        
            return result;
        }
        
        return string.Empty;
    }

    public string GetDativeArticleForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.DATIVE_ARTICLE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.DATIVE_ARTICLE_MALE_SINGULAR,
                Genders.Neutrum => BaseGrammar.DATIVE_ARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.DATIVE_ARTICLE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.DATIVE_ARTICLE_NEUTRUM_SINGULAR
            };
        
            return result;
        }
        else
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.DATIVE_ARTICLE_FEMALE_PLURAL,
                Genders.Male => BaseGrammar.DATIVE_ARTICLE_MALE_PLURAL,
                Genders.Neutrum => BaseGrammar.DATIVE_ARTICLE_NEUTRUM_PLURAL,
                Genders.Unknown => BaseGrammar.DATIVE_ARTICLE_NEUTRUM_PLURAL,
                _ => BaseGrammar.DATIVE_ARTICLE_NEUTRUM_PLURAL
            };
        
            return result;
        }
    }

    public string GetAccusativeIndefiniteArticleForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => Resources.BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => Resources.BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => processingObject.Grammar.IsAbstract ? ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR : Resources.BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => Resources.BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                _ => Resources.BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR
            };

            return result;
        }

        return string.Empty;
    }

    public string GetAccusativeArticleForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.ACCUSATIVE_ARTICLE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.ACCUSATIVE_ARTICLE_MALE_SINGULAR,
                Genders.Neutrum => BaseGrammar.ACCUSATIVE_ARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.ACCUSATIVE_ARTICLE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.ACCUSATIVE_ARTICLE_NEUTRUM_SINGULAR
            };
        
            return result;
        }
        else
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.ACCUSATIVE_ARTICLE_FEMALE_PLURAL,
                Genders.Male => BaseGrammar.ACCUSATIVE_ARTICLE_MALE_PLURAL,
                Genders.Neutrum => BaseGrammar.ACCUSATIVE_ARTICLE_NEUTRUM_PLURAL,
                Genders.Unknown => BaseGrammar.ACCUSATIVE_ARTICLE_NEUTRUM_PLURAL,
                _ => BaseGrammar.ACCUSATIVE_ARTICLE_NEUTRUM_PLURAL
            };
        
            return result;
        }
    }

    public string GetNominativePronounForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.NOMINATIVE_PRONOUN_SHE,
                Genders.Male => BaseGrammar.NOMINATIVE_PRONOUN_HE,
                Genders.Neutrum => BaseGrammar.NOMINATIVE_PRONOUN_IT,
                Genders.Unknown => BaseGrammar.NOMINATIVE_PRONOUN_SHE,
                _ => BaseGrammar.NOMINATIVE_PRONOUN_SHE
            };

            return processingObject.Grammar.IsPlayer ? BaseGrammar.NOMINATIVE_PRONOUN_YOU : result;
        }
        
        return BaseGrammar.NOMINATIVE_PRONOUN_THEY;
    }

    public string GetDativePronounForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.DATIVE_PRONOUN_SHE,
                Genders.Male => BaseGrammar.DATIVE_PRONOUN_HE,
                Genders.Neutrum => BaseGrammar.DATIVE_PRONOUN_IT,
                Genders.Unknown => BaseGrammar.DATIVE_PRONOUN_SHE,
                _ => BaseGrammar.DATIVE_PRONOUN_SHE
            };

            return processingObject.Grammar.IsPlayer ? BaseGrammar.DATIVE_PRONOUN_YOU : result;
        }
        
        return BaseGrammar.DATIVE_PRONOUN_THEY;
    }

    public string GetAccusativePronounForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.ACCUSATIVE_PRONOUN_SHE,
                Genders.Male => BaseGrammar.ACCUSATIVE_PRONOUN_HE,
                Genders.Neutrum => BaseGrammar.ACCUSATIVE_PRONOUN_IT,
                Genders.Unknown => BaseGrammar.ACCUSATIVE_PRONOUN_SHE,
                _ => BaseGrammar.ACCUSATIVE_PRONOUN_SHE
            };

            return processingObject.Grammar.IsPlayer ? BaseGrammar.ACCUSATIVE_PRONOUN_YOU : result;
        }
        
        return BaseGrammar.ACCUSATIVE_PRONOUN_THEY;
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