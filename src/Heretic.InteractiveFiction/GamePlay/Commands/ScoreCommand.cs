using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record ScoreCommand(Universe Universe, IPrintingSubsystem PrintingSubsystem) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        return PrintingSubsystem.Score(Universe.Score, Universe.MaxScore);
    }
}