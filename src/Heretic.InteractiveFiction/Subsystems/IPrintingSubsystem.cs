﻿using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Subsystems;

public interface IPrintingSubsystem
{
    int ConsoleWidth { get; set; }
    TextColor ForegroundColor { get; set; }
    TextColor BackgroundColor { get; set; }
    string ReadInput();
    void WaitForUserAction();
    void ResetColors();
    void DeactivateOutput();
    void ActivateOutput();
    bool ActiveLocation(Location? activeLocation, IDictionary<Location, IEnumerable<DestinationNode>> locationMap);
    bool ActivePlayer(Player? activePlayer);
    bool AlterEgo(AHereticObject? item);
    bool CanNotUseObject(string objectName);
    void ClearScreen();
    bool Credits();
    bool PrintObject(AHereticObject? item);
    bool Hint(AHereticObject? item);
    bool History(ICollection<string> historyCollection);
    bool ItemNotVisible();
    bool ItemNotVisible(AHereticObject? item);
    bool KeyNotVisible();
    bool ImpossiblePickup(AHereticObject? containerObject);
    bool ItemToHeavy();
    bool ItemPickupSuccess(AHereticObject? item);
    bool ImpossibleDrop(AHereticObject? item);
    bool ImpossibleLock(AHereticObject? item);
    bool ImpossibleUnlock(AHereticObject? item);
    bool ItemAlreadyClosed(AHereticObject? item);
    bool ItemClosed(AHereticObject? item);
    bool ItemStillClosed(AHereticObject? item);
    bool ItemAlreadyBroken(AHereticObject? item);
    bool ItemAlreadyOpen(AHereticObject? item);
    bool ItemAlreadyLocked(AHereticObject? item);
    bool ItemAlreadyUnlocked(AHereticObject? item);
    bool ItemUnbreakable(AHereticObject? item);
    bool ItemUnknown(AdventureEvent? adventureEvent);
    bool ItemSeated(AHereticObject? item);
    bool ItemNotSeatable(AHereticObject? item);
    bool ItemStillLocked(AHereticObject? item);
    bool ItemLocked(AHereticObject? item);
    bool ItemUnlocked(AHereticObject? item);
    bool ItemNotLockAble(AHereticObject? item);
    bool ItemOpen(AHereticObject? item);
    bool ItemDropSuccess(AHereticObject? item);
    bool ItemDropSuccess(AHereticObject? itemToDrop, AHereticObject? containerItem);
    bool ItemIsNotAContainer(AHereticObject? item);
    bool ItemNotOwned();
    bool ItemNotOwned(AHereticObject? item);
    bool ItemAlreadyOwned();
    bool DestinationNode(Location? activeLocation, IDictionary<Location, IEnumerable<DestinationNode>> locationMap);
    bool Misconcept();
    bool NothingToTake();
    bool NoAnswer(string phrase);
    bool NoAnswerToInvisibleObject(Character? character);
    bool NoAnswerToQuestion(string phrase);
    bool Opening();
    bool Closing();
    bool Resource(string? resource, bool endWithLineBreak = true, bool wordWrap = true);
    bool Resource(object? resource, bool endWithLineBreak = true, bool wordWrap = true);
    bool FormattedResource(string? resource, string? text, bool lowerFirstLetter = false, bool endWithLineBreak = true, bool wordWrap = true);
    bool Score(int score, int maxScore);
    bool Talk(Character? character);
    bool TitleAndScore(int score, int maxScore);
    bool ToolNotVisible();
    bool WrongKey(AHereticObject? item);
    bool Prompt();
    bool PayWithWhat();
    bool NoEvent();
    bool WayIsLocked(AHereticObject? item);
    bool WayIsClosed(AHereticObject? item);
}
