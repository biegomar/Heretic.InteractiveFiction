using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record GoCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem, IDictionary<Directions, ICommand> DirectionCommands) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne is Location location)
        {
            if (Universe.LocationMap.ContainsKey(Universe.ActiveLocation))
            {
                var mappings = Universe.LocationMap[Universe.ActiveLocation];
                var direction = mappings.Where(i => !i.IsHidden && i.Location?.Key == location.Key)
                    .Select(x => x.Direction).SingleOrDefault();

                if (direction != default && DirectionCommands.ContainsKey(direction))
                {
                    var command = DirectionCommands[direction];
                    return command.Execute(adventureEvent);
                }
                
                return PrintingSubsystem.Resource(BaseDescriptions.NO_WAY);
            }

            return PrintingSubsystem.Resource(BaseDescriptions.ONLY_DIRECT_WAY);

        }

        return PrintingSubsystem.Resource(BaseDescriptions.NO_WAY);
    }
}