using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record RestartCommand : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        throw new RestartException(BaseDescriptions.RESTART_GAME);
    }
}