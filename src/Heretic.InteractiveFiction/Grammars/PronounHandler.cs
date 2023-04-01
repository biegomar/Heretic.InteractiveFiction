using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Grammars;

public static class PronounHandler
{
    public static bool IsPronounRepresentingActiveObject(AHereticObject? activeObject, string pronoun)
    {
        var upperItemName = pronoun.ToUpperInvariant();
        
        if (activeObject != null && (upperItemName == GetNominativePronounForObject(activeObject, PersonView.FirstPerson).ToUpperInvariant() 
                                     || upperItemName == GetNominativePronounForObject(activeObject, PersonView.SecondPerson).ToUpperInvariant() 
                                     || upperItemName == GetGenitivePronounForObject(activeObject, PersonView.FirstPerson).ToUpperInvariant()
                                     || upperItemName == GetGenitivePronounForObject(activeObject, PersonView.SecondPerson).ToUpperInvariant()
                                     || upperItemName == GetDativePronounForObject(activeObject, PersonView.FirstPerson).ToUpperInvariant()
                                     || upperItemName == GetDativePronounForObject(activeObject, PersonView.SecondPerson).ToUpperInvariant()
                                     || upperItemName == GetAccusativePronounForObject(activeObject, PersonView.FirstPerson).ToUpperInvariant()
                                     || upperItemName == GetAccusativePronounForObject(activeObject, PersonView.SecondPerson).ToUpperInvariant()))
        {
            return true;
        }

        return false;
    }
    
    public static string GetPronounForObject(AHereticObject processingObject, GrammarCase grammarCase, PersonView personView = PersonView.SecondPerson)
    {
        return grammarCase switch
        {
            GrammarCase.Nominative => GetNominativePronounForObject(processingObject, personView),
            GrammarCase.Genitive => GetGenitivePronounForObject(processingObject, personView),
            GrammarCase.Dative => GetDativePronounForObject(processingObject, personView),
            GrammarCase.Accusative => GetAccusativePronounForObject(processingObject, personView),
            _ => throw new ArgumentOutOfRangeException(nameof(grammarCase), grammarCase, null)
        };
    }
    
    private static string GetNominativePronounForObject(AHereticObject processingObject, PersonView personView)
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

            var personViewPronoun = BaseGrammar.NOMINATIVE_PRONOUN_YOU;
            if (personView == PersonView.FirstPerson)
            {
                personViewPronoun = BaseGrammar.NOMINATIVE_PRONOUN_ME;
            }
            return processingObject is Player ? personViewPronoun : result;
        }
        
        return BaseGrammar.NOMINATIVE_PRONOUN_THEY;
    }

    private static string GetGenitivePronounForObject(AHereticObject processingObject, PersonView personView)
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

            var personViewPronoun = BaseGrammar.GENITIVE_PRONOUN_YOU;
            if (personView == PersonView.FirstPerson)
            {
                personViewPronoun = BaseGrammar.GENITIVE_PRONOUN_ME;
            }
            return processingObject is Player ? personViewPronoun : result;
        }
        
        return BaseGrammar.GENITIVE_PRONOUN_THEY;
    }
    
    private static string GetDativePronounForObject(AHereticObject processingObject, PersonView personView)
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

            var personViewPronoun = BaseGrammar.DATIVE_PRONOUN_YOU;
            if (personView == PersonView.FirstPerson)
            {
                personViewPronoun = BaseGrammar.DATIVE_PRONOUN_ME;
            }
            return processingObject is Player ? personViewPronoun : result;
        }
        
        return BaseGrammar.DATIVE_PRONOUN_THEY;
    }

    private static string GetAccusativePronounForObject(AHereticObject processingObject, PersonView personView)
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

            var personViewPronoun = BaseGrammar.ACCUSATIVE_PRONOUN_YOU;
            if (personView == PersonView.FirstPerson)
            {
                personViewPronoun = BaseGrammar.ACCUSATIVE_PRONOUN_ME;
            }
            return processingObject is Player ? personViewPronoun : result;
        }
        
        return BaseGrammar.ACCUSATIVE_PRONOUN_THEY;
    }
}