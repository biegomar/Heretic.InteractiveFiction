using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Grammars;

public static class AdjectiveDeclinationHandler
{
    public static string GetAdjectiveDeclinationForObject(AHereticObject processingObject, GrammarCase grammarCase)
    {
        return grammarCase switch
        {
            GrammarCase.Nominative => GetNominativeAdjectiveDeclination(processingObject),
            GrammarCase.Genitive => GetGenitiveAdjectiveDeclination(processingObject),
            GrammarCase.Dative => GetDativeAdjectiveDeclination(processingObject),
            GrammarCase.Accusative => GetAccusativeAdjectiveDeclination(processingObject),
            _ => throw new ArgumentOutOfRangeException(nameof(grammarCase), grammarCase, null)
        };
    }

    public static IEnumerable<string> GetAllDeclinedAdjectivesForObject(AHereticObject processingObject,
        GrammarCase grammarCase)
    {
        var declination = GetAdjectiveDeclinationForObject(processingObject, grammarCase);
        
        var splitList = processingObject.Adjectives.Split('|').ToList();
        var adjectiveList = splitList.Select(item => item + declination).ToList();

        return adjectiveList;
    }
    
    private static string GetNominativeAdjectiveDeclination(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.ADJECTIVE_DECLINATION_NOMINATIVE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.ADJECTIVE_DECLINATION_NOMINATIVE_MALE_SINGULAR,
                Genders.Neutrum => BaseGrammar.ADJECTIVE_DECLINATION_NOMINATIVE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.ADJECTIVE_DECLINATION_NOMINATIVE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.ADJECTIVE_DECLINATION_NOMINATIVE_NEUTRUM_SINGULAR
            };

            return result;
        }

        return BaseGrammar.ADJECTIVE_DECLINATION_PLURAL;
    }
    
    private static string GetGenitiveAdjectiveDeclination(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.ADJECTIVE_DECLINATION_GENITIVE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.ADJECTIVE_DECLINATION_GENITIVE_MALE_SINGULAR,
                Genders.Neutrum => BaseGrammar.ADJECTIVE_DECLINATION_GENITIVE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.ADJECTIVE_DECLINATION_GENITIVE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.ADJECTIVE_DECLINATION_GENITIVE_NEUTRUM_SINGULAR
            };

            return result;
        }

        return BaseGrammar.ADJECTIVE_DECLINATION_PLURAL;
    }
    
    private static string GetDativeAdjectiveDeclination(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.ADJECTIVE_DECLINATION_DATIVE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.ADJECTIVE_DECLINATION_DATIVE_MALE_SINGULAR,
                Genders.Neutrum => BaseGrammar.ADJECTIVE_DECLINATION_DATIVE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.ADJECTIVE_DECLINATION_DATIVE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.ADJECTIVE_DECLINATION_DATIVE_NEUTRUM_SINGULAR
            };

            return result;
        }

        return BaseGrammar.ADJECTIVE_DECLINATION_PLURAL;
    }
    
    private static string GetAccusativeAdjectiveDeclination(AHereticObject processingObject)
    {
        if (processingObject.Grammar.IsSingular)
        {
            var result = processingObject.Grammar.Gender switch
            {
                Genders.Female => BaseGrammar.ADJECTIVE_DECLINATION_ACCUSATIVE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.ADJECTIVE_DECLINATION_ACCUSATIVE_MALE_SINGULAR,
                Genders.Neutrum => BaseGrammar.ADJECTIVE_DECLINATION_ACCUSATIVE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.ADJECTIVE_DECLINATION_ACCUSATIVE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.ADJECTIVE_DECLINATION_ACCUSATIVE_NEUTRUM_SINGULAR
            };

            return result;
        }

        return BaseGrammar.ADJECTIVE_DECLINATION_PLURAL;
    }
}