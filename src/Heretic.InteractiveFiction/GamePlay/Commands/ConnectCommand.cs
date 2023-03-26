using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record ConnectCommand(IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler): ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_CONNECT);
        }

        if (adventureEvent.AllObjects.Count == 1 && adventureEvent.ObjectOne != null)
        {
            return PrintingSubsystem.FormattedResource(BaseDescriptions.WHAT_TO_CONNECT_TO,
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

        return this.HandleConnect(adventureEvent);
    }
    
    private bool HandleConnect(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is Item item)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
            {
                ObjectHandler.StoreAsActiveObject(item);

                if (item.IsLinkable)
                {
                    if (adventureEvent.ObjectTwo is Item itemToUse)
                    {
                        if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(itemToUse))
                        {
                            if (itemToUse.IsLinkable)
                            {
                                if (item.LinkedTo.Contains(itemToUse) && itemToUse.LinkedTo.Contains(item))
                                {
                                    return PrintingSubsystem.Resource(BaseDescriptions.ITEMS_ALREADY_CONNECTED);
                                }
                                
                                try
                                {
                                    string optionalErrorMessage = string.Empty;
                                    Description errorMessage = adventureEvent.Predicate != default
                                        ? adventureEvent.Predicate.ErrorMessage
                                        : string.Empty;
                                    if (!string.IsNullOrEmpty(errorMessage))
                                    {
                                        var itemName =
                                            ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Dative,
                                                lowerFirstCharacter: true);
                                        optionalErrorMessage = string.Format(errorMessage, itemName);
                                    }

                                    var itemOneEventArgs = new ConnectEventArgs()
                                    {
                                        OptionalErrorMessage = optionalErrorMessage,
                                        ItemToUse = itemToUse
                                    };

                                    var itemTwoEventArgs = new ConnectEventArgs()
                                    {
                                        OptionalErrorMessage = optionalErrorMessage,
                                        ItemToUse = item
                                    };

                                    item.OnBeforeConnect(itemOneEventArgs);
                                    itemToUse.OnBeforeConnect(itemTwoEventArgs);

                                    item.OnConnect(itemOneEventArgs);
                                    itemToUse.OnConnect(itemTwoEventArgs);
                                    
                                    item.LinkedTo.Add(itemToUse);
                                    itemToUse.LinkedTo.Add(item);

                                    PrintingSubsystem.Resource(string.Format(BaseDescriptions.ITEM_NOW_CONNECTED_TO,
                                        ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Nominative),
                                        ArticleHandler.GetNameWithArticleForObject(itemToUse, GrammarCase.Dative,
                                            lowerFirstCharacter: true)));

                                    item.OnAfterConnect(itemOneEventArgs);
                                    itemToUse.OnAfterConnect(itemTwoEventArgs);

                                    return true;
                                }
                                catch (ConnectException ex)
                                {
                                    return PrintingSubsystem.Resource(ex.Message);
                                }
                            }
                            
                            return PrintingSubsystem.FormattedResource(BaseDescriptions.IMPOSSIBLE_CONNECT,
                                ArticleHandler.GetNameWithArticleForObject(itemToUse, GrammarCase.Nominative));
                        }

                        return PrintingSubsystem.ItemNotVisible(itemToUse);
                    }

                    return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_USE);
                }
                
                return PrintingSubsystem.FormattedResource(BaseDescriptions.IMPOSSIBLE_CONNECT,
                    ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative));
            }

            return PrintingSubsystem.ItemNotVisible(item);
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_USE);
    }
}