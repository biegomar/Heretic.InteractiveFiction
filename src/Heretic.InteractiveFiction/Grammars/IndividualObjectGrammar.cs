using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Grammars;

public sealed class IndividualObjectGrammar
{
    public Genders Gender { get; set; }
    public bool IsSingular { get; set; }
    public bool IsPlayer { get; set; }
    public bool IsAbstract { get; set; }

    public IndividualObjectGrammar(Genders gender = Genders.Female, bool isSingular = true, bool isPlayer = false, bool isAbstract = false)
    {
        Initialize(gender, isSingular, isPlayer, isAbstract);
    }

    private void Initialize(Genders gender, bool isSingular, bool isPlayer, bool isAbstract)
    {
        this.IsSingular = isSingular;
        this.IsPlayer = isPlayer;
        this.Gender = gender;
        this.IsAbstract = isAbstract;
    }

    public string GetNominativeAdjectiveDeclination()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
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
    
    public string GetGenitiveAdjectiveDeclination()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
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
    
    public string GetDativeAdjectiveDeclination()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
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
    
    public string GetAccusativeAdjectiveDeclination()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
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