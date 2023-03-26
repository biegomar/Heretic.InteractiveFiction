using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record TasteCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            return this.HandleTasteEventOnActiveLocation(adventureEvent);
        }

        if (adventureEvent.AllObjects.Count == 1)
        {
            return this.HandleTasteEventOnSingleObject(adventureEvent);
        }

        return true;
    }
    
    private bool HandleTasteEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        try
        {
            Description optionalErrorMessage = adventureEvent.Predicate != default
                ? adventureEvent.Predicate.ErrorMessage
                : string.Empty;
            
            var containerObjectEventArgs = new ContainerObjectEventArgs
            {
                OptionalErrorMessage = !string.IsNullOrEmpty(optionalErrorMessage)
                    ? optionalErrorMessage
                    : BaseDescriptions.WHAT_TO_TASTE
            };

            Universe.ActiveLocation.OnTaste(containerObjectEventArgs);

            return true;
        }
        catch (TasteException ex)
        {
            return PrintingSubsystem.Resource(ex.Message);
        }
    }
    
    private bool HandleTasteEventOnSingleObject(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is {} item)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var containerObjectEventArgs = new ContainerObjectEventArgs()
                    {
                        OptionalErrorMessage = adventureEvent.Predicate != default
                            ? adventureEvent.Predicate.ErrorMessage
                            : string.Empty
                    };
                    
                    item.OnTaste(containerObjectEventArgs);
                    
                    return true;
                }
                catch (TasteException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }
        }
        
        return PrintingSubsystem.ItemNotVisible();
    }
}