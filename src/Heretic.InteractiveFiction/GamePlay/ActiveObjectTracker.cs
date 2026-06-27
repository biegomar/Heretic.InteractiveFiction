using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class ActiveObjectTracker
{
    private readonly Universe universe;

    internal ActiveObjectTracker(Universe universe)
    {
        this.universe = universe;
    }

    internal AHereticObject? Current => this.universe.ActiveObject;

    internal void Store(AHereticObject hereticObject)
    {
        this.universe.ActiveObject = hereticObject;
    }

    internal void Clear()
    {
        this.universe.ActiveObject = default;
    }

    internal void RemoveIfIs(AHereticObject hereticObject)
    {
        if (this.universe.ActiveObject == hereticObject)
            Clear();
    }

    internal void ClearIfNotInInventory()
    {
        var universeActiveObject = this.universe.ActiveObject;
        if (universeActiveObject != default && !this.universe.ActivePlayer.OwnsItem(universeActiveObject.Key))
            Clear();
    }
}
