using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record UnlockCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.AllObjects.Count == 1)
        {
            return HandleUnlockWithoutKey(adventureEvent);
        }

        return HandleUnlock(adventureEvent);
    }
    
    private bool HandleUnlockWithoutKey(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is Item item)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);
                if (!string.IsNullOrEmpty(item.UnlockWithKey) &&
                    Universe.ActivePlayer.OwnsItem(item.UnlockWithKey))
                {
                    if (item.IsLockable)
                    {
                        if (item.IsLocked)
                        {
                            if (!item.IsCloseable || item.IsCloseable && item.IsClosed)
                            {
                                try
                                {
                                    var key = Universe.ActivePlayer.GetItem(item.UnlockWithKey);
                                    if (key != default)
                                    {
                                        var unlockContainerEventArgs = new LockContainerEventArgs
                                        {
                                            Key = key,
                                            OptionalErrorMessage = adventureEvent.Predicate != default
                                                ? adventureEvent.Predicate.ErrorMessage
                                                : string.Empty
                                        };

                                        item.OnBeforeUnlock(unlockContainerEventArgs);

                                        item.IsLocked = false;
                                        item.OnUnlock(unlockContainerEventArgs);
                                        var keyName = ArticleHandler.GetNameWithArticleForObject(key,
                                            GrammarCase.Accusative, lowerFirstCharacter: true);
                                    
                                        PrintingSubsystem.Resource(string.Format(
                                            BaseDescriptions.ITEM_UNLOCKED_WITH_KEY_FROM_INVENTORY, keyName,
                                            item.Name));

                                        item.OnAfterUnlock(unlockContainerEventArgs);

                                        return true; 
                                    }

                                    return PrintingSubsystem.KeyNotVisible();
                                }
                                catch (UnlockException e)
                                {
                                    return PrintingSubsystem.Resource(e.Message);
                                }
                            }
                        }

                        return PrintingSubsystem.ItemAlreadyUnlocked(item);
                    }

                    return PrintingSubsystem.ItemNotLockAble(item);
                }

                return PrintingSubsystem.ImpossibleUnlock(item);
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_UNLOCK);
    }
    
    private bool HandleUnlock(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is Item item)
        {
            ObjectHandler.StoreAsActiveObject(item);

            if (item.IsLockable)
            {
                if (item.IsLocked)
                {
                    if (!item.IsCloseable || item.IsCloseable && item.IsClosed)
                    {
                        if (adventureEvent.ObjectTwo is Item key)
                        {
                            if (Universe.ActivePlayer.OwnsObject(key))
                            {
                                try
                                {
                                    var unlockContainerEventArgs = new LockContainerEventArgs
                                    {
                                        Key = key,
                                        OptionalErrorMessage = adventureEvent.Predicate != default
                                            ? adventureEvent.Predicate.ErrorMessage
                                            : string.Empty
                                    };

                                    item.OnBeforeUnlock(unlockContainerEventArgs);

                                    if (!string.IsNullOrEmpty(item.UnlockWithKey) && item.UnlockWithKey == key.Key)
                                    {
                                        item.IsLocked = false;
                                        item.OnUnlock(unlockContainerEventArgs);
                                        PrintingSubsystem.ItemUnlocked(item);
                                    }
                                    else
                                    {
                                        item.OnUnlock(unlockContainerEventArgs);
                                        PrintingSubsystem.Resource(string.Format(
                                            BaseDescriptions.IMPOSSIBLE_UNLOCK_WITH_WRONG_KEY, item.Name, key.Name));
                                    }

                                    item.OnAfterUnlock(unlockContainerEventArgs);

                                    return true;
                                }
                                catch (UnlockException e)
                                {
                                    return PrintingSubsystem.Resource(e.Message);
                                }
                            }

                            PrintingSubsystem.ItemNotOwned(key);
                        }

                        return PrintingSubsystem.KeyNotVisible();
                    }
                }

                return PrintingSubsystem.ItemAlreadyUnlocked(item);
            }

            return PrintingSubsystem.ItemNotLockAble(item);
        }

        return PrintingSubsystem.ItemNotVisible();
    }
}