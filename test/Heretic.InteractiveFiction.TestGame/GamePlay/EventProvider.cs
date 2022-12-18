using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;
using Heretic.InteractiveFiction.TestGame.Resources;

namespace Heretic.InteractiveFiction.TestGame.GamePlay;

internal class EventProvider
{
    private readonly Universe universe;
    private readonly ObjectHandler objectHandler;
    private readonly IPrintingSubsystem printingSubsystem;
    private bool isPaperInStove;
    private bool isPetroleumInStove;
    private bool isPetroleumInLamp;
    private int waitCounter;

    internal IDictionary<string, int> ScoreBoard => this.universe.ScoreBoard;

    public EventProvider(Universe universe, IPrintingSubsystem printingSubsystem)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
        this.objectHandler = new ObjectHandler(this.universe);
        this.waitCounter = 0;
    }
    
    internal void SetPlayersName(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Player)
        {
            this.universe.ActivePlayer.Name = eventArgs.ExternalItemKey;
            this.universe.ActivePlayer.IsStranger = false;
            printingSubsystem.ActivePlayer(this.universe.ActivePlayer);
        }
    }

    internal void UnhideMainEntrance(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item {Key: Keys.DOOR})
        {
            var destination = this.universe.GetDestinationNodeFromActiveLocationByDirection(Directions.S);
            destination.IsHidden = false;
        }
    }
    
    internal void TakeCandle(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.CANDLE } candle)
        {
            printingSubsystem.Resource(Descriptions.CANDLE_PICKUP);
            this.universe.Score += this.universe.ScoreBoard[nameof(this.TakeCandle)];
            candle.AfterTake -= this.TakeCandle;
        }
    }
    
    internal void EnterRoomWithoutLight(object sender, EnterLocationEventArgs eventArgs)
    {
        if (sender is Location)
        {
            if (!this.universe.ActivePlayer.Items.Any(x => x.IsLighter && x.IsLighterSwitchedOn))
            {
                throw new LeaveLocationException(Descriptions.CANT_LEAVE_ROOM_WITHOUT_LIGHT); 
            }
        }
    }

    internal void AddEventsForUniverse()
    {
        this.universe.PeriodicEvents += this.WaitForCandleToMelt;
        this.ScoreBoard.Add(nameof(WaitForCandleToMelt), 1);
    }

    internal void WaitForCandleToMelt(object sender, PeriodicEventArgs eventArgs)
    {
        if (sender is Universe)
        {
            var candleObject = this.objectHandler.GetObjectFromWorldByKey(Keys.CANDLE);
            var cookTopObject = this.objectHandler.GetObjectFromWorldByKey(Keys.COOKTOP);

            if (cookTopObject is Item { IsLighterSwitchedOn: true } cookTop && candleObject is Item candle && cookTop.OwnsObject(candle) && this.universe.ActiveLocation.Key == Keys.LIVINGROOM)
            {
                switch (waitCounter)
                {
                    case 0:
                    {
                        waitCounter++;
                        throw new PeriodicException(Descriptions.MELT_CANDLE_I);
                    }
                    case 1:
                    {
                        waitCounter++;
                        throw new PeriodicException(Descriptions.MELT_CANDLE_II);
                    }
                    case 2:
                    {
                        this.universe.PeriodicEvents -= WaitForCandleToMelt;
                        
                        var ironKey = candle.Items.Single(i => i.Key == Keys.IRON_KEY);
                        ironKey.IsHidden = false;
                        candle.RemoveItem(ironKey);
                        
                        cookTop.Items.Add(ironKey);
                        cookTop.RemoveItem(candle);
                        
                        this.universe.Score += this.universe.ScoreBoard[nameof(this.WaitForCandleToMelt)];
                        
                        throw new PeriodicException(Descriptions.MELT_CANDLE_III);
                    }
                }
            }
        }
    }
    
    internal void ReadNote(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.NOTE } note)
        {
            printingSubsystem.ForegroundColor = TextColor.Magenta;
            printingSubsystem.Resource(Descriptions.CENTRAL_MESSAGE_UNDERSTOOD);
            printingSubsystem.ResetColors();
            
            this.universe.Score += this.universe.ScoreBoard[nameof(this.ReadNote)];
            note.AfterRead -= this.ReadNote;
        }
    }
    
    internal void PutNoteInStove(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.NOTE })
        {
            if (this.isPetroleumInStove)
            {
                throw new DropException(Descriptions.PETROLEUM_IN_STOVE);
            }
            
            this.isPaperInStove = true;
        }
    }

    internal void PoorPetroleumInStove(object sender, UseItemEventArgs eventArgs)
    {
        if (sender is Item itemOne && eventArgs.ItemToUse is Item itemTwo && itemOne.Key != itemTwo.Key)
        {
            var itemList = new List<Item> { itemOne, itemTwo };
            var petroleum = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM);
            var destinationItem = itemList.SingleOrDefault(i => i.Key == Keys.STOVE);

            if (destinationItem == default)
            {
                destinationItem = itemList.SingleOrDefault(i => i.Key == Keys.PILE_OF_WOOD);
            }

            if (petroleum != default && destinationItem != default)
            {
                if (!this.universe.ActivePlayer.OwnsObject(petroleum))
                {
                    throw new UseException(BaseDescriptions.ITEM_NOT_OWNED);     
                }
                
                var stove = this.universe.ActiveLocation.GetItem(Keys.STOVE);
                if (stove is { IsClosed: true })
                {
                    throw new UseException(Descriptions.STOVE_MUST_BE_OPEN);
                }

                if (this.isPetroleumInStove)
                {
                    throw new UseException(Descriptions.PETROLEUM_IN_STOVE);
                }
                
                if (this.isPaperInStove)
                {
                    throw new UseException(Descriptions.PAPER_IN_STOVE);
                }

                PreparePileOfWoodWithPetroleum();
            }
        }
    }

    internal void PoorPetroleumInPetroleumLamp(object sender, UseItemEventArgs eventArgs)
    {
        if (sender is Item itemOne && eventArgs.ItemToUse is Item itemTwo && itemOne.Key != itemTwo.Key)
        {
            var itemList = new List<Item> { itemOne, itemTwo };
            var petroleum = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM);
            var destinationItem = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM_LAMP);

            if (petroleum != default && destinationItem != default)
            {
                if (!this.universe.ActivePlayer.OwnsObject(petroleum) || !this.universe.ActivePlayer.OwnsObject(petroleum))
                {
                    throw new UseException(BaseDescriptions.ITEM_NOT_OWNED);     
                }

                if (isPetroleumInLamp)
                {
                    throw new UseException(Descriptions.ENOUGH_PETROLEUM_IN_LAMP);
                }

                isPetroleumInLamp = true;
                destinationItem.FirstLookDescription = Descriptions.PETROLEUM_LAMP_FIRSTLOOK_POORED;
                printingSubsystem.Resource(Descriptions.POOR_PETROLEUM_IN_LAMP);
                this.universe.Score += this.universe.ScoreBoard[nameof(this.PoorPetroleumInPetroleumLamp)];
            }
        }
    }

    private void PreparePileOfWoodWithPetroleum()
    {
        var pileOfWood = this.universe.ActiveLocation.GetItem(Keys.PILE_OF_WOOD);
        pileOfWood.Description = Descriptions.PILE_OF_WOOD_WITH_PETROLEUM;
        this.isPetroleumInStove = true;
        printingSubsystem.Resource(Descriptions.POOR_PETROLEUM_OVER_WOOD);
    }

    internal void UseLightersOnThings(object sender, KindleItemEventArgs eventArgs)
    {
        if (sender is Item itemOne && eventArgs.ItemToUse is Item itemTwo && itemOne.Key != itemTwo.Key)
        {
            var itemList = new List<Item> { itemOne, itemTwo };
            
            //Candle and...
            var candle = itemList.SingleOrDefault(i => i.Key == Keys.CANDLE);
            if (candle != default)
            {
                if (!this.universe.ActivePlayer.OwnsObject(candle))
                {
                    var candleName = ArticleHandler.GetNameWithArticleForObject(candle, GrammarCase.Accusative, lowerFirstCharacter: true);
                    throw new KindleException(string.Format(BaseDescriptions.ITEM_NOT_OWNED_FORMATTED, candleName));     
                }
                
                //... pile of wood
                var pileOfWood = itemList.SingleOrDefault(i => i.Key == Keys.PILE_OF_WOOD);
                if (pileOfWood != default)
                {
                    StartFireInStoveWithLighterAndWood();
                }
                else
                {
                    //... note
                    var note = itemList.SingleOrDefault(i => i.Key == Keys.NOTE);
                    if (note != default)
                    {
                        StartFireInStoveWithLighterAndNote(candle, note);
                    }
                    else
                    {
                        //... petroleum lamp
                        var lamp = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM_LAMP);
                        if (lamp != default)
                        {
                            StartPetroleumLampWithCandle(lamp);
                        }
                    }
                }
            }
            else
            {
                //Petroleum lamp and...
                var petroleumLamp = itemList.SingleOrDefault(i => i.Key == Keys.PETROLEUM_LAMP);
                if (petroleumLamp != default)
                {
                    if (!this.universe.ActivePlayer.OwnsObject(petroleumLamp))
                    {
                        throw new KindleException(BaseDescriptions.ITEM_NOT_OWNED);     
                    }
                    
                    if (!petroleumLamp.IsLighterSwitchedOn)
                    {
                        var petroleumLampName = ArticleHandler.GetNameWithArticleForObject(petroleumLamp, GrammarCase.Accusative);
                        throw new KindleException(string.Format(Descriptions.PETROLEUM_LAMP_NOT_BURNING, petroleumLampName));
                    }
                
                    //... pile of wood
                    var pileOfWood = itemList.SingleOrDefault(i => i.Key == Keys.PILE_OF_WOOD);
                    if (pileOfWood != default)
                    {
                        StartFireInStoveWithLighterAndWood();
                    }
                    else
                    {
                        //... note
                        var note = itemList.SingleOrDefault(i => i.Key == Keys.NOTE);
                        if (note != default)
                        {
                            StartFireInStoveWithLighterAndNote(petroleumLamp, note);
                        }
                    } 
                }
            }
        }

        if (eventArgs.ItemToUse == default)
        {
            printingSubsystem.Resource(Descriptions.HOW_TO_DO);
            return;
        }

        if (sender is Item senderItem && eventArgs.ItemToUse is Item itemToUse && senderItem.Key == itemToUse.Key)
        {
            var itemName = ArticleHandler.GetNameWithArticleForObject(itemToUse, GrammarCase.Dative, lowerFirstCharacter: true);
            var senderItemName = ArticleHandler.GetNameWithArticleForObject(senderItem, GrammarCase.Accusative, lowerFirstCharacter: true);
            printingSubsystem.Resource(string.Format(Descriptions.FIRE_FIRE_WITH_FIRE, senderItemName, itemName));
        }
    }

    internal void StartPetroleumLampWithCandle(Item lamp)
    {
        if (!this.isPetroleumInLamp)
        {
            throw new KindleException(Descriptions.NO_PETROLEUM_IN_LAMP);
        }

        if (lamp.IsLighterSwitchedOn)
        {
            throw new KindleException(Descriptions.PETROLEUM_LAMP_IS_BURNING);
        }

        lamp.IsLighterSwitchedOn = true;
        printingSubsystem.Resource(Descriptions.PETROLEUM_LAMP_SWITCH_ON);
        this.universe.Score += this.universe.ScoreBoard[nameof(StartPetroleumLampWithCandle)];
    }

    private void StartFireInStoveWithLighterAndNote(Item lighter, Item note)
    {
        if (this.isPaperInStove)
        {
            CheckIfStoveIsOpen();
            
            RemovePaperFromStove();
            
            printingSubsystem.Resource(Descriptions.NOTE_BURNED);
            printingSubsystem.Resource(Descriptions.FIRE_STARTER);
        
            CloseStoveBecauseItIsToHot();

            IndicateThatCookingTopIsHot();

            AssignEventForCombustionChamber();
        
            this.universe.Score += this.universe.ScoreBoard[nameof(UseLightersOnThings)];
            this.universe.SolveQuest(MetaData.QUEST_II);
        }
        else
        {
            if (!this.universe.ActivePlayer.OwnsObject(note))
            {
                throw new KindleException(BaseDescriptions.ITEM_NOT_OWNED);     
            }

            this.universe.ActivePlayer.RemoveItem(note);
            var lighterName = ArticleHandler.GetNameWithArticleForObject(lighter, GrammarCase.Accusative);
            printingSubsystem.FormattedResource(Descriptions.BURN_NOTE, lighterName, true);
        }
    }

    internal void CantDropCandleInStove(object sender, DropItemEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.CANDLE } && eventArgs.ItemContainer is Item {Key: Keys.STOVE})
        {
            throw new DropException(Descriptions.CANT_DROP_CANDLE_IN_STOVE);
        }
    }
    
    internal void CantTakePetroleum(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.PETROLEUM})
        {
            throw new TakeException(Descriptions.CANT_TAKE_PETROLEUM);
        }
    }

    internal void ReadBooks(object sender, ReadItemEventArgs eventArgs)
    {
        if (sender is Item {Key: Keys.BOOKS})
        {
            Description desc = this.GetBookTitle();
            throw new ReadException(desc);
        }
    }
    
    internal Func<string> GetBookTitle()
    {
        var bookList = new List<string>
        {
            Descriptions.BOOK_I, Descriptions.BOOK_II, Descriptions.BOOK_III,
            Descriptions.BOOK_IV, Descriptions.BOOK_V
        }; 
        
        var rnd = new Random();
        
        return () => string.Format(Descriptions.BOOKS, bookList[rnd.Next(0, bookList.Count)]);
    }

    private void CantOpenStoveOnFire(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Item { Key: Keys.STOVE or Keys.COMBUSTION_CHAMBER})
        {
            throw new OpenException(Descriptions.CANT_OPEN_STOVE_ON_FIRE);
        }
    }

    internal void SmellInLivingRoom(object sender, ContainerObjectEventArgs eventArgs)
    {
        if (sender is Location {Key: Keys.LIVINGROOM})
        {
            throw new SmellException("Es liegt der feine Geruch von verbranntem Wachs in der Luft.");
        }
    }
    
    private void StartFireInStoveWithLighterAndWood()
    {
        CheckIfStoveIsOpen();

        CheckForFireStarters();
                
        if (this.isPaperInStove)
        {
            RemovePaperFromStove();
            printingSubsystem.Resource(Descriptions.NOTE_BURNED);
        }
        else if (this.isPetroleumInStove)
        {
            printingSubsystem.Resource(Descriptions.PETROLEUM_BURNED);
        }
        
        printingSubsystem.Resource(Descriptions.FIRE_STARTER);
        
        CloseStoveBecauseItIsToHot();

        IndicateThatCookingTopIsHot();

        AssignEventForCombustionChamber();
        
        this.universe.Score += this.universe.ScoreBoard[nameof(UseLightersOnThings)];
        this.universe.SolveQuest(MetaData.QUEST_II);
    }

    private void AssignEventForCombustionChamber()
    {
        var combustionChamber = this.objectHandler.GetObjectFromWorldByKey(Keys.COMBUSTION_CHAMBER);
        combustionChamber.BeforeOpen += this.CantOpenStoveOnFire;
    }

    private void CloseStoveBecauseItIsToHot()
    {
        var stove = this.universe.ActiveLocation.GetItem(Keys.STOVE);
        stove.IsClosed = true;
        stove.BeforeOpen += this.CantOpenStoveOnFire;
    }

    private void IndicateThatCookingTopIsHot()
    {
        var cookTop = this.universe.ActiveLocation.GetItem(Keys.COOKTOP);
        cookTop.IsLighterSwitchedOn = true;
    }

    private void CheckForFireStarters()
    {
        if (!this.isPetroleumInStove && !this.isPaperInStove)
        {
            throw new KindleException(Descriptions.NO_FIRE_ACCELERATOR);
        }
    }

    private void CheckIfStoveIsOpen()
    {
        var stove = this.universe.ActiveLocation.GetItem(Keys.STOVE);
        if (stove is { IsClosed: true })
        {
            throw new KindleException(Descriptions.STOVE_MUST_BE_OPEN);
        }
    }

    private void RemovePaperFromStove()
    {
        var stove = this.universe.ActiveLocation.GetItem(Keys.STOVE);
        stove.Items.Remove(stove.Items.Single(i => i.Key is Keys.NOTE));
        this.isPaperInStove = false;
    }
    
}