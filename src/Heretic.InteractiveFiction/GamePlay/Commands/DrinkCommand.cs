using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record DrinkCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                var itemName =
                    ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                        lowerFirstCharacter: true);

                if (item.IsDrinkable)
                {
                    try
                    {
                        var itemEventArgs = new ContainerObjectEventArgs()
                        {
                            OptionalErrorMessage = adventureEvent.Predicate != default
                                ? adventureEvent.Predicate.ErrorMessage
                                : string.Empty
                        };

                        item.OnBeforeDrink(itemEventArgs);


                        if (Universe.ActivePlayer.Items.Contains(item))
                        {
                            Universe.ActivePlayer.RemoveItem((Item)item);
                        }
                        else
                        {
                            Universe.ActiveLocation.RemoveItem((Item)item);
                        }

                        ObjectHandler.RemoveAsActiveObject(item);
                        item.OnDrink(itemEventArgs);

                        item.OnAfterDrink(itemEventArgs);

                        return PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_DRUNK, itemName, true);
                    }
                    catch (DrinkException ex)
                    {
                        return PrintingSubsystem.Resource(ex.Message);
                    }
                }

                ObjectHandler.StoreAsActiveObject(item);

                return PrintingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_DRINK, itemName, true);
            }
        }
        
        return PrintingSubsystem.ItemNotVisible();
    }
}