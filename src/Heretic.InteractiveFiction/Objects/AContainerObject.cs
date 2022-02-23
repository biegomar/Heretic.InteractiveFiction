using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public abstract class AContainerObject
{
    private string name;

    /// <summary>
    /// This is the name of the object. This name is used as headline during printout.
    /// </summary>
    public string Name
    {
        get => this.GetObjectName();
        set => name = value;
    }

    /// <summary>
    /// The unique key that is representing the object.
    /// </summary>
    public string Key { get; init; }
    /// <summary>
    /// The detailed description of the object. It is used during printout.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// The first look description is only used during the first printout and contains additional information.
    /// </summary>
    public string FirstLookDescription { get; set; }
    /// <summary>
    /// The hint description is shown when the hint system has been activated.
    /// </summary>
    public string Hint { get; set; }
    /// <summary>
    /// This description can be used to describe the discover situation in more detail. It is used during printout instead of the name of the object.
    /// It is only valid in the context of the location where it was found and is deleted after a pickup.
    /// </summary>
    public string ContainmentDescription { get; set; }
    /// <summary>
    /// Is shown when the item is broken.
    /// </summary>
    public string BrokenDescription { get; set; }
    /// <summary>
    /// This description is used when the object cannot be broken.
    /// If the description is empty, a default text is generated.
    /// </summary>
    public string UnbreakableDescription { get; set; }
    /// <summary>
    /// If the object cannot be taken, this description can explain why.
    /// </summary>
    public string UnPickAbleDescription { get; set; }
    /// <summary>
    /// If the object cannot be dropped, this description can explain why.
    /// </summary>
    public string UnDropAbleDescription { get; set; }
    /// <summary>
    /// Gives a more detailed description about the state of a locked object.
    /// </summary>
    public string LockDescription { get; set; }
    /// <summary>
    /// Gives a more detailed description about the state of an opened object.
    /// </summary>
    public string OpenDescription { get; set; }
    /// <summary>
    /// Gives a more detailed description about the state of a closed object.
    /// </summary>
    public string CloseDescription { get; set; }
    /// <summary>
    /// This description can be used if the object is linked to another.
    /// </summary>
    public string LinkedToDescription { get; set; }
    /// <summary>
    /// This description can be used if the object is climbed by the player.
    /// </summary>
    public string ClimbedDescription { get; set; }
    /// <summary>
    /// Can this object be broken?
    /// </summary>
    public bool IsBreakable { get; set; }
    /// <summary>
    /// Is this object broken?
    /// </summary>
    public bool IsBroken { get; set; }
    /// <summary>
    /// Is this object eatable?
    /// </summary>
    public bool IsEatable { get; set; }
    /// <summary>
    /// Is the object visible or hidden?
    /// </summary>
    public bool IsHidden { get; set; }
    /// <summary>
    /// Should this object be hidden when the container it is placed in is closed?
    /// </summary>
    public bool HideOnContainerClose { get; set; }
    /// <summary>
    /// Is this a physical object or just virtual? 
    /// </summary>
    public bool IsVirtual { get; set; }
    /// <summary>
    /// Can the object picked up?
    /// </summary>
    public bool IsPickAble { get; set; }

    /// <summary>
    /// Can the object be dropped? 
    /// </summary>
    public bool IsDropAble { get; set; }

    /// <summary>
    /// Can the player sit on this object?
    /// </summary>
    public bool IsSeatAble { get; set; }
    /// <summary>
    /// Can th player climb on this object?
    /// </summary>
    public bool IsClimbAble { get; set; }
    /// <summary>
    /// The weight of the object.
    /// </summary>
    public int Weight { get; set; }
    /// <summary>
    /// If this object is a container, you can specify how much weight it can hold.
    /// </summary>
    public int MaxPayload { get; init; }
    /// <summary>
    /// Determines whether simply looking at the surroundings will make this object visible.
    /// </summary>
    public bool IsUnveilAble { get; set; }
    /// <summary>
    /// Determines whether an object is lockable.
    /// </summary>
    public bool IsLockAble { get; init; }
    /// <summary>
    /// Is the object locked?
    /// </summary>
    public bool IsLocked { get; set; }
    /// <summary>
    /// Can this object be closed?
    /// </summary>
    public bool IsCloseAble { get; init; }
    /// <summary>
    /// Is this object closed?
    /// </summary>
    public bool IsClosed { get; set; }
    /// <summary>
    /// The list of contained objects.
    /// </summary>
    public ICollection<Item> Items { get; init; }

    /// <summary>
    /// The list of surrounding objects.
    /// </summary>
    public IDictionary<string, Func<string>> Surroundings { get; init; }

    public ICollection<Item> LinkedTo { get; init; }

    /// <summary>
    /// The list of contained characters.
    /// <example>
    /// A magician in a huge box.
    /// </example>
    /// </summary>
    public ICollection<Character> Characters { get; init; }
    public event EventHandler<ChangeLocationEventArgs> BeforeChangeLocation;
    public event EventHandler<ChangeLocationEventArgs> AfterChangeLocation;
    public event EventHandler<BreakItemEventArg> Break;
    public event EventHandler<ContainerObjectEventArgs> BeforeClose;
    public event EventHandler<ContainerObjectEventArgs> AfterClose;
    public event EventHandler<ContainerObjectEventArgs> BeforeDrop;
    public event EventHandler<ContainerObjectEventArgs> AfterDrop;
    public event EventHandler<ContainerObjectEventArgs> BeforeEat;
    public event EventHandler<ContainerObjectEventArgs> AfterEat; 
    public event EventHandler<ContainerObjectEventArgs> AfterGive;
    public event EventHandler<ContainerObjectEventArgs> Open;
    public event EventHandler<ContainerObjectEventArgs> BeforeOpen;
    public event EventHandler<ContainerObjectEventArgs> AfterOpen;
    public event EventHandler<ContainerObjectEventArgs> AfterLook;
    public event EventHandler<ContainerObjectEventArgs> BeforeTake;
    public event EventHandler<ContainerObjectEventArgs> AfterTake;
    public event EventHandler<ContainerObjectEventArgs> BeforeSitDown;
    public event EventHandler<ContainerObjectEventArgs> AfterSitDown; 
    public event EventHandler<ContainerObjectEventArgs> BeforeStandUp;
    public event EventHandler<ContainerObjectEventArgs> AfterStandUp;
    public event EventHandler<ContainerObjectEventArgs> BeforeDescend;
    public event EventHandler<ContainerObjectEventArgs> AfterDescend;
    public event EventHandler<ContainerObjectEventArgs> Buy;
    public event EventHandler<ContainerObjectEventArgs> Jump;
    public event EventHandler<PullItemEventArgs> Pull;
    public event EventHandler<PushItemEventArgs> Push;
    public event EventHandler<ContainerObjectEventArgs> Turn;
    public event EventHandler<UnlockContainerEventArgs> Unlock;
    public event EventHandler<UseItemEventArgs> Use;
    public event EventHandler<WriteEventArgs> Write;

    public virtual void OnUse(UseItemEventArgs eventArgs)
    {
        var localEventHandler = this.Use;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new UseException(BaseDescriptions.NOTHING_HAPPENS);
        }
    }
    
    public virtual void OnJump(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Jump;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new JumpException(BaseDescriptions.NOTHING_HAPPENS);
        }
    }

    public virtual void OnPull(PullItemEventArgs eventArgs)
    {
        var localEventHandler = this.Pull;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new PullException(BaseDescriptions.NOTHING_HAPPENS);
        }
    }
    
    public virtual void OnPush(PushItemEventArgs eventArgs)
    {
        var localEventHandler = this.Push;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new PushException(BaseDescriptions.DOES_NOT_WORK);
        }
    }
    
    public virtual void OnOpen(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Open;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new OpenException(BaseDescriptions.IMPOSSIBLE_OPEN);
        }
    }
    
    public virtual void OnWrite(WriteEventArgs eventArgs)
    {
        var localEventHandler = this.Write;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new WriteException(BaseDescriptions.NOTHING_HAPPENS);
        }
    }

    public virtual void OnBuy(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Buy;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new BuyException(BaseDescriptions.ON_BUY_EXCEPTION);
        }
    }

    public virtual void OnBeforeDescend(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeDescend;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterDescend(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterDescend;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBeforeDrop(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeDrop;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterDrop(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterDrop;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBeforeEat(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeEat;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterEat(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterEat;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBreak(BreakItemEventArg eventArgs)
    {
        var localEventHandler = this.Break;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new BreakException(BaseDescriptions.IMPOSSIBLE_BREAK);
        }
    }
    
    public virtual void OnTurn(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Turn;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new BreakException(BaseDescriptions.NOTHING_HAPPENS);
        }
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBeforeSitDown(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeSitDown;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterSitDown(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterSitDown;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBeforeStandUp(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeStandUp;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterStandUp(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterStandUp;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterGive(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterGive;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterLook(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterLook;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeTake(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeTake;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterTake(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterTake;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeChangeLocation(ChangeLocationEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeChangeLocation;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual ChangeLocationStatus OnAfterChangeLocation(ChangeLocationEventArgs eventArgs)
    {
        var localEventHandler = this.AfterChangeLocation;
        if (localEventHandler != null)
        {
            localEventHandler(this, eventArgs);
        }
        else
        {
            if (!eventArgs.NewDestinationNode.Location.IsLocked)
            {
                if (eventArgs.NewDestinationNode.Location.IsClosed)
                {
                    return ChangeLocationStatus.IsClosed;
                }
            }
            else
            {
                return ChangeLocationStatus.IsLocked;
            }
        }

        return ChangeLocationStatus.Ok;
    }

    public virtual void OnBeforeClose(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeClose;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterClose(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterClose;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeOpen(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeOpen;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterOpen(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterOpen;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnUnlock(UnlockContainerEventArgs eventArgs)
    {
        var localEventHandler = this.Unlock;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new UnlockException(string.Format(BaseDescriptions.IMPOSSIBLE_UNLOCK_WITH_WRONG_KEY, this.Name, eventArgs.Key.Name));
        }
    }
    
    protected AContainerObject()
    {
        this.Items = new List<Item>();
        this.Characters = new List<Character>();
        this.Surroundings = new Dictionary<string, Func<string>>();
        this.LinkedTo = new List<Item>();
        this.FirstLookDescription = string.Empty;
        this.IsBreakable = false;
        this.IsBroken = false;
        this.IsEatable = false;
        this.IsHidden = false;
        this.HideOnContainerClose = true;
        this.IsVirtual = false;
        this.IsPickAble = true;
        this.IsDropAble = true;
        this.IsUnveilAble = true;
        this.IsLockAble = false;
        this.IsLocked = false;
        this.IsClosed = false;
        this.IsCloseAble = false;
        this.IsSeatAble = false;
        this.IsClimbAble = false;
        this.name = string.Empty;
        this.Description = string.Empty;
        this.OpenDescription = string.Empty;
        this.CloseDescription = string.Empty;
        this.UnPickAbleDescription = string.Empty;
        this.UnDropAbleDescription = string.Empty;
        this.LockDescription = string.Empty;
        this.ContainmentDescription = string.Empty;
        this.BrokenDescription = string.Empty;
        this.UnbreakableDescription = string.Empty;
        this.LinkedToDescription = string.Empty;
        this.ClimbedDescription = string.Empty;
        this.Hint = string.Empty;
    }

    protected virtual string GetVariationOfYouSee()
    {
        return BaseDescriptions.YOU_SEE;
    }

    protected virtual string GetVariationOfHereSingle()
    {
        return BaseDescriptions.HERE_SINGLE;
    }

    protected virtual string GetVariationOfHerePlural()
    {
        return BaseDescriptions.HERE_PLURAL;
    }

    private string PrintUnhiddenObjects(ICollection<AContainerObject> unhiddenObjects, bool subItems = false)
    {
        var description = new StringBuilder();

        if (unhiddenObjects.Any())
        {
            var unHiddenObjectsWithContainmentDescription = unhiddenObjects.Where(x => !string.IsNullOrEmpty(x.ContainmentDescription)).ToList();
            var unHiddenObjectsWithoutContainmentDescription = unhiddenObjects.Where(x => string.IsNullOrEmpty(x.ContainmentDescription)).ToList();
            
            if (unHiddenObjectsWithContainmentDescription.Any())
            {
                if (subItems)
                {
                    unHiddenObjectsWithoutContainmentDescription = unhiddenObjects.ToList();
                }
                else
                {
                    foreach (var item in unHiddenObjectsWithContainmentDescription)
                    {
                        description.Append(item.ContainmentDescription);
                        description.AppendLine(GetLinkedObjectsDescription(item));
                    }
                }
            }

            if (unHiddenObjectsWithoutContainmentDescription.Any())
            {
                if (subItems)
                {
                    description.Append($" ({GetVariationOfYouSee()} ");
                }

                var index = 0;

                foreach (var item in unHiddenObjectsWithoutContainmentDescription)
                {
                    if (index != 0)
                    {
                        if (unHiddenObjectsWithoutContainmentDescription.Count == 2)
                        {
                            description.Append($" {BaseDescriptions.AND} ");
                        }
                        else
                        {
                            description.Append(index == unHiddenObjectsWithoutContainmentDescription.Count - 1 ? $" {BaseDescriptions.AND} " : ", ");
                        }
                    }

                    if (index != 0)
                    {
                        var lowerName = this.LowerFirstChar(item.Name);
                        description.Append($"{lowerName}");
                    }
                    else
                    {
                        if (subItems)
                        {
                            var lowerName = this.LowerFirstChar(item.Name);
                            description.Append($"{lowerName}");
                        }
                        else
                        {
                            description.Append($"{item.Name}");
                        }
                    }

                    if (!item.IsCloseAble || item.IsCloseAble && !item.IsClosed)
                    {
                        if (item.Items.Any(i => i.IsHidden == false))
                        {
                            var subItemList = item.Items.Where(i => i.IsHidden == false).ToList<AContainerObject>();
                            description.Append(item.PrintUnhiddenObjects(subItemList, true));
                        }
                    }

                    if (!subItems)
                    {
                        description.Append(GetLinkedObjectsDescription(item));
                    }

                    index++;
                }

                if (subItems)
                {
                    description.Append(')');
                }
                else
                {
                    description.AppendLine(index == 1
                        ? $" {GetVariationOfHereSingle()}"
                        : $" {GetVariationOfHerePlural()}");
                }
            }
        }

        return description.ToString();
    }
    
    protected string LowerFirstChar(string description)
    {
        return description[..1].ToLower() + description[1..];
    }

    private string GetLinkedObjectsDescription(AContainerObject item, bool useBracket = true)
    {
        var description = new StringBuilder();
        var unhiddenLinkedItemsWithLinkedTo = item.LinkedTo.Where(x => !x.IsHidden && !string.IsNullOrEmpty(x.LinkedToDescription)).ToList();
        var unhiddenLinkedItemsWithoutLinkedTo =item.LinkedTo.Where(x => !x.IsHidden && string.IsNullOrEmpty(x.LinkedToDescription)).ToList();
        if (unhiddenLinkedItemsWithLinkedTo.Any() ||unhiddenLinkedItemsWithoutLinkedTo.Any())
        {
            if (useBracket)
            {
                description.Append(" (");    
            }

            var linkedItemIndex = 0;
            foreach (var linkedItem in unhiddenLinkedItemsWithLinkedTo)
            {
                if (linkedItemIndex > 0)
                {
                    description.Append(' ');
                }
                
                description.Append(linkedItem.LinkedToDescription);
                
                linkedItemIndex++;
            }
            
            linkedItemIndex = 0;
            foreach (var linkedItem in unhiddenLinkedItemsWithoutLinkedTo)
            {
                if (linkedItemIndex == 0)
                {
                    description.Append(BaseDescriptions.LINKED_TO);
                    
                }
                else
                {
                    description.Append(", ");
                }
                
                description.Append(this.LowerFirstChar(linkedItem.Name));

                linkedItemIndex++;
            }

            if (useBracket)
            {
                description.Append(')');    
            }
        }

        return description.ToString();
    }

    protected virtual string PrintCharacters()
    {
        var unhiddenItems = this.Characters.Where(i => i.IsHidden == false).ToList<AContainerObject>();

        return this.PrintUnhiddenObjects(unhiddenItems);
    }

    protected virtual string PrintItems(bool subItems = false)
    {

        var unhiddenItems = this.Items.Where(i => i.IsHidden == false).ToList<AContainerObject>();

        return this.PrintUnhiddenObjects(unhiddenItems);
    }

    public Item GetUnhiddenItemByKey(string key, IList<Item> items = null)
    {
        items = items ?? new List<Item>();
        var unhiddenItems = this.FilterUnhiddenItems();

        if (unhiddenItems.Any())
        {
            foreach (var item in unhiddenItems)
            {
                if (!items.Contains(item))
                {
                    if (item.Key == key)
                    {
                        return item;
                    }
                    items.Add(item);
                    var result = item.GetUnhiddenItemByKey(key, items);

                    if (result != default)
                    {
                        return result;
                    }    
                }
            }
        }

        return default;
    }
    
    protected virtual IList<Item> FilterUnhiddenItems()
    {
        var itemsFromCharacter = this.Characters.Where(c => c.IsHidden == false).SelectMany(c => c.Items).Where(i => i.IsHidden == false)
            .Union(this.Characters.Where(c => c.IsHidden == false).SelectMany(c => c.LinkedTo).Where(i => i.IsHidden == false));
        
        var unhiddenItems = this.Items.Where(i => i.IsHidden == false)
            .Union(this.LinkedTo.Where(i => i.IsHidden == false))
            .Union(itemsFromCharacter).ToList();

        return unhiddenItems;
    }

    public Item GetVirtualItemByKey(string key)
    {
        var itemsFromCharacter = this.Characters.SelectMany(c => c.Items);
        var items = this.Items.Union(itemsFromCharacter).ToList();

        if (items.Any())
        {
            foreach (var item in items)
            {
                if (item.Key == key && item.IsVirtual)
                {
                    return item;
                }
                var result = item.GetVirtualItemByKey(key);

                if (result != default)
                {
                    return result;
                }
            }
        }

        return default;
    }

    public Item GetItemByKey(string key)
    {
        foreach (var item in this.Items)
        {
            if (item.Key == key)
            {
                return item;
            }
            var result = item.GetItemByKey(key);

            if (result != default)
            {
                return result;
            }
        }

        foreach (var character in this.Characters)
        {
            foreach (var item in character.Items)
            {
                if (item.Key == key)
                {
                    return item;
                }
                var result = item.GetItemByKey(key);

                if (result != default)
                {
                    return result;
                }
            }
        }

        return default;
    }

    public virtual bool RemoveItem(Item itemToRemove)
    {
        foreach (var item in this.Items)
        {
            if (item.Key == itemToRemove.Key)
            {
                return this.Items.Remove(itemToRemove);
            }
            var result = item.RemoveItem(itemToRemove);

            if (result)
            {
                return true;
            }
        }

        foreach (var character in this.Characters)
        {
            foreach (var item in character.Items)
            {
                if (item.Key == itemToRemove.Key)
                {
                    return character.Items.Remove(itemToRemove);
                }
                var result = item.RemoveItem(itemToRemove);

                if (result)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public Character GetUnhiddenCharacterByKey(string key)
    {
        var unhiddenItems = this.Characters.Where(i => i.IsHidden == false).ToList();

        if (unhiddenItems.Any())
        {
            foreach (var character in unhiddenItems)
            {
                if (character.Key == key)
                {
                    return character;
                }
                var result = character.GetUnhiddenCharacterByKey(key);

                if (result != default)
                {
                    return result;
                }
            }
        }

        return default;
    }

    public Character GetCharacterByKey(string key)
    {
        if (this.Characters.Any())
        {
            foreach (var character in this.Characters)
            {
                if (character.Key == key)
                {
                    return character;
                }
                var result = character.GetCharacterByKey(key);

                if (result != default)
                {
                    return result;
                }
            }
        }

        return default;
    }

    public AContainerObject GetOwnerOfUnhiddenItemByKey(string key)
    {
        var unhiddenItems = this.Items.Where(i => !i.IsHidden).ToList();

        if (unhiddenItems.Any())
        {
            foreach (var item in unhiddenItems)
            {
                if (item.Key == key)
                {
                    return this;
                }
                var result = item.GetOwnerOfUnhiddenItemByKey(key);

                if (result != default)
                {
                    return result;
                }
            }
        }

        var unhiddenCharacters = this.Characters.Where(c => !c.IsHidden).ToList();
        if (unhiddenCharacters.Any())
        {
            foreach (var item in unhiddenCharacters)
            {
                if (item.Key == key)
                {
                    return this;
                }
                var result = item.GetOwnerOfUnhiddenItemByKey(key);

                if (result != default)
                {
                    return result;
                }
            }
        }

        return default;
    }

    public virtual int GetActualPayload()
    {
        var sum = this.Weight;
        if (this.Items.Any())
        {
            foreach (var item in this.Items)
            {
                if (item.Items.Any())
                {
                    sum += item.GetActualPayload();
                }
                else
                {
                    sum += item.Weight;
                }
            }
        }

        return sum;
    }

    public string AlterEgo()
    {
        var result = new StringBuilder();
        result.AppendFormat(BaseDescriptions.ALTER_EGO_DESCRIPTION, this.Name);
        result.AppendLine(this.GetResourceByKey());

        return result.ToString();
    }

    private string GetResourceByKey()
    {
        var characterNames = this.name.Split('|');
        return string.Join(", ", characterNames);
    }

    private string GetObjectName()
    {
        var sentence = this.name.Split('|');
        return sentence[0].Trim();
    }

    public override string ToString()
    {
        var description = new StringBuilder();
        description.AppendLine(this.Name);
        description.AppendLine(new string('-', this.Name.Length));
        description.AppendLine(this.Description);

        if (this.FirstLookDescription != string.Empty)
        {
            description.AppendLine(this.FirstLookDescription);
            this.FirstLookDescription = string.Empty;
        }

        if (this.IsLocked && !string.IsNullOrEmpty(this.LockDescription))
        {
            description.AppendLine(this.LockDescription);
        }
        else
        {
            if (this.IsCloseAble)
            {
                if (this.IsClosed && !string.IsNullOrEmpty(this.CloseDescription))
                {
                    description.AppendLine(this.CloseDescription);
                }

                if (!this.IsClosed && !string.IsNullOrEmpty(this.OpenDescription))
                {
                    description.AppendLine(this.OpenDescription);
                }
            }

        }

        if (!this.IsCloseAble || this.IsCloseAble && !this.IsClosed)
        {
            if (this.Items.Any(i => !i.IsHidden) || this.Characters.Any(c => !c.IsHidden))
            {
                description.AppendLine();
            }

            description.Append(this.PrintCharacters());
            description.Append(this.PrintItems());
        }
        
        description.Append(GetLinkedObjectsDescription(this, false));

        return description.ToString();
    }
}
