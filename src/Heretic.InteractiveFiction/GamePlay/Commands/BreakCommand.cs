using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record BreakCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.AllObjects.Count > 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                var adventureEventWithoutPlayer = new AdventureEvent();
                adventureEventWithoutPlayer.Predicate = adventureEvent.Predicate;
                adventureEventWithoutPlayer.AllObjects.AddRange(adventureEvent.AllObjects.Skip(1));
                return this.HandleBreakEventOnObjects(adventureEventWithoutPlayer);
            }
        }
            
        return this.HandleBreakEventOnObjects(adventureEvent);
    }
    
    private bool HandleBreakEventOnObjects(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);
                if (item.IsBreakable)
                {
                    if (!item.IsBroken)
                    {
                        try
                        {
                            var eventArgs = new BreakItemEventArgs()
                            {
                                OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage,
                                ItemToUse = adventureEvent.ObjectTwo
                            };
                            item.OnBeforeBreak(eventArgs);

                            item.IsBroken = true;
                            item.OnBreak(eventArgs);

                            item.OnAfterBreak(eventArgs);
                            return true;
                        }
                        catch (BreakException ex)
                        {
                            item.IsBroken = false;
                            return PrintingSubsystem.Resource(ex.Message);
                        }
                    }

                    return PrintingSubsystem.ItemAlreadyBroken(item);
                }

                return PrintingSubsystem.ItemUnbreakable(item);

            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.ItemNotVisible();
    }
}