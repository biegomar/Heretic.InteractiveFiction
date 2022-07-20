using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class ObjectHandler
{
    private readonly Universe universe;

    internal ObjectHandler(Universe universe)
    {
        this.universe = universe;
    }
    
    internal string GetCharacterKeyByName(string itemName)
    {
        var key = this.GetKeyByName(itemName, this.universe.CharacterResources);

        if (string.IsNullOrEmpty(key))
        {
            var upperItemName = itemName.ToUpperInvariant();
            if (upperItemName == this.universe.ActivePlayer.Grammar.GetAccusativePronoun().ToUpperInvariant() 
                || upperItemName == this.universe.ActivePlayer.Grammar.GetDativePronoun().ToUpperInvariant()  
                || upperItemName == this.universe.ActivePlayer.Grammar.GetAccusativePronoun().ToUpperInvariant())
            {
                key = this.universe.ActivePlayer.Key;
            }
        }

        return key;
    }
    
    internal Character GetUnhiddenCharacterByName(string itemName)
    {
        return this.GetUnhiddenCharacterByKey(this.GetCharacterKeyByName(itemName));
    }
    
    internal Character GetUnhiddenCharacterByNameFromActiveLocation(string itemName)
    {
        return this.GetUnhiddenCharacterByKeyFromActiveLocation(this.GetCharacterKeyByName(itemName));
    }
    
    private Character GetUnhiddenCharacterByKey(string key)
    {
        if (this.universe.GetObjectFromWorldByKey(key) is Character { IsHidden: false } character)
        {
            return character;
        }

        return default;
    }
    
    internal string GetLocationKeyByName(string locationName)
    {
        return this.GetKeyByName(locationName, this.universe.LocationResources);
    }
    
    internal Item GetVirtualItemByName(string itemName)
    {
        return this.GetVirtualItemByKey(this.GetItemKeyByName(itemName));
    }

    
    internal string GetItemKeyByName(string itemName)
    {
        if (GetPrioritizedItemKeys(itemName) is { } itemKey && !string.IsNullOrEmpty(itemKey))
        {
            return itemKey;
        }

        return this.GetKeyByName(itemName, this.universe.ItemResources);
    }
    
    internal Item GetUnhiddenItemByNameActive(string itemName)
    {
        return this.GetUnhiddenItemByKeyActive(this.GetItemKeyByName(itemName));
    }
    
    internal Item GetUnhiddenItemByNameFromActiveLocation(string itemName)
    {
        return this.GetUnhiddenItemByKeyFromActiveLocation(this.GetItemKeyByName(itemName));
    }
    
    internal Item GetUnhiddenItemByNameFromActivePlayer(string itemName)
    {
        return this.GetUnhiddenItemByKeyFromActivePlayer(this.GetItemKeyByName(itemName));
    }
    
    internal AHereticObject GetUnhiddenObjectFromWorldByName(string itemName)
    {
        var item = this.universe.GetObjectFromWorldByKey(this.GetItemKeyByName(itemName));

        if (item == default || item.IsHidden)
        {
            return default;
        }

        return item;
    }
    
    internal AHereticObject GetUnhiddenObjectByName(string objectName)
    {
        AHereticObject containerObject = this.GetUnhiddenItemByNameActive(objectName);
        if (containerObject == default)
        {
            containerObject = this.GetUnhiddenCharacterByName(objectName);
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

    internal string GetConversationAnswerKeyByName(string phrase)
    {
        return this.GetKeyByName(phrase, this.universe.ConversationAnswersResources);
    }

    internal void HideItemsOnClose(AHereticObject item)
    {
        if (item.IsClosed)
        {
            foreach (var child in item.Items.Where(x => x.HideOnContainerClose))
            {
                child.IsHidden = true;
            }
        }
    }
    
    internal void ClearActiveObject()
    {
        this.universe.ActiveObject = default;
    }

    internal void ClearActiveObjectIfNotInInventory()
    {
        var universeActiveObject = this.universe.ActiveObject;
        if (universeActiveObject != default)
        {
            var item = this.universe.ActivePlayer.GetUnhiddenItemByKey(universeActiveObject.Key);
            if (item == default)
            {
                this.ClearActiveObject();
            }
        }
    }
    
    internal void StoreAsActiveObject(AHereticObject hereticObject)
    {
        if (hereticObject != default)
        {
            this.universe.ActiveObject = hereticObject;
        }
    }

    private Item GetUnhiddenItemByKeyActive(string key)
    {
        var result = this.universe.ActiveLocation.GetUnhiddenItemByKey(key);
        if (result == default)
        {
            result = this.universe.ActivePlayer.GetUnhiddenItemByKey(key);
        }

        return result;
    }
    
    private Character GetUnhiddenCharacterByKeyFromActiveLocation(string key)
    {
        return this.universe.ActiveLocation.GetUnhiddenCharacterByKey(key);
    }
    
    private Item GetUnhiddenItemByKeyFromActiveLocation(string key)
    {
        return this.universe.ActiveLocation.GetUnhiddenItemByKey(key);
    }
    
    private Item GetUnhiddenItemByKeyFromActivePlayer(string key)
    {
        return this.universe.ActivePlayer.GetUnhiddenItemByKey(key);
    }
    
    private Item GetVirtualItemByKey(string key)
    {
        var result = this.universe.ActiveLocation.GetVirtualItemByKey(key);
        if (result == default)
        {
            result = this.universe.ActivePlayer.GetVirtualItemByKey(key);
        }

        return result;
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
        if (universeActiveObject != null && (upperItemName == universeActiveObject.Grammar.GetAccusativePronoun().ToUpperInvariant() 
                                             || upperItemName == universeActiveObject.Grammar.GetDativePronoun().ToUpperInvariant()  
                                             || upperItemName == universeActiveObject.Grammar.GetNominativePronoun().ToUpperInvariant()))
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
            if (value.Contains(itemName, StringComparer.InvariantCultureIgnoreCase))
            {
                return key;
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