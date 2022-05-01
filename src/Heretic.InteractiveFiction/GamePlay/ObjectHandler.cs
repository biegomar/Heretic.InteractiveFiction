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
        return this.GetKeyByName(itemName, this.universe.CharacterResources);
    }
    
    internal string GetLocationKeyByName(string locationName)
    {
        return this.GetKeyByName(locationName, this.universe.LocationResources);
    }
    
    internal string GetConversationAnswerKeyByName(string phrase)
    {
        return this.GetKeyByName(phrase, this.universe.ConversationAnswersResources);
    }
    
    internal string GetItemKeyByName(string itemName)
    {
        if (GetPrioritizedItemKeys(itemName) is { } itemKey && !string.IsNullOrEmpty(itemKey))
        {
            return itemKey;
        }

        return this.GetKeyByName(itemName, this.universe.ItemResources);
    }
    
    internal Item GetVirtualItemByName(string itemName)
    {
        return this.GetVirtualItemByKey(this.GetItemKeyByName(itemName));
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
        var allActiveLocationItemKeys = this.GetItemKeysRecursive(this.universe.ActiveLocation.Items)
            .Union(this.GetSurroundingKeys(this.universe.ActiveLocation.Surroundings));
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
    
    private IEnumerable<string> GetSurroundingKeys(IDictionary<string, Func<string>> surroundings)
    {
        return surroundings.Keys.ToList();
    }
}