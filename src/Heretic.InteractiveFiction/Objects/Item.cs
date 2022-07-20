namespace Heretic.InteractiveFiction.Objects;

public sealed class Item : AHereticObject
{
    public bool IsBlockingPickUp { get; set; }
    public Item()
    {
        this.IsBlockingPickUp = false;
    }
}
