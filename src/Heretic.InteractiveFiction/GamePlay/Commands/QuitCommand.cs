using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record QuitCommand() : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        throw new QuitGameException(BaseDescriptions.QUIT_GAME);
    }
}