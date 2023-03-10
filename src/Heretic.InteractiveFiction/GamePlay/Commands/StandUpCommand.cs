using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record StandUpCommand(Universe Universe, IGrammar Grammar, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler, ICommand DropCommand) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is { } item && item.Key != Universe.ActivePlayer.Key)
        {
            var playerAdventureEvent = new AdventureEvent();
            playerAdventureEvent.Predicate = Grammar.Verbs.SingleOrDefault(v => v.Key == VerbKey.DROP);
            playerAdventureEvent.AllObjects.AddRange(adventureEvent.AllObjects);
            return DropCommand.Execute(playerAdventureEvent);
        }

        return this.HandleStandUp(adventureEvent);
    }
    
    private bool HandleStandUp(AdventureEvent adventureEvent)
    {
        if (Universe.ActivePlayer.IsSitting && Universe.ActivePlayer.Seat != default)
        {
            var item = Universe.ActivePlayer.Seat;
            try
            {
                ObjectHandler.StoreAsActiveObject(item);

                var eventArgs = new ContainerObjectEventArgs()
                    { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                Universe.ActivePlayer.OnBeforeStandUp(eventArgs);
                item.OnBeforeStandUp(eventArgs);

                Universe.ActivePlayer.StandUpFromSeat();
                item.OnStandUp(eventArgs);
                var result = PrintingSubsystem.Resource(BaseDescriptions.STANDING_UP);

                item.OnAfterStandUp(eventArgs);
                Universe.ActivePlayer.OnAfterStandUp(eventArgs);
                return result;
            }
            catch (StandUpException ex)
            {
                Universe.ActivePlayer.SitDownOnSeat(item);
                return PrintingSubsystem.Resource(ex.Message);
            }
        }

        return PrintingSubsystem.Resource(BaseDescriptions.NOT_SITTING);
    }
}