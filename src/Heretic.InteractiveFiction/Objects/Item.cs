namespace Heretic.InteractiveFiction.Objects;

public sealed class Item : AContainerObject
{
    public bool IsBlockingPickUp { get; set; }
    public Item() : base()
    {
        this.IsBlockingPickUp = false;
    }
}
