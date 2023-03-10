using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record EatCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            if (adventureEvent.UnidentifiedSentenceParts.Any())
            {
                return PrintingSubsystem.ItemUnknown(adventureEvent);
            }

            return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_EAT);
        }

        if (adventureEvent.AllObjects.Count == 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                if (adventureEvent.UnidentifiedSentenceParts.Any())
                {
                    return PrintingSubsystem.ItemUnknown(adventureEvent);
                }

                return PrintingSubsystem.Resource(BaseDescriptions.PLAYER_NOT_EATABLE);
            }

            return HandleEat(adventureEvent);
        }

        if (adventureEvent.ObjectOne is { } gamer && gamer.Key == Universe.ActivePlayer.Key)
        {
            var adventureEventWithoutPlayer = ICommand.GetAdventureEventWithoutPlayer(adventureEvent);
            return this.HandleEat(adventureEventWithoutPlayer);
        }

        return this.HandleEat(adventureEvent);
    }
    
    private bool HandleEat(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                var itemName =
                    ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                        lowerFirstCharacter: true);

                if (item.IsEatable)
                {
                    try
                    {
                        var itemEventArgs = new ContainerObjectEventArgs()
                            { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                        item.OnBeforeEat(itemEventArgs);
                        
                        if (Universe.ActivePlayer.Items.Contains(item))
                        {
                            Universe.ActivePlayer.RemoveItem((Item)item);
                        }
                        else
                        {
                            Universe.ActiveLocation.RemoveItem((Item)item);
                        }

                        ObjectHandler.RemoveAsActiveObject(item);
                        item.OnEat(itemEventArgs);

                        item.OnAfterEat(itemEventArgs);

                        return PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_EATEN, itemName, true);
                    }
                    catch (EatException ex)
                    {
                        return PrintingSubsystem.Resource(ex.Message);
                    }
                }

                ObjectHandler.StoreAsActiveObject(item);

                return PrintingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_EAT, itemName, true);
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_EAT);


        return false;
    }

}