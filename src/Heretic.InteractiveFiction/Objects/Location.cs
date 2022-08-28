using System.Collections.ObjectModel;

namespace Heretic.InteractiveFiction.Objects;

public sealed partial class Location : AHereticObject
{
    public IDictionary<string, IList<Verb>> OptionalVerbs { get; set; }

    public Location()
    {
        this.OptionalVerbs = new Dictionary<string, IList<Verb>>();
        this.IsPickable = false;
    }

    public void AddOptionalVerb(string originalVerbKey, string newVerbName, Description newErrorMessage)
    {
        if (originalVerbKey == null)
        {
            throw new ArgumentNullException(nameof(originalVerbKey));
        }
        
        var names = newVerbName.Split('|').ToList();
        
        var optionalVerb = new Verb
        {
            Key = originalVerbKey,
            PrimaryName = names.FirstOrDefault(),
            Names = names,
            ErrorMessage = newErrorMessage
        };
        
        if (!this.OptionalVerbs.ContainsKey(originalVerbKey))
        {
            this.OptionalVerbs.Add(originalVerbKey, new List<Verb> {optionalVerb});
        }
        else
        {
            this.OptionalVerbs[originalVerbKey].Add(optionalVerb);
        }
    }

    public ReadOnlyCollection<Verb> GetOptionalVerbs(string verbKey)
    {
        if (this.OptionalVerbs.ContainsKey(verbKey))
        {
            return new ReadOnlyCollection<Verb>(this.OptionalVerbs[verbKey]);    
        }

        return new ReadOnlyCollection<Verb>(new List<Verb>());
    }

    public ICollection<Item> GetAllPickableAndUnHiddenItems()
    {
        var firstLevel = this.Items.Where(i => !i.IsHidden && i.IsPickable);
        var secondLevel = this.Items.Where(i => !i.IsHidden && !i.IsPickable);

        foreach (var item in secondLevel)
        {
            firstLevel = firstLevel.Union(GetDeeperLevelOfPickableAndUnHiddenItems(item));
        }

        return firstLevel.ToList<Item>();
    }

    private IEnumerable<Item> GetDeeperLevelOfPickableAndUnHiddenItems(Item itemToAnalyse)
    {
        var firstLevel = itemToAnalyse.Items.Where(i => !i.IsHidden && i.IsPickable);
        var secondLevel = itemToAnalyse.Items.Where(i => !i.IsHidden && !i.IsPickable);

        foreach (var item in secondLevel)
        {
            firstLevel = firstLevel.Union(GetDeeperLevelOfPickableAndUnHiddenItems(item));
        }

        return firstLevel.ToList<Item>();
    }
}
