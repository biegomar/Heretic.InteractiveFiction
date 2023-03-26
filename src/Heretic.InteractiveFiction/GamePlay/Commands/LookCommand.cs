using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record LookCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            if (adventureEvent.UnidentifiedSentenceParts.Any())
            {
                return PrintingSubsystem.ItemUnknown(adventureEvent);
            }

            return this.HandleLookEventOnActiveLocation(adventureEvent);
        }

        if (adventureEvent.AllObjects.Count == 1)
        {
            if (adventureEvent.UnidentifiedSentenceParts.Any() && adventureEvent.ObjectOne is { } player &&
                player.Key == Universe.ActivePlayer.Key)
            {
                return PrintingSubsystem.ItemUnknown(adventureEvent);
            }

            return this.HandleLookEventOnObjects(adventureEvent);
        }

        if (adventureEvent.AllObjects.Count > 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                var adventureEventWithoutPlayer = ICommand.GetAdventureEventWithoutPlayer(adventureEvent);
                return this.HandleLookEventOnObjects(adventureEventWithoutPlayer);
            }

            return this.HandleLookEventOnObjects(adventureEvent);
        }

        return true;
    }
    
    private bool HandleLookEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        try
        {
            var eventArgs = new ContainerObjectEventArgs()
            {
                OptionalErrorMessage = adventureEvent.Predicate != default
                    ? adventureEvent.Predicate.ErrorMessage
                    : string.Empty
            };

            Universe.ActiveLocation.OnBeforeLook(eventArgs);

            ObjectHandler.UnveilFirstLevelObjects(Universe.ActiveLocation);
            Universe.ActiveLocation.OnLook(eventArgs);

            Universe.ActiveLocation.OnAfterLook(eventArgs);
            return PrintingSubsystem.ActiveLocation(Universe.ActiveLocation, Universe.LocationMap);
        }
        catch (LookException ex)
        {
            return PrintingSubsystem.Resource(ex.Message);
        }
    }
    
    private bool HandleLookEventOnObjects(AdventureEvent adventureEvent)
    {
        foreach (var item in adventureEvent.AllObjects)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                try
                {
                    ObjectHandler.StoreAsActiveObject(item);

                    var eventArgs = new ContainerObjectEventArgs()
                    {
                        OptionalErrorMessage = adventureEvent.Predicate != default
                            ? adventureEvent.Predicate.ErrorMessage
                            : string.Empty
                    };
                    var eventArgsForActiveLocation = new ContainerObjectEventArgs()
                    {
                        ExternalItemKey = item.Key,
                        OptionalErrorMessage = adventureEvent.Predicate != default
                            ? adventureEvent.Predicate.ErrorMessage
                            : string.Empty
                    };

                    item.OnBeforeLook(eventArgs);
                    Universe.ActiveLocation.OnBeforeLook(eventArgsForActiveLocation);

                    ObjectHandler.UnveilFirstLevelObjects(item);
                    item.OnLook(eventArgs);
                    Universe.ActiveLocation.OnLook(eventArgsForActiveLocation);
                    PrintingSubsystem.PrintObject(item);

                    item.OnAfterLook(new ContainerObjectEventArgs());
                    Universe.ActiveLocation.OnAfterLook(eventArgsForActiveLocation);
                }
                catch (LookException ex)
                {
                    PrintingSubsystem.Resource(ex.Message);
                }
            }
            else
            {
                PrintingSubsystem.ItemNotVisible();    
            }
        }

        return true;
    }
}