using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.TestGame.Resources;

namespace Heretic.InteractiveFiction.TestGame.GamePlay;

internal static class LivingRoomPrerequisites
{
    internal static Location Get(EventProvider eventProvider)
    {
        var livingRoom = new Location()
        {
            Key = Keys.LIVINGROOM,
            Name = Locations.LIVINGROOM,
            Description = Descriptions.LIVINGROOM,
            FirstLookDescription = Descriptions.CANDLE_CONTAINMENT
        };

        livingRoom.AddOptionalVerb(VerbKey.USE, OptionalVerbs.POOR, string.Empty);
        livingRoom.AddOptionalVerb(VerbKey.USE, OptionalVerbs.HOLD, Descriptions.NOTHING_TO_HOLD);
        livingRoom.AddOptionalVerb(VerbKey.DROP, OptionalVerbs.PUT, string.Empty);
        
        livingRoom.Items.Add(GetTable(eventProvider));
        livingRoom.Items.Add(GetChest());
        livingRoom.Items.Add(GetStove(eventProvider));
        livingRoom.Items.Add(GetKitchenCabinet(eventProvider));
        livingRoom.Items.Add(GetBookShelf(eventProvider));
        livingRoom.Items.Add(GetDoor(eventProvider));

        AddChangeLocationEvents(livingRoom, eventProvider);
        AddSmellEvents(livingRoom, eventProvider);
        
        AddSurroundings(livingRoom);

        return livingRoom;
    }

    private static Item GetKitchenCabinet(EventProvider eventProvider)
    {
        var cabinet = new Item
        {
            Key = Keys.KITCHEN_CABINET,
            Name = Items.KITCHEN_CABINET,
            Description = Descriptions.KITCHEN_CABINET,
            ContainmentDescription = Descriptions.KITCHEN_CABINET_CONTAINMENT,
            IsPickable = false,
            IsContainer = true,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };
        
        cabinet.Items.Add(GetSausage());
        cabinet.Items.Add(GetLampOilBucket(eventProvider));

        return cabinet;
    }

    private static Item GetSausage()
    {
        var sausage = new Item
        {
            Key = Keys.SAUSAGE,
            Name = Items.SAUSAGE,
            Description = Descriptions.SAUSAGE,
            ContainmentDescription = Descriptions.SAUSAGE_CONTAINMENT,
            IsHidden = true,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };
        
        return sausage;
    }

    private static Item GetLampOilBucket(EventProvider eventProvider)
    {
        var lampOilBucket = new Item
        {
            Key = Keys.LAMP_OIL_BUCKET,
            Name = Items.LAMP_OIL_BUCKET,
            Description = Descriptions.LAMP_OIL_BUCKET,
            Adjectives = Adjectives.LAMP_OIL_BUCKET,
            FirstLookDescription = Descriptions.LAMP_OIL_BUCKET_FIRSTLOOK,
            ContainmentDescription = Descriptions.LAMP_OIL_BUCKET_CONTAINMENT,
            IsHidden = true
        };
        
        lampOilBucket.Items.Add(GetPetroleum(eventProvider));

        return lampOilBucket;
    }

    private static Item GetPetroleum(EventProvider eventProvider)
    {
        var petroleum = new Item
        {
            Key = Keys.PETROLEUM,
            Name = Items.PETROLEUM,
            Description = Descriptions.PETROLEUM,
            IsHidden = true,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum, isAbstract: true)
        };

        AddTakeEvents(petroleum, eventProvider);
        AddPoorEvents(petroleum, eventProvider);
        return petroleum;
    }

    private static void AddTakeEvents(Item petroleum, EventProvider eventProvider)
    {
        petroleum.BeforeTake += eventProvider.CantTakePetroleum;
    }

    private static Item GetTable(EventProvider eventProvider)
    {
        var table = new Item()
        {
            Key = Keys.TABLE,
            Name = Items.TABLE,
            Description = Descriptions.TABLE,
            IsPickable = false,
            IsContainer = true,
            IsSurfaceContainer = true,
            IsShownInObjectList = false,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };

        table.Items.Add(GetCandle(eventProvider));
        table.Items.Add(GetNote(eventProvider));

        return table;
    }

    private static Item GetCandle(EventProvider eventProvider)
    {
        var candle = new Item()
        {
            Key = Keys.CANDLE,
            Name = Items.CANDLE,
            Description = Descriptions.CANDLE,
            ContainmentDescription = Descriptions.CANDLE_CONTAINMENT,
            IsLighter = true,
            IsLighterSwitchedOn = true,
            LighterSwitchedOnDescription = Descriptions.LIGHTER_ON
        };
        
        candle.Items.Add(GetIronKey());

        AddAfterTakeEvents(candle, eventProvider);
        AddKindleEvents(candle, eventProvider);
        AddBeforeDropEvents(candle, eventProvider);

        return candle;
    }

    private static Item GetNote(EventProvider eventProvider)
    {
        var note = new Item
        {
            Key = Keys.NOTE,
            Name = Items.NOTE,
            Description = Descriptions.NOTE,
            IsHidden = true,
            IsReadable = true,
            LetterContentDescription = Descriptions.NOTE_LETTER_CONTENT
        };
        
        AddReadEvents(note, eventProvider);
        AddDropEvents(note, eventProvider);
        AddKindleEvents(note, eventProvider);
        
        return note;
    }

    private static Item GetIronKey()
    {
        var ironKey = new Item()
        {
            Key = Keys.IRON_KEY,
            Name = Items.IRON_KEY,
            Description = Descriptions.IRON_KEY,
            IsHidden = true,
            IsUnveilable = false,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };

        return ironKey;
    }
    
    private static Item GetDoor(EventProvider eventProvider)
    {
        var door = new Item()
        {
            Key = Keys.DOOR,
            Name = Items.DOOR,
            Description = Descriptions.DOOR,
            FirstLookDescription = Descriptions.DOOR_FIRSTLOOK,
            IsPickable = false,
            IsLocked = true,
            IsClosed = true,
            IsCloseable = true
        };
        
        AddLookEvents(door, eventProvider);

        return door;
    }

    private static void AddLookEvents(Item door, EventProvider eventProvider)
    {
        door.Look += eventProvider.UnhideMainEntrance;
    }

    private static Item GetChest()
    {
        var chest = new Item()
        {
            Key = Keys.CHEST,
            Name = Items.CHEST,
            Description = Descriptions.CHEST,
            Adjectives = "groß",
            LockDescription = Descriptions.CHEST_LOCKED,
            IsLocked = true,
            IsLockable = true,
            UnlockWithKey = Keys.IRON_KEY,
            IsPickable = false,
            IsSeatable = true,
            IsClosed = true,
            IsCloseable = true
        };

        return chest;
    }

    private static Item GetStove(EventProvider eventProvider)
    {
        var stove = new Item()
        {
            Key = Keys.STOVE,
            Name = Items.STOVE,
            Description = Descriptions.STOVE,
            FirstLookDescription = Descriptions.STOVE_FIRSTLOOK,
            CloseDescription = Descriptions.STOVE_CLOSED,
            OpenDescription = Descriptions.STOVE_OPEN,
            ContainmentDescription = Descriptions.STOVE_CONTAINMENT,
            IsPickable = false,
            IsClosed = true,
            IsCloseable = true,
            IsContainer = true,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };

        AddPoorEvents(stove, eventProvider);
        
        stove.Items.Add(GetPileOfWood(eventProvider));
        stove.LinkedTo.Add(GetCookTop(eventProvider));

        return stove;
    }

    private static Item GetPileOfWood(EventProvider eventProvider)
    {
        var wood = new Item()
        {
            Key = Keys.PILE_OF_WOOD,
            Name = Items.PILE_OF_WOOD,
            Description = Descriptions.PILE_OF_WOOD,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum, isAbstract: true)
        };

        AddKindleEvents(wood, eventProvider);
        AddPoorEvents(wood, eventProvider);

        return wood;
    }

    private static Item GetCookTop(EventProvider eventProvider)
    {
        var cookTop = new Item
        {
            Key = Keys.COOKTOP,
            Name = Items.COOKTOP,
            Description = Descriptions.COOKTOP,
            LinkedToDescription = Descriptions.COOCKTOP_LINKEDTO,
            IsPickable = false,
            IsContainer = true,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };

        return cookTop;
    }
    
    private static Item GetBooks(EventProvider eventProvider)
    {
        var books = new Item()
        {
            Key = Keys.BOOKS,
            Name = Items.BOOKS,
            Description = eventProvider.GetBookTitle(),
            IsSurrounding = true,
            IsPickable = false,
            IsReadable = true,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        
        AddReadBookEvents(books, eventProvider);
        
        return books;
    }

    private static Item GetBookShelf(EventProvider eventProvider)
    {
        var bookShelf = new Item()
        {
            Key = Keys.BOOKSHELF,
            Name = Items.BOOKSHELF,
            Description = Descriptions.BOOKSHELF,
            ContainmentDescription = Descriptions.BOOKSHELF_CONTAINMENT,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        
        bookShelf.Items.Add(GetBooks(eventProvider));
        
        return bookShelf;
    }

    private static void AddReadBookEvents(Item item, EventProvider eventProvider)
    {
        item.BeforeRead += eventProvider.ReadBooks;
    }

    private static void AddAfterTakeEvents(Item item, EventProvider eventProvider)
    {
        item.AfterTake += eventProvider.TakeCandle;
        eventProvider.RegisterScore(nameof(eventProvider.TakeCandle), 1);
    }

    private static void AddChangeLocationEvents(Location room, EventProvider eventProvider)
    {
        room.BeforeEnterLocation += eventProvider.EnterRoomWithoutLight;
    }

    private static void AddKindleEvents(Item item, EventProvider eventProvider)
    {
        item.Kindle += eventProvider.UseLightersOnThings;
        eventProvider.RegisterScore(nameof(eventProvider.UseLightersOnThings), 1);
    }

    private static void AddPoorEvents(Item item, EventProvider eventProvider)
    {
        item.Use += eventProvider.PoorPetroleumInStove;
        item.Use += eventProvider.PoorPetroleumInPetroleumLamp;
    }
    
    private static void AddBeforeDropEvents(Item candle, EventProvider eventProvider)
    {
        candle.BeforeDrop += eventProvider.CantDropCandleInStove;
    }
    
    private static void AddReadEvents(Item note, EventProvider eventProvider)
    {
        note.AfterRead += eventProvider.ReadNote;
        eventProvider.RegisterScore(nameof(eventProvider.ReadNote), 1);
    }
    
    private static void AddDropEvents(Item note, EventProvider eventProvider)
    {
        note.BeforeDrop += eventProvider.PutNoteInStove;
    }

    private static void AddSmellEvents(Location room, EventProvider eventProvider)
    {
        room.Smell += eventProvider.SmellInLivingRoom;
    }

    private static void AddSurroundings(Location livingRoom)
    {
        var plank = new Item()
        {
            Key = Keys.PLANK,
            Name = Items.PLANK,
            Description = Descriptions.PLANK,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Female, false)
        };
        livingRoom.Items.Add(plank);

        var keyHole = new Item()
        {
            Key = Keys.KEY_HOLE,
            Name = Items.KEY_HOLE,
            Description = Descriptions.KEY_HOLE,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        livingRoom.Items.Add(keyHole);
        
        var keyHoleShield = new Item()
        {
            Key = Keys.KEY_HOLE_SHIELD,
            Name = Items.KEY_HOLE_SHIELD,
            Description = Descriptions.KEY_HOLE_SHIELD,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        livingRoom.Items.Add(keyHoleShield);
        
        var chestLock = new Item()
        {
            Key = Keys.CHEST_LOCK,
            Name = Items.CHEST_LOCK,
            Description = Descriptions.CHEST_LOCK,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        livingRoom.Items.Add(chestLock);
        
        var wall = new Item()
        {
            Key = Keys.WALL,
            Name = Items.WALL,
            Description = Descriptions.WALL,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar()
        };
        livingRoom.Items.Add(wall);
        
        var floor = new Item()
        {
            Key = Keys.FLOOR,
            Name = Items.FLOOR,
            Description = Descriptions.FLOOR,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };
        livingRoom.Items.Add(floor);
        
        var ceiling = new Item()
        {
            Key = Keys.CEILING,
            Name = Items.CEILING,
            Description = Descriptions.CEILING,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar()
        };
        livingRoom.Items.Add(ceiling);
        
        var livingRoomWindows = new Item()
        {
            Key = Keys.LIVINGROOM_WINDOW,
            Name = Items.LIVINGROOM_WINDOW,
            Description = Descriptions.LIVINGROOM_WINDOW,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        livingRoom.Items.Add(livingRoomWindows);
        
        var shutter = new Item()
        {
            Key = Keys.SHUTTER,
            Name = Items.SHUTTER,
            Description = Descriptions.SHUTTER,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(isSingular:false)
        };
        livingRoom.Items.Add(shutter);
        
        var inspectionWindows = new Item()
        {
            Key = Keys.INSPECTION_WINDOW,
            Name = Items.INSPECTION_WINDOW,
            Description = Descriptions.INSPECTION_WINDOW,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        };
        livingRoom.Items.Add(inspectionWindows);
        
        var combustionChamber = new Item()
        {
            Key = Keys.COMBUSTION_CHAMBER,
            Name = Items.COMBUSTION_CHAMBER,
            Description = Descriptions.COMBUSTION_CHAMBER,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar()
        };
        livingRoom.Items.Add(combustionChamber);

        var chimney = new Item()
        {
            Key = Keys.CHIMNEY,
            Name = Items.CHIMNEY,
            Description = Descriptions.CHIMNEY,
            IsSurrounding = true,
            IsPickable = false,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };
        livingRoom.Items.Add(chimney);
    }
}