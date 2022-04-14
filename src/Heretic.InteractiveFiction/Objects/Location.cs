namespace Heretic.InteractiveFiction.Objects;

public sealed class Location : AHereticObject
{
    public Location() : base()
    {
        this.IsPickAble = false;
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
