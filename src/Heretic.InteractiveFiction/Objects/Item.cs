namespace Heretic.InteractiveFiction.Objects;

public sealed class Item : AHereticObject
{
    public bool IsBlockingPickUp { get; set; }
    public Item(Func<string> descriptionFunc = null) : base(descriptionFunc)
    {
        this.IsBlockingPickUp = false;
    }
}
