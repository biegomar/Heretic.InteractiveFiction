namespace Heretic.InteractiveFiction.Grammars;

using Heretic.InteractiveFiction.Objects;

public sealed class IndividualObjectGrammar
{
    public Genders Gender { get; init; }
    public bool IsSingular { get; init; }
    public bool IsAbstract { get; init; }

    public IndividualObjectGrammar(Genders gender = Genders.Female, bool isSingular = true, bool isAbstract = false)
    {
        this.IsSingular = isSingular;
        this.Gender = gender;
        this.IsAbstract = isAbstract;
    }
}