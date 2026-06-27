using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

public sealed class ObjectHandler
{
    private readonly ObjectResolver resolver;
    private readonly ActiveObjectTracker tracker;
    private readonly WorldMutator mutator;

    public ObjectHandler(Universe universe)
    {
        this.resolver = new ObjectResolver(universe);
        this.tracker = new ActiveObjectTracker(universe);
        this.mutator = new WorldMutator(universe);
    }

    // --- Resolution ---

    public string GetObjectKeyByNameAndAdjectives<T>(string objectName, IEnumerable<string>? adjectives = null) where T : AHereticObject
        => resolver.GetObjectKeyByNameAndAdjectives<T>(objectName, adjectives);

    public AHereticObject? GetObjectFromWorldByKey(string key)
        => resolver.GetObjectFromWorldByKey(key);

    public T? GetObjectFromWorldByKey<T>(string key) where T : AHereticObject
        => resolver.GetObjectFromWorldByKey<T>(key);

    public AHereticObject? GetObjectFromWorldByNameAndAdjectives(string objectName, IEnumerable<string>? adjectives = null)
        => resolver.GetObjectFromWorldByNameAndAdjectives(objectName, adjectives);

    public Item? GetUnhiddenItemByNameAndAdjectivesActive(string itemName, IEnumerable<string>? adjectives = null)
        => resolver.GetUnhiddenItemByNameAndAdjectivesActive(itemName, adjectives);

    public AHereticObject? GetUnhiddenObjectFromWorldByNameAndAdjectives(string objectName, IEnumerable<string>? adjectives = null)
        => resolver.GetUnhiddenObjectFromWorldByNameAndAdjectives(objectName, adjectives);

    public AHereticObject? GetUnhiddenObjectByNameAndAdjectivesActive(string objectName, IEnumerable<string>? adjectives = null)
        => resolver.GetUnhiddenObjectByNameAndAdjectivesActive(objectName, adjectives);

    public Character? GetUnhiddenCharacterByNameAndAdjectives(string itemName, IEnumerable<string>? adjectives = null)
        => resolver.GetUnhiddenCharacterByNameAndAdjectives(itemName, adjectives);

    public Character? GetUnhiddenCharacterByNameAndAdjectivesFromActiveLocation(string itemName, IEnumerable<string>? adjectives = null)
        => resolver.GetUnhiddenCharacterByNameAndAdjectivesFromActiveLocation(itemName, adjectives);

    public Item? GetVirtualItemByNameAndAdjectives(string itemName, IEnumerable<string>? adjectives = null)
        => resolver.GetVirtualItemByNameAndAdjectives(itemName, adjectives);

    public bool IsObjectUnhiddenAndInInventoryOrActiveLocation(AHereticObject item)
        => resolver.IsObjectUnhiddenAndInInventoryOrActiveLocation(item);

    public bool IsObjectUnhiddenAndInInventory(AHereticObject item)
        => resolver.IsObjectUnhiddenAndInInventory(item);

    public string GetConversationAnswerKeyByName(string phrase)
        => resolver.GetConversationAnswerKeyByName(phrase);

    public DestinationNode? GetDestinationNodeFromActiveLocationByDirection(Directions key)
        => resolver.GetDestinationNodeFromActiveLocationByDirection(key);

    public Location? GetLocationByKey(string key)
        => resolver.GetLocationByKey(key);

    // --- Active object tracking ---

    public void StoreAsActiveObject(AHereticObject hereticObject)
        => tracker.Store(hereticObject);

    public void ClearActiveObject()
        => tracker.Clear();

    public void RemoveAsActiveObject(AHereticObject hereticObject)
        => tracker.RemoveIfIs(hereticObject);

    public void ClearActiveObjectIfNotInInventory()
        => tracker.ClearIfNotInInventory();

    // --- World mutations ---

    public void HideItemsOnClose(AHereticObject item)
        => mutator.HideItemsOnClose(item);

    public void UnhideItemsOnOpen(AHereticObject item)
        => mutator.UnhideItemsOnOpen(item);

    public void UnveilFirstLevelObjects(AHereticObject container)
        => mutator.UnveilFirstLevelObjects(container);

    public bool MoveCharacter(Character person, Location newLocation)
        => mutator.MoveCharacter(person, newLocation);
}
