using System.Collections.Generic;
using Heretic.InteractiveFiction.Comparer;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Test.Mocks;

internal class SmallWorldAssembler: IGamePrerequisitesAssembler
{
    public GamePrerequisites AssembleGame()
    {
        var livingRoom = GetLivingRoom();
        
        var map = new LocationMap(new LocationComparer())
        {
            { livingRoom, LivingRoomLocationMap() }
        };

        var activeLocation = livingRoom;
        var activePlayer = GetPlayer();
        var actualQuests = GetQuests();
        
        return new GamePrerequisites(map, activeLocation, activePlayer, null, actualQuests);
    }
    
    private static ICollection<string> GetQuests()
    {
        var result = new List<string>
        {
            "QUEST_I",
            "QUEST_II"
        };

        return result;
    }
    
    private static Player GetPlayer()
    {
        var player = new Player
        {
            Key = "PLAYER",
            Name = "Player",
            Description = "A player",
            Grammar = new Objects.Grammars(isPlayer:true)
        };
        
        player.Items.Add(GetKnife());

        return player;
    }
    
    private static Location GetLivingRoom()
    {
        var livingRoom = new Location()
        {
            Key = "LIVINGROOM",
            Name = "LIVINGROOM",
            Description = "LIVINGROOM",
            FirstLookDescription = "LIVINGROOM FIRSTLOOK"
        };
        
        livingRoom.Items.Add(GetTable());

        return livingRoom;
    }
    
    private static Item GetTable()
    {
        var table = new Item()
        {
            Key = "TABLE",
            Name = "TABLE",
            Description = "TABLE",
            IsPickable = false,
            IsContainer = true,
            IsSurfaceContainer = true,
            Grammar = new Objects.Grammars(Genders.Male)
        };

        table.Items.Add(GetCandle());
        
        return table;
    }
    
    private static Item GetCandle()
    {
        var candle = new Item()
        {
            Key = "CANDLE",
            Name = "CANDLE",
            Description = "CANDLE",
            ContainmentDescription = "CANDLE_CONTAINMENT"
        };

        return candle;
    }
    
    private static Item GetKnife()
    {
        var knife = new Item()
        {
            Key = "KNIFE",
            Name = "KNIFE",
            Description = "KNIFE",
            ContainmentDescription = "KNIFE_CONTAINMENT",
            Grammar = new Objects.Grammars(gender: Genders.Neutrum)
        };

        return knife;
    }
    
    private static IEnumerable<DestinationNode> LivingRoomLocationMap()
    {
        var locationMap = new List<DestinationNode>();
        return locationMap;
    }
}