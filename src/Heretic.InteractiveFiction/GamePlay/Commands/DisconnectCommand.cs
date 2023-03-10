using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record DisconnectCommand(IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_DISCONNECT);

        }

        if (adventureEvent.AllObjects.Count == 1)
        {
            return PrintingSubsystem.FormattedResource(BaseDescriptions.WHAT_TO_DISCONNECT_FROM,
                ArticleHandler.GetNameWithArticleForObject(adventureEvent.ObjectOne, GrammarCase.Dative,
                    lowerFirstCharacter: true));
        }

        if (adventureEvent.AllObjects.Count > 2)
        {
            if (adventureEvent.AllObjects.Any(x => !x.IsLinkable))
            {
                var nonLinkableObjects = adventureEvent.AllObjects.Where(x => !x.IsLinkable).ToList();
                foreach (var nonLinkableObject in nonLinkableObjects)
                {
                    adventureEvent.AllObjects.Remove(nonLinkableObject);
                    adventureEvent.AllObjects.Add(nonLinkableObject);
                }
            }
        }

        return this.HandleDisconnect(adventureEvent);
    }
    
    private bool HandleDisconnect(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is Item item)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);

                if (item.IsLinked)
                {
                    if (adventureEvent.ObjectTwo is Item itemToUse)
                    {
                        if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(itemToUse))
                        {
                            if (itemToUse.IsLinked)
                            {
                                if (!item.LinkedTo.Contains(itemToUse) && !itemToUse.LinkedTo.Contains(item))
                                {
                                    return PrintingSubsystem.Resource(BaseDescriptions.ITEMS_NOT_CONNECTED);
                                }
                                
                                try
                                {
                                    string optionalErrorMessage = string.Empty;
                                    var errorMessage = adventureEvent.Predicate.ErrorMessage;
                                    if (!string.IsNullOrEmpty(errorMessage))
                                    {
                                        var itemName =
                                            ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Nominative,
                                                lowerFirstCharacter: true);
                                        optionalErrorMessage = string.Format(errorMessage, itemName);
                                    }

                                    var itemOneEventArgs = new DisconnectEventArgs()
                                    {
                                        OptionalErrorMessage = optionalErrorMessage,
                                        ItemToUse = itemToUse
                                    };

                                    var itemTwoEventArgs = new DisconnectEventArgs()
                                    {
                                        OptionalErrorMessage = optionalErrorMessage,
                                        ItemToUse = item
                                    };

                                    item.OnBeforeDisconnect(itemOneEventArgs);
                                    itemToUse.OnBeforeDisconnect(itemTwoEventArgs);

                                    item.OnDisconnect(itemOneEventArgs);
                                    itemToUse.OnDisconnect(itemTwoEventArgs);
                                    
                                    item.LinkedTo.Remove(itemToUse);
                                    itemToUse.LinkedTo.Remove(item);

                                    PrintingSubsystem.Resource(string.Format(BaseDescriptions.ITEM_NOT_CONNECTED_TO,
                                        ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative),
                                        ArticleHandler.GetNameWithArticleForObject(itemToUse, GrammarCase.Dative,
                                            lowerFirstCharacter: true)));

                                    item.OnAfterDisconnect(itemOneEventArgs);
                                    itemToUse.OnAfterDisconnect(itemTwoEventArgs);

                                    return true;
                                }
                                catch (DisconnectException ex)
                                {
                                    return PrintingSubsystem.Resource(ex.Message);
                                }
                            }
                            
                            return PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_NOT_CONNECTED_TO_OTHER_ITEMS,
                                ArticleHandler.GetNameWithArticleForObject(itemToUse, GrammarCase.Nominative));
                        }

                        return PrintingSubsystem.ItemNotVisible(itemToUse);
                    }

                    return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_USE);
                }
                
                return PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_NOT_CONNECTED_TO_OTHER_ITEMS,
                    ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Nominative));
            }

            return PrintingSubsystem.ItemNotVisible(item);
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_USE);
    }
}