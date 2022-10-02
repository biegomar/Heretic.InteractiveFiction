using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Grammars;

public static class PronounHandler
{
    public static bool IsPronounRepresentingActiveObject(AHereticObject activeObject, string pronoun)
    {
        var upperItemName = pronoun.ToUpperInvariant();
        
        if (activeObject != null && (upperItemName == GetNominativePronounForObject(activeObject).ToUpperInvariant() 
                                     || upperItemName == GetGenitivePronounForObject(activeObject).ToUpperInvariant() 
                                     || upperItemName == GetDativePronounForObject(activeObject).ToUpperInvariant() 
                                     || upperItemName == GetAccusativePronounForObject(activeObject).ToUpperInvariant()))
        {
            return true;
        }

        return false;
    }
    
    public static string GetPronounForObject(AHereticObject processingObject, GrammarCase grammarCase)
    {
        return grammarCase switch
        {
            GrammarCase.Nominative => GetNominativePronounForObject(processingObject),
            GrammarCase.Genitive => GetGenitivePronounForObject(processingObject),
            GrammarCase.Dative => GetDativePronounForObject(processingObject),
            GrammarCase.Accusative => GetAccusativePronounForObject(processingObject),
            _ => throw new ArgumentOutOfRangeException(nameof(grammarCase), grammarCase, null)
        };
    }
    
    private static string GetNominativePronounForObject(AHereticObject processingObject)
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

    private static string GetGenitivePronounForObject(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.GENITIVE_PRONOUN_SHE,
                Genders.Male => BaseGrammar.GENITIVE_PRONOUN_HE,
                Genders.Neutrum => BaseGrammar.GENITIVE_PRONOUN_IT,
                Genders.Unknown => BaseGrammar.GENITIVE_PRONOUN_SHE,
                _ => BaseGrammar.GENITIVE_PRONOUN_SHE
            };

            return processingObject.Grammar.IsPlayer ? BaseGrammar.GENITIVE_PRONOUN_YOU : result;
        }
        
        return BaseGrammar.GENITIVE_PRONOUN_THEY;
    }
    
    private static string GetDativePronounForObject(AHereticObject processingObject)
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

    private static string GetAccusativePronounForObject(AHereticObject processingObject)
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
}