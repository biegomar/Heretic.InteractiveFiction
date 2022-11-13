using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.TestGame.Resources;

namespace Heretic.InteractiveFiction.TestGame.GamePlay;

internal static class AtticPrerequisites
{
    internal static Location Get(EventProvider eventProvider)
    {
        var attic = new Location()
        {
            Key = Keys.ATTIC,
            Name = Locations.ATTIC,
            Description = Descriptions.ATTIC,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };
        
        AddChangeLocationEvents(attic, eventProvider);
        
        return attic;
    }   
    
    private static void AddChangeLocationEvents(Location room, EventProvider eventProvider)
    {
        room.BeforeChangeLocation += eventProvider.ChangeRoomWithoutLight;
    }
}