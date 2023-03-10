using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record HelpCommand(IGrammar Grammar, IPrintingSubsystem PrintingSubsystem) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        return PrintingSubsystem.Help(Grammar.Verbs);
    }
}