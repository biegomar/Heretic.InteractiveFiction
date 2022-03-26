using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public class Grammars
{
    public Genders Gender { get; set; }
    public bool IsSingular { get; set; }
    public string Article { get; set; }
    public string IndefiniteArticle { get; set; }

    public Grammars()
    {
        this.GenderFemale();
    }

    public Grammars(Genders gender, bool isSingular = true)
    {
        switch (gender)
        {
            case Genders.Female:
            {
                GenderFemale(isSingular);
                break;
            }
            case Genders.Male:
            {
                GenderMale(isSingular);
                break;
            }
            case Genders.Neutrum:
            {
                GenderNeutrum(isSingular);
                break;
            }
            case Genders.Unknown:
            {
                GenderNeutrum(isSingular);
                break;
            }
            default:
            {
                GenderNeutrum(isSingular);
                break;
            }
        }
    }
    
    public string GetArticle()
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
    
    public string GetNominativeIndefiniteArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => Grammar.NOMINATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => Grammar.NOMINATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => Grammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                Genders.Unknown => Grammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
                _ => Grammar.NOMINATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR
            };
        
            return result;
        }
        
        return string.Empty;
    }
    
    public string GetDativeIndefiniteArticle()
    {
        if (IsSingular)
        {
            var result = this.Gender switch
            {
                Genders.Female => Grammar.DATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR,
                Genders.Male => Grammar.DATIVE_INDEFINITEARTICLE_MALE_SINGULAR,
                Genders.Neutrum => Grammar.DATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
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
                Genders.Neutrum => Resources.Grammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR,
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

    private void GenderFemale(bool isSingular = true)
    {
        this.Gender = Genders.Female;
        this.IsSingular = isSingular;
        this.Article = Grammar.ARTICLE_FEMALE_SINGULAR;
        this.IndefiniteArticle = Grammar.ACCUSATIVE_INDEFINITEARTICLE_FEMALE_SINGULAR;
    }
    
    private void GenderMale(bool isSingular = true)
    {
        this.Gender = Genders.Male;
        this.IsSingular = isSingular;
        this.Article = Grammar.ARTICLE_MALE_SINGULAR;
        this.IndefiniteArticle = Grammar.ACCUSATIVE_INDEFINITEARTICLE_MALE_SINGULAR;
    }
    
    private void GenderNeutrum(bool isSingular = true)
    {
        this.Gender = Genders.Neutrum;
        this.IsSingular = isSingular;
        this.Article = Grammar.ARTICLE_NEUTRUM_SINGULAR;
        this.IndefiniteArticle = Grammar.ACCUSATIVE_INDEFINITEARTICLE_NEUTRUM_SINGULAR;
    }
}