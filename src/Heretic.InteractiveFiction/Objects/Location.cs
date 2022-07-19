using System.Collections.ObjectModel;

namespace Heretic.InteractiveFiction.Objects;

public sealed class Location : AHereticObject
{
    private IDictionary<string, IList<string>> VerbResources;
    
    public Location(Func<string> descriptionFunc = null) : base(descriptionFunc)
    {
        this.VerbResources = new Dictionary<string, IList<string>>();
        this.IsPickAble = false;
    }

    public void AddVerbAlternative(string verbKey, string verbAlternative)
    {
        if (this.VerbResources.ContainsKey(verbKey))
        {
            this.VerbResources[verbKey].Add(verbAlternative);
        }
        else
        {
            this.VerbResources.Add(verbKey, new []{ verbAlternative });
        }
    }

    public ReadOnlyCollection<string> GetVerbAlternatives(string verbKey)
    {
        if (this.VerbResources.ContainsKey(verbKey))
        {
            return new ReadOnlyCollection<string>(this.VerbResources[verbKey]);    
        }

        return new ReadOnlyCollection<string>(new List<string>());
    }

    public ICollection<Item> GetAllPickableAndUnHiddenItems()
    {
        var firstLevel = this.Items.Where(i => !i.IsHidden && i.IsPickAble);
        var secondLevel = this.Items.Where(i => !i.IsHidden && !i.IsPickAble);

        foreach (var item in secondLevel)
        {
            firstLevel = firstLevel.Union(GetDeeperLevelOfPickableAndUnHiddenItems(item));
        }

        return firstLevel.ToList<Item>();
    }

    private IEnumerable<Item> GetDeeperLevelOfPickableAndUnHiddenItems(Item itemToAnalyse)
    {
        var firstLevel = itemToAnalyse.Items.Where(i => !i.IsHidden && i.IsPickAble);
        var secondLevel = itemToAnalyse.Items.Where(i => !i.IsHidden && !i.IsPickAble);

        foreach (var item in secondLevel)
        {
            firstLevel = firstLevel.Union(GetDeeperLevelOfPickableAndUnHiddenItems(item));
        }

        return firstLevel.ToList<Item>();
    }
}
