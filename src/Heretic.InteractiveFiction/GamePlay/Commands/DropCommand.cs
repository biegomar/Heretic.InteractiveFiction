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
            var playerAdventureEvent = new AdventureEvent
            {
                Predicate = Grammar.Verbs.SingleOrDefault(v => v.Key == VerbKey.SLEEP),
                AllObjects = adventureEvent.AllObjects.Skip(1).ToList()
            };
            return SleepCommand.Execute(playerAdventureEvent);
        }

        return this.HandleDrop(adventureEvent);
    }
    
    private bool HandleDrop(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is Item item)
        {
            var isPlayerItem = Universe.ActivePlayer.Items.Any(x => x.Key == item.Key);
            var isPlayerCloths = Universe.ActivePlayer.Clothes.Any(x => x.Key == item.Key);
            if (isPlayerItem || isPlayerCloths)
            {
                if (item.IsDropable)
                {
                    try
                    {
                        var dropItemEventArgs = new DropItemEventArgs()
                        {
                            OptionalErrorMessage = adventureEvent.Predicate != default
                                ? adventureEvent.Predicate.ErrorMessage
                                : string.Empty,
                            ItemToUse = adventureEvent.ObjectTwo
                        };

                        item.OnBeforeDrop(dropItemEventArgs);

                        if (adventureEvent.ObjectTwo is { } container)
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
                                    ArticleHandler.GetNameWithArticleForObject(container, GrammarCase.Accusative,
                                        lowerFirstCharacter: true));
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

                return PrintingSubsystem.ImpossibleDrop(item);
            }

            return PrintingSubsystem.ItemNotOwned();
        }
        
        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_DROP);
    }
}