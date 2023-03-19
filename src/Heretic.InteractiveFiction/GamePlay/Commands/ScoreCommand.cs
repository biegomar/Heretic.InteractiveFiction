using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal sealed record ScoreCommand(ScoreBoard ScoreBoard) : ICommand
{
    public bool Execute(AdventureEvent adventureEvent)
    {
        return this.ScoreBoard.PrintScore();
    }
}