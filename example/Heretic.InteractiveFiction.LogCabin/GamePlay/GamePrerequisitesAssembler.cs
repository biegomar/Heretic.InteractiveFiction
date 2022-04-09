using Heretic.InteractiveFiction.Comparer;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.LogCabin.Resources;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.LogCabin.GamePlay;

internal sealed class GamePrerequisitesAssembler: IGamePrerequisitesAssembler
{
    public GamePrerequisites AssembleGame()
    {
        var livingRoom = GetLivingRoom();
        
        var map = new LocationMap(new LocationComparer());

        var activeLocation = livingRoom;
        var activePlayer = GetPlayer();
        var actualQuests = GetQuests();
        
        return new GamePrerequisites(map, activeLocation, activePlayer, null, actualQuests);
    }

    private static Location GetLivingRoom()
    {
        var livingRoom = new Location()
        {
            Key = Keys.LIVINGROOM,
            Name = Locations.LIVINGROOM,
            Description = Descriptions.LIVINGROOM
        };

        return livingRoom;
    }

    private static Player GetPlayer()
    {
        var player = new Player()
        {
            Key = Keys.PLAYER,
            Name = "",
            Description = Descriptions.PLAYER,
        };

        return player;
    }
    
    private static ICollection<string> GetQuests()
    {
        var result = new List<string>
        {
            Descriptions.QUEST_I
        };

        return result;
    }
}