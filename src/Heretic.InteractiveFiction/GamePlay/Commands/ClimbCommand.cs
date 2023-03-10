using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record ClimbCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.AllObjects.Count > 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                var adventureEventWithoutPlayer = ICommand.GetAdventureEventWithoutPlayer(adventureEvent);
                return this.HandleClimbEvent(adventureEventWithoutPlayer);
            }
        }

        return this.HandleClimbEvent(adventureEvent);
    }

    private bool HandleClimbEvent(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);

                var itemName =
                    ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                        lowerFirstCharacter: true);

                if (item.IsClimbable)
                {
                    if (!Universe.ActivePlayer.HasClimbed)
                    {
                        try
                        {
                            var eventArgs = new ContainerObjectEventArgs()
                                { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                            item.OnBeforeClimb(eventArgs);

                            Universe.ActivePlayer.HasClimbed = true;
                            Universe.ActivePlayer.ClimbedObject = item;
                            item.OnClimb(eventArgs);

                            item.OnAfterClimb(eventArgs);

                            return PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_CLIMBED, itemName);
                        }
                        catch (ClimbException ex)
                        {
                            Universe.ActivePlayer.HasClimbed = false;
                            Universe.ActivePlayer.ClimbedObject = default;
                            return PrintingSubsystem.Resource(ex.Message);
                        }
                    }

                    return Universe.ActivePlayer.ClimbedObject == item
                        ? PrintingSubsystem.FormattedResource(BaseDescriptions.ALREADY_CLIMBED_ITEM, itemName)
                        : PrintingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
                }

                return PrintingSubsystem.Resource(BaseDescriptions.IMPOSSIBLE_CLIMB);
            }

            if (Universe.ActivePlayer.HasClimbed && Universe.ActivePlayer.ClimbedObject != null)
            {
                return PrintingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_CLIMB);

    }
}