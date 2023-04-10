using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

public record RestartCommand : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        throw new RestartException(BaseDescriptions.RESTART_GAME);
    }
}