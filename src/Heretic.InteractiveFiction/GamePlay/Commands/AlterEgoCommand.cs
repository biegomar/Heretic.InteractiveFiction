using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record AlterEgoCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, ObjectHandler ObjectHandler): ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (!adventureEvent.AllObjects.Any())
        {
            return this.HandleAlterEgoEventOnActiveLocation();
        }

        if (adventureEvent.AllObjects.Count == 1)
        {
            return this.HandleAlterEgoEventOnSingleObject(adventureEvent);
        }

        return true;
    }
    
    private bool HandleAlterEgoEventOnActiveLocation()
    {
        var item = Universe.ActiveObject ?? Universe.ActivePlayer;
        var result = PrintingSubsystem.AlterEgo(item);
        return result;
    }
    
    private bool HandleAlterEgoEventOnSingleObject(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (item != null && ObjectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
        {
            ObjectHandler.StoreAsActiveObject(item);
            var result = PrintingSubsystem.AlterEgo(item);
            return result;
        }

        return PrintingSubsystem.ItemNotVisible();
    }
}