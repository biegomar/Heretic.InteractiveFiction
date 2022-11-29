using System.Reflection;
using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.GamePlay;

public sealed class ObjectHandler
{
    private readonly Universe universe;

    public ObjectHandler(Universe universe)
    {
        this.universe = universe;
    }

    public string GetObjectKeyByNameAndAdjectives<T>(string objectName, IEnumerable<string> adjectives = null) where T : AHereticObject
    {
        var typeofT = typeof(T);
        var typeOfCharacter = typeof(Character);
        var typeOfItem = typeof(Item);
        var typeOfLocation = typeof(Location);
        var typeOfPlayer = typeof(Player);
        
        if (typeofT == typeOfCharacter || typeofT == typeOfPlayer)
        {
            return GetCharacterKeyByNameAndAdjectives(objectName, adjectives);
        }
        else if (typeofT == typeOfItem)
        {
            return GetItemKeyByNameAndAdjectives(objectName, adjectives);
        }
        else if (typeofT == typeOfLocation)
        {
            return GetLocationKeyByNameAndAdjectives(objectName, adjectives);
        }

        return string.Empty;
    }
    public Character GetUnhiddenCharacterByNameAndAdjectives(string itemName, IEnumerable<string> adjectives = null)
    {
        return this.GetUnhiddenCharacterByKey(this.GetCharacterKeyByNameAndAdjectives(itemName, adjectives));
    }
    public Character GetUnhiddenCharacterByNameAndAdjectivesFromActiveLocation(string itemName, IEnumerable<string> adjectives = null)
    {
        return this.GetUnhiddenCharacterByKeyFromActiveLocation(this.GetCharacterKeyByNameAndAdjectives(itemName, adjectives));
    }
    public Item GetVirtualItemByNameAndAdjectives(string itemName, IEnumerable<string> adjectives = null)
    {
        return this.GetVirtualItemByKey(this.GetItemKeyByNameAndAdjectives(itemName, adjectives));
    }
    public AHereticObject GetObjectFromWorldByKey(string key)
    {
        if (key == this.universe.ActivePlayer.Key)
        {
            return this.universe.ActivePlayer;
        }
        
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
    public AHereticObject GetObjectFromWorldByNameAndAdjectives(string objectName, IEnumerable<string> adjectives = null)
    {
        var objectKey = this.GetKeyByNameAndAdjectivesFromAllResources(objectName, adjectives);
        return !string.IsNullOrEmpty(objectKey) ? this.GetObjectFromWorldByKey(objectKey) : default;
    }
    public Item GetUnhiddenItemByNameAndAdjectivesActive(string itemName, IEnumerable<string> adjectives = null)
    {
        return this.GetUnhiddenItemByKeyActive(this.GetItemKeyByNameAndAdjectives(itemName, adjectives));
    }
    public AHereticObject GetUnhiddenObjectFromWorldByNameAndAdjectives(string objectName, IEnumerable<string> adjectives = null)
    {
        var item = this.GetObjectFromWorldByKey(this.GetKeyByNameAndAdjectivesFromAllResources(objectName, adjectives));

        if (item == default || item.IsHidden)
        {
            return default;
        }

        return item;
    }
    public AHereticObject GetUnhiddenObjectByNameAndAdjectivesActive(string objectName, IEnumerable<string> adjectives = null)
    {
        AHereticObject containerObject = this.GetUnhiddenItemByNameAndAdjectivesActive(objectName, adjectives);
        if (containerObject == default)
        {
            containerObject = this.GetUnhiddenCharacterByNameAndAdjectivesFromActiveLocation(objectName, adjectives);
        }

        if (containerObject == default)
        {
            var key = this.GetCharacterKeyByNameAndAdjectives(objectName, adjectives);
            if (key == this.universe.ActivePlayer.Key)
            {
                containerObject = this.universe.ActivePlayer;
            }
        }

        return containerObject;
    }

    public bool IsObjectUnhiddenAndInInventoryOrActiveLocation(AHereticObject item)
    {
        return !item.IsHidden && (this.universe.ActiveLocation.OwnsObject(item) || this.universe.ActivePlayer.OwnsObject(item));
    }
    
    public string GetConversationAnswerKeyByName(string phrase)
    {
        return this.GetKeyByNameAndAdjectivesFromResource(phrase, this.universe.ConversationAnswersResources, new List<string>());
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
    private string GetKeyByNameAndAdjectivesFromAllResources(string name, IEnumerable<string> adjectives)
    {
        var key = this.GetCharacterKeyByNameAndAdjectives(name, adjectives);
        if (string.IsNullOrEmpty(key))
        {
            key = this.GetItemKeyByNameAndAdjectives(name, adjectives);
            
            if (string.IsNullOrEmpty(key))
            {
                key = this.GetLocationKeyByNameAndAdjectives(name, adjectives);
                
                if (string.IsNullOrEmpty(key))
                {
                    key = this.GetConversationAnswerKeyByName(name);
                }
            }
        }

        return key;
    }
    private string GetKeyByNameAndAdjectivesFromResource(string name, IDictionary<string, IEnumerable<string>> resource, IEnumerable<string> adjectives)
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
    private string GetPrioritizedItemKeysByNameAndAdjectives(string itemName, IEnumerable<string> adjectives)
    {
        var result = this.GetFirstPriorityItemKeyByName(itemName);
        if (string.IsNullOrEmpty(result))
        {
            return this.GetSecondPriorityItemKeysByNameAndAdjectives(itemName, adjectives);
        }

        return result;
    }
    private string GetFirstPriorityItemKeyByName(string itemName)
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
    private string GetSecondPriorityItemKeysByNameAndAdjectives(string itemName, IEnumerable<string> adjectives)
    {
        var allItemKeysFromActiveLocation = this.GetItemKeysRecursive(this.universe.ActiveLocation.Items);
        var allItemKeysFromActivePlayer = this.GetItemKeysRecursive(this.universe.ActivePlayer.Items);
        var prioritizedKeysOfActiveLocationAndPlayer = allItemKeysFromActiveLocation.Union(allItemKeysFromActivePlayer).ToList();
        var prioritizedItemResources =
            this.universe.ItemResources.Where(x => prioritizedKeysOfActiveLocationAndPlayer.Contains(x.Key)).ToList();
        var onlyItemsWithItemNameInValues =
            prioritizedItemResources.Where(res => res.Value.Contains(itemName)).ToList();

        if (onlyItemsWithItemNameInValues.Any())
        {
            switch (onlyItemsWithItemNameInValues.Count)
            {
                case 1:
                    return onlyItemsWithItemNameInValues.Single().Key;
                case > 1:
                    return this.GetItemKeyMatchingAdjectives(onlyItemsWithItemNameInValues.Select(x => x.Key).ToList(),
                        adjectives);
            }
        }

        return string.Empty;
    }

    private string GetItemKeyMatchingAdjectives(IEnumerable<string> itemKeys, IEnumerable<string> adjectives)
    {
        var itemList = itemKeys.Select(this.GetObjectFromWorldByKey<Item>).ToList();
        List<KeyValuePair<string, List<string>>> reducedType = new();
        foreach (var item in itemList)
        {
            var allDeclinedAdjectives = new List<string>(AdjectiveDeclinationHandler.GetAllDeclinedAdjectivesForAllCases(item));
            reducedType.Add(new KeyValuePair<string, List<string>>(item.Key, allDeclinedAdjectives));
        }

        var result = reducedType.Where(x => x.Value.Intersect(adjectives).Any()).Select(x => x.Key).ToList();

        if (result.Any())
        {
            if (result.Count == 1)
            {
                return result.Single();
            }
            throw new AmbiguousHereticObjectException(BaseDescriptions.AMBIGUOUS_HERETICOBJECT);
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
    private Character GetUnhiddenCharacterByKey(string key)
    {
        if (this.GetObjectFromWorldByKey(key) is Character { IsHidden: false } character)
        {
            return character;
        }

        return default;
    }
    private string GetLocationKeyByNameAndAdjectives(string locationName, IEnumerable<string> adjectives = null)
    {
        return this.GetKeyByNameAndAdjectivesFromResource(locationName, this.universe.LocationResources, adjectives);
    }
    private string GetCharacterKeyByNameAndAdjectives(string itemName, IEnumerable<string> adjectives = null)
    {
        var key = this.GetKeyByNameAndAdjectivesFromResource(itemName, this.universe.CharacterResources, adjectives);

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
    private string GetItemKeyByNameAndAdjectives(string itemName, IEnumerable<string> adjectives = null)
    {
        var adjectivesList = adjectives?.ToList();
        if (GetPrioritizedItemKeysByNameAndAdjectives(itemName, adjectivesList) is { } itemKey && !string.IsNullOrEmpty(itemKey))
        {
            return itemKey;
        }

        return this.GetKeyByNameAndAdjectivesFromResource(itemName, this.universe.ItemResources, adjectivesList);
    }
}