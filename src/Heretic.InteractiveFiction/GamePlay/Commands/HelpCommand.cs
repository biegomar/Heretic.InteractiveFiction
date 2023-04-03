using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record HelpCommand(IHelpSubsystem HelpSubsystem) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        return HelpSubsystem.Help();
    }
}