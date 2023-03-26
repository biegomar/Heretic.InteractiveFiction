using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record SleepCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            return this.HandleSleepEventOnActiveLocation(adventureEvent);
        }

        if (adventureEvent.AllObjects.Count == 1)
        {
            return this.HandleSleepEventOnSingleObject(adventureEvent);
        }

        return true;
    }

    private bool HandleSleepEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        try
        {
            var containerObjectEventArgs = new ContainerObjectEventArgs()
            {
                OptionalErrorMessage = adventureEvent.Predicate != default
                    ? adventureEvent.Predicate.ErrorMessage
                    : string.Empty
            };

            Universe.ActiveLocation.OnSleep(containerObjectEventArgs);

            return true;
        }
        catch (SleepException ex)
        {
            return PrintingSubsystem.Resource(ex.Message);
        }
    }
    
    private bool HandleSleepEventOnSingleObject(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is { } item)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                this.ObjectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var containerObjectEventArgs = new ContainerObjectEventArgs()
                    {
                        OptionalErrorMessage = adventureEvent.Predicate != default
                            ? adventureEvent.Predicate.ErrorMessage
                            : string.Empty
                    };
                    
                    item.OnSleep(containerObjectEventArgs);
                    
                    return true;
                }
                catch (SleepException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }
        }
        

        return PrintingSubsystem.ItemNotVisible();
    }
}