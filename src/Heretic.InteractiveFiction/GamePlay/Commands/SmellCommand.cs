using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record SmellCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            return this.HandleSmellEventOnActiveLocation(adventureEvent);
        }

        if (adventureEvent.AllObjects.Count == 1)
        {
            return this.HandleSmellEventOnSingleObject(adventureEvent);
        }

        return true;
    }
    
    private bool HandleSmellEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        try
        {
            var containerObjectEventArgs = new ContainerObjectEventArgs()
            {
                OptionalErrorMessage = adventureEvent.Predicate != default
                    ? adventureEvent.Predicate.ErrorMessage
                    : string.Empty
            };

            Universe.ActiveLocation.OnSmell(containerObjectEventArgs);

            return true;
        }
        catch (SmellException ex)
        {
            return PrintingSubsystem.Resource(ex.Message);
        }
    }
    
    private bool HandleSmellEventOnSingleObject(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is {} activeObject)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(activeObject))
            {
                ObjectHandler.StoreAsActiveObject(activeObject);
                
                try
                {
                    Description optionalErrorMessage = adventureEvent.Predicate != default
                        ? adventureEvent.Predicate.ErrorMessage
                        : string.Empty;
                
                    if (activeObject is Character && string.IsNullOrEmpty(optionalErrorMessage))
                    {
                        optionalErrorMessage = BaseDescriptions.DONT_SMELL_ON_PERSON;
                    }
                    var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = optionalErrorMessage};
                    
                    activeObject.OnSmell(containerObjectEventArgs);
                    
                    return true;
                }
                catch (SmellException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }
        }
        
        return PrintingSubsystem.ItemNotVisible();
    }
}