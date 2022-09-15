using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Test.Mocks;

internal static class Fixture
{
    internal static Player GetPlayer()
    {
        return new Player
        {
            Key = "PLAYER",
            Name = "Player",
            Description = "A player",
            Grammar = new Objects.Grammars(isPlayer:true)
        };
    }
}