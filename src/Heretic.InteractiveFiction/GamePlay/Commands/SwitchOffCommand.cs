using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record SwitchOffCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.AllObjects.Count > 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                var adventureEventWithoutPlayer = ICommand.GetAdventureEventWithoutPlayer(adventureEvent);
                return HandleSwitchOff(adventureEventWithoutPlayer);
            }
        }
            
        return HandleSwitchOff(adventureEvent);
    }
    
    private bool HandleSwitchOff(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is Item item)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                var itemName =
                    ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                        lowerFirstCharacter: true);
                
                var itemNameGenitive =
                    ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Genitive,
                        lowerFirstCharacter: true);

                ObjectHandler.StoreAsActiveObject(item);

                if (item.IsSwitchable)
                {
                    if (item.IsSwitchedOn)
                    {
                        try
                        {
                            var itemEventArgs = new ContainerObjectEventArgs()
                            {
                                OptionalErrorMessage = adventureEvent.Predicate != default
                                    ? adventureEvent.Predicate.ErrorMessage
                                    : string.Empty
                            };

                            item.OnBeforeSwitchOff(itemEventArgs);

                            item.IsSwitchedOn = false;
                            item.OnSwitchOff(itemEventArgs);

                            item.OnAfterSwitchOff(itemEventArgs);

                            return PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_SWITCHEDOFF, itemName,
                                true);
                        }
                        catch (SwitchOffException ex)
                        {
                            return PrintingSubsystem.Resource(ex.Message);
                        }
                    }

                    return PrintingSubsystem.FormattedResource(BaseDescriptions.ALREADY_SWITCHEDOFF, itemName,
                        true);
                }
                
                if (item.IsSwitchedOn || item.IsLighterSwitchedOn)
                {
                    return item.IsLighter
                        ? PrintingSubsystem.FormattedResource(BaseDescriptions.CANT_SWITCHOFF_LIGHTER, itemNameGenitive,
                            true)
                        : PrintingSubsystem.FormattedResource(BaseDescriptions.CANT_SWITCHOFF_ITEM, itemName,
                            true);
                }
                    
                return PrintingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_SWITCHOFF, itemName, true); 
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_SWITCHOFF);
    }
}