using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public sealed class Grammars
{
    private const string NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR = "";
    private const string ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR = "";
    private const string DATIV_INDEFINITEARTICLE_NEUTRUM_SINGULAR = "";
    
    public Genders Gender { get; set; }
    public bool IsSingular { get; set; }
    public bool IsPlayer { get; set; }
    public bool IsAbstract { get; set; }

    public Grammars(Genders gender = Genders.Female, bool isSingular = true, bool isPlayer = false, bool isAbstract = false)
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

    public string GetNominativeArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
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
    
    public string GetNominativeIndefiniteArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => IsAbstract ? NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR : BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR
            };
        
            return result;
        }
        
        return string.Empty;
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

        return BaseGrammar.ADJECTIVE_DECLINATION_NOMINATIVE_PLURAL;
    }
    
    public string GetNominativePronoun()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => BaseGrammar.NOMINATIVE_PRONOUN_SHE,
                Genders.Male => BaseGrammar.NOMINATIVE_PRONOUN_HE,
                Genders.Neutrum => BaseGrammar.NOMINATIVE_PRONOUN_IT,
                Genders.Unknown => BaseGrammar.NOMINATIVE_PRONOUN_SHE,
                _ => BaseGrammar.NOMINATIVE_PRONOUN_SHE
            };

            return IsPlayer ? BaseGrammar.NOMINATIVE_PRONOUN_YOU : result;
        }
        
        return BaseGrammar.NOMINATIVE_PRONOUN_THEY;
    }
    
    public string GetDativePronoun()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => BaseGrammar.DATIVE_PRONOUN_SHE,
                Genders.Male => BaseGrammar.DATIVE_PRONOUN_HE,
                Genders.Neutrum => BaseGrammar.DATIVE_PRONOUN_IT,
                Genders.Unknown => BaseGrammar.DATIVE_PRONOUN_SHE,
                _ => BaseGrammar.DATIVE_PRONOUN_SHE
            };

            return IsPlayer ? BaseGrammar.DATIVE_PRONOUN_YOU : result;
        }
        
        return BaseGrammar.DATIVE_PRONOUN_THEY;
    }
    
    public string GetAccusativePronoun()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => BaseGrammar.ACCUSATIVE_PRONOUN_SHE,
                Genders.Male => BaseGrammar.ACCUSATIVE_PRONOUN_HE,
                Genders.Neutrum => BaseGrammar.ACCUSATIVE_PRONOUN_IT,
                Genders.Unknown => BaseGrammar.ACCUSATIVE_PRONOUN_SHE,
                _ => BaseGrammar.ACCUSATIVE_PRONOUN_SHE
            };

            return IsPlayer ? BaseGrammar.ACCUSATIVE_PRONOUN_YOU : result;
        }
        
        return BaseGrammar.ACCUSATIVE_PRONOUN_THEY;
    }
    
    public string GetDativeIndefiniteArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => BaseGrammar.DATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => BaseGrammar.DATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => IsAbstract ? DATIV_INDEFINITEARTICLE_NEUTRUM_SINGULAR : BaseGrammar.DATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => BaseGrammar.DATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                _ => BaseGrammar.DATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR
            };
        
            return result;
        }
        
        return string.Empty;
    }
    
    public string GetDativeArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
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
    
    public string GetAccusativeIndefiniteArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => Resources.BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => Resources.BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => IsAbstract ? ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR : Resources.BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => Resources.BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                _ => Resources.BaseGrammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR
            };

            return result;
        }
        
        return string.Empty;
    }
    
    public string GetAccusativeArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
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