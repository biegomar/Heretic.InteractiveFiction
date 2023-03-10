using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record SwitchOnCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.AllObjects.Count > 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                var adventureEventWithoutPlayer = ICommand.GetAdventureEventWithoutPlayer(adventureEvent);
                return this.HandleSwitchOn(adventureEventWithoutPlayer);
            }
        }

        return this.HandleSwitchOn(adventureEvent);
    }
    
    private bool HandleSwitchOn(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is Item item)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                var itemName =
                    ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                        lowerFirstCharacter: true);

                ObjectHandler.StoreAsActiveObject(item);

                if (item.IsSwitchable)
                {
                    if (!item.IsSwitchedOn)
                    {
                        try
                        {
                            var itemEventArgs = new ContainerObjectEventArgs()
                                { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                            item.OnBeforeSwitchOn(itemEventArgs);

                            item.IsSwitchedOn = true;
                            item.OnSwitchOn(itemEventArgs);

                            item.OnAfterSwitchOn(itemEventArgs);

                            return PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_SWITCHEDON,
                                itemName,
                                true);
                        }
                        catch (SwitchOnException ex)
                        {
                            return PrintingSubsystem.Resource(ex.Message);
                        }
                    }

                    return PrintingSubsystem.FormattedResource(BaseDescriptions.ALREADY_SWITCHEDON,
                        itemName, true);
                }

                return PrintingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_SWITCHON, itemName,
                    true);
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_SWITCHON);
    }
}