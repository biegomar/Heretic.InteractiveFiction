using Heretic.InteractiveFiction.Grammars;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record RemarkCommand() : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        return true;
    }
}