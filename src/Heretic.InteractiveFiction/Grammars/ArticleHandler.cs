using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Grammars;

public static class ArticleHandler
{
    private const string NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR = "";
    private const string ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR = "";
    private const string DATIV_INDEFINITEARTICLE_NEUTRUM_SINGULAR = "";
    
    public static bool HasArticle(IEnumerable<string> sentence)
    {
        var allArticles = new List<string>
        {
            BaseGrammar.NOMINATIVE_ARTICLE_PLURAL,
            BaseGrammar.NOMINATIVE_ARTICLE_FEMALE_SINGULAR,
            BaseGrammar.NOMINATIVE_ARTICLE_MALE_SINGULAR,
            BaseGrammar.NOMINATIVE_ARTICLE_NEUTRUM_SINGULAR,
            BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
            BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
            BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
            BaseGrammar.GENITIVE_ARTICLE_PLURAL,
            BaseGrammar.GENITIVE_ARTICLE_MALE_SINGULAR,
            BaseGrammar.GENITIVE_ARTICLE_FEMALE_SINGULAR,
            BaseGrammar.GENITIVE_ARTICLE_NEUTRUM_SINGULAR,
            BaseGrammar.GENITIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
            BaseGrammar.GENITIVE_INDEFINITEARTICLE_MALE_SINGULAR,
            BaseGrammar.GENITIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
            BaseGrammar.DATIVE_ARTICLE_PLURAL,
            BaseGrammar.DATIVE_ARTICLE_MALE_SINGULAR,
            BaseGrammar.DATIVE_ARTICLE_FEMALE_SINGULAR,
            BaseGrammar.DATIVE_ARTICLE_NEUTRUM_SINGULAR,
            BaseGrammar.DATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
            BaseGrammar.DATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
            BaseGrammar.DATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
            BaseGrammar.ACCUSATIVE_ARTICLE_MALE_SINGULAR,
            BaseGrammar.ACCUSATIVE_ARTICLE_FEMALE_SINGULAR,
            BaseGrammar.ACCUSATIVE_ARTICLE_NEUTRUM_SINGULAR,
            BaseGrammar.ACCUSATIVE_ARTICLE_PLURAL,
            BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
            BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
            BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
            BaseGrammar.NOMINATIVE_ARTICLE_MALE_SINGULAR,
            BaseGrammar.NOMINATIVE_ARTICLE_FEMALE_SINGULAR,
            BaseGrammar.NOMINATIVE_ARTICLE_NEUTRUM_SINGULAR,
        };

        var distinctArticles = allArticles.Distinct();
        return sentence.Any(s => distinctArticles.Contains(s,StringComparer.InvariantCultureIgnoreCase)); 
    }
    
    public static string GetArticleForObject(AHereticObject processingObject, GrammarCase grammarCase, ArticleState articleState = ArticleState.Definite)
    {
        return articleState switch
        {
            ArticleState.Definite => GetDefiniteArticleForObject(processingObject, grammarCase),
            ArticleState.Indefinite => GetIndefiniteArticleForObject(processingObject, grammarCase),
            _ => throw new ArgumentOutOfRangeException(nameof(articleState), articleState, null)
        };
    }
    
    public static string GetNameWithArticleForObject(AHereticObject processingObject, GrammarCase grammarCase, ArticleState articleState = ArticleState.Definite)
    {
        var article= GetArticleForObject(processingObject, grammarCase, articleState);

        var name = GetObjectName(processingObject);
        
        if (!string.IsNullOrEmpty(processingObject.Adjectives))
        {
            return string.Format($"{article} {AdjectiveDeclinationHandler.GetAdjectiveDeclinationForObject(processingObject, grammarCase)} {name}").Trim();    
        }
        return string.Format($"{article} {name}").Trim();
    }

    private static string GetObjectName(AHereticObject processingObject)
    {
        var names = processingObject.GetNames();
        return names.Any() ? names.First().Trim() : string.Empty;
    }

    private static string GetDefiniteArticleForObject(AHereticObject processingObject, GrammarCase grammarCase)
    {
        return grammarCase switch
        {
            GrammarCase.Nominative => GetNominativeArticleForObject(processingObject),
            GrammarCase.Genitive => GetGenitiveArticleForObject(processingObject),
            GrammarCase.Dative => GetDativeArticleForObject(processingObject),
            GrammarCase.Accusative => GetAccusativeArticleForObject(processingObject),
            _ => throw new ArgumentOutOfRangeException(nameof(grammarCase), grammarCase, null)
        };
    }
    
    private static string GetIndefiniteArticleForObject(AHereticObject processingObject, GrammarCase grammarCase)
    {
        return grammarCase switch
        {
            GrammarCase.Nominative => GetNominativeIndefiniteArticleForObject(processingObject),
            GrammarCase.Genitive => GetGenitiveIndefiniteArticleForObject(processingObject),
            GrammarCase.Dative => GetDativeIndefiniteArticleForObject(processingObject),
            GrammarCase.Accusative => GetAccusativeIndefiniteArticleForObject(processingObject),
            _ => throw new ArgumentOutOfRangeException(nameof(grammarCase), grammarCase, null)
        };
    }
    
    private static string GetNominativeArticleForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.NOMINATIVE_ARTICLE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.NOMINATIVE_ARTICLE_MALE_SINGULAR,
                Genders.Neutrum => BaseGrammar.NOMINATIVE_ARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.NOMINATIVE_ARTICLE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.NOMINATIVE_ARTICLE_NEUTRUM_SINGULAR
            };

            return result;
        }

        return BaseGrammar.NOMINATIVE_ARTICLE_PLURAL;
    }

    private static string GetNominativeIndefiniteArticleForObject(AHereticObject processingObject)
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
    
    private static string GetGenitiveArticleForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.GENITIVE_ARTICLE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.GENITIVE_ARTICLE_MALE_SINGULAR,
                Genders.Neutrum => BaseGrammar.GENITIVE_ARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.GENITIVE_ARTICLE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.GENITIVE_ARTICLE_NEUTRUM_SINGULAR
            };

            return result;
        }

        return BaseGrammar.GENITIVE_ARTICLE_PLURAL;
    }
    
    private static string GetGenitiveIndefiniteArticleForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.GENITIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.GENITIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => BaseGrammar.GENITIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.GENITIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.GENITIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR
            };

            return result;
        }

        return string.Empty;
    }

    private static string GetDativeIndefiniteArticleForObject(AHereticObject processingObject)
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

    private static string GetDativeArticleForObject(AHereticObject processingObject)
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
        
        return BaseGrammar.DATIVE_ARTICLE_PLURAL;
    }

    private static string GetAccusativeIndefiniteArticleForObject(AHereticObject processingObject)
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

    private static string GetAccusativeArticleForObject(AHereticObject processingObject)
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

        return BaseGrammar.ACCUSATIVE_ARTICLE_PLURAL;
    }
}