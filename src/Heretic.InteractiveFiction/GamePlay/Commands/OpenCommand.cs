using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record OpenCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);
                if (item.IsCloseable)
                {
                    if (item.IsLocked)
                    {
                        return PrintingSubsystem.ItemStillLocked(item);
                    }

                    if (!item.IsClosed)
                    {
                        return PrintingSubsystem.ItemAlreadyOpen(item);
                    }

                    if (Universe.ActivePlayer.IsSitting && Universe.ActivePlayer.Seat == item)
                    {
                        return PrintingSubsystem.FormattedResource(
                            BaseDescriptions.CANT_OPEN_ITEM_WHILE_SITTING_ON_IT, item.Name);
                    }

                    if (Universe.ActivePlayer.HasClimbed && Universe.ActivePlayer.ClimbedObject == item)
                    {
                        return PrintingSubsystem.FormattedResource(
                            BaseDescriptions.CANT_OPEN_ITEM_WHILE_STANDING_ON_IT, item.Name);
                    }

                    try
                    {
                        var containerObjectEventArgs = new ContainerObjectEventArgs
                        {
                            OptionalErrorMessage = adventureEvent.Predicate != default
                                ? adventureEvent.Predicate.ErrorMessage
                                : string.Empty
                        };

                        item.OnBeforeOpen(containerObjectEventArgs);

                        item.IsClosed = false;
                        ObjectHandler.UnveilFirstLevelObjects(item);

                        item.OnOpen(containerObjectEventArgs);

                        var result = PrintingSubsystem.ItemOpen(item);

                        item.OnAfterOpen(containerObjectEventArgs);

                        return result;
                    }
                    catch (OpenException e)
                    {
                        item.IsClosed = true;
                        ObjectHandler.HideItemsOnClose(item);
                        return PrintingSubsystem.Resource(e.Message);
                    }
                }

                return PrintingSubsystem.FormattedResource(BaseDescriptions.IMPOSSIBLE_OPEN_ITEM,
                    item.Name.LowerFirstChar());
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_OPEN);
    }
}