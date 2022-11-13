using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.TestGame.Resources;

namespace Heretic.InteractiveFiction.TestGame.GamePlay;

internal static class BedRoomPrerequisites
{
    internal static Location Get(EventProvider eventProvider)
    {
        var bedRoom = new Location()
        {
            Key = Keys.BEDROOM,
            Name = Locations.BEDROOM,
            Description = Descriptions.BEDROOM,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        
        bedRoom.AddOptionalVerb(VerbKeys.USE, OptionalVerbs.POOR, string.Empty);
        bedRoom.AddOptionalVerb(VerbKeys.USE, OptionalVerbs.HOLD, Descriptions.NOTHING_TO_HOLD);
        bedRoom.AddOptionalVerb(VerbKeys.USE, OptionalVerbs.KINDLE, string.Empty);
        bedRoom.AddOptionalVerb(VerbKeys.DROP, OptionalVerbs.PUT, string.Empty);
        
        bedRoom.Items.Add(GetPetroleumLamp(eventProvider));
        
        AddChangeLocationEvents(bedRoom, eventProvider);
        
        return bedRoom;
    }
    
    private static Item GetPetroleumLamp(EventProvider eventProvider)
    {
        var lamp = new Item()
        {
            Key = Keys.PETROLEUM_LAMP,
            Name = Items.PETROLEUM_LAMP,
            Description = Descriptions.PETROLEUM_LAMP,
            FirstLookDescription = Descriptions.PETROLEUM_LAMP_FIRSTLOOK,
            IsLighter = true,
            LighterSwitchedOffDescription = Descriptions.LIGHTER_OFF,
            LighterSwitchedOnDescription = Descriptions.LIGHTER_ON
        };

        AddKindleEvents(lamp, eventProvider);
        AddPoorEvents(lamp, eventProvider);
        
        return lamp;
    }
    
    private static void AddChangeLocationEvents(Location room, EventProvider eventProvider)
    {
        room.BeforeChangeLocation += eventProvider.ChangeRoomWithoutLight;
    }
    
    private static void AddKindleEvents(Item item, EventProvider eventProvider)
    {
        item.Kindle += eventProvider.UseLightersOnThings;
        if (!eventProvider.ScoreBoard.ContainsKey(nameof(eventProvider.UseLightersOnThings)))
        {
            eventProvider.ScoreBoard.Add(nameof(eventProvider.UseLightersOnThings), 1);
        }
        if (!eventProvider.ScoreBoard.ContainsKey(nameof(eventProvider.StartPetroleumLampWithCandle)))
        {
            eventProvider.ScoreBoard.Add(nameof(eventProvider.StartPetroleumLampWithCandle), 1);
        }
    }
    
    private static void AddPoorEvents(Item item, EventProvider eventProvider)
    {
        item.Use += eventProvider.PoorPetroleumInPetroleumLamp;
        if (!eventProvider.ScoreBoard.ContainsKey(nameof(eventProvider.PoorPetroleumInPetroleumLamp)))
        {
            eventProvider.ScoreBoard.Add(nameof(eventProvider.PoorPetroleumInPetroleumLamp), 1);
        }
    }
}