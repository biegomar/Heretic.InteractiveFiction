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
            var containerObjectEventArgs = new ContainerObjectEventArgs()
            {
                OptionalErrorMessage = adventureEvent.Predicate != default
                    ? adventureEvent.Predicate.ErrorMessage
                    : string.Empty
            };

            Universe.ActiveLocation.OnTaste(containerObjectEventArgs);

            var result = string.IsNullOrWhiteSpace(Universe.ActiveLocation.TasteDescription)
                ? PrintingSubsystem.Resource(BaseDescriptions.NOTHING_TO_TASTE)
                : PrintingSubsystem.Resource(Universe.ActiveLocation.TasteDescription);
                    
            return result;
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
                    var containerObjectEventArgs = new ContainerObjectEventArgs() {
                        OptionalErrorMessage = adventureEvent.Predicate != default
                        ? adventureEvent.Predicate.ErrorMessage
                        : string.Empty};
                    
                    item.OnTaste(containerObjectEventArgs);
                    
                    if (item is Character)
                    {
                        return PrintingSubsystem.Resource(BaseDescriptions.DONT_TASTE_A_PERSON);
                    }
                    
                    return string.IsNullOrWhiteSpace(item.TasteDescription)
                        ? PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_DOES_NOT_TASTE,
                            ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Dative, lowerFirstCharacter: true))
                        : PrintingSubsystem.Resource(item.TasteDescription);
                    
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