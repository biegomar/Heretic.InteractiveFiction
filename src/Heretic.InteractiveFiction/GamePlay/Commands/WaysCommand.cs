using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record WaysCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (Universe.LocationMap.ContainsKey(Universe.ActiveLocation)
            && Universe.LocationMap[Universe.ActiveLocation].Any(l => !l.IsHidden))
        {
            return PrintingSubsystem.DestinationNode(Universe.ActiveLocation, Universe.LocationMap);
        }

        return PrintingSubsystem.Resource(BaseDescriptions.NO_WAYS);
    }
}