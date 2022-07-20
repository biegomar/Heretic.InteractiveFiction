using System.Collections.ObjectModel;

namespace Heretic.InteractiveFiction.Objects;

public sealed class Location : AHereticObject
{
    private IDictionary<string, IList<string>> VerbResources;
    
    public Location()
    {
        this.VerbResources = new Dictionary<string, IList<string>>();
        this.IsPickAble = false;
    }

    public void AddVerbAlternative(string verbKey, string verbAlternative)
    {
        var inputList = verbAlternative.Split('|').ToList();
        var normalizedList = this.NormalizeResourceList(inputList);
        foreach (var verb in normalizedList)
        {
            if (this.VerbResources.ContainsKey(verbKey))
            {
                this.VerbResources[verbKey].Add(verb);
            }
            else
            {
                this.VerbResources.Add(verbKey, new List<string> {verb});
            }   
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

    public ReadOnlyDictionary<string, IList<string>> GetAllVerbAlternatives()
    {
        return new ReadOnlyDictionary<string, IList<string>>(this.VerbResources);
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
    
    private IEnumerable<string> NormalizeResourceList(IEnumerable<string> inputList)
    {
        var result = new List<string>();
        foreach (var item in inputList)
        {
            result.Add(item);
            var trimmedItem = string.Concat(item.Where(c => !char.IsWhiteSpace(c)));
            if (item != trimmedItem)
            {
                result.Add(trimmedItem);
            }
        }

        return result;
    }
}
