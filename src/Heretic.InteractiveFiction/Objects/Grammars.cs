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

    public string GetArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => Resources.Grammar.ARTICLE_FEMALE_SINGULAR,
                Genders.Male => Resources.Grammar.ARTICLE_MALE_SINGULAR,
                Genders.Neutrum => Resources.Grammar.ARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => Resources.Grammar.ARTICLE_NEUTRUM_SINGULAR,
                _ => Resources.Grammar.ARTICLE_NEUTRUM_SINGULAR
            };

            return result;
        }
        
        return string.Empty;
    }
    
    public string GetNominativeIndefiniteArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => Grammar.NOMINATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => Grammar.NOMINATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => IsAbstract ? NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR : Grammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => Grammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                _ => Grammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR
            };
        
            return result;
        }
        
        return string.Empty;
    }
    
    public string GetNominativePronoun()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => Grammar.NOMINATIVE_PRONOUN_SHE,
                Genders.Male => Grammar.NOMINATIVE_PRONOUN_HE,
                Genders.Neutrum => Grammar.NOMINATIVE_PRONOUN_IT,
                Genders.Unknown => Grammar.NOMINATIVE_PRONOUN_SHE,
                _ => Grammar.NOMINATIVE_PRONOUN_SHE
            };

            return IsPlayer ? Grammar.NOMINATIVE_PRONOUN_YOU : result;
        }
        
        return Grammar.NOMINATIVE_PRONOUN_THEY;
    }
    
    public string GetDativePronoun()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => Grammar.DATIVE_PRONOUN_SHE,
                Genders.Male => Grammar.DATIVE_PRONOUN_HE,
                Genders.Neutrum => Grammar.DATIVE_PRONOUN_IT,
                Genders.Unknown => Grammar.DATIVE_PRONOUN_SHE,
                _ => Grammar.DATIVE_PRONOUN_SHE
            };

            return IsPlayer ? Grammar.DATIVE_PRONOUN_YOU : result;
        }
        
        return Grammar.DATIVE_PRONOUN_THEY;
    }
    
    public string GetAccusativePronoun()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => Grammar.ACCUSATIVE_PRONOUN_SHE,
                Genders.Male => Grammar.ACCUSATIVE_PRONOUN_HE,
                Genders.Neutrum => Grammar.ACCUSATIVE_PRONOUN_IT,
                Genders.Unknown => Grammar.ACCUSATIVE_PRONOUN_SHE,
                _ => Grammar.ACCUSATIVE_PRONOUN_SHE
            };

            return IsPlayer ? Grammar.ACCUSATIVE_PRONOUN_YOU : result;
        }
        
        return Grammar.ACCUSATIVE_PRONOUN_THEY;
    }
    
    public string GetDativeIndefiniteArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => Grammar.DATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => Grammar.DATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => IsAbstract ? DATIV_INDEFINITEARTICLE_NEUTRUM_SINGULAR : Grammar.DATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => Grammar.DATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                _ => Grammar.DATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR
            };
        
            return result;
        }
        
        return string.Empty;
    }
    
    public string GetDativeArticle()
    {
        var result = this.Gender switch
        {
            Genders.Female => Resources.Grammar.DATIVE_ARTICLE_FEMALE_SINGULAR,
            Genders.Male => Resources.Grammar.DATIVE_ARTICLE_MALE_SINGULAR,
            Genders.Neutrum => Resources.Grammar.DATIVE_ARTICLE_NEUTRUM_SINGULAR,
            Genders.Unknown => Resources.Grammar.DATIVE_ARTICLE_NEUTRUM_SINGULAR,
            _ => Resources.Grammar.DATIVE_ARTICLE_NEUTRUM_SINGULAR
        };
        
        return result;
    }
    
    public string GetAccusativeIndefiniteArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => Resources.Grammar.ACCUSATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => Resources.Grammar.ACCUSATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => IsAbstract ? ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR : Resources.Grammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => Resources.Grammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                _ => Resources.Grammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR
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
                Genders.Female => Grammar.ACCUSATIVE_ARTICLE_FEMALE_SINGULAR,
                Genders.Male => Grammar.ACCUSATIVE_ARTICLE_MALE_SINGULAR,
                Genders.Neutrum => Grammar.ACCUSATIVE_ARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => Grammar.ACCUSATIVE_ARTICLE_NEUTRUM_SINGULAR,
                _ => Grammar.ACCUSATIVE_ARTICLE_NEUTRUM_SINGULAR
            };
        
            return result;
        }
        else
        {
            var result = this.Gender switch
            {
                Genders.Female => Grammar.ACCUSATIVE_ARTICLE_FEMALE_PLURAL,
                Genders.Male => Grammar.ACCUSATIVE_ARTICLE_MALE_PLURAL,
                Genders.Neutrum => Grammar.ACCUSATIVE_ARTICLE_NEUTRUM_PLURAL,
                Genders.Unknown => Grammar.ACCUSATIVE_ARTICLE_NEUTRUM_PLURAL,
                _ => Grammar.ACCUSATIVE_ARTICLE_NEUTRUM_PLURAL
            };
        
            return result;
        }
    }
}