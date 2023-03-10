using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record WearCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.AllObjects.Count > 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                var adventureEventWithoutPlayer = ICommand.GetAdventureEventWithoutPlayer(adventureEvent);
                return this.HandleWear(adventureEventWithoutPlayer);
            }
        }
            
        return this.HandleWear(adventureEvent);
    }
    
    private bool HandleWear(AdventureEvent adventureEvent)
    {
        void SwapItem(Item item)
        {
            Universe.ActivePlayer.Items.Remove(item);
            Universe.ActivePlayer.Clothes.Add(item);
        }

        if (adventureEvent.ObjectOne != default)
        {
            if (adventureEvent.ObjectOne is Item item)
            {
                if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    var itemName =
                        ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                            lowerFirstCharacter: true);

                    ObjectHandler.StoreAsActiveObject(item);

                    if (item.IsWearable)
                    {
                        if (!Universe.ActivePlayer.Clothes.Contains(item))
                        {
                            try
                            {
                                var itemEventArgs = new ContainerObjectEventArgs()
                                    { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                                item.OnBeforeWear(itemEventArgs);

                                if (Universe.ActivePlayer.Items.Contains(item))
                                {
                                    SwapItem(item);
                                }
                                else
                                {
                                    Universe.PickObject(item, true);
                                    if (Universe.ActivePlayer.Items.Contains(item))
                                    {
                                        SwapItem(item);
                                    }
                                    else
                                    {
                                        //TODO refactor PickUp!
                                        return true;
                                    }
                                }

                                item.OnWear(itemEventArgs);

                                item.OnAfterWear(itemEventArgs);

                                return PrintingSubsystem.FormattedResource(BaseDescriptions.PULLON_WEARABLE,
                                    itemName,
                                    true);
                            }
                            catch (WearException ex)
                            {
                                return PrintingSubsystem.Resource(ex.Message);
                            }
                        }

                        PrintingSubsystem.Resource(BaseDescriptions.ALREADY_WEARING);
                    }

                    return PrintingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_WEAR, itemName, true);
                }

                return PrintingSubsystem.ItemNotVisible();
            }

            return PrintingSubsystem.Resource(BaseDescriptions.ITEM_NOT_WEARABLE);
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_WEAR);
    }
}