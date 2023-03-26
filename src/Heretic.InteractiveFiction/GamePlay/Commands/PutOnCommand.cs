using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record PutOnCommand(Universe Universe, IGrammar Grammar, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler, ICommand ClimbCommand) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            if (adventureEvent.UnidentifiedSentenceParts.Any())
            {
                return PrintingSubsystem.ItemUnknown(adventureEvent);
            }
                
            return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_PUTON);
        }

        if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
        {
            var playerAdventureEvent = new AdventureEvent
            {
                Predicate = Grammar.Verbs.SingleOrDefault(v => v.Key == VerbKey.CLIMB),
                AllObjects = adventureEvent.AllObjects.Skip(1).ToList()
            };
            return ClimbCommand.Execute(playerAdventureEvent);
        }

        return this.HandlePutOn(adventureEvent);
    }
    
    private bool HandlePutOn(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        var target = adventureEvent.ObjectTwo;
        if (item != default && target != default)
        {
            if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item)
                || ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(target))
            {
                ObjectHandler.StoreAsActiveObject(item);

                if (target.IsSurfaceContainer)
                {
                    try
                    {
                        var putOnEventArgs = new PutOnEventArgs()
                        {
                            OptionalErrorMessage = adventureEvent.Predicate != default
                                ? adventureEvent.Predicate.ErrorMessage
                                : string.Empty,
                            ItemToUse = target
                        };

                        var targetEventArgs = new PutOnEventArgs()
                        {
                            OptionalErrorMessage = adventureEvent.Predicate != default
                                ? adventureEvent.Predicate.ErrorMessage
                                : string.Empty,
                            ItemToUse = item
                        };

                        item.OnBeforePutOn(putOnEventArgs);
                        target.OnBeforePutOn(targetEventArgs);

                        var playerItem = Universe.ActivePlayer.GetItem(item);
                        if (playerItem != default)
                        {
                            Universe.ActivePlayer.RemoveItem(playerItem);
                            target.Items.Add(playerItem);
                            playerItem.IsOnSurface = true;
                        }
                        else
                        {
                            var locationItem = Universe.ActiveLocation.GetItem(item);
                            if (locationItem != default)
                            {
                                Universe.ActiveLocation.RemoveItem(locationItem);
                                target.Items.Add(locationItem);
                                locationItem.IsOnSurface = true;
                            }
                            else
                            {
                                throw new PutOnException(BaseDescriptions.DOES_NOT_WORK);
                            }
                        }

                        item.OnPutOn(putOnEventArgs);
                        target.OnPutOn(targetEventArgs);
                        PrintingSubsystem.Resource(string.Format(BaseDescriptions.ITEM_PUTON, ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative),
                            ArticleHandler.GetNameWithArticleForObject(target, GrammarCase.Dative, lowerFirstCharacter: true)));

                        item.OnAfterPutOn(putOnEventArgs);
                        target.OnAfterPutOn(targetEventArgs);

                        return true;
                    }
                    catch (PutOnException ex)
                    {
                        return PrintingSubsystem.Resource(ex.Message);
                    }
                }

                return PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_NOT_A_TARGET,
                    ArticleHandler.GetNameWithArticleForObject(target, GrammarCase.Dative, lowerFirstCharacter: true));
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_PUTON);
    }

}