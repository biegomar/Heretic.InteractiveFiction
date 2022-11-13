using Heretic.InteractiveFiction.Comparer;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.TestGame.Resources;

namespace Heretic.InteractiveFiction.TestGame.GamePlay;

internal sealed class GamePrerequisitesAssembler: IGamePrerequisitesAssembler
{
    private EventProvider eventProvider;

    public GamePrerequisitesAssembler(EventProvider eventProvider)
    {
        this.eventProvider = eventProvider;
    }
    
    public GamePrerequisites AssembleGame()
    {
        var livingRoom = LivingRoomPrerequisites.Get(this.eventProvider);
        var bedRoom = BedRoomPrerequisites.Get(this.eventProvider);
        var cellar = CellarPrerequisites.Get(this.eventProvider);
        var attic = AtticPrerequisites.Get(this.eventProvider);
        var freedom = FreedomPrerequisites.Get();
        
        this.eventProvider.AddEventsForUniverse();
        
        var map = new LocationMap(new LocationComparer())
        {
            { livingRoom, LivingRoomLocationMap(bedRoom, cellar, freedom) },
            { bedRoom, BedRoomLocationMap(livingRoom, attic) },
            { cellar, CellarLocationMap(livingRoom) },
            { attic, AtticLocationMap(bedRoom) }
        };

        var activeLocation = livingRoom;
        var activePlayer = PlayerPrerequisites.Get(this.eventProvider);
        var actualQuests = GetQuests();
        
        return new GamePrerequisites(map, activeLocation, activePlayer, null, actualQuests);
    }

    private static ICollection<string> GetQuests()
    {
        var result = new List<string>
        {
            MetaData.QUEST_I,
            MetaData.QUEST_II
        };

        return result;
    }
    
    private static IEnumerable<DestinationNode> LivingRoomLocationMap(Location bedRoom, Location cellar, Location freedom)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.N, Location = bedRoom},
            new() {Direction = Directions.DOWN, Location = cellar, IsHidden = true},
            new() {Direction = Directions.S, Location = freedom, IsHidden = true, DestinationDescription = Descriptions.FREEDOM_DESTINATION_DESCRIPTION}
        };
        return locationMap;
    }
    
    private static IEnumerable<DestinationNode> BedRoomLocationMap(Location livingRoom, Location attic)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.S, Location = livingRoom},
            new() {Direction = Directions.UP, Location = attic}
        };
        return locationMap;
    }
    
    private static IEnumerable<DestinationNode> CellarLocationMap(Location livingRoom)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.UP, Location = livingRoom},
        };
        return locationMap;
    }
    
    private static IEnumerable<DestinationNode> AtticLocationMap(Location bedRoom)
    {
        var locationMap = new List<DestinationNode>
        {
            new() {Direction = Directions.DOWN, Location = bedRoom},
        };
        return locationMap;
    }
}