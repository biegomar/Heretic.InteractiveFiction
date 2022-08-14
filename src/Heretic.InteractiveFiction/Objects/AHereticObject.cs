using System.Runtime.CompilerServices;
using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public abstract partial class AHereticObject
{
    protected string name;

    /// <summary>
    /// This is the name of the object. This name is used as headline during printout.
    /// </summary>
    public string Name
    {
        get => this.GetObjectName();
        set => name = value;
    }
    
    public string AccusativeIndefiniteArticleName
    {
        get => this.GetAccusativeIndefiniteArticleName();
        set => name = value;
    }
    
    public string AccusativeArticleName
    {
        get => this.GetAccusativeArticleName();
        set => name = value;
    }
    
    public string NominativeIndefiniteArticleName
    {
        get => this.GetNominativeIndefiniteArticleName();
        set => name = value;
    }
    
    public string DativeIndefiniteArticleName
    {
        get => this.GetDativeIndefiniteArticleName();
        set => name = value;
    }
    
    public string DativeArticleName
    {
        get => this.GetDativeArticleName();
        set => name = value;
    }

    /// <summary>
    /// The unique key that is representing the object.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Used to define some grammar rules.
    /// </summary>
    public Grammars Grammar { get; set; }
    
    /// <summary>
    /// Can this object be broken?
    /// </summary>
    public bool IsBreakable { get; set; }
    /// <summary>
    /// Is this object broken?
    /// </summary>
    public bool IsBroken { get; set; }
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
    /// Determines whether simply looking at the surroundings will make this object visible.
    /// </summary>
    public bool IsUnveilAble { get; set; }
    /// <summary>
    /// Determines whether an object is lockable.
    /// </summary>
    public bool IsLockAble { get; set; }
    /// <summary>
    /// Is the object locked?
    /// </summary>
    public bool IsLocked { get; set; }
    /// <summary>
    /// Can this object be closed?
    /// </summary>
    public bool IsCloseAble { get; set; }
    /// <summary>
    /// Is this object closed?
    /// </summary>
    public bool IsClosed { get; set; }
    /// <summary>
    /// Is there anything to read on the object?
    /// </summary>
    public bool IsReadable { get; set; }
    /// <summary>
    /// Is this object a container and can it hold other objects?
    /// </summary>
    public bool IsContainer { get; set; }
    /// <summary>
    /// Is the object a container that holds its contents on the surface (e.g. a table)?
    /// </summary>
    public bool IsSurfaceContainer { get; set; }
    /// <summary>
    /// If the object is a surrounding, then it will not be listed in the room description.
    /// </summary>
    public bool IsSurrounding { get; set; }

    /// <summary>
    /// Is the object a light source?
    /// </summary>
    public bool IsLighter { get; set; }

    /// <summary>
    /// Is the light source switched on?
    /// </summary>
    public bool IsLighterSwitchedOn { get; set; }
    
    /// <summary>
    /// The weight of the object.
    /// </summary>
    public int Weight { get; set; }
    /// <summary>
    /// If this object is a container, you can specify how much weight it can hold.
    /// </summary>
    public int MaxPayload { get; set; }
    
    /// <summary>
    /// The list of contained objects.
    /// </summary>
    public ICollection<Item> Items { get; set; }

    public ICollection<Item> LinkedTo { get; set; }

    /// <summary>
    /// The list of contained characters.
    /// <example>
    /// A magician in a huge box.
    /// </example>
    /// </summary>
    public ICollection<Character> Characters { get; set; }

    protected AHereticObject()
    {
        this.name = string.Empty;
        this.Grammar = new Grammars();
        this.Items = new List<Item>();
        this.Characters = new List<Character>();
        this.LinkedTo = new List<Item>();
        
        InitializeStates();

        InitialzeDescriptions();
    }

    private void InitializeStates()
    {
        this.IsContainer = false;
        this.IsSurfaceContainer = false;
        this.IsSurrounding = false;
        this.IsBreakable = false;
        this.IsBroken = false;
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
        this.IsReadable = false;
        this.IsLighter = false;
        this.IsLighterSwitchedOn = false;
    }

    private void InitialzeDescriptions()
    {
        this.Description = string.Empty;
        this.FirstLookDescription = string.Empty;
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
        this.LetterContentDescription = string.Empty;
        this.LighterSwitchedOffDescription = string.Empty;
        this.LighterSwitchedOnDescription = string.Empty;
        this.Hint = string.Empty;
    }

    protected virtual string GetVariationOfYouSee(int itemCount)
    {
        if (this.IsSurfaceContainer)
        {
            return itemCount == 1 ? BaseDescriptions.YOU_SEE_SURFACE_SINGULAR : BaseDescriptions.YOU_SEE_SURFACE;
        }
            
        return itemCount == 1 ? BaseDescriptions.YOU_SEE_SINGULAR : BaseDescriptions.YOU_SEE;
    }

    protected virtual string GetVariationOfHereSingle()
    {
        return BaseDescriptions.HERE;
    }

    private string PrintUnhiddenObjects(ICollection<AHereticObject> unhiddenObjects, bool subItems = false)
    {
        var unhiddenObjectDescription = new StringBuilder();

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
                    var index = 1;
                    foreach (var item in unHiddenObjectsWithContainmentDescription)
                    {
                        unhiddenObjectDescription.Append(item.ContainmentDescription);
                        if (index == unHiddenObjectsWithContainmentDescription.Count && !unHiddenObjectsWithoutContainmentDescription.Any())
                        {
                            unhiddenObjectDescription.Append(GetLinkedObjectsDescription(item));    
                        }
                        else
                        {
                            unhiddenObjectDescription.AppendLine(GetLinkedObjectsDescription(item));
                        }

                        index++;
                    }
                }
            }

            if (unHiddenObjectsWithoutContainmentDescription.Any())
            {
                if (subItems)
                {
                    unhiddenObjectDescription.Append($" ({GetVariationOfYouSee(unHiddenObjectsWithoutContainmentDescription.Count)} ");
                }
                else
                {
                    unhiddenObjectDescription.Append($"{GetVariationOfHereSingle()}");
                }

                var index = 0;

                foreach (var item in unHiddenObjectsWithoutContainmentDescription)
                {
                    if (index != 0)
                    {
                        if (unHiddenObjectsWithoutContainmentDescription.Count == 2)
                        {
                            unhiddenObjectDescription.Append($" {BaseDescriptions.AND} ");
                        }
                        else
                        {
                            unhiddenObjectDescription.Append(index == unHiddenObjectsWithoutContainmentDescription.Count - 1 ? $" {BaseDescriptions.AND} " : ", ");
                        }
                    }

                    if (subItems)
                    {
                        var lowerName = item.NominativeIndefiniteArticleName.LowerFirstChar();
                        unhiddenObjectDescription.Append($"{lowerName}");
                    }
                    else
                    {
                        var lowerName = item.AccusativeIndefiniteArticleName.LowerFirstChar();
                        unhiddenObjectDescription.Append($"{lowerName}");
                    }

                    if (!item.IsCloseAble || item.IsCloseAble && !item.IsClosed)
                    {
                        if (item.Items.Any(i => i.IsHidden == false))
                        {
                            var subItemList = item.Items.Where(i => i.IsHidden == false).ToList<AHereticObject>();
                            unhiddenObjectDescription.Append(item.PrintUnhiddenObjects(subItemList, true));
                        }
                    }

                    if (!subItems)
                    {
                        unhiddenObjectDescription.Append(GetLinkedObjectsDescription(item));
                    }

                    index++;
                }

                if (subItems)
                {
                    unhiddenObjectDescription.Append(')');
                }
                else
                {
                    unhiddenObjectDescription.Append('.');
                }
            }

            if (!subItems)
            {
                unhiddenObjectDescription.AppendLine();    
            }
                
        }

        return unhiddenObjectDescription.ToString();
    }
    
    private string GetLinkedObjectsDescription(AHereticObject item, bool useBracket = true)
    {
        var linkedObjectDescription = new StringBuilder();
        var unhiddenLinkedItemsWithLinkedTo = item.LinkedTo.Where(x => !x.IsHidden && !string.IsNullOrEmpty(x.LinkedToDescription)).ToList();
        var unhiddenLinkedItemsWithoutLinkedTo = item.LinkedTo.Where(x => !x.IsHidden && string.IsNullOrEmpty(x.LinkedToDescription)).ToList();
        if (unhiddenLinkedItemsWithLinkedTo.Any() ||unhiddenLinkedItemsWithoutLinkedTo.Any())
        {
            if (useBracket)
            {
                linkedObjectDescription.Append(" (");    
            }

            var linkedItemIndex = 0;
            foreach (var linkedItem in unhiddenLinkedItemsWithLinkedTo)
            {
                if (linkedItemIndex > 0)
                {
                    linkedObjectDescription.Append(' ');
                }
                
                linkedObjectDescription.Append(linkedItem.LinkedToDescription);
                
                linkedItemIndex++;
            }
            
            linkedItemIndex = 0;
            foreach (var linkedItem in unhiddenLinkedItemsWithoutLinkedTo)
            {
                if (linkedItemIndex == 0)
                {
                    linkedObjectDescription.Append(string.Format(BaseDescriptions.LINKED_TO, item.Name));
                    
                }
                else
                {
                    linkedObjectDescription.Append(", ");
                }
                
                linkedObjectDescription.Append(linkedItem.DativeIndefiniteArticleName.LowerFirstChar());

                linkedItemIndex++;
            }

            if (linkedItemIndex > 0)
            {
                linkedObjectDescription.Append('.');
            }

            if (useBracket)
            {
                linkedObjectDescription.Append(')');    
            }
        }

        return linkedObjectDescription.ToString();
    }

    protected virtual string PrintCharacters()
    {
        var unhiddenItems = this.Characters.Where(i => !i.IsHidden && !i.IsSurrounding).ToList<AHereticObject>();

        return this.PrintUnhiddenObjects(unhiddenItems);
    }

    protected virtual string PrintItems(bool subItems = false)
    {

        var unhiddenNonSurroundingItems = this.Items.Where(i => !i.IsHidden && !i.IsSurrounding).ToList<AHereticObject>();

        return this.PrintUnhiddenObjects(unhiddenNonSurroundingItems);
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

    /// <summary>
    /// Tries to get an item from the list of items or from the list of characters recursively.
    /// </summary>
    /// <param name="key">Key of the item.</param>
    /// <returns>The item, otherwise default.</returns>
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

    /// <summary>
    /// Tries to remove an item from the list of items or from the list of characters recursively.
    /// </summary>
    /// <param name="itemToRemove">The item to be removed.</param>
    /// <returns>True if successful, false otherwise.</returns>
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
            
        foreach (var item in this.Items)
        {
            var result = item.GetCharacterByKey(key);

            if (result != default)
            {
                return result;
            }
        }

        return default;
    }

    public AHereticObject GetOwnerOfUnhiddenItemByKey(string key)
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

    protected virtual string GetObjectName()
    {
        var sentence = this.name.Split('|');
        return string.Format($"{this.Grammar.GetArticle()} {sentence[0].Trim()}").Trim();
    }

    private string GetNominativeIndefiniteArticleName()
    {
        var sentence = this.name.Split('|');
        var nominative = this.Grammar.GetNominativeIndefiniteArticle();
        if (string.IsNullOrEmpty(nominative))
        {
            return $" {sentence[0].Trim()}";    
        }
        
        return $"{nominative} {sentence[0].Trim()}";
    }
    
    private string GetDativeIndefiniteArticleName()
    {
        var sentence = this.name.Split('|');
        return string.Format($"{this.Grammar.GetDativeIndefiniteArticle()} {sentence[0].Trim()}");
    }
    
    private string GetDativeArticleName()
    {
        var sentence = this.name.Split('|');
        return string.Format($"{this.Grammar.GetDativeArticle()} {sentence[0].Trim()}");
    }
    
    private string GetAccusativeIndefiniteArticleName()
    {
        var sentence = this.name.Split('|');
        return string.Format($"{this.Grammar.GetAccusativeIndefiniteArticle()} {sentence[0].Trim()}");
    }
    
    private string GetAccusativeArticleName()
    {
        var sentence = this.name.Split('|');
        return string.Format($"{this.Grammar.GetAccusativeArticle()} {sentence[0].Trim()}");
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