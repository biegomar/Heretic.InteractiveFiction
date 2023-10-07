namespace Heretic.InteractiveFiction.Objects;

public abstract partial class AHereticObject
{
    /// <summary>
    /// The detailed description of the object. It is used during printout.
    /// </summary>
    public Description Description { get; set; } = string.Empty;

    /// <summary>
    /// The first look description is only used during the first printout and contains additional information.
    /// </summary>
    public Description FirstLookDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// The hint description is shown when the hint system has been activated.
    /// </summary>
    public Description Hint { get; set; } = string.Empty;
    
    /// <summary>
    /// This description can be used to describe the discover situation in more detail. It is used during printout instead of the name of the object.
    /// It is only valid in the context of the location where it was found and is deleted after a pickup.
    /// </summary>
    public Description ContainmentDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// Is shown when the item is broken.
    /// </summary>
    public Description BrokenDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// This description is used when the object cannot be broken.
    /// If the description is empty, a default text is generated.
    /// </summary>
    public Description UnbreakableDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// If the object cannot be taken, this description can explain why.
    /// </summary>
    public Description UnPickAbleDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// If the object cannot be dropped, this description can explain why.
    /// </summary>
    public Description UnDropAbleDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// Gives a more detailed description about the state of a locked object.
    /// </summary>
    public Description LockDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// Gives a more detailed description about the state of an opened object.
    /// </summary>
    public Description OpenDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// Gives a more detailed description about the state of a closed object.
    /// </summary>
    public Description CloseDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// This description can be used if the object is linked to another.
    /// </summary>
    public Description LinkedToDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// This description can be used if the object is climbed by the player.
    /// </summary>
    public Description ClimbedDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// This description can be used if the object is readable.
    /// </summary>
    public Description LetterContentDescription { get; set; } = string.Empty;

    /// <summary>
    /// This description can be used if the object as a container is empty.
    /// </summary>
    public Description ContainerEmptyDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// This description can be used if the object as a surface container is empty.
    /// </summary>
    public Description SurfaceContainerEmptyDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// This description can be used if the object has a specific smell.
    /// </summary>
    public Description SmellDescription { get; set; } = string.Empty;
    
    /// <summary>
    /// This description can be used if the object has a specific taste.
    /// </summary>
    public Description TasteDescription { get; set; } = string.Empty;
}