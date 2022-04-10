using Heretic.InteractiveFiction.LogCabin.Resources;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.LogCabin.GamePlay;

internal static class LivingRoomPrerequisites
{
    internal static Location Get()
    {
        var livingRoom = new Location()
        {
            Key = Keys.LIVINGROOM,
            Name = Locations.LIVINGROOM,
            Description = Descriptions.LIVINGROOM
        };
        
        livingRoom.Items.Add(GetChest());
        livingRoom.Items.Add(GetDoor());
        
        AddSurroundings(livingRoom);

        return livingRoom;
    }
    
    private static Item GetDoor()
    {
        var door = new Item()
        {
            Key = Keys.DOOR,
            Name = Items.DOOR,
            Description = Descriptions.DOOR,
            FirstLookDescription = Descriptions.DOOR_FIRSTLOOK,
            IsPickAble = false,
            IsLocked = true,
            IsClosed = true,
            IsCloseAble = true
        };

        return door;
    }

    private static Item GetChest()
    {
        var chest = new Item()
        {
            Key = Keys.CHEST,
            Name = Items.CHEST,
            Description = Descriptions.CHEST,
            IsPickAble = false,
            IsSeatAble = true,
            IsLocked = true,
            IsClosed = true,
            IsCloseAble = true
        };
        
        chest.LinkedTo.Add(GetNumberLock());
        
        return chest;
    }

    private static Item GetNumberLock()
    {
        var numberLock = new Item()
        {
            Key = Keys.NUMBER_LOCK,
            Name = Items.NUMBER_LOCK,
            Description = Descriptions.NUMBER_LOCK,
            LinkedToDescription = Descriptions.CHEST_LOCKED,
            IsPickAble = false,
            IsHidden = true,
            IsBreakable = true,
            Grammar = new Grammars(Genders.Neutrum)
        };
        
        return numberLock;
    }

    private static void AddSurroundings(Location livingRoom)
    {
        livingRoom.Surroundings.Add(Keys.PLANK, () => Descriptions.PLANK);
    }
}