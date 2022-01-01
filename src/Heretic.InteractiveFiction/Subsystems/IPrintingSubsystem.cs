using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Subsystems;

public interface IPrintingSubsystem
{
    TextColor ForegroundColor { get; set; }
    TextColor BackgroundColor { get; set; }
    void ResetColors();
    bool ActiveLocation(Location activeLocation, IDictionary<Location, IEnumerable<DestinationNode>> locationMap);
    bool ActivePlayer(Player activePlayer);
    bool AlterEgo(AContainerObject item);
    bool AlterEgo(string itemName);
    bool CanNotUseObject(string objectName);
    void ClearScreen();
    bool Credits();
    bool PrintObject(AContainerObject item);
    bool Help(IDictionary<string, IEnumerable<string>> verbResource);
    bool History(ICollection<string> historyCollection);
    bool ItemNotVisible();
    bool KeyNotVisible();
    bool ImpossiblePickup(AContainerObject containerObject);
    bool ItemToHeavy();
    bool ItemPickupSuccess(AContainerObject item);
    bool ImpossibleDrop(AContainerObject item);
    bool ImpossibleUnlock(AContainerObject item);
    bool ItemAlreadyClosed(AContainerObject item);
    bool ItemClosed(AContainerObject item);
    bool ItemAlreadyOpen(AContainerObject item);
    bool ItemAlreadyUnlocked(AContainerObject item);
    bool ItemEaten(AContainerObject item);
    bool ItemNotEatable(AContainerObject item);
    bool ItemSeated(AContainerObject item);
    bool ItemNotSeatable(AContainerObject item);
    bool ItemStillLocked(AContainerObject item);
    bool ItemUnlocked(AContainerObject item);
    bool ItemNotLockAble(AContainerObject item);
    bool ItemOpen(AContainerObject item);
    bool ItemDropSuccess(AContainerObject item);
    bool ItemNotOwned();
    bool ItemAlreadyOwned();
    bool DestinationNode(Location activeLocation, IDictionary<Location, IEnumerable<DestinationNode>> locationMap);
    bool Misconcept();
    bool NothingToTake();
    bool NoAnswer(string phrase);
    bool NoAnswerToInvisibleObject(Character character);
    bool NoAnswerToQuestion(string phrase);
    bool Opening();
    bool Resource(string resource);
    bool FormattedResource(string resource, string text);
    bool Score(int score, int maxScore);
    bool Talk(Character character);
    bool TitleAndScore(int score, int maxScore);
    bool WrongKey(AContainerObject item);
    bool Prompt();
    bool PayWithWhat();
    bool SameActionAgain();
    bool NoEvent();
    bool WayIsLocked(AContainerObject item);
    bool WayIsClosed(AContainerObject item);
}
