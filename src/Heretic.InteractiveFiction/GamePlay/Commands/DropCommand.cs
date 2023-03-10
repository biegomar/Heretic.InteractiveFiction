using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record DropCommand(Universe Universe, IGrammar Grammar, IPrintingSubsystem PrintingSubsystem, ICommand SleepCommand) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            if (adventureEvent.UnidentifiedSentenceParts.Any())
            {
                return PrintingSubsystem.ItemUnknown(adventureEvent);
            }

            return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_DROP);
        }

        if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
        {
            var playerAdventureEvent = new AdventureEvent();
            playerAdventureEvent.Predicate = Grammar.Verbs.SingleOrDefault(v => v.Key == VerbKey.SLEEP);
            playerAdventureEvent.AllObjects.AddRange(adventureEvent.AllObjects.Skip(1));
            return SleepCommand.Execute(playerAdventureEvent);
        }

        return this.HandleDrop(adventureEvent);
    }
    
    private bool HandleDrop(AdventureEvent adventureEvent)
    {
        var processingObject = adventureEvent.ObjectOne;
        var isPlayerItem = Universe.ActivePlayer.Items.Any(x => x.Key == processingObject.Key);
        var isPlayerCloths = Universe.ActivePlayer.Clothes.Any(x => x.Key == processingObject.Key);
        if (isPlayerItem || isPlayerCloths)
        {
            if (processingObject is Item { IsDropable: true } item)
            {
                try
                {
                    var dropItemEventArgs = new DropItemEventArgs()
                    {
                        OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage,
                        ItemToUse = adventureEvent.ObjectTwo
                    };

                    item.OnBeforeDrop(dropItemEventArgs);
                    
                    if (adventureEvent.ObjectTwo is {} container)
                    {
                        if (container.IsContainer)
                        {
                            if (!container.IsCloseable || container is { IsCloseable: true, IsClosed: false })
                            {
                                Universe.ActivePlayer.RemoveItem(item);
                                container.Items.Add(item);
                            
                                item.OnDrop(dropItemEventArgs);
                                PrintingSubsystem.ItemDropSuccess(item, container);   
                            }
                            else
                            {
                                PrintingSubsystem.ItemStillClosed(container);
                            }
                        }
                        else
                        {
                            return PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_NOT_A_DROPTARGET,
                                ArticleHandler.GetNameWithArticleForObject(container, GrammarCase.Accusative, lowerFirstCharacter: true));
                        }
                    }
                    else
                    {
                        Universe.ActivePlayer.RemoveItem(item);
                        Universe.ActiveLocation.Items.Add(item);
                        
                        item.OnDrop(dropItemEventArgs);
                        PrintingSubsystem.ItemDropSuccess(item);
                    }
                    
                    item.OnAfterDrop(dropItemEventArgs);

                    return true;
                }
                catch (DropException e)
                {
                    return PrintingSubsystem.Resource(e.Message);
                }
            }
            
            return PrintingSubsystem.ImpossibleDrop(processingObject);
        }
        
        return PrintingSubsystem.ItemNotOwned();
    }
}