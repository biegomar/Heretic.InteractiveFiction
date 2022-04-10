using Heretic.InteractiveFiction.LogCabin.Resources;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.LogCabin.GamePlay;

internal static class PlayerPrerequisites
{
    internal static Player Get()
    {
        var player = new Player()
        {
            Key = Keys.PLAYER,
            Name = "",
            Description = Descriptions.PLAYER,
        };

        return player;
    }
}