using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record SitDownCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            return this.HandleSitDownEventOnActiveLocation(adventureEvent);
        }

        if (adventureEvent.AllObjects.Count == 1)
        {
            return this.HandleSitDownEventOnSingleObject(adventureEvent);
        }

        if (adventureEvent.AllObjects.Count > 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                var adventureEventWithoutPlayer = ICommand.GetAdventureEventWithoutPlayer(adventureEvent);
                return this.Execute(adventureEventWithoutPlayer);
            }
        }

        return true;
    }
    
    private bool HandleSitDownEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        var seatCount = Universe.ActiveLocation.Items.Count(x => x.IsSeatable);

        if (seatCount == 0)
        {
            return PrintingSubsystem.Resource(BaseDescriptions.NO_SEAT);
        }

        if (seatCount == 1)
        {
            var onlySeat = Universe.ActiveLocation.Items.Single(x => x.IsSeatable);
            ObjectHandler.StoreAsActiveObject(onlySeat);

            try
            {
                var sitDownEventArgs = new SitDownEventArgs
                    { ItemToSitOn = onlySeat, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                var downEventArgs = new SitDownEventArgs
                {
                    ItemToSitOn = Universe.ActivePlayer,
                    OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage
                };

                Universe.ActivePlayer.OnBeforeSitDown(sitDownEventArgs);
                onlySeat.OnBeforeSitDown(downEventArgs);

                Universe.ActivePlayer.SitDownOnSeat(onlySeat);
                Universe.ActivePlayer.OnSitDown(sitDownEventArgs);
                onlySeat.OnSitDown(downEventArgs);

                var result = PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_ONLY_SEAT,
                    ArticleHandler.GetNameWithArticleForObject(onlySeat, GrammarCase.Dative), true);

                onlySeat.OnAfterSitDown(downEventArgs);
                Universe.ActivePlayer.OnAfterSitDown(sitDownEventArgs);

                return result;
            }
            catch (SitDownException ex)
            {
                Universe.ActivePlayer.StandUpFromSeat();
                return PrintingSubsystem.Resource(ex.Message);
            }
        }

        return PrintingSubsystem.Resource(BaseDescriptions.MORE_SEATS);
    }
    
    private bool HandleSitDownEventOnSingleObject(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne.Key == Universe.ActivePlayer.Key)
        {
            return this.HandleSitDownEventOnActiveLocation(adventureEvent);
        }

        var item = adventureEvent.ObjectOne;
        if (ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
        {
            ObjectHandler.StoreAsActiveObject(item);
            if (item.IsSeatable)
            {
                try
                {
                    var sitDownEventArgs = new SitDownEventArgs
                        { ItemToSitOn = item, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                    var downEventArgs = new SitDownEventArgs
                    {
                        ItemToSitOn = Universe.ActivePlayer,
                        OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage
                    };

                    Universe.ActivePlayer.OnBeforeSitDown(sitDownEventArgs);
                    item.OnBeforeSitDown(downEventArgs);

                    Universe.ActivePlayer.SitDownOnSeat(item);
                    Universe.ActivePlayer.OnSitDown(sitDownEventArgs);
                    item.OnSitDown(downEventArgs);

                    var result = PrintingSubsystem.ItemSeated(item);

                    item.OnAfterSitDown(downEventArgs);
                    Universe.ActivePlayer.OnAfterSitDown(sitDownEventArgs);

                    return result;
                }
                catch (SitDownException ex)
                {
                    Universe.ActivePlayer.StandUpFromSeat();
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotSeatable(item);
        }

        return PrintingSubsystem.ItemNotVisible();
    }
}