using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record HistoryCommand(IPrintingSubsystem PrintingSubsystem, HistoryAdministrator HistoryAdministrator) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        return PrintingSubsystem.History(HistoryAdministrator.All);
    }
}