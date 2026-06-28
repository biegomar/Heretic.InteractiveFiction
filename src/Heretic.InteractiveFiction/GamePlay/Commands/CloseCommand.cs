using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record CloseCommand(IPrintingSubsystem PrintingSubsystem, ObjectResolver Resolver, ActiveObjectTracker Tracker, WorldMutator Mutator) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (Resolver.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                Tracker.Store(item);
                if (item.IsCloseable)
                {
                    if (item.IsClosed)
                    {
                        return PrintingSubsystem.ItemAlreadyClosed(item);
                    }

                    try
                    {
                        var eventArgs = new ContainerObjectEventArgs()
                        {
                            OptionalErrorMessage = adventureEvent.Predicate != default
                                ? adventureEvent.Predicate.ErrorMessage
                                : string.Empty
                        };
                    
                        item.OnBeforeClose(eventArgs);

                        item.IsClosed = true;
                        Mutator.HideItemsOnClose(item);
                        item.OnClose(eventArgs);

                        item.OnAfterClose(eventArgs);

                        return PrintingSubsystem.ItemClosed(item);
                    }
                    catch (CloseException ex)
                    {
                        item.IsClosed = false;
                        Mutator.UnhideItemsOnOpen(item);
                        return PrintingSubsystem.Resource(ex.Message);
                    }
                }

                return PrintingSubsystem.FormattedResource(BaseDescriptions.IMPOSSIBLE_CLOSE, item.Name);
            }
                
            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_CLOSE);
    }
}