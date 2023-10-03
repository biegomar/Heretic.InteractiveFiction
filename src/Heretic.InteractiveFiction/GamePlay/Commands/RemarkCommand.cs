using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record RemarkCommand() : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        return true;
    }
}