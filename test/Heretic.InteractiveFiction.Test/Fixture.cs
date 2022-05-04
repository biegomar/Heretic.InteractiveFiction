using Heretic.InteractiveFiction.Objects;

namespace Heretic.Test;

internal static class Fixture
{
    internal static Player GetPlayer()
    {
        return new Player
        {
            Key = "PLAYER",
            Name = "Player",
            Description = "A player",
            Grammar = new Grammars(isPlayer:true)
        };
    }
}