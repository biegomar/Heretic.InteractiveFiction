using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record TakeOffCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.AllObjects.Count > 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                var adventureEventWithoutPlayer = ICommand.GetAdventureEventWithoutPlayer(adventureEvent);
                return this.HandleTakeOff(adventureEventWithoutPlayer);
            }
        }

        return this.HandleTakeOff(adventureEvent);
    }
    
    private bool HandleTakeOff(AdventureEvent adventureEvent)
    {
        void SwapItem(Item item)
        {
            Universe.ActivePlayer.Clothes.Remove(item);
            Universe.ActivePlayer.Items.Add(item);
        }

        if (adventureEvent.ObjectOne is Item item)
        {
            if (Universe.ActivePlayer.Clothes.Contains(item))
            {
                var itemName =
                    ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
                var itemPronoun = PronounHandler.GetPronounForObject(item, GrammarCase.Accusative).LowerFirstChar();

                ObjectHandler.StoreAsActiveObject(item);

                try
                {
                    var itemEventArgs = new ContainerObjectEventArgs()
                        { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                    item.OnBeforeTakeOff(itemEventArgs);

                    SwapItem(item);
                    item.OnTakeOff(itemEventArgs);

                    item.OnAfterTakeOff(itemEventArgs);

                    return PrintingSubsystem.Resource(string.Format(BaseDescriptions.TAKEOFF_WEARABLE, itemName, itemPronoun));
                }
                catch (TakeOffException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_TAKEOFF);
    }
}