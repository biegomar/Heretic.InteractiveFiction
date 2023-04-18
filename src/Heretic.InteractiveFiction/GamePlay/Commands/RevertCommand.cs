using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record RevertCommand : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        throw new RevertException(BaseDescriptions.REVERT_COMMAND);
    }
}