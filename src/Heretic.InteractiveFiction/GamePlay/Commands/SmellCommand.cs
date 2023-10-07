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

            var result = string.IsNullOrWhiteSpace(Universe.ActiveLocation.SmellDescription)
                ? PrintingSubsystem.Resource(BaseDescriptions.NOTHING_TO_SMELL)
                : PrintingSubsystem.Resource(Universe.ActiveLocation.SmellDescription);
                    
            return result;
        }
        catch (SmellException ex)
        {
            return PrintingSubsystem.Resource(ex.Message);
        }
    }
    
    private bool HandleSmellEventOnSingleObject(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is {} item)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var containerObjectEventArgs = new ContainerObjectEventArgs() {
                        OptionalErrorMessage = adventureEvent.Predicate != default
                        ? adventureEvent.Predicate.ErrorMessage
                        : string.Empty};
                    
                    item.OnSmell(containerObjectEventArgs);
                    
                    if (item is Character)
                    {
                        return PrintingSubsystem.Resource(BaseDescriptions.DONT_SMELL_ON_PERSON);
                    }
                    
                    return string.IsNullOrWhiteSpace(item.SmellDescription)
                        ? PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_DOES_NOT_SMELL,
                            ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Dative, lowerFirstCharacter: true))
                        : PrintingSubsystem.Resource(item.SmellDescription);
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