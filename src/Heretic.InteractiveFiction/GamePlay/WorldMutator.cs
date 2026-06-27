using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class WorldMutator
{
    private readonly Universe universe;

    internal WorldMutator(Universe universe)
    {
        this.universe = universe;
    }

    internal void HideItemsOnClose(AHereticObject item)
    {
        if (item.IsClosed)
        {
            foreach (var child in item.Items.Where(x => x.HideOnContainerClose))
                child.IsHidden = true;
        }
    }

    internal void UnhideItemsOnOpen(AHereticObject item)
    {
        if (!item.IsClosed)
        {
            foreach (var child in item.Items.Where(x => x.HideOnContainerClose))
                child.IsHidden = false;
        }
    }

    internal void UnveilFirstLevelObjects(AHereticObject container)
    {
        var unveilAbleLinkedItems = container.LinkedTo.Where(i => i.IsUnveilable).ToList();
        foreach (var linkedItem in unveilAbleLinkedItems)
            linkedItem.IsHidden = false;

        if (container.IsCloseable && container.IsClosed)
            return;

        var unveilAbleItems = container.Items.Where(i => i.IsUnveilable).ToList();
        foreach (var item in unveilAbleItems)
            item.IsHidden = false;

        var unveilAbleCharacters = container.Characters.Where(c => c.IsUnveilable).ToList();
        foreach (var character in unveilAbleCharacters)
            character.IsHidden = false;
    }

    internal bool MoveCharacter(Character person, Location newLocation)
    {
        var oldLocation = this.universe.LocationMap.Keys.SingleOrDefault(l => l.Characters.Contains(person));
        if (oldLocation != default)
        {
            oldLocation.Characters.Remove(person);
            newLocation.Characters.Add(person);
            return true;
        }

        return false;
    }
}
