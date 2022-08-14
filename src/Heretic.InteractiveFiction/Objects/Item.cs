namespace Heretic.InteractiveFiction.Objects;

public sealed class Item : AHereticObject
{
    /// <summary>
    /// Does this item prevent another item from being picked up?
    /// </summary>
    public bool IsBlockingPickUp { get; set; }
    
    public Item()
    {
        this.IsBlockingPickUp = false;
    }
}
