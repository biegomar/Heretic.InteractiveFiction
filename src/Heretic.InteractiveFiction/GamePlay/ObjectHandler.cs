using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

public sealed class ObjectHandler
{
    private readonly Universe universe;

    public ObjectHandler(Universe universe)
    {
        this.universe = universe;
    }

    public string GetObjectKeyByName<T>(string objectName) where T : AHereticObject
    {
        var typeofT = typeof(T);
        var typeOfCharacter = typeof(Character);
        var typeOfItem = typeof(Item);
        var typeOfLocation = typeof(Location);
        var typeOfPlayer = typeof(Player);
        
        if (typeofT == typeOfCharacter || typeofT == typeOfPlayer)
        {
            return GetCharacterKeyByName(objectName);
        }
        else if (typeofT == typeOfItem)
        {
            return GetItemKeyByName(objectName);
        }
        else if (typeofT == typeOfLocation)
        {
            return GetLocationKeyByName(objectName);
        }

        return string.Empty;
    }
    public string GetCharacterKeyByName(string itemName)
    {
        var key = this.GetKeyByName(itemName, this.universe.CharacterResources);

        if (string.IsNullOrEmpty(key))
        {
            var upperItemName = itemName.ToUpperInvariant();

            if (upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Nominative, PersonView.FirstPerson).ToUpperInvariant()
                || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Nominative).ToUpperInvariant()
                || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Genitive,PersonView.FirstPerson).ToUpperInvariant()
                || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Genitive).ToUpperInvariant()
                || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Dative,PersonView.FirstPerson).ToUpperInvariant()
                || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Dative).ToUpperInvariant()
                || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Accusative,PersonView.FirstPerson).ToUpperInvariant()
                || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Accusative).ToUpperInvariant()
                || upperItemName == this.universe.ActivePlayer.Name.ToUpperInvariant())
            {
                key = this.universe.ActivePlayer.Key;
            }
        }

        return key;
    }
    
    public Character GetUnhiddenCharacterByName(string itemName)
    {
        return this.GetUnhiddenCharacterByKey(this.GetCharacterKeyByName(itemName));
    }
    
    public Character GetUnhiddenCharacterByNameFromActiveLocation(string itemName)
    {
        return this.GetUnhiddenCharacterByKeyFromActiveLocation(this.GetCharacterKeyByName(itemName));
    }
    
    private Character GetUnhiddenCharacterByKey(string key)
    {
        if (this.GetObjectFromWorldByKey(key) is Character { IsHidden: false } character)
        {
            return character;
        }

        return default;
    }
    
    public string GetLocationKeyByName(string locationName)
    {
        return this.GetKeyByName(locationName, this.universe.LocationResources);
    }
    
    public Item GetVirtualItemByName(string itemName)
    {
        return this.GetVirtualItemByKey(this.GetItemKeyByName(itemName));
    }
    
    public string GetItemKeyByName(string itemName)
    {
        if (GetPrioritizedItemKeys(itemName) is { } itemKey && !string.IsNullOrEmpty(itemKey))
        {
            return itemKey;
        }

        return this.GetKeyByName(itemName, this.universe.ItemResources);
    }
    
    public AHereticObject GetObjectFromWorldByKey(string key)
    {
        foreach (var location in this.universe.LocationMap.Keys)
        {
            var result = location.GetObject(key);
            if (result != default && result.Key == key)
            {
                return result;
            }
        }

        return this.universe.ActivePlayer.GetObject(key);
    }
    
    public T GetObjectFromWorldByKey<T>(string key) where T: AHereticObject
    {
        if (key == this.universe.ActivePlayer.Key)
        {
            return this.universe.ActivePlayer as T;
        }
        
        foreach (var location in this.universe.LocationMap.Keys)
        {
            var result = location.GetObject<T>(key);
            if (result != default && result.Key == key)
            {
                return result;
            }
        }

        return (T)this.universe.ActivePlayer.GetObject(key);
    }
    
    public AHereticObject GetObjectFromWorldByName(string key)
    {
        var objectKey = this.GetKeyByNameFromAllResources(key);
        return !string.IsNullOrEmpty(objectKey) ? this.GetObjectFromWorldByKey(objectKey) : default;
    }
    
    public Item GetUnhiddenItemByNameActive(string itemName)
    {
        return this.GetUnhiddenItemByKeyActive(this.GetItemKeyByName(itemName));
    }

    public AHereticObject GetUnhiddenObjectFromWorldByName(string itemName)
    {
        var item = this.GetObjectFromWorldByKey(this.GetItemKeyByName(itemName));

        if (item == default || item.IsHidden)
        {
            return default;
        }

        return item;
    }
    
    public AHereticObject GetUnhiddenObjectByNameActive(string objectName)
    {
        AHereticObject containerObject = this.GetUnhiddenItemByNameActive(objectName);
        if (containerObject == default)
        {
            containerObject = this.GetUnhiddenCharacterByNameFromActiveLocation(objectName);
        }

        if (containerObject == default)
        {
            var key = this.GetCharacterKeyByName(objectName);
            if (key == this.universe.ActivePlayer.Key)
            {
                containerObject = this.universe.ActivePlayer;
            }
        }

        return containerObject;
    }

    public string GetConversationAnswerKeyByName(string phrase)
    {
        return this.GetKeyByName(phrase, this.universe.ConversationAnswersResources);
    }

    public void HideItemsOnClose(AHereticObject item)
    {
        if (item.IsClosed)
        {
            foreach (var child in item.Items.Where(x => x.HideOnContainerClose))
            {
                child.IsHidden = true;
            }
        }
    }
    
    public void UnhideItemsOnOpen(AHereticObject item)
    {
        if (!item.IsClosed)
        {
            foreach (var child in item.Items.Where(x => x.HideOnContainerClose))
            {
                child.IsHidden = false;
            }
        }
    }
    
    public void ClearActiveObject()
    {
        this.universe.ActiveObject = default;
    }

    public void RemoveAsActiveObject(AHereticObject hereticObject)
    {
        if (hereticObject != default && this.universe.ActiveObject == hereticObject)
        {
            this.ClearActiveObject();
        }
    }

    public void ClearActiveObjectIfNotInInventory()
    {
        var universeActiveObject = this.universe.ActiveObject;
        if (universeActiveObject != default)
        {
            if (!this.universe.ActivePlayer.OwnsItem(universeActiveObject.Key))
            {
                this.ClearActiveObject();
            }
        }
    }
    
    public void StoreAsActiveObject(AHereticObject hereticObject)
    {
        if (hereticObject != default)
        {
            this.universe.ActiveObject = hereticObject;
        }
    }

    private Item GetUnhiddenItemByKeyActive(string key)
    {
        var result = this.universe.ActiveLocation.GetUnhiddenItem(key);
        if (result == default)
        {
            result = this.universe.ActivePlayer.GetUnhiddenItem(key);
        }

        return result;
    }
    
    private Character GetUnhiddenCharacterByKeyFromActiveLocation(string key)
    {
        return this.universe.ActiveLocation.GetUnhiddenCharacter(key);
    }
    
    private Item GetVirtualItemByKey(string key)
    {
        var result = this.universe.ActiveLocation.GetVirtualItem(key);
        if (result == default)
        {
            result = this.universe.ActivePlayer.GetVirtualItem(key);
        }

        return result;
    }

    private string GetKeyByNameFromAllResources(string name)
    {
        var key = this.GetCharacterKeyByName(name);
        if (string.IsNullOrEmpty(key))
        {
            key = this.GetItemKeyByName(name);
            
            if (string.IsNullOrEmpty(key))
            {
                key = this.GetLocationKeyByName(name);
                
                if (string.IsNullOrEmpty(key))
                {
                    key = this.GetConversationAnswerKeyByName(name);
                }
            }
        }

        return key;
    }
    
    private string GetKeyByName(string name, IDictionary<string, IEnumerable<string>> resource)
    {
        foreach (var (key, value) in resource)
        {
            if (value.Contains(name, StringComparer.InvariantCultureIgnoreCase))
            {
                return key;
            }
        }

        return string.Empty;
    }
    
    private string GetPrioritizedItemKeys(string itemName)
    {
        var result = this.GetFirstPriorityItemKey(itemName);
        if (string.IsNullOrEmpty(result))
        {
            return this.GetSecondPriorityItemKeys(itemName);
        }

        return result;
    }

    private string GetFirstPriorityItemKey(string itemName)
    {
        var upperItemName = itemName.ToUpperInvariant();
        var universeActiveObject = this.universe.ActiveObject;
        if (universeActiveObject != null && (upperItemName == PronounHandler.GetPronounForObject(universeActiveObject, GrammarCase.Nominative).ToUpperInvariant() 
                                             || upperItemName == PronounHandler.GetPronounForObject(universeActiveObject, GrammarCase.Genitive).ToUpperInvariant()
                                             || upperItemName == PronounHandler.GetPronounForObject(universeActiveObject, GrammarCase.Dative).ToUpperInvariant()
                                             || upperItemName == PronounHandler.GetPronounForObject(universeActiveObject, GrammarCase.Accusative).ToUpperInvariant()))
        {
            return this.universe.ActiveObject.Key;
        }

        return string.Empty;
    }

    private string GetSecondPriorityItemKeys(string itemName)
    {
        var allActiveLocationItemKeys = this.GetItemKeysRecursive(this.universe.ActiveLocation.Items);
        var allActivePlayerItemKeys = this.GetItemKeysRecursive(this.universe.ActivePlayer.Items);
        var prioritizedKeysOfActiveLocationAndPlayer = allActiveLocationItemKeys.Union(allActivePlayerItemKeys).ToList();
        var prioritizedItemResources =
            this.universe.ItemResources.Where(x => prioritizedKeysOfActiveLocationAndPlayer.Contains(x.Key));

        foreach (var (key, value) in prioritizedItemResources)
        {
            var enumerable = value.ToList();
            var keys = enumerable.Where(x => x.Equals(itemName, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (keys.Any())
            {
                if (keys.Count == 1)
                {
                    return key;
                }

                var item = this.GetObjectFromWorldByKey<Item>(key);
            }
        }

        return string.Empty;
    }
    
    private IEnumerable<string> GetItemKeysRecursive(IEnumerable<Item> items)
    {
        var result = new List<string>();
        foreach (var item in items)
        {
            if (item.Items.Any())
            {
                result.AddRange(this.GetItemKeysRecursive(item.Items));
            }

            //TODO - also recursive, but break on circular references.
            if (item.LinkedTo.Any())
            {
                result.AddRange(item.LinkedTo.Select(x => x.Key));
            }
            
            result.Add(item.Key);
        }

        return result;
    }
}