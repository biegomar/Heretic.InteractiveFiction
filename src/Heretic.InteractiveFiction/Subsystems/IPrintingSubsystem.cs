using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Subsystems;

public interface IPrintingSubsystem
{
    int ConsoleWidth { get; set; }
    TextColor ForegroundColor { get; set; }
    TextColor BackgroundColor { get; set; }
    void ResetColors();
    bool ActiveLocation(Location activeLocation, IDictionary<Location, IEnumerable<DestinationNode>> locationMap);
    bool ActivePlayer(Player activePlayer);
    bool AlterEgo(AHereticObject item);
    bool AlterEgo(string itemName);
    bool CanNotUseObject(string objectName);
    void ClearScreen();
    bool Credits();
    bool PrintObject(AHereticObject item);
    bool Help(IList<Verb> verbs);
    bool History(ICollection<string> historyCollection);
    bool ItemNotVisible();
    bool KeyNotVisible();
    bool ImpossiblePickup(AHereticObject containerObject);
    bool ItemToHeavy();
    bool ItemPickupSuccess(AHereticObject item);
    bool ImpossibleDrop(AHereticObject item);
    bool ImpossibleUnlock(AHereticObject item);
    bool ItemAlreadyClosed(AHereticObject item);
    bool ItemClosed(AHereticObject item);
    bool ItemStillClosed(AHereticObject item);
    bool ItemAlreadyBroken(AHereticObject item);
    bool ItemAlreadyOpen(AHereticObject item);
    bool ItemAlreadyUnlocked(AHereticObject item);
    bool ItemUnbreakable(AHereticObject item);
    bool ItemSeated(AHereticObject item);
    bool ItemNotSeatable(AHereticObject item);
    bool ItemStillLocked(AHereticObject item);
    bool ItemUnlocked(AHereticObject item);
    bool ItemNotLockAble(AHereticObject item);
    bool ItemOpen(AHereticObject item);
    bool ItemDropSuccess(AHereticObject item);
    bool ItemDropSuccess(AHereticObject itemToDrop, AHereticObject containerItem);
    bool ItemIsNotAContainer(AHereticObject item);
    bool ItemNotOwned();
    bool ItemNotOwned(AHereticObject item);
    bool ItemAlreadyOwned();
    bool DestinationNode(Location activeLocation, IDictionary<Location, IEnumerable<DestinationNode>> locationMap);
    bool Misconcept();
    bool NothingToTake();
    bool NoAnswer(string phrase);
    bool NoAnswerToInvisibleObject(Character character);
    bool NoAnswerToQuestion(string phrase);
    bool Opening();
    bool Closing();
    bool Resource(string resource);
    bool FormattedResource(string resource, string text, bool lowerFirstLetter = false);
    bool Score(int score, int maxScore);
    bool Talk(Character character);
    bool TitleAndScore(int score, int maxScore);
    bool ToolNotVisible();
    bool WrongKey(AHereticObject item);
    bool Prompt();
    bool PayWithWhat();
    bool SameActionAgain();
    bool NoEvent();
    bool WayIsLocked(AHereticObject item);
    bool WayIsClosed(AHereticObject item);
}
