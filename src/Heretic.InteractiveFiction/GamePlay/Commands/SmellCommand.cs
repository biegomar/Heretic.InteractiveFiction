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
                { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

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
        var activeObject = adventureEvent.ObjectOne;
        if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(activeObject))
        {
            ObjectHandler.StoreAsActiveObject(activeObject);
                
            try
            {
                var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                if (activeObject is Character && string.IsNullOrEmpty(containerObjectEventArgs.OptionalErrorMessage))
                {
                    containerObjectEventArgs.OptionalErrorMessage = BaseDescriptions.DONT_SMELL_ON_PERSON;
                }
                    
                activeObject.OnSmell(containerObjectEventArgs);
                    
                return true;
            }
            catch (SmellException ex)
            {
                return PrintingSubsystem.Resource(ex.Message);
            }
        }

        return PrintingSubsystem.ItemNotVisible();
    }
}