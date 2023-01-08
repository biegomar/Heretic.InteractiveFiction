using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.TestGame.Resources;

namespace Heretic.InteractiveFiction.TestGame.GamePlay;

internal static class PlayerPrerequisites
{
    internal static Player Get(EventProvider eventProvider)
    {
        var player = new Player()
        {
            Key = Keys.PLAYER,
            Name = "",
            Description = Descriptions.PLAYER,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };
        
        AddToBeEvents(player, eventProvider);

        return player;
    }
    
    private static void AddToBeEvents(Player you, EventProvider eventProvider)
    {
        you.ToBe += eventProvider.SetPlayersName;
    }
}