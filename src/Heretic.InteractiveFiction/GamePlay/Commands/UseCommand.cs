using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record UseCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.AllObjects.Count > 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                var adventureEventWithoutPlayer = ICommand.GetAdventureEventWithoutPlayer(adventureEvent);
                if (adventureEventWithoutPlayer.AllObjects.Count == 1)
                {
                    return HandleUseOnSingleObject(adventureEventWithoutPlayer);            
                }
                    
                return HandleUse(adventureEventWithoutPlayer);
            }
                
            return HandleUse(adventureEvent);
        }
            
        return HandleUseOnSingleObject(adventureEvent);
    }
    
    private bool HandleUse(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);

                if (adventureEvent.ObjectTwo is { } itemToUse && ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(itemToUse))
                {
                    try
                    {
                        string optionalErrorMessage = string.Empty;
                        var errorMessage = adventureEvent.Predicate.ErrorMessage;
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            var itemName =
                                ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Dative,
                                    lowerFirstCharacter: true);
                            optionalErrorMessage = string.Format(errorMessage, itemName);
                        }

                        var useItemEventArgs = new UseItemEventArgs()
                        {
                            OptionalErrorMessage = optionalErrorMessage,
                            ItemToUse = itemToUse
                        };

                        item.OnUse(useItemEventArgs);

                        return true;
                    }
                    catch (UseException ex)
                    {
                        return PrintingSubsystem.Resource(ex.Message);
                    }
                }
                
                return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_USE);
            }

            return PrintingSubsystem.ItemNotVisible(item);
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_USE);
    }
    
    private bool HandleUseOnSingleObject(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                return PrintingSubsystem.Resource(BaseDescriptions.CANT_USE_YOURSELF);
            }
            
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);
                
                try
                {
                    string optionalErrorMessage = string.Empty;
                    var errorMessage = adventureEvent.Predicate.ErrorMessage;
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        var itemName =
                            ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Dative,
                                lowerFirstCharacter: true);
                        optionalErrorMessage = string.Format(errorMessage, itemName);
                    }

                    var useItemEventArgs = new UseItemEventArgs()
                    {
                        OptionalErrorMessage = optionalErrorMessage
                    };

                    item.OnUse(useItemEventArgs);

                    return true;
                }
                catch (UseException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible(item);
        }
        
        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_USE);
    }
}