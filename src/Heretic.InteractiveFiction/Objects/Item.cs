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

    /// <summary>
    /// Is the item switchable?
    /// </summary>
    public bool IsSwitchable { get; set; }
    /// <summary>
    /// Is the item source switched on?
    /// </summary>
    public bool IsSwitchedOn { get; set; }
    
    /// <summary>
    /// Is the object a light source?
    /// </summary>
    public bool IsLighter { get; set; }

    /// <summary>
    /// Is the light source switched on?
    /// </summary>
    public bool IsLighterSwitchedOn { get; set; }
    
    /// <summary>
    /// This description can be used if the object is a lighter and is switched on.
    /// </summary>
    public Description LighterSwitchedOnDescription { get; set; }
    
    /// <summary>
    /// This description can be used if the object is a lighter and is switched off.
    /// </summary>
    public Description LighterSwitchedOffDescription { get; set; }
    
    /// <summary>
    /// This description can be used if the object is switched on.
    /// </summary>
    public Description SwitchedOnDescription { get; set; }
    
    /// <summary>
    /// This description can be used if the object is switched off.
    /// </summary>
    public Description SwitchedOffDescription { get; set; }

    public Item()
    {
        this.InitializeStates();
        
        this.InitializeDescriptions();
    }

    protected override StringBuilder ToStringExtension()
    {
        var description = new StringBuilder();
        
        if (this.IsLighter)
        {
            if (this.IsLighterSwitchedOn && !string.IsNullOrEmpty(this.LighterSwitchedOnDescription))
            {
                description.AppendLine(this.LighterSwitchedOnDescription);    
            }

            if (!this.IsLighterSwitchedOn && !string.IsNullOrEmpty(this.LighterSwitchedOffDescription))
            {
                description.AppendLine(this.LighterSwitchedOffDescription);
            }
        }
        
        if (this.IsSwitchable)
        {
            if (this.IsSwitchedOn && !string.IsNullOrEmpty(this.SwitchedOnDescription))
            {
                description.AppendLine(this.SwitchedOnDescription);    
            }

            if (!this.IsSwitchedOn && !string.IsNullOrEmpty(this.SwitchedOffDescription))
            {
                description.AppendLine(this.SwitchedOffDescription);
            }
        }

        return description;
    }

    private void InitializeStates()
    {
        this.IsBlockingPickUp = false;
        this.IsWearable = false;
        this.IsLighter = false;
        this.IsLighterSwitchedOn = false;
        this.IsSwitchable = false;
        this.IsSwitchedOn = false;
    }
    
    private void InitializeDescriptions()
    {
        this.LighterSwitchedOffDescription = string.Empty;
        this.LighterSwitchedOnDescription = string.Empty;
        this.SwitchedOffDescription = string.Empty;
        this.SwitchedOnDescription = string.Empty;
    }
}
