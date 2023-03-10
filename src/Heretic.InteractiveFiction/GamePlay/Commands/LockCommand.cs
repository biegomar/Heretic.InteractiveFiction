using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record LockCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.AllObjects.Count == 1)
        {
            return HandleLockWithoutKey(adventureEvent);
        }

        return HandleLock(adventureEvent);
    }
    
    private bool HandleLockWithoutKey(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);
                if (!string.IsNullOrEmpty(item.UnlockWithKey) &&
                    Universe.ActivePlayer.OwnsItem(item.UnlockWithKey))
                {
                    var key = Universe.ActivePlayer.GetItem(item.UnlockWithKey);
                    if (item.IsLockable)
                    {
                        if (!item.IsLocked)
                        {
                            if (!item.IsCloseable || item.IsCloseable && item.IsClosed)
                            {
                                try
                                {
                                    var lockContainerEventArgs = new LockContainerEventArgs
                                        { Key = key, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                                    item.OnBeforeLock(lockContainerEventArgs);

                                    item.IsLocked = true;
                                    item.OnLock(lockContainerEventArgs);
                                    var keyName = ArticleHandler.GetNameWithArticleForObject(key,
                                        GrammarCase.Accusative, lowerFirstCharacter: true);
                                    PrintingSubsystem.Resource(string.Format(
                                        BaseDescriptions.ITEM_LOCKED_WITH_KEY_FROM_INVENTORY, keyName, item.Name));

                                    item.OnAfterLock(lockContainerEventArgs);

                                    return true;
                                }
                                catch (LockException e)
                                {
                                    return PrintingSubsystem.Resource(e.Message);
                                }
                            }
                        }

                        return PrintingSubsystem.ItemAlreadyLocked(item);
                    }

                    return PrintingSubsystem.ItemNotLockAble(item);
                }

                return PrintingSubsystem.ImpossibleLock(item);
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_LOCK);
    }
    
    private bool HandleLock(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        var key = adventureEvent.ObjectTwo;

        if (item != default)
        {
            ObjectHandler.StoreAsActiveObject(item);
            if (key != default)
            {
                if (item.IsLockable)
                {
                    if (!item.IsLocked)
                    {
                        if (!item.IsCloseable || item.IsCloseable && item.IsClosed)
                        {
                            if (Universe.ActivePlayer.OwnsObject(key))
                            {
                                try
                                {
                                    var lockContainerEventArgs = new LockContainerEventArgs
                                        { Key = key, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                                    item.OnBeforeLock(lockContainerEventArgs);

                                    if (!string.IsNullOrEmpty(item.UnlockWithKey) && item.UnlockWithKey == key.Key)
                                    {
                                        item.IsLocked = true;
                                        item.OnLock(lockContainerEventArgs);
                                        PrintingSubsystem.ItemLocked(item);
                                    }
                                    else
                                    {
                                        item.OnLock(lockContainerEventArgs);
                                        PrintingSubsystem.Resource(string.Format(
                                            BaseDescriptions.IMPOSSIBLE_UNLOCK_WITH_WRONG_KEY, item.Name, key.Name));
                                    }

                                    item.OnAfterLock(lockContainerEventArgs);

                                    return true;
                                }
                                catch (LockException e)
                                {
                                    return PrintingSubsystem.Resource(e.Message);
                                }
                            }

                            PrintingSubsystem.ItemNotOwned(key);
                        }
                    }

                    return PrintingSubsystem.ItemAlreadyLocked(item);
                }

                return PrintingSubsystem.ItemNotLockAble(item);
            }

            return PrintingSubsystem.KeyNotVisible();
        }

        return PrintingSubsystem.ItemNotVisible();
    }
}