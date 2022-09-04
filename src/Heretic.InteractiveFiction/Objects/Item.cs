namespace Heretic.InteractiveFiction.Objects;

public sealed class Item : AHereticObject
{
    /// <summary>
    /// Does this item prevent another item from being picked up?
    /// </summary>
    public bool IsBlockingPickUp { get; set; }

    /// <summary>
    /// Can this object be worn?
    /// </summary>
    public bool IsWearable { get; set; }
    
    public Item()
    {
        this.IsBlockingPickUp = false;
        this.IsWearable = false;
    }
}
