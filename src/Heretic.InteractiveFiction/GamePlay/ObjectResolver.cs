using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class ObjectResolver
{
    private readonly Universe universe;

    internal ObjectResolver(Universe universe)
    {
        this.universe = universe;
    }

    internal ResolveResult<T> Resolve<T>(string name, IEnumerable<string> context) where T : AHereticObject
    {
        try
        {
            var key = GetObjectKeyByNameAndAdjectives<T>(name, context);
            if (string.IsNullOrEmpty(key))
                return new ResolveResult<T>.NotFound();

            var obj = GetObjectFromWorldByKey<T>(key);
            return obj != null
                ? new ResolveResult<T>.Found(obj)
                : new ResolveResult<T>.NotFound();
        }
        catch (AmbiguousHereticObjectException)
        {
            var candidates = FindCandidates<T>(name);
            return new ResolveResult<T>.Ambiguous(candidates);
        }
    }

    internal string GetObjectKeyByNameAndAdjectives<T>(string objectName, IEnumerable<string>? adjectives = null) where T : AHereticObject
    {
        var typeofT = typeof(T);

        if (typeofT == typeof(Player))
            return GetPlayerKeyByName(objectName);

        if (typeofT == typeof(Character))
            return GetCharacterKeyByNameAndAdjectives(objectName, adjectives);

        if (typeofT == typeof(Item))
            return GetItemKeyByNameAndAdjectives(objectName, adjectives);

        if (typeofT == typeof(Location))
            return GetLocationKeyByNameAndAdjectives(objectName, adjectives);

        return string.Empty;
    }

    internal AHereticObject? GetObjectFromWorldByKey(string key)
    {
        if (key == this.universe.ActivePlayer.Key)
            return this.universe.ActivePlayer;

        foreach (var location in this.universe.LocationMap.Keys)
        {
            var result = location.GetObject(key);
            if (result != default && result.Key == key)
                return result;
        }

        return this.universe.ActivePlayer.GetObject(key);
    }

    internal T? GetObjectFromWorldByKey<T>(string key) where T : AHereticObject
    {
        if (key == this.universe.ActivePlayer.Key)
            return this.universe.ActivePlayer as T;

        foreach (var location in this.universe.LocationMap.Keys)
        {
            var result = location.GetObject<T>(key);
            if (result != default && result.Key == key)
                return result;
        }

        return (T?)this.universe.ActivePlayer.GetObject(key);
    }

    internal AHereticObject? GetObjectFromWorldByNameAndAdjectives(string objectName, IEnumerable<string>? adjectives = null)
    {
        var objectKey = GetKeyByNameAndAdjectivesFromAllResources(objectName, adjectives);
        return !string.IsNullOrEmpty(objectKey) ? GetObjectFromWorldByKey(objectKey) : default;
    }

    internal Item? GetUnhiddenItemByNameAndAdjectivesActive(string itemName, IEnumerable<string>? adjectives = null)
    {
        return GetUnhiddenItemByKeyActive(GetItemKeyByNameAndAdjectives(itemName, adjectives));
    }

    internal AHereticObject? GetUnhiddenObjectFromWorldByNameAndAdjectives(string objectName, IEnumerable<string>? adjectives = null)
    {
        var item = GetObjectFromWorldByKey(GetKeyByNameAndAdjectivesFromAllResources(objectName, adjectives));

        if (item == default || item.IsHidden)
            return default;

        return item;
    }

    internal AHereticObject? GetUnhiddenObjectByNameAndAdjectivesActive(string objectName, IEnumerable<string>? adjectives = null)
    {
        var enumerable = (adjectives ?? Array.Empty<string>()).ToList();

        AHereticObject? containerObject = GetUnhiddenItemByNameAndAdjectivesActive(objectName, enumerable);
        if (containerObject == default)
            containerObject = GetUnhiddenCharacterByNameAndAdjectivesFromActiveLocation(objectName, enumerable);

        if (containerObject == default)
        {
            var key = GetCharacterKeyByNameAndAdjectives(objectName, enumerable);
            if (key == this.universe.ActivePlayer.Key)
                containerObject = this.universe.ActivePlayer;
        }

        return containerObject;
    }

    internal Character? GetUnhiddenCharacterByNameAndAdjectives(string itemName, IEnumerable<string>? adjectives = null)
    {
        return GetUnhiddenCharacterByKey(GetCharacterKeyByNameAndAdjectives(itemName, adjectives));
    }

    internal Character? GetUnhiddenCharacterByNameAndAdjectivesFromActiveLocation(string itemName, IEnumerable<string>? adjectives = null)
    {
        return GetUnhiddenCharacterByKeyFromActiveLocation(GetCharacterKeyByNameAndAdjectives(itemName, adjectives));
    }

    internal Item? GetVirtualItemByNameAndAdjectives(string itemName, IEnumerable<string>? adjectives = null)
    {
        return GetVirtualItemByKey(GetItemKeyByNameAndAdjectives(itemName, adjectives));
    }

    internal bool IsObjectUnhiddenAndInInventoryOrActiveLocation(AHereticObject item)
    {
        return !item.IsHidden && (this.universe.ActiveLocation.OwnsObject(item) || this.universe.ActivePlayer.OwnsObject(item));
    }

    internal bool IsObjectUnhiddenAndInInventory(AHereticObject item)
    {
        return !item.IsHidden && this.universe.ActivePlayer.OwnsObject(item);
    }

    internal string GetConversationAnswerKeyByName(string phrase)
    {
        return GetKeyByNameAndAdjectivesFromResource(phrase, this.universe.ConversationAnswersResources);
    }

    internal DestinationNode? GetDestinationNodeFromActiveLocationByDirection(Directions key)
    {
        if (universe.LocationMap.ContainsKey(universe.ActiveLocation))
            return universe.LocationMap[universe.ActiveLocation].FirstOrDefault(l => l.Direction == key);

        return default;
    }

    internal Location? GetLocationByKey(string key)
    {
        return universe.LocationMap.Keys.SingleOrDefault(l => l.Key == key);
    }

    private IReadOnlyList<T> FindCandidates<T>(string name) where T : AHereticObject
    {
        var resource = GetResourceForType<T>();
        if (resource == null)
            return Array.Empty<T>();

        var matchingKeys = resource
            .Where(r => r.Value.Contains(name, StringComparer.InvariantCultureIgnoreCase))
            .Select(r => r.Key);

        return matchingKeys
            .Select(k => GetObjectFromWorldByKey<T>(k))
            .Where(o => o != null)
            .Cast<T>()
            .ToList();
    }

    private IDictionary<string, IEnumerable<string>>? GetResourceForType<T>() where T : AHereticObject
    {
        if (typeof(T) == typeof(Item)) return universe.ItemResources;
        if (typeof(T) == typeof(Location)) return universe.LocationResources;
        if (typeof(T) == typeof(Character)) return universe.CharacterResources;
        return null;
    }

    private string GetPrioritizedItemKeysByNameAndAdjectives(string itemName, IEnumerable<string>? adjectives)
    {
        var result = GetFirstPriorityKeyByName(itemName);
        if (string.IsNullOrEmpty(result))
            return GetSecondPriorityItemKeysByNameAndAdjectives(itemName, adjectives);

        return result;
    }

    private string GetFirstPriorityKeyByName(string itemName)
    {
        var upperItemName = itemName.ToUpperInvariant();
        var universeActiveObject = this.universe.ActiveObject;
        if (universeActiveObject != null && (
            upperItemName == PronounHandler.GetPronounForObject(universeActiveObject, GrammarCase.Nominative).ToUpperInvariant()
            || upperItemName == PronounHandler.GetPronounForObject(universeActiveObject, GrammarCase.Genitive).ToUpperInvariant()
            || upperItemName == PronounHandler.GetPronounForObject(universeActiveObject, GrammarCase.Dative).ToUpperInvariant()
            || upperItemName == PronounHandler.GetPronounForObject(universeActiveObject, GrammarCase.Accusative).ToUpperInvariant()))
        {
            if (this.universe.ActiveObject != null)
                return this.universe.ActiveObject.Key;
        }

        return string.Empty;
    }

    private string GetSecondPriorityItemKeysByNameAndAdjectives(string itemName, IEnumerable<string>? adjectives)
    {
        var allItemKeysFromActiveLocation = GetItemKeysRecursive(this.universe.ActiveLocation.Items);
        var allItemKeysFromActivePlayer = GetItemKeysRecursive(this.universe.ActivePlayer.Items);
        var prioritizedKeysOfActiveLocationAndPlayer = allItemKeysFromActiveLocation.Union(allItemKeysFromActivePlayer).ToList();
        var prioritizedItemResources =
            this.universe.ItemResources.Where(x => prioritizedKeysOfActiveLocationAndPlayer.Contains(x.Key)).ToList();
        var onlyItemsWithItemNameInValues =
            prioritizedItemResources.Where(res => res.Value.Contains(itemName, StringComparer.InvariantCultureIgnoreCase)).ToList();

        if (onlyItemsWithItemNameInValues.Any())
        {
            switch (onlyItemsWithItemNameInValues.Count)
            {
                case 1:
                    return onlyItemsWithItemNameInValues.Single().Key;
                case > 1:
                    var result = GetItemKeyMatchingAdjectives(
                        onlyItemsWithItemNameInValues.Select(x => x.Key).ToList(), adjectives);
                    if (string.IsNullOrEmpty(result))
                        throw new AmbiguousHereticObjectException(BaseDescriptions.AMBIGUOUS_HERETICOBJECT);
                    return result;
            }
        }

        return string.Empty;
    }

    private string GetFirstPriorityLocationKeysByName(string locationName, IEnumerable<string>? adjectives)
    {
        if (this.universe.LocationMap.ContainsKey(this.universe.ActiveLocation))
        {
            var mappings = this.universe.LocationMap[this.universe.ActiveLocation].ToList();
            var allLocationKeysFromMappings = mappings.Select(map => map.Location?.Key);
            var prioritizedLocationResources =
                this.universe.LocationResources.Where(x => allLocationKeysFromMappings.Contains(x.Key)).ToList();
            var possibleDestinations =
                prioritizedLocationResources.Where(res => res.Value.Contains(locationName, StringComparer.InvariantCultureIgnoreCase)).ToList();

            switch (possibleDestinations.Count)
            {
                case 1:
                    return possibleDestinations.Single().Key;
                case > 1:
                    var result = GetLocationKeyMatchingAdjectives(
                        possibleDestinations.Select(x => x.Key).ToList(), adjectives);
                    if (string.IsNullOrEmpty(result))
                        throw new AmbiguousHereticObjectException(BaseDescriptions.AMBIGUOUS_LOCATION);
                    return result;
            }
        }

        return string.Empty;
    }

    private string GetLocationKeyMatchingAdjectives(IEnumerable<string> locationKeys, IEnumerable<string>? adjectives)
    {
        var locations = locationKeys.Select(GetObjectFromWorldByKey<Location>).ToList();
        var reducedType = new List<KeyValuePair<string, List<string>>>();
        foreach (var location in locations)
        {
            if (location != null)
            {
                var allDeclinedAdjectives = new List<string>(AdjectiveDeclinationHandler.GetAllDeclinedAdjectivesForAllCases(location));
                reducedType.Add(new KeyValuePair<string, List<string>>(location.Key, allDeclinedAdjectives));
            }
        }

        var result = reducedType.Where(x => adjectives != null && x.Value.Intersect(adjectives).Any()).Select(x => x.Key).ToList();

        if (result.Any())
        {
            if (result.Count == 1)
                return result.Single();
            throw new AmbiguousHereticObjectException(BaseDescriptions.AMBIGUOUS_LOCATION);
        }

        return string.Empty;
    }

    private string GetItemKeyMatchingAdjectives(IEnumerable<string> itemKeys, IEnumerable<string>? adjectives)
    {
        var itemList = itemKeys.Select(GetObjectFromWorldByKey<Item>).ToList();
        var reducedType = new List<KeyValuePair<string, List<string>>>();
        foreach (var item in itemList)
        {
            if (item != null)
            {
                var allDeclinedAdjectives = new List<string>(AdjectiveDeclinationHandler.GetAllDeclinedAdjectivesForAllCases(item));
                reducedType.Add(new KeyValuePair<string, List<string>>(item.Key, allDeclinedAdjectives));
            }
        }

        var result = reducedType.Where(x => adjectives != null && x.Value.Intersect(adjectives).Any()).Select(x => x.Key).ToList();

        if (result.Any())
        {
            if (result.Count == 1)
                return result.Single();
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
                result.AddRange(GetItemKeysRecursive(item.Items));

            //TODO - also recursive, but break on circular references.
            if (item.LinkedTo.Any())
                result.AddRange(item.LinkedTo.Select(x => x.Key));

            result.Add(item.Key);
        }

        return result;
    }

    private Item? GetUnhiddenItemByKeyActive(string key)
    {
        var result = this.universe.ActiveLocation.GetUnhiddenItem(key);
        if (result == default)
            result = this.universe.ActivePlayer.GetUnhiddenItem(key);

        return result;
    }

    private Character? GetUnhiddenCharacterByKeyFromActiveLocation(string key)
    {
        return this.universe.ActiveLocation.GetUnhiddenCharacter(key);
    }

    private Item? GetVirtualItemByKey(string key)
    {
        var result = this.universe.ActiveLocation.GetVirtualItem(key);
        if (result == default)
            result = this.universe.ActivePlayer.GetVirtualItem(key);

        return result;
    }

    private string GetKeyByNameAndAdjectivesFromAllResources(string name, IEnumerable<string>? adjectives)
    {
        var enumerable = (adjectives ?? Array.Empty<string>()).ToList();

        var key = GetCharacterKeyByNameAndAdjectives(name, enumerable);
        if (string.IsNullOrEmpty(key))
        {
            key = GetItemKeyByNameAndAdjectives(name, enumerable);
            if (string.IsNullOrEmpty(key))
            {
                key = GetLocationKeyByNameAndAdjectives(name, enumerable);
                if (string.IsNullOrEmpty(key))
                    key = GetConversationAnswerKeyByName(name);
            }
        }

        return key;
    }

    private string GetKeyByNameAndAdjectivesFromResource(string name, IDictionary<string, IEnumerable<string>> resource)
    {
        foreach (var (key, value) in resource)
        {
            if (value.Contains(name, StringComparer.InvariantCultureIgnoreCase))
                return key;
        }

        return string.Empty;
    }

    private string GetPlayerKeyByName(string playerName)
    {
        var upperItemName = playerName.ToUpperInvariant();

        if (upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Nominative, PersonView.FirstPerson).ToUpperInvariant()
            || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Nominative).ToUpperInvariant()
            || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Genitive, PersonView.FirstPerson).ToUpperInvariant()
            || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Genitive).ToUpperInvariant()
            || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Dative, PersonView.FirstPerson).ToUpperInvariant()
            || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Dative).ToUpperInvariant()
            || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Accusative, PersonView.FirstPerson).ToUpperInvariant()
            || upperItemName == PronounHandler.GetPronounForObject(this.universe.ActivePlayer, GrammarCase.Accusative).ToUpperInvariant()
            || upperItemName == this.universe.ActivePlayer.Name.ToUpperInvariant())
        {
            return this.universe.ActivePlayer.Key;
        }

        return string.Empty;
    }

    private string GetCharacterKeyByNameAndAdjectives(string itemName, IEnumerable<string>? adjectives = null)
    {
        return GetKeyByNameAndAdjectivesFromResource(itemName, this.universe.CharacterResources);
    }

    private string GetItemKeyByNameAndAdjectives(string itemName, IEnumerable<string>? adjectives = null)
    {
        var adjectivesList = adjectives?.ToList();
        if (GetPrioritizedItemKeysByNameAndAdjectives(itemName, adjectivesList) is { } itemKey && !string.IsNullOrEmpty(itemKey))
            return itemKey;

        return GetKeyByNameAndAdjectivesFromResource(itemName, this.universe.ItemResources);
    }

    private string GetLocationKeyByNameAndAdjectives(string locationName, IEnumerable<string>? adjectives = null)
    {
        var result = GetFirstPriorityLocationKeysByName(locationName, adjectives);

        if (string.IsNullOrEmpty(result))
            result = GetKeyByNameAndAdjectivesFromResource(locationName, this.universe.LocationResources);

        return result;
    }

    private Character? GetUnhiddenCharacterByKey(string key)
    {
        if (GetObjectFromWorldByKey(key) is Character { IsHidden: false } character)
            return character;

        return default;
    }
}
