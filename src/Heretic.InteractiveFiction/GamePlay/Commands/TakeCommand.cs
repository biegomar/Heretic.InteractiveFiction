using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record TakeCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any() && !adventureEvent.UnidentifiedSentenceParts.Any())
        {
            return HandleTakeEventOnAllPickableAndUnhiddenItems(adventureEvent);
        }

        if (!adventureEvent.AllObjects.Any() && adventureEvent.UnidentifiedSentenceParts.Any())
        {
            return PrintingSubsystem.ItemNotVisible();
        }

        if (adventureEvent.AllObjects.Count == 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                if (adventureEvent.UnidentifiedSentenceParts.Any())
                {
                    return PrintingSubsystem.ItemUnknown(adventureEvent);
                }
                    
                return PrintingSubsystem.Resource(BaseDescriptions.PLAYER_NOT_PICKABLE);
            }

            return HandleTakeEventOnObjects(adventureEvent);
        }

        if (adventureEvent.ObjectOne is { } gamer && gamer.Key == Universe.ActivePlayer.Key)
        {
            var adventureEventWithoutPlayer = ICommand.GetAdventureEventWithoutPlayer(adventureEvent);
            return this.HandleTakeEventOnObjects(adventureEventWithoutPlayer);
        }

        return this.HandleTakeEventOnObjects(adventureEvent);
    }
    
    private bool HandleTakeEventOnObjects(AdventureEvent adventureEvent)
    {
        foreach (var hereticObject in adventureEvent.AllObjects)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(hereticObject))
            {
                if (hereticObject is Character character)
                {
                    PrintingSubsystem.ImpossiblePickup(character);
                } 
                else if (hereticObject is Item item)
                {
                    try
                    {
                        ObjectHandler.StoreAsActiveObject(hereticObject);

                        var eventArgs = new ContainerObjectEventArgs()
                            { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                        hereticObject.OnBeforeTake(eventArgs);
                        Universe.PickObject(item);
                        hereticObject.OnTake(eventArgs);
                        hereticObject.OnAfterTake(eventArgs);
                    }
                    catch (TakeException ex)
                    {
                        PrintingSubsystem.Resource(ex.Message);
                    }   
                }
                else
                {
                    PrintingSubsystem.ImpossiblePickup(hereticObject);    
                }
            }
            else
            {
                PrintingSubsystem.ItemNotVisible();    
            }
        }

        return true;
    }
    
    private bool HandleTakeEventOnAllPickableAndUnhiddenItems(AdventureEvent adventureEvent)
    {
        var subjects = Universe.ActiveLocation.GetAllPickableAndUnHiddenItems();
        if (subjects.Any())
        {
            var eventArgs = new ContainerObjectEventArgs() { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
            var result = true;
            foreach (var item in subjects)
            {
                try
                {
                    item.OnBeforeTake(eventArgs);
                    result = result && Universe.PickObject(item);
                    item.OnTake(eventArgs);
                    item.OnAfterTake(eventArgs);
                }
                catch (TakeException ex)
                {
                    result = result && PrintingSubsystem.Resource(ex.Message);
                }
            }

            return result;
        }

        return PrintingSubsystem.NothingToTake();
    }
}