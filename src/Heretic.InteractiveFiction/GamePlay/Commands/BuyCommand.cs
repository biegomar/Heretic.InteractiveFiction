using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record BuyCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.AllObjects.Count > 1)
        {
            if (adventureEvent.ObjectOne is { } player && player.Key == Universe.ActivePlayer.Key)
            {
                var adventureEventWithoutPlayer = ICommand.GetAdventureEventWithoutPlayer(adventureEvent);
                return this.HandleBuy(adventureEventWithoutPlayer);
            }
        }

        return this.HandleBuy(adventureEvent);
    }
    
    private bool HandleBuy(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != default)
        {
            if (Universe.ActiveLocation.OwnsObject(item))
            {
                if (!Universe.ActivePlayer.HasPaymentMethod)
                {
                    return PrintingSubsystem.PayWithWhat();
                }

                if (Universe.ActivePlayer.OwnsObject(item))
                {
                    return PrintingSubsystem.ItemAlreadyOwned();
                }

                ObjectHandler.StoreAsActiveObject(item);

                try
                {
                    var containerObjectEventArgs = new ContainerObjectEventArgs()
                        { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                    adventureEvent.ObjectOne.OnBuy(containerObjectEventArgs);

                    return true;
                }
                catch (BuyException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return PrintingSubsystem.Resource(BaseDescriptions.WHAT_TO_BUY);
    }
}