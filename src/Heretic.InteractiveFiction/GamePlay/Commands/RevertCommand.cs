using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record RevertCommand(HistoryAdministrator HistoryAdministrator) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        var revertedCommand = HistoryAdministrator.All.Count >= 2
            ? HistoryAdministrator.All.Skip(HistoryAdministrator.All.Count - 2).First()
            : string.Empty;

        throw new RevertException(string.Format(BaseDescriptions.REVERT_COMMAND, revertedCommand));
    }
}