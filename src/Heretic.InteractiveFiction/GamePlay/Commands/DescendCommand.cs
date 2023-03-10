using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record DescendCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            return this.HandleDescendEventOnActiveLocation(adventureEvent);
        }

        if (adventureEvent.AllObjects.Count == 1)
        {
            return this.HandleDescendEventOnSingleObject(adventureEvent);
        }

        return true;
    }

    private bool HandleDescendEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        if (Universe.ActivePlayer.HasClimbed && Universe.ActivePlayer.ClimbedObject != default)
        {
            var item = Universe.ActivePlayer.ClimbedObject;
            try
            {
                var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                
                Universe.ActivePlayer.OnBeforeDescend(eventArgs);
                item.OnBeforeDescend(eventArgs);
                    
                Universe.ActivePlayer.DescendFromObject();
                Universe.ActivePlayer.OnDescend(eventArgs);
                item.OnDescend(eventArgs);
                    
                var result = PrintingSubsystem.Resource(BaseDescriptions.DESCENDING);
                    
                item.OnAfterDescend(eventArgs);
                Universe.ActivePlayer.OnAfterDescend(eventArgs);
                    
                return result;
            }
            catch (DescendException ex)
            {
                return PrintingSubsystem.Resource(ex.Message);
            }
        }
        return PrintingSubsystem.Resource(BaseDescriptions.NOT_CLIMBED);
    }
    
    private bool HandleDescendEventOnSingleObject(AdventureEvent adventureEvent)
    {
        if (Universe.ActivePlayer.HasClimbed && Universe.ActivePlayer.ClimbedObject != default)
        {
            var compareItem = adventureEvent.ObjectOne;
            var item = Universe.ActivePlayer.ClimbedObject;
            if (item.Key == compareItem.Key)
            {
                try
                {
                    var eventArgs = new ContainerObjectEventArgs() { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                    Universe.ActivePlayer.OnBeforeDescend(eventArgs);
                    item.OnBeforeDescend(eventArgs);

                    Universe.ActivePlayer.DescendFromObject();
                    Universe.ActivePlayer.OnDescend(eventArgs);
                    item.OnDescend(eventArgs);

                    var result = PrintingSubsystem.Resource(BaseDescriptions.DESCENDING);

                    item.OnAfterDescend(eventArgs);
                    Universe.ActivePlayer.OnAfterDescend(eventArgs);

                    return result;
                }
                catch (DescendException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            var itemName = ArticleHandler.GetNameWithArticleForObject(compareItem, GrammarCase.Dative, lowerFirstCharacter: true);
            return PrintingSubsystem.FormattedResource(BaseDescriptions.NOT_CLIMBED_ON_ITEM, itemName);
        }
            
        return PrintingSubsystem.Resource(BaseDescriptions.NOT_CLIMBED);
    }
}