using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

public interface IGamePrerequisitesAssembler
{
    (LocationMap map, Location activeLocation, Player activePlayer) AssembleGame();
}
