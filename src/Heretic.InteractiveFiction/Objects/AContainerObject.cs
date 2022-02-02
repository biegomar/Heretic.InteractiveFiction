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
    public string Description { get; init; }
    /// <summary>
    /// The first look description is only used during the first printout and contains additional information.
    /// </summary>
    public string FirstLookDescription { get; set; }
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
    /// Is this a physical object or just virtual? 
    /// </summary>
    public bool IsVirtual { get; set; }
    /// <summary>
    /// Can the object picked up?
    /// </summary>
    public bool IsPickAble { get; set; }

    /// <summary>
    /// Can the player sit on this object?
    /// </summary>
    public bool IsSeatAble { get; set; }

    /// <summary>
    /// If the object cannot be taken, this description can explain why.
    /// </summary>
    public string UnPickAbleDescription { get; set; }
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
    /// Gives a more detailed description about the state of a locked object.
    /// </summary>
    public string LockDescription { get; set; }
    /// <summary>
    /// Can this object be closed?
    /// </summary>
    public bool IsCloseAble { get; init; }
    /// <summary>
    /// Is this object closed?
    /// </summary>
    public bool IsClosed { get; set; }
    /// <summary>
    /// Gives a more detailed description about the state of an opened object.
    /// </summary>
    public string OpenDescription { get; init; }
    /// <summary>
    /// Gives a more detailed description about the state of a closed object.
    /// </summary>
    public string CloseDescription { get; init; }

    /// <summary>
    /// The list of contained objects.
    /// </summary>
    public ICollection<Item> Items { get; init; }

    /// <summary>
    /// The list of surrounding objects.
    /// </summary>
    public IDictionary<string, string> Surroundings { get; init; }

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
    public event EventHandler<ContainerObjectEventArgs> AfterTake;
    public event EventHandler<ContainerObjectEventArgs> BeforeSitDown;
    public event EventHandler<ContainerObjectEventArgs> AfterSitDown; 
    public event EventHandler<ContainerObjectEventArgs> BeforeStandUp;
    public event EventHandler<ContainerObjectEventArgs> AfterStandUp;
    public event EventHandler<ContainerObjectEventArgs> Buy;
    public event EventHandler<ContainerObjectEventArgs> Pull;
    public event EventHandler<PushItemEventArgs> Push;
    public event EventHandler<ContainerObjectEventArgs> Turn;
    public event EventHandler<UnlockContainerEventArgs> Unlock;
    public event EventHandler<UseItemEventArgs> Use;
    public event EventHandler<WriteEventArgs> Write;

    public virtual void OnUse(UseItemEventArgs eventArgses)
    {
        EventHandler<UseItemEventArgs> localEventHandler = this.Use;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgses);
        }
        else
        {
            throw new UseException(BaseDescriptions.NOTHING_HAPPENS);
        }
    }
    
    public virtual void OnPull(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.Pull;
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
        EventHandler<PushItemEventArgs> localEventHandler = this.Push;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new PushException(BaseDescriptions.NOTHING_HAPPENS);
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
        EventHandler<WriteEventArgs> localEventHandler = this.Write;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBuy(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.Buy;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            throw new BuyException(BaseDescriptions.ON_BUY_EXCEPTION);
        }
    }

    public virtual void OnBeforeDrop(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.BeforeDrop;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterDrop(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.AfterDrop;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBeforeEat(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.BeforeEat;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterEat(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.AfterEat;
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
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.BeforeSitDown;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterSitDown(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.AfterSitDown;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBeforeStandUp(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.BeforeStandUp;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterStandUp(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.AfterStandUp;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterGive(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.AfterGive;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterLook(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.AfterLook;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterTake(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.AfterTake;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeChangeLocation(ChangeLocationEventArgs eventArgs)
    {
        EventHandler<ChangeLocationEventArgs> localEventHandler = this.BeforeChangeLocation;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual ChangeLocationStatus OnAfterChangeLocation(ChangeLocationEventArgs eventArgs)
    {
        EventHandler<ChangeLocationEventArgs> localEventHandler = this.AfterChangeLocation;
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
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.BeforeClose;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterClose(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.AfterClose;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeOpen(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.BeforeOpen;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterOpen(ContainerObjectEventArgs eventArgs)
    {
        EventHandler<ContainerObjectEventArgs> localEventHandler = this.AfterOpen;
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
        this.Surroundings = new Dictionary<string, string>();
        this.LinkedTo = new List<Item>();
        this.FirstLookDescription = string.Empty;
        this.IsBreakable = false;
        this.IsBroken = false;
        this.IsEatable = false;
        this.IsHidden = false;
        this.IsVirtual = false;
        this.IsPickAble = true;
        this.IsUnveilAble = true;
        this.IsLockAble = false;
        this.IsLocked = false;
        this.IsClosed = false;
        this.IsCloseAble = false;
        this.IsSeatAble = false;
        this.name = string.Empty;
        this.Description = string.Empty;
        this.OpenDescription = string.Empty;
        this.CloseDescription = string.Empty;
        this.UnPickAbleDescription = string.Empty;
        this.LockDescription = string.Empty;
        this.ContainmentDescription = string.Empty;
        this.BrokenDescription = string.Empty;
        this.UnbreakableDescription = string.Empty;
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
                        if (unhiddenObjects.Count == 2)
                        {
                            description.Append($" {BaseDescriptions.AND} ");
                        }
                        else
                        {
                            description.Append(index == unhiddenObjects.Count - 1 ? $" {BaseDescriptions.AND} " : ", ");
                        }
                    }

                    if (index != 0)
                    {
                        var lowerName = item.Name.First().ToString().ToLower() + item.Name.Substring(1);
                        description.Append($"{lowerName}");
                    }
                    else
                    {
                        if (subItems)
                        {
                            var lowerName = item.Name.First().ToString().ToLower() + item.Name.Substring(1);
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

    private string GetLinkedObjectsDescription(AContainerObject item)
    {
        var description = new StringBuilder();
        if (item.LinkedTo.Any())
        {
            
            var unhiddenLinkedItemsWithContainment = item.LinkedTo.Where(x => !x.IsHidden && !string.IsNullOrEmpty(x.ContainmentDescription)).ToList();
            var unhiddenLinkedItemsWithoutContainment =
                item.LinkedTo.Except(unhiddenLinkedItemsWithContainment).ToList();
            
            description.Append(" (");
            
            int linkedItemIndex = 0;
            foreach (var linkedItem in unhiddenLinkedItemsWithContainment)
            {
                if (linkedItemIndex > 0)
                {
                    description.Append(' ');
                }
                
                description.Append(linkedItem.ContainmentDescription);
                
                linkedItemIndex++;
            }
            
            linkedItemIndex = 0;
            foreach (var linkedItem in unhiddenLinkedItemsWithoutContainment)
            {
                if (linkedItemIndex == 0)
                {
                    description.Append(' ').Append(BaseDescriptions.LINKED_TO);
                    
                }
                else
                {
                    description.Append(", ");
                }
                
                description.Append(linkedItem.Name.First().ToString().ToLower());
                description.Append(linkedItem.Name.Substring(1));

                linkedItemIndex++;
            }

            description.Append(')');
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

    public Item GetUnhiddenItemByKey(string key)
    {
        var itemsFromCharacter = this.Characters.Where(c => c.IsHidden == false).SelectMany(c => c.Items).Where(i => i.IsHidden == false)
            .Union(this.Characters.Where(c => c.IsHidden == false).SelectMany(c => c.LinkedTo).Where(i => i.IsHidden == false));
        
        var unhiddenItems = this.Items.Where(i => i.IsHidden == false)
            .Union(this.LinkedTo.Where(i => i.IsHidden == false))
            .Union(itemsFromCharacter).ToList();

        if (unhiddenItems.Any())
        {
            foreach (var item in unhiddenItems)
            {
                if (item.Key == key)
                {
                    return item;
                }
                var result = item.GetUnhiddenItemByKey(key);

                if (result != default)
                {
                    return result;
                }
            }
        }

        return default;
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

    public bool RemoveItem(Item itemToRemove)
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

    public int GetActualPayload()
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
        
        description.Append(GetLinkedObjectsDescription(this));

        return description.ToString();
    }
}
