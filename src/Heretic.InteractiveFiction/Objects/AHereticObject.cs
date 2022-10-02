using Heretic.InteractiveFiction.Grammars;
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
    public IndividualObjectGrammar Grammar { get; set; }
    
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
    public bool IsPickable { get; set; }

    /// <summary>
    /// Can the object be dropped? 
    /// </summary>
    public bool IsDropable { get; set; }

    /// <summary>
    /// Can the player sit on this object?
    /// </summary>
    public bool IsSeatable { get; set; }
    /// <summary>
    /// Can th player climb on this object?
    /// </summary>
    public bool IsClimbable { get; set; }
    /// <summary>
    /// Determines whether simply looking at the surroundings will make this object visible.
    /// </summary>
    public bool IsUnveilable { get; set; }
    /// <summary>
    /// Determines whether an object is lockable.
    /// </summary>
    public bool IsLockable { get; set; }
    /// <summary>
    /// Is the object locked?
    /// </summary>
    public bool IsLocked { get; set; }
    /// <summary>
    /// This is the id <see cref="AHereticObject.Key"/> of the object, that can unlock/lock this item.
    /// </summary>
    public string UnlockWithKey { get; set; }
    /// <summary>
    /// Can this object be closed?
    /// </summary>
    public bool IsCloseable { get; set; }
    /// <summary>
    /// Is this object closed?
    /// </summary>
    public bool IsClosed { get; set; }
    /// <summary>
    /// Can this object be eaten?
    /// </summary>
    public bool IsEatable { get; set; }
    /// <summary>
    /// Can this object be drunk?
    /// </summary>
    public bool IsDrinkable { get; set; }
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
    /// Indicates whether the object should be listed in the corresponding object list of a container or not.
    /// </summary>
    public bool IsShownInObjectList { get; set; }
    
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

    /// <summary>
    /// The list of additional adjectives for the object.
    /// </summary>
    public string Adjectives { get; set; }

    protected AHereticObject()
    {
        this.name = string.Empty;
        this.Grammar = new IndividualObjectGrammar();
        this.Items = new List<Item>();
        this.Characters = new List<Character>();
        this.LinkedTo = new List<Item>();
        
        InitializeStates();

        InitializeDescriptions();
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
    
    protected abstract StringBuilder ToStringExtension(); 
    
    
    public bool OwnsItem(string itemKey)
    {
        var item = this.GetItem(itemKey);
        return item != default;
    }

    public bool OwnsItem(Item item)
    {
        return this.OwnsItem(item.Key);
    }
    
    public bool OwnsCharacter(string itemKey)
    {
        var character = this.GetCharacter(itemKey);
        return character != default;
    }
    
    public bool OwnsCharacter(Character character)
    {
        return this.OwnsCharacter(character.Key);
    }

    internal virtual AHereticObject GetObject(string itemKey, ICollection<AHereticObject> visitedItems)
    {
        if (visitedItems.Contains(this))
        {
            return default;
        }
        
        visitedItems.Add(this);
        
        if (this.Key == itemKey)
        {
            return this;
        }

        foreach (var item in this.Items)
        {
            var result = item.GetObject(itemKey, visitedItems);
            if (result != default)
            {
                return result;
            }
        }
        
        foreach (var character in this.Characters)
        {
            var result = character.GetObject(itemKey, visitedItems);
            if (result != default)
            {
                return result;
            }
        }

        foreach (var item in this.LinkedTo)
        {
            var result = item.GetObject(itemKey, visitedItems);
            if (result != default)
            {
                return result;
            }
        }

        return default;
    }
    
    public AHereticObject GetObject(string itemKey)
    {
        var result = this.GetObject(itemKey, new List<AHereticObject>());
        return result ?? default;
    }
    
    public AHereticObject GetObject(AHereticObject item)
    {
        return this.GetObject(item.Key);
    }

    public Item GetItem(string itemKey)
    {
        var result = this.GetObject(itemKey, new List<AHereticObject>());
        return result as Item ?? default;
    }
    
    public Item GetItem(AHereticObject item)
    {
        return this.GetItem(item.Key);
    }
    
    public Item GetUnhiddenItem(string key)
    {
        var item = this.GetItem(key);

        return item is { IsHidden: false } ? item : default;
    }
    
    public Character GetCharacter(string itemKey)
    {
        var result = this.GetObject(itemKey, new List<AHereticObject>());
        return result as Character ?? default;
    }

    public Character GetCharacter(Character character)
    {
        return this.GetCharacter(character.Key);
    }
    
    public Character GetUnhiddenCharacter(string key)
    {
        var character = this.GetCharacter(key);

        return character is { IsHidden: false } ? character : default;
    }

    protected virtual string PrintCharacters()
    {
        var unhiddenItems = this.Characters.Where(i => !i.IsHidden && !i.IsSurrounding && i.IsShownInObjectList).ToList<AHereticObject>();

        return this.PrintUnhiddenObjects(unhiddenItems);
    }

    protected virtual string PrintItems(bool subItems = false)
    {

        var unhiddenNonSurroundingItems = this.Items.Where(i => !i.IsHidden && !i.IsSurrounding && i.IsShownInObjectList).ToList<AHereticObject>();

        return this.PrintUnhiddenObjects(unhiddenNonSurroundingItems);
    }

    public Item GetVirtualItem(string key)
    {
        var item = this.GetItem(key);

        return item is { IsVirtual: true } ? item : default;
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
        
        var unhiddenLinkedObjects = this.LinkedTo.Where(c => !c.IsHidden).ToList();
        if (unhiddenLinkedObjects.Any())
        {
            foreach (var item in unhiddenLinkedObjects)
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

    protected virtual string GetObjectName()
    {
        
        var names = this.name.Split('|');
        var article = ArticleHandler.GetArticleForObject(this, GrammarCase.Nominative);
        
        if (!string.IsNullOrEmpty(this.Adjectives))
        {
            return string.Format($"{article} {this.GetAdjectivesForName(GrammarCase.Nominative)} {names[0].Trim()}").Trim();    
        }
        return string.Format($"{article} {names[0].Trim()}").Trim();
    }

    internal virtual ICollection<string> GetNames()
    {
        return this.name.Split('|').ToList();
    }
    
    protected virtual string GetAdjectivesForName(GrammarCase grammarCase)
    {
        var adjectiveList = new List<string>();
        var splitList = this.Adjectives.Split('|').ToList();

        foreach (var item in splitList)
        {
            switch (grammarCase)
            {
                case GrammarCase.Nominative:
                {
                    adjectiveList.Add(item + this.Grammar.GetNominativeAdjectiveDeclination());
                    break;
                }
                case GrammarCase.Genitive:
                {
                    adjectiveList.Add(item + this.Grammar.GetGenitiveAdjectiveDeclination());
                    break;
                }
                case GrammarCase.Dative:
                {
                    adjectiveList.Add(item + this.Grammar.GetDativeAdjectiveDeclination());
                    break;
                }
                case GrammarCase.Accusative:
                {
                    adjectiveList.Add(item + this.Grammar.GetAccusativeAdjectiveDeclination());
                    break;
                }
            }
        }

        return string.Join(", ", adjectiveList);
    }
    
    private string GetNominativeIndefiniteArticleName()
    {
        var names = this.name.Split('|');
        var article = ArticleHandler.GetArticleForObject(this, GrammarCase.Nominative, ArticleState.Indefinite);
        
        if (!string.IsNullOrEmpty(this.Adjectives))
        {
            return string.Format($"{article} {this.GetAdjectivesForName(GrammarCase.Nominative)} {names[0].Trim()}").Trim();    
        }
        return string.Format($"{article} {names[0].Trim()}").Trim();
    }
    
    private string GetDativeIndefiniteArticleName()
    {
        var names = this.name.Split('|');
        var article = ArticleHandler.GetArticleForObject(this, GrammarCase.Dative, ArticleState.Indefinite);
        
        if (!string.IsNullOrEmpty(this.Adjectives))
        {
            return string.Format($"{article} {this.GetAdjectivesForName(GrammarCase.Dative)} {names[0].Trim()}").Trim();    
        }
        return string.Format($"{article} {names[0].Trim()}").Trim();
    }
    
    private string GetDativeArticleName()
    {
        var names = this.name.Split('|');
        var article = ArticleHandler.GetArticleForObject(this, GrammarCase.Dative);
        
        if (!string.IsNullOrEmpty(this.Adjectives))
        {
            return string.Format($"{article} {this.GetAdjectivesForName(GrammarCase.Dative)} {names[0].Trim()}").Trim();    
        }
        return string.Format($"{article} {names[0].Trim()}").Trim();
    }
    
    private string GetAccusativeIndefiniteArticleName()
    {
        var names = this.name.Split('|');
        var article = ArticleHandler.GetArticleForObject(this, GrammarCase.Accusative, ArticleState.Indefinite);
        
        if (!string.IsNullOrEmpty(this.Adjectives))
        {
            return string.Format($"{article} {this.GetAdjectivesForName(GrammarCase.Accusative)} {names[0].Trim()}").Trim();    
        }
        return string.Format($"{article} {names[0].Trim()}").Trim();
    }
    
    private string GetAccusativeArticleName()
    {
        var names = this.name.Split('|');
        var article = ArticleHandler.GetArticleForObject(this, GrammarCase.Accusative);
        
        if (!string.IsNullOrEmpty(this.Adjectives))
        {
            return string.Format($"{article} {this.GetAdjectivesForName(GrammarCase.Accusative)} {names[0].Trim()}").Trim();    
        }
        return string.Format($"{article} {names[0].Trim()}").Trim();
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

        var extension = this.ToStringExtension();
        if (extension != default)
        {
            description.Append(extension);
        }

        if (this.IsLocked && !string.IsNullOrEmpty(this.LockDescription))
        {
            description.AppendLine(this.LockDescription);
        }
        else
        {
            if (this.IsCloseable)
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

        if (!this.IsCloseable || this.IsCloseable && !this.IsClosed)
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

    private string GetResourceByKey()
    {
        var characterNames = this.name.Split('|');
        return string.Join(", ", characterNames);
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
                        var lowerName = item.NominativeIndefiniteArticleName.LowerFirstChar().Trim();
                        unhiddenObjectDescription.Append($"{lowerName}");
                    }
                    else
                    {
                        var lowerName = item.AccusativeIndefiniteArticleName.LowerFirstChar().Trim();
                        unhiddenObjectDescription.Append($"{lowerName}");
                    }

                    if (!item.IsCloseable || item.IsCloseable && !item.IsClosed)
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
        this.IsPickable = true;
        this.IsDropable = true;
        this.IsUnveilable = true;
        this.IsLockable = false;
        this.IsLocked = false;
        this.IsClosed = false;
        this.IsCloseable = false;
        this.IsSeatable = false;
        this.IsClimbable = false;
        this.IsEatable = false;
        this.IsDrinkable = false;
        this.IsReadable = false;
        this.IsShownInObjectList = true;
    }

    private void InitializeDescriptions()
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
        this.Hint = string.Empty;
        this.Adjectives = string.Empty;
    }
}