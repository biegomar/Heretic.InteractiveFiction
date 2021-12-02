namespace Heretic.InteractiveFiction.Objects;

public class Item : AContainerObject
{
    public bool IsBlockingPickUp { get; set; }
    public Item() : base()
    {
        this.IsBlockingPickUp = false;
    }
}
