using System.Reflection;
using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

public class CommandExecutor
{
    private readonly Universe universe;
    private readonly IGrammar grammar;
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly ObjectHandler objectHandler;
    
    private bool isHintActive;
    
    internal CommandExecutor(Universe universe, IGrammar grammar, IPrintingSubsystem printingSubsystem)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
        this.grammar = grammar;
        this.objectHandler = new ObjectHandler(universe);
        this.isHintActive = false;
    }
    
    internal bool AlterEgo(AdventureEvent adventureEvent)
    {
        if (VerbKeys.ALTER_EGO == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne == default && adventureEvent.ObjectTwo == default)
            {
                return this.HandleAlterEgoEventOnActiveLocation(adventureEvent);    
            }
            else if (adventureEvent.ObjectOne != default)
            {
                return this.HandleAlterEgoEventOnSingleObject(adventureEvent);
            }

            return true;
        }

        return false;
    }
    
    private bool HandleAlterEgoEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        var item = this.universe.ActiveObject ?? this.universe.ActivePlayer;
        if (item != default)
        {
            var result = printingSubsystem.AlterEgo(item);
            return result;
        }

        return printingSubsystem.ItemNotVisible();
    }
    
    private bool HandleAlterEgoEventOnSingleObject(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
        {
            this.objectHandler.StoreAsActiveObject(item);
            var result = printingSubsystem.AlterEgo(item);
            return result;
        }

        return printingSubsystem.ItemNotVisible();
    }
    
    internal bool Buy(AdventureEvent adventureEvent)
    {
        if (VerbKeys.BUY == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    if (!this.universe.ActivePlayer.HasPaymentMethod)
                    {
                        return printingSubsystem.PayWithWhat();
                    }
                
                    if (this.universe.ActivePlayer.OwnsObject(item))
                    {
                        return printingSubsystem.ItemAlreadyOwned();
                    }

                    if (this.universe.ActiveLocation.OwnsObject(item) && !item.IsHidden)
                    {
                        this.objectHandler.StoreAsActiveObject(item);
                
                        try
                        {
                            var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                            adventureEvent.ObjectOne.OnBuy(containerObjectEventArgs);
                    
                            return true;
                        }
                        catch (BuyException ex)
                        {
                            return printingSubsystem.Resource(ex.Message);
                        }
                    } 
                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Was genau möchtest Du kaufen?");
        }

        return false;
    }
    
    internal bool Break(AdventureEvent adventureEvent)
    {
        if (VerbKeys.BREAK == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    if (item.IsBreakable)
                    {
                        if (!item.IsBroken)
                        {
                            try
                            {
                                var eventArgs = new BreakItemEventArgs()
                                    { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                                item.OnBeforeBreak(eventArgs);

                                item.IsBroken = true;
                                item.OnBreak(eventArgs);

                                item.OnAfterBreak(eventArgs);
                                return true;
                            }
                            catch (BreakException ex)
                            {
                                item.IsBroken = false;
                                return printingSubsystem.Resource(ex.Message);
                            }
                        }

                        return printingSubsystem.ItemAlreadyBroken(item);
                    }

                    return printingSubsystem.ItemUnbreakable(item);

                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Was genau möchtest Du zerbrechen?");
        }

        return false;
    }
    
    internal bool Climb(AdventureEvent adventureEvent)
    {
        if (VerbKeys.CLIMB == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    
                    var itemName =
                        ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                            lowerFirstCharacter: true);

                    if (item.IsClimbable)
                    {
                        if (!this.universe.ActivePlayer.HasClimbed)
                        {
                            try
                            {
                                var eventArgs = new ContainerObjectEventArgs()
                                    { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                                item.OnBeforeClimb(eventArgs);

                                this.universe.ActivePlayer.HasClimbed = true;
                                this.universe.ActivePlayer.ClimbedObject = item;
                                item.OnClimb(eventArgs);

                                item.OnAfterClimb(eventArgs);

                                return printingSubsystem.FormattedResource(BaseDescriptions.ITEM_CLIMBED, itemName);
                            }
                            catch (ClimbException ex)
                            {
                                this.universe.ActivePlayer.HasClimbed = false;
                                this.universe.ActivePlayer.ClimbedObject = default;
                                return printingSubsystem.Resource(ex.Message);
                            }
                        }

                        return this.universe.ActivePlayer.ClimbedObject == item
                            ? printingSubsystem.FormattedResource(BaseDescriptions.ALREADY_CLIMBED_ITEM, itemName)
                            : printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
                    }

                    return printingSubsystem.Resource(BaseDescriptions.IMPOSSIBLE_CLIMB);
                }

                if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != null)
                {
                    return printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Worauf genau möchtest Du klettern?");
        }

        return false;
    }
    
    internal bool Close(AdventureEvent adventureEvent)
    {
        if (VerbKeys.CLOSE == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    if (item.IsCloseable)
                    {
                        if (item.IsClosed)
                        {
                            return printingSubsystem.ItemAlreadyClosed(item);
                        }

                        try
                        {
                            var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                    
                            item.OnBeforeClose(eventArgs);

                            item.IsClosed = true;
                            this.objectHandler.HideItemsOnClose(item);
                            item.OnClose(eventArgs);

                            item.OnAfterClose(eventArgs);

                            return printingSubsystem.ItemClosed(item);
                        }
                        catch (CloseException ex)
                        {
                            item.IsClosed = false;
                            this.objectHandler.UnhideItemsOnOpen(item);
                            return printingSubsystem.Resource(ex.Message);
                        }
                    }

                    return printingSubsystem.FormattedResource(BaseDescriptions.IMPOSSIBLE_CLOSE, item.Name);
                }
                
                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Was genau möchtest Du schliessen?");
        }

        return false;
    }
    
    internal bool Credits(AdventureEvent adventureEvent)
    {
        if (VerbKeys.CREDITS == adventureEvent.Predicate.Key)
        {
            return printingSubsystem.Credits();
        }

        return false;
    }
    
    internal bool Cut(AdventureEvent adventureEvent)
    {
        if (VerbKeys.CUT == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);

                    try
                    {
                        var containerObjectEventArgs = new CutItemEventArgs
                            { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                        item.OnCut(containerObjectEventArgs);

                        return true;
                    }
                    catch (CutException ex)
                    {
                        return printingSubsystem.Resource(ex.Message);
                    }
                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Was genau möchtest Du schneiden?");
        }

        return false;
    }
    
    internal bool Descend(AdventureEvent adventureEvent)
    {
        if (VerbKeys.DESCEND == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne == default && adventureEvent.ObjectTwo == default)
            {
                return this.HandleDescendEventOnActiveLocation(adventureEvent);    
            }
            else if (adventureEvent.ObjectOne != default)
            {
                return this.HandleDescendEventOnSingleObject(adventureEvent);
            }

            return true;
        }

        return false;
    }

    internal bool Drink(AdventureEvent adventureEvent)
    {
        if (VerbKeys.DRINK == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    var itemName =
                        ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                            lowerFirstCharacter: true);

                    if (item.IsDrinkable)
                    {
                        try
                        {
                            var itemEventArgs = new ContainerObjectEventArgs()
                                { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                            item.OnBeforeDrink(itemEventArgs);


                            if (this.universe.ActivePlayer.Items.Contains(item))
                            {
                                this.universe.ActivePlayer.RemoveItem((Item)item);
                            }
                            else
                            {
                                this.universe.ActiveLocation.RemoveItem((Item)item);
                            }

                            this.objectHandler.RemoveAsActiveObject(item);
                            item.OnDrink(itemEventArgs);

                            item.OnAfterDrink(itemEventArgs);

                            return printingSubsystem.FormattedResource(BaseDescriptions.ITEM_DRUNK, itemName, true);
                        }
                        catch (DrinkException ex)
                        {
                            return printingSubsystem.Resource(ex.Message);
                        }
                    }

                    this.objectHandler.StoreAsActiveObject(item);

                    return printingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_DRINK, itemName, true);
                }

                return printingSubsystem.ItemNotVisible();
            }
        }

        return false;
    }

    private bool HandleDescendEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != default)
        {
            var item = this.universe.ActivePlayer.ClimbedObject;
            try
            {
                var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                
                this.universe.ActivePlayer.OnBeforeDescend(eventArgs);
                item.OnBeforeDescend(eventArgs);
                    
                this.universe.ActivePlayer.DescendFromObject();
                this.universe.ActivePlayer.OnDescend(eventArgs);
                item.OnDescend(eventArgs);
                    
                var result = printingSubsystem.Resource(BaseDescriptions.DESCENDING);
                    
                item.OnAfterDescend(eventArgs);
                this.universe.ActivePlayer.OnAfterDescend(eventArgs);
                    
                return result;
            }
            catch (DescendException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }
        return printingSubsystem.Resource(BaseDescriptions.NOT_CLIMBED);
    }
    
    private bool HandleDescendEventOnSingleObject(AdventureEvent adventureEvent)
    {
        if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != default)
        {
            var compareItem = adventureEvent.ObjectOne;
            var item = this.universe.ActivePlayer.ClimbedObject;
            if (item.Key == compareItem.Key)
            {
                try
                {
                    var eventArgs = new ContainerObjectEventArgs() { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                    this.universe.ActivePlayer.OnBeforeDescend(eventArgs);
                    item.OnBeforeDescend(eventArgs);

                    this.universe.ActivePlayer.DescendFromObject();
                    this.universe.ActivePlayer.OnDescend(eventArgs);
                    item.OnDescend(eventArgs);

                    var result = printingSubsystem.Resource(BaseDescriptions.DESCENDING);

                    item.OnAfterDescend(eventArgs);
                    this.universe.ActivePlayer.OnAfterDescend(eventArgs);

                    return result;
                }
                catch (DescendException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            var itemName = ArticleHandler.GetNameWithArticleForObject(compareItem, GrammarCase.Dative, lowerFirstCharacter: true);
            return printingSubsystem.FormattedResource(BaseDescriptions.NOT_CLIMBED_ON_ITEM, itemName);
        }
            
        return printingSubsystem.Resource(BaseDescriptions.NOT_CLIMBED);
    }
    
    internal bool Directions(AdventureEvent adventureEvent)
    {
        var verbKey = adventureEvent.Predicate.Key;
        var optionalErrorMessage = adventureEvent.Predicate.ErrorMessage;

        if (VerbKeys.E == verbKey)
        {
            return this.ChangeLocation(Objects.Directions.E, optionalErrorMessage);
        }

        if (VerbKeys.W == verbKey)
        {
            return this.ChangeLocation(Objects.Directions.W, optionalErrorMessage);
        }
        if (VerbKeys.N == verbKey)
        {
            return this.ChangeLocation(Objects.Directions.N, optionalErrorMessage);
        }
        if (VerbKeys.S == verbKey)
        {
            return this.ChangeLocation(Objects.Directions.S, optionalErrorMessage);
        }
        if (VerbKeys.SE == verbKey)
        {
            return this.ChangeLocation(Objects.Directions.SE, optionalErrorMessage);
        }
        if (VerbKeys.SW == verbKey)
        {
            return this.ChangeLocation(Objects.Directions.SW, optionalErrorMessage);
        }
        if (VerbKeys.NE == verbKey)
        {
            return this.ChangeLocation(Objects.Directions.NE, optionalErrorMessage);
        }
        if (VerbKeys.NW == verbKey)
        {
            return this.ChangeLocation(Objects.Directions.NW, optionalErrorMessage);
        }
        if (VerbKeys.UP == verbKey)
        {
            return this.ChangeLocation(Objects.Directions.UP, optionalErrorMessage);
        }
        if (VerbKeys.DOWN == verbKey)
        {
            return this.ChangeLocation(Objects.Directions.DOWN, optionalErrorMessage);
        }

        return false;
    }
    
    internal bool Eat(AdventureEvent adventureEvent)
    {
        if (VerbKeys.EAT == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    var itemName =
                        ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                            lowerFirstCharacter: true);

                    if (item.IsEatable)
                    {
                        try
                        {
                            var itemEventArgs = new ContainerObjectEventArgs()
                                { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                            item.OnBeforeEat(itemEventArgs);


                            if (this.universe.ActivePlayer.Items.Contains(item))
                            {
                                this.universe.ActivePlayer.RemoveItem((Item)item);
                            }
                            else
                            {
                                this.universe.ActiveLocation.RemoveItem((Item)item);
                            }

                            this.objectHandler.RemoveAsActiveObject(item);
                            item.OnEat(itemEventArgs);

                            item.OnAfterEat(itemEventArgs);

                            return printingSubsystem.FormattedResource(BaseDescriptions.ITEM_EATEN, itemName, true);
                        }
                        catch (EatException ex)
                        {
                            return printingSubsystem.Resource(ex.Message);
                        }
                    }

                    this.objectHandler.StoreAsActiveObject(item);

                    return printingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_EAT, itemName, true);
                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Was genau möchtest Du essen?");
        }

        return false;
    }
    
    internal bool Go(AdventureEvent adventureEvent)
    {
        if (VerbKeys.GO == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne is Location location)
            {
                if (this.universe.LocationMap.ContainsKey(this.universe.ActiveLocation))
                {
                    var mappings = this.universe.LocationMap[this.universe.ActiveLocation];
                    var direction = mappings.Where(i => !i.IsHidden && i.Location.Key == location.Key).Select(x => x.Direction).Single();
                    return this.ChangeLocation(direction, adventureEvent.Predicate.ErrorMessage);
                }

                return printingSubsystem.Resource(BaseDescriptions.NO_WAY);
            }

            return printingSubsystem.Resource("Im Moment kannst Du nur einen angrenzenden Raum direkt betreten.");
        }

        return false;
    }

    internal bool Help(AdventureEvent adventureEvent)
    {
        if (VerbKeys.HELP == adventureEvent.Predicate.Key)
        {
            return printingSubsystem.Help(this.grammar.Verbs);
        }

        return false;
    }
    
    internal bool Hint(AdventureEvent adventureEvent)
    {
        if (VerbKeys.HINT == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.UnidentifiedSentenceParts.Count == 1)
            {
                if (string.Equals(adventureEvent.UnidentifiedSentenceParts.Single(), BaseDescriptions.ON, StringComparison.CurrentCultureIgnoreCase))
                {
                    isHintActive = true;
                    return printingSubsystem.Resource(BaseDescriptions.HINT_ON);
                }
            
                if (string.Equals(adventureEvent.UnidentifiedSentenceParts.Single(), BaseDescriptions.OFF, StringComparison.CurrentCultureIgnoreCase))
                {
                    isHintActive = false;
                    return printingSubsystem.Resource(BaseDescriptions.HINT_OFF);
                }
            }
        }

        return false;
    }
    
    internal bool History(AdventureEvent adventureEvent, ICollection<string> historyCollection)
    {
        if (VerbKeys.HISTORY == adventureEvent.Predicate.Key)
        {
            return printingSubsystem.History(historyCollection);
        }

        return false;
    }
    
    // internal bool ChangeLocationByName(AdventureEvent adventureEvent)
    // {
    //     var locationKey = this.objectHandler.GetObjectKeyByNameAndAdjectives<Location>(input);
    //     if (!string.IsNullOrEmpty(locationKey))
    //     {
    //         if (this.universe.LocationMap.ContainsKey(this.universe.ActiveLocation))
    //         {
    //             if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != null)
    //             {
    //                 return printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
    //             }
    //             
    //             if (this.universe.ActivePlayer.IsSitting && this.universe.ActivePlayer.Seat != null)    
    //             {
    //                 return printingSubsystem.Resource(BaseDescriptions.ALREADY_SITTING);
    //             }
    //             
    //             var mappings = this.universe.LocationMap[this.universe.ActiveLocation];
    //             var newLocation = mappings.Where(i => !i.IsHidden).SingleOrDefault(x => x.Location.Key == locationKey);
    //             if (newLocation != default)
    //             {
    //                 if (!newLocation.Location.IsLocked)
    //                 {
    //                     this.universe.ActiveLocation = newLocation.Location;
    //                     this.objectHandler.ClearActiveObjectIfNotInInventory();
    //                     return printingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
    //                 }
    //
    //                 return printingSubsystem.Resource(BaseDescriptions.ROOM_LOCKDESCRIPTION);
    //             }
    //         }
    //
    //         return printingSubsystem.Resource(BaseDescriptions.NO_WAY);
    //     }
    //
    //     return false;
    // }
    
    internal bool Open(AdventureEvent adventureEvent)
    {
        if (VerbKeys.OPEN == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    if (item.IsCloseable)
                    {
                        if (item.IsLocked)
                        {
                            return printingSubsystem.ItemStillLocked(item);
                        }

                        if (!item.IsClosed)
                        {
                            return printingSubsystem.ItemAlreadyOpen(item);
                        }

                        try
                        {
                            var containerObjectEventArgs = new ContainerObjectEventArgs {OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                        
                            item.OnBeforeOpen(containerObjectEventArgs);
                    
                            item.IsClosed = false;
                            this.universe.UnveilFirstLevelObjects(item);
                        
                            item.OnOpen(containerObjectEventArgs);
                        
                            var result = printingSubsystem.ItemOpen(item);

                            item.OnAfterOpen(containerObjectEventArgs);

                            return result;
                        }
                        catch (OpenException e)
                        {
                            item.IsClosed = true;
                            this.objectHandler.HideItemsOnClose(item);
                            return printingSubsystem.Resource(e.Message);
                        }
                    }

                    return printingSubsystem.FormattedResource(BaseDescriptions.IMPOSSIBLE_OPEN_ITEM, item.Name.LowerFirstChar());
                }

                return printingSubsystem.ItemNotVisible();
            }
            
            return printingSubsystem.Resource("Was genau möchtest Du öffnen?");
        }

        return false;
    }
    
    internal bool Pull(AdventureEvent adventureEvent)
    {
        if (VerbKeys.PULL == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);
                
                    try
                    {
                        var pullItemEventArgs = new PullItemEventArgs() {OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                        item.OnPull(pullItemEventArgs);
                    
                        return true;
                    }
                    catch (PullException ex)
                    {
                        return printingSubsystem.Resource(ex.Message);
                    }
                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Was genau möchtest Du ziehen?");
        }

        return false;
    }
    
    internal bool Push(AdventureEvent adventureEvent)
    {
        if (VerbKeys.PUSH == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    {
                        this.objectHandler.StoreAsActiveObject(item);

                        try
                        {
                            var pushItemEventArgs = new PushItemEventArgs()
                                { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                            item.OnPush(pushItemEventArgs);

                            return true;
                        }
                        catch (PushException ex)
                        {
                            return printingSubsystem.Resource(ex.Message);
                        }
                    }
                }

                return printingSubsystem.ItemNotVisible();
            }
            
            return printingSubsystem.Resource("Was genau möchtest Du drücken?");
        }

        return false;
    }
    
    internal bool Quit(AdventureEvent adventureEvent)
    {
        if (VerbKeys.QUIT == adventureEvent.Predicate.Key)
        {
            throw new QuitGameException(BaseDescriptions.QUIT_GAME);
        }

        return false;
    }
    
    internal bool Read(AdventureEvent adventureEvent)
    {
        if (VerbKeys.READ == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    
                    if (item.IsReadable)
                    {
                        try
                        {
                            var readItemEventArgs = new ReadItemEventArgs()
                                { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                            item.OnBeforeRead(readItemEventArgs);

                            var result = string.IsNullOrWhiteSpace(item.LetterContentDescription)
                                ? printingSubsystem.Resource(BaseDescriptions.NO_LETTER_CONTENT)
                                : printingSubsystem.FormattedResource(BaseDescriptions.LETTER_CONTENT,
                                    item.LetterContentDescription);

                            item.OnRead(readItemEventArgs);

                            item.OnAfterRead(readItemEventArgs);

                            return result;
                        }
                        catch (ReadException ex)
                        {
                            return printingSubsystem.Resource(ex.Message);
                        }
                    }

                    return printingSubsystem.Resource(BaseDescriptions.NOTHING_TO_READ);
                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Was genau möchtest Du lesen?");
        }

        return false;
    }
    
    internal bool Save(AdventureEvent adventureEvent, ICollection<string> historyCollection)
    {
        try
        {
            if (VerbKeys.SAVE == adventureEvent.Predicate.Key)
            {
                var history = new StringBuilder(historyCollection.Count);
                history.AppendJoin(Environment.NewLine, historyCollection);

                var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                var combine = Path.Combine(docPath, BaseDescriptions.SAVE_NAME);
                using (var outputFile = new StreamWriter(combine))
                {
                    outputFile.Write(history.ToString());
                }

                return printingSubsystem.FormattedResource(BaseDescriptions.GAME_SAVED, combine);
            }

            return false;
        }
        catch (Exception e)
        {
            printingSubsystem.Resource(e.Message);
            return false;
        }
    }
    
    internal bool Say(AdventureEvent adventureEvent)
    {
        if (VerbKeys.SAY == adventureEvent.Predicate.Key)
        {
            //I can only speak to visible people
            if (adventureEvent.ObjectOne is Character character && this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(character))
            {
                var phrase = string.Join(" ", adventureEvent.UnidentifiedSentenceParts);
                var key = this.objectHandler.GetConversationAnswerKeyByName(phrase);
                if (string.IsNullOrEmpty(key))
                {
                    return printingSubsystem.NoAnswer(phrase);
                }

                try
                {
                    var conversationEventArgs = new ConversationEventArgs { Phrase = key, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                
                    character.OnSay(conversationEventArgs);
                }
                catch (SayException ex)
                {
                    printingSubsystem.Resource(ex.Message);
                }

                return true;
            }

            return printingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
        }

        return false;
    }
    
    internal bool Turn(AdventureEvent adventureEvent)
    {
        if (VerbKeys.TURN == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    try
                    {
                        var turnItemEventArgs = new TurnItemEventArgs()
                            { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                        item.OnTurn(turnItemEventArgs);

                        return true;
                    }
                    catch (TurnException ex)
                    {
                        return printingSubsystem.Resource(ex.Message);
                    }
                }

                return printingSubsystem.ItemNotVisible();
            }
            
            return printingSubsystem.Resource("Was genau möchtest Du drehen?");
        }

        return false;
    }
    
    internal bool Jump(AdventureEvent adventureEvent)
    {
        if (VerbKeys.JUMP == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);
                
                    try
                    {
                        var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                    
                        item.OnJump(containerObjectEventArgs);
                    
                        return true;
                    }
                    catch (JumpException ex)
                    {
                        return printingSubsystem.Resource(ex.Message);
                    }    
                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Worauf möchtest Du denn springen?");
        }

        return false;
    }
    
    internal bool Kindle(AdventureEvent adventureEvent)
    {
        if (VerbKeys.KINDLE == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);

                    try
                    {
                        var containerObjectEventArgs = new KindleItemEventArgs()
                            { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                        item.OnKindle(containerObjectEventArgs);

                        return true;
                    }
                    catch (KindleException ex)
                    {
                        return printingSubsystem.Resource(ex.Message);
                    }
                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Was genau möchtest Du anzünden?");
        }

        return false;
    }

    internal bool Lock(AdventureEvent adventureEvent)
    {
        if (VerbKeys.LOCK == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    if (!string.IsNullOrEmpty(item.UnlockWithKey) &&
                        this.universe.ActivePlayer.OwnsItem(item.UnlockWithKey))
                    {
                        var key = this.universe.ActivePlayer.GetItem(item.UnlockWithKey);
                        if (item.IsLockable)
                        {
                            if (!item.IsLocked)
                            {
                                if (!item.IsCloseable || item.IsCloseable && item.IsClosed)
                                {
                                    try
                                    {
                                        var lockContainerEventArgs = new LockContainerEventArgs
                                            { Key = key, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                                        item.OnBeforeLock(lockContainerEventArgs);

                                        item.IsLocked = true;
                                        item.OnLock(lockContainerEventArgs);
                                        var keyName = ArticleHandler.GetNameWithArticleForObject(key,
                                            GrammarCase.Accusative, lowerFirstCharacter: true);
                                        printingSubsystem.Resource(string.Format(
                                            BaseDescriptions.ITEM_LOCKED_WITH_KEY_FROM_INVENTORY, keyName, item.Name));

                                        item.OnAfterLock(lockContainerEventArgs);

                                        return true;
                                    }
                                    catch (LockException e)
                                    {
                                        return printingSubsystem.Resource(e.Message);
                                    }
                                }
                            }

                            return printingSubsystem.ItemAlreadyLocked(item);
                        }

                        return printingSubsystem.ItemNotLockAble(item);
                    }

                    return printingSubsystem.ImpossibleLock(item);
                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Was genau möchtest Du verschliessen?");
        }

        return false;
    }
    
    internal bool Look(AdventureEvent adventureEvent)
    {
        if (VerbKeys.LOOK == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne == default && adventureEvent.ObjectTwo == default)
            {
                return this.HandleLookEventOnActiveLocation(adventureEvent);    
            }
            
            if (adventureEvent.ObjectOne != default && adventureEvent.ObjectTwo == default)
            {
                return this.HandleLookEventOnSingleObject(adventureEvent);
            }

            if (adventureEvent.ObjectOne != default && adventureEvent.ObjectTwo != default)
            {
                if (adventureEvent.ObjectOne is { } player && player.Key == this.universe.ActivePlayer.Key)
                {
                    return this.HandleLookEventOnSingleObject(adventureEvent);
                }   
            }

            return true;
        }

        return false;
    }

    private bool HandleLookEventOnSingleObject(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
        {
            try
            {
                this.objectHandler.StoreAsActiveObject(item);

                var eventArgs = new ContainerObjectEventArgs()
                    { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                var eventArgsForActiveLocation = new ContainerObjectEventArgs()
                    { ExternalItemKey = item.Key, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                item.OnBeforeLook(eventArgs);
                this.universe.ActiveLocation.OnBeforeLook(eventArgsForActiveLocation);

                this.universe.UnveilFirstLevelObjects(item);
                item.OnLook(eventArgs);
                this.universe.ActiveLocation.OnLook(eventArgsForActiveLocation);
                var result = printingSubsystem.PrintObject(item);

                item.OnAfterLook(new ContainerObjectEventArgs());
                this.universe.ActiveLocation.OnAfterLook(eventArgsForActiveLocation);

                return result;
            }
            catch (LookException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return printingSubsystem.ItemNotVisible();
    }

    internal bool Take(AdventureEvent adventureEvent)
    {
        if (VerbKeys.TAKE == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne == default && adventureEvent.ObjectTwo == default)
            {
                return HandleTakeEventOnAllPickableAndUnhiddenItems(adventureEvent);    
            }

            if (adventureEvent.ObjectOne != default && adventureEvent.ObjectTwo == default)
            {
                return HandleTakeEventOnSinglePickableAndUnhiddenItems(adventureEvent);
            }
            
            if (adventureEvent.ObjectOne != default && adventureEvent.ObjectTwo != default)
            {
                if (adventureEvent.ObjectOne is { } player && player.Key == this.universe.ActivePlayer.Key)
                {
                    return this.HandleTakeEventOnSinglePickableAndUnhiddenItems(adventureEvent);
                }   
            }

            return true;
        }

        return false;
    }
    
    internal bool Inventory(AdventureEvent adventureEvent)
    {
        if (VerbKeys.INV == adventureEvent.Predicate.Key)
        {
            printingSubsystem.ActivePlayer(this.universe.ActivePlayer);
            return true;
        }

        return false;
    }
    
    internal bool Score(AdventureEvent adventureEvent)
    {
        if (VerbKeys.SCORE == adventureEvent.Predicate.Key)
        {
            return printingSubsystem.Score(universe.Score, universe.MaxScore);
        }

        return false;
    }
    
    internal bool SitDown(AdventureEvent adventureEvent)
    {
        if (VerbKeys.SIT == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne == default && adventureEvent.ObjectTwo == default)
            {
                return this.HandleSitDownEventOnActiveLocation(adventureEvent);    
            }
            else if (adventureEvent.ObjectOne != default)
            {
                return this.HandleSitDownEventOnSingleObject(adventureEvent);
            }

            return true;
        }

        return false;
    }

    private bool HandleSitDownEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        var seatCount = this.universe.ActiveLocation.Items.Count(x => x.IsSeatable);

        if (seatCount == 0)
        {
            return printingSubsystem.Resource(BaseDescriptions.NO_SEAT);
        }

        if (seatCount == 1)
        {
            var onlySeat = this.universe.ActiveLocation.Items.Single(x => x.IsSeatable);
            this.objectHandler.StoreAsActiveObject(onlySeat);

            try
            {
                var sitDownEventArgs = new SitDownEventArgs
                    { ItemToSitOn = onlySeat, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                var downEventArgs = new SitDownEventArgs
                {
                    ItemToSitOn = this.universe.ActivePlayer,
                    OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage
                };

                this.universe.ActivePlayer.OnBeforeSitDown(sitDownEventArgs);
                onlySeat.OnBeforeSitDown(downEventArgs);

                this.universe.ActivePlayer.SitDownOnSeat(onlySeat);
                this.universe.ActivePlayer.OnSitDown(sitDownEventArgs);
                onlySeat.OnSitDown(downEventArgs);

                var result = printingSubsystem.FormattedResource(BaseDescriptions.ITEM_ONLY_SEAT,
                    ArticleHandler.GetNameWithArticleForObject(onlySeat, GrammarCase.Dative), true);

                onlySeat.OnAfterSitDown(downEventArgs);
                this.universe.ActivePlayer.OnAfterSitDown(sitDownEventArgs);

                return result;
            }
            catch (SitDownException ex)
            {
                this.universe.ActivePlayer.StandUpFromSeat();
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return printingSubsystem.Resource(BaseDescriptions.MORE_SEATS);
    }

    private bool HandleSitDownEventOnSingleObject(AdventureEvent adventureEvent)
    {
        if (adventureEvent.ObjectOne.Key == this.universe.ActivePlayer.Key)
        {
            return this.HandleSitDownEventOnActiveLocation(adventureEvent);
        }

        var item = adventureEvent.ObjectOne;
        if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
        {
            this.objectHandler.StoreAsActiveObject(item);
            if (item.IsSeatable)
            {
                try
                {
                    var sitDownEventArgs = new SitDownEventArgs
                        { ItemToSitOn = item, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                    var downEventArgs = new SitDownEventArgs
                    {
                        ItemToSitOn = this.universe.ActivePlayer,
                        OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage
                    };

                    this.universe.ActivePlayer.OnBeforeSitDown(sitDownEventArgs);
                    item.OnBeforeSitDown(downEventArgs);

                    this.universe.ActivePlayer.SitDownOnSeat(item);
                    this.universe.ActivePlayer.OnSitDown(sitDownEventArgs);
                    item.OnSitDown(downEventArgs);

                    var result = printingSubsystem.ItemSeated(item);

                    item.OnAfterSitDown(downEventArgs);
                    this.universe.ActivePlayer.OnAfterSitDown(sitDownEventArgs);

                    return result;
                }
                catch (SitDownException ex)
                {
                    this.universe.ActivePlayer.StandUpFromSeat();
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.ItemNotSeatable(item);
        }

        return printingSubsystem.ItemNotVisible();
    }

    internal bool StandUp(AdventureEvent adventureEvent)
    {
        if (VerbKeys.STANDUP == adventureEvent.Predicate.Key)
        {
            if (this.universe.ActivePlayer.IsSitting && this.universe.ActivePlayer.Seat != default)
            {
                var item = this.universe.ActivePlayer.Seat;
                try
                {
                    this.objectHandler.StoreAsActiveObject(item);
                
                    var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                
                    this.universe.ActivePlayer.OnBeforeStandUp(eventArgs);
                    item.OnBeforeStandUp(eventArgs);
                
                    this.universe.ActivePlayer.StandUpFromSeat();
                    item.OnStandUp(eventArgs);
                    var result = printingSubsystem.Resource(BaseDescriptions.STANDING_UP);
                
                    item.OnAfterStandUp(eventArgs);
                    this.universe.ActivePlayer.OnAfterStandUp(eventArgs);
                    return result;
                }
                catch (StandUpException ex)
                {
                    this.universe.ActivePlayer.SitDownOnSeat(item);
                    return printingSubsystem.Resource(ex.Message);
                }
            }
            return printingSubsystem.Resource(BaseDescriptions.NOT_SITTING);
        }

        return false;
    }

    internal bool SwitchOff(AdventureEvent adventureEvent)
    {
        if (VerbKeys.SWITCHOFF == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne is Item item)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    var itemName =
                        ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                            lowerFirstCharacter: true);

                    this.objectHandler.StoreAsActiveObject(item);

                    if (item.IsSwitchable)
                    {
                        if (item.IsSwitchedOn)
                        {
                            try
                            {
                                var itemEventArgs = new ContainerObjectEventArgs()
                                    { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                                item.OnBeforeSwitchOff(itemEventArgs);

                                item.IsSwitchedOn = false;
                                item.OnSwitchOff(itemEventArgs);

                                item.OnAfterSwitchOff(itemEventArgs);

                                return printingSubsystem.FormattedResource(BaseDescriptions.ITEM_SWITCHEDOFF, itemName,
                                    true);
                            }
                            catch (SwitchOffException ex)
                            {
                                return printingSubsystem.Resource(ex.Message);
                            }
                        }

                        return printingSubsystem.FormattedResource(BaseDescriptions.ALREADY_SWITCHEDOFF, itemName,
                            true);
                    }

                    return printingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_SWITCHOFF, itemName, true);
                }

                return printingSubsystem.ItemNotVisible();
            }
            
            return printingSubsystem.Resource("Was genau möchtest Du ausschalten?");
        }

        return false;
    }
    
    internal bool SwitchOn(AdventureEvent adventureEvent)
    {
        if (VerbKeys.SWITCHON == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne is Item item)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    var itemName =
                        ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                            lowerFirstCharacter: true);

                    this.objectHandler.StoreAsActiveObject(item);

                    if (item.IsSwitchable)
                    {
                        if (!item.IsSwitchedOn)
                        {
                            try
                            {
                                var itemEventArgs = new ContainerObjectEventArgs()
                                    { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                                item.OnBeforeSwitchOn(itemEventArgs);

                                item.IsSwitchedOn = true;
                                item.OnSwitchOn(itemEventArgs);

                                item.OnAfterSwitchOn(itemEventArgs);

                                return printingSubsystem.FormattedResource(BaseDescriptions.ITEM_SWITCHEDON,
                                    itemName,
                                    true);
                            }
                            catch (SwitchOnException ex)
                            {
                                return printingSubsystem.Resource(ex.Message);
                            }
                        }

                        return printingSubsystem.FormattedResource(BaseDescriptions.ALREADY_SWITCHEDON,
                            itemName, true);
                    }

                    return printingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_SWITCHON, itemName,
                        true);
                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Was genau möchtest Du anschalten?");
        }

        return false;
    }

    internal bool Talk(AdventureEvent adventureEvent)
    {
        if (VerbKeys.TALK == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne != default)
            {
                if (adventureEvent.ObjectOne is Character character)
                {
                    if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(character))
                    {
                        try
                        {
                            var containerObjectEventArgs = new ContainerObjectEventArgs()
                                { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                            character.OnBeforeTalk(containerObjectEventArgs);

                            var result = printingSubsystem.Talk(character);
                            character.OnTalk(containerObjectEventArgs);

                            character.OnAfterTalk(containerObjectEventArgs);

                            return result;
                        }
                        catch (Exception ex)
                        {
                            return printingSubsystem.Resource(ex.Message);
                        }
                    }
                    
                    return printingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
                }

                return printingSubsystem.Resource("Im Moment macht es keinen Sinn mit einem Gegenstand zu reden.");
            }

            return printingSubsystem.Resource("Sag mir doch bitte, mit wem Du reden möchtest?");
        }

        return false;
    }
    
    internal bool Unlock(AdventureEvent adventureEvent)
    {
        if (VerbKeys.UNLOCK == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    if (!string.IsNullOrEmpty(item.UnlockWithKey) &&
                        this.universe.ActivePlayer.OwnsItem(item.UnlockWithKey))
                    {
                        var key = this.universe.ActivePlayer.GetItem(item.UnlockWithKey);
                        if (item.IsLockable)
                        {
                            if (item.IsLocked)
                            {
                                if (!item.IsCloseable || item.IsCloseable && item.IsClosed)
                                {
                                    try
                                    {
                                        var unlockContainerEventArgs = new LockContainerEventArgs
                                            { Key = key, OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                                        item.OnBeforeUnlock(unlockContainerEventArgs);

                                        item.IsLocked = false;
                                        item.OnUnlock(unlockContainerEventArgs);
                                        var keyName = ArticleHandler.GetNameWithArticleForObject(key,
                                            GrammarCase.Accusative, lowerFirstCharacter: true);
                                        printingSubsystem.Resource(string.Format(
                                            BaseDescriptions.ITEM_UNLOCKED_WITH_KEY_FROM_INVENTORY, keyName,
                                            item.Name));

                                        item.OnAfterUnlock(unlockContainerEventArgs);

                                        return true;
                                    }
                                    catch (UnlockException e)
                                    {
                                        return printingSubsystem.Resource(e.Message);
                                    }
                                }
                            }

                            return printingSubsystem.ItemAlreadyUnlocked(item);
                        }

                        return printingSubsystem.ItemNotLockAble(item);
                    }

                    return printingSubsystem.ImpossibleUnlock(item);
                }

                return printingSubsystem.ItemNotVisible();
            }

            return printingSubsystem.Resource("Was genau möchtest Du aufschliessen?");
        }

        return false;
    }

    internal bool Use(AdventureEvent adventureEvent)
    {
        if (VerbKeys.USE == adventureEvent.Predicate.Key)
        {
            var item = adventureEvent.ObjectOne;
            if (item != default)
            {
                if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    try
                    {
                        string optionalErrorMessage = string.Empty;
                        var errorMessage = adventureEvent.Predicate.ErrorMessage;
                        if (!string.IsNullOrEmpty(errorMessage))
                        {
                            var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Dative, lowerFirstCharacter: true);
                            optionalErrorMessage = string.Format(errorMessage, itemName);
                        }

                        var useItemEventArgs = new UseItemEventArgs() {OptionalErrorMessage = optionalErrorMessage};
                    
                        item.OnUse(useItemEventArgs);
                    
                        return true;
                    }
                    catch (UseException ex)
                    {
                        return printingSubsystem.Resource(ex.Message);
                    }
                }

                return printingSubsystem.ItemNotOwned();
            }

            return printingSubsystem.Resource("Was genau möchtest Du benutzen?");
        }

        return false;
    }
    
    internal bool Wait(AdventureEvent adventureEvent)
    {
        if (VerbKeys.WAIT == adventureEvent.Predicate.Key)
        {
            try
            {
                var containerObjectEventArgs = new ContainerObjectEventArgs()
                    { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                this.universe.ActiveLocation.OnWait(containerObjectEventArgs);

                return true;
            }
            catch (WaitException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }

    internal bool Wear(AdventureEvent adventureEvent)
    {
        void SwapItem(Item item)
        {
            this.universe.ActivePlayer.Items.Remove(item);
            this.universe.ActivePlayer.Clothes.Add(item);
        }

        if (VerbKeys.WEAR == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne != default)
            {
                if (adventureEvent.ObjectOne is Item item)
                {
                    if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
                    {
                        var itemName =
                            ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative,
                                lowerFirstCharacter: true);

                        this.objectHandler.StoreAsActiveObject(item);

                        if (item.IsWearable)
                        {
                            if (!this.universe.ActivePlayer.Clothes.Contains(item))
                            {
                                try
                                {
                                    var itemEventArgs = new ContainerObjectEventArgs()
                                        { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

                                    item.OnBeforeWear(itemEventArgs);

                                    if (this.universe.ActivePlayer.Items.Contains(item))
                                    {
                                        SwapItem(item);
                                    }
                                    else
                                    {
                                        this.universe.PickObject(item, true);
                                        if (this.universe.ActivePlayer.Items.Contains(item))
                                        {
                                            SwapItem(item);
                                        }
                                        else
                                        {
                                            //TODO refactor PickUp!
                                            return true;
                                        }
                                    }

                                    item.OnWear(itemEventArgs);

                                    item.OnAfterWear(itemEventArgs);

                                    return printingSubsystem.FormattedResource(BaseDescriptions.PULLON_WEARABLE,
                                        itemName,
                                        true);
                                }
                                catch (WearException ex)
                                {
                                    return printingSubsystem.Resource(ex.Message);
                                }
                            }

                            printingSubsystem.Resource(BaseDescriptions.ALREADY_WEARING);
                        }

                        return printingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_WEAR, itemName, true);
                    }
                    return printingSubsystem.ItemNotVisible();
                }
                
                return printingSubsystem.Resource("Das kannst Du nicht anziehen!");
            }
        }

        return false;
    }
    
    internal bool Write(AdventureEvent adventureEvent)
    {
        if (VerbKeys.WRITE == adventureEvent.Predicate.Key)
        {
            try
            {
                var writeEventArgs = new WriteEventArgs()
                {
                    Text = string.Join(" ", adventureEvent.UnidentifiedSentenceParts),
                    OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage
                };
                
                this.universe.ActiveLocation.OnWrite(writeEventArgs);
                
                return true;
            }
            catch (WriteException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }

    internal bool Sleep(AdventureEvent adventureEvent)
    {
        if (VerbKeys.SLEEP == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne == default && adventureEvent.ObjectTwo == default)
            {
                return this.HandleSleepEventOnActiveLocation(adventureEvent);    
            }
            else if (adventureEvent.ObjectOne != default)
            {
                return this.HandleSleepEventOnSingleObject(adventureEvent);
            }

            return true;
        }

        return false;
    }
    
    private bool HandleSleepEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        try
        {
            var containerObjectEventArgs = new ContainerObjectEventArgs()
                { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

            this.universe.ActiveLocation.OnSleep(containerObjectEventArgs);

            return true;
        }
        catch (SleepException ex)
        {
            return printingSubsystem.Resource(ex.Message);
        }
    }
    
    private bool HandleSleepEventOnSingleObject(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
        {
            this.objectHandler.StoreAsActiveObject(item);
                
            try
            {
                var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                    
                item.OnSleep(containerObjectEventArgs);
                    
                return true;
            }
            catch (SleepException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return printingSubsystem.ItemNotVisible();
    }
    
    internal bool Smell(AdventureEvent adventureEvent)
    {
        if (VerbKeys.SMELL == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne == default && adventureEvent.ObjectTwo == default)
            {
                return this.HandleSmellEventOnActiveLocation(adventureEvent);    
            }
            else if (adventureEvent.ObjectOne != default)
            {
                return this.HandleSmellEventOnSingleObject(adventureEvent);
            }

            return true;
        }

        return false;
    }
    
    private bool HandleSmellEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        try
        {
            var containerObjectEventArgs = new ContainerObjectEventArgs()
                { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

            this.universe.ActiveLocation.OnSmell(containerObjectEventArgs);

            return true;
        }
        catch (SmellException ex)
        {
            return printingSubsystem.Resource(ex.Message);
        }
    }
    
    private bool HandleSmellEventOnSingleObject(AdventureEvent adventureEvent)
    {
        var activeObject = adventureEvent.ObjectOne;
        if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(activeObject))
        {
            this.objectHandler.StoreAsActiveObject(activeObject);
                
            try
            {
                var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                if (activeObject is Character && string.IsNullOrEmpty(containerObjectEventArgs.OptionalErrorMessage))
                {
                    containerObjectEventArgs.OptionalErrorMessage = BaseDescriptions.DONT_SMELL_ON_PERSON;
                }
                    
                activeObject.OnSmell(containerObjectEventArgs);
                    
                return true;
            }
            catch (SmellException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return printingSubsystem.ItemNotVisible();
    }
    
    internal bool Taste(AdventureEvent adventureEvent)
    {
        if (VerbKeys.TASTE == adventureEvent.Predicate.Key)
        {
            if (adventureEvent.ObjectOne == default && adventureEvent.ObjectTwo == default)
            {
                return this.HandleTasteEventOnActiveLocation(adventureEvent);    
            }
            else if (adventureEvent.ObjectOne != default)
            {
                return this.HandleTasteEventOnSingleObject(adventureEvent);
            }

            return true;
        }

        return false;
    }
    
    private bool HandleTasteEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        try
        {
            var containerObjectEventArgs = new ContainerObjectEventArgs
                { OptionalErrorMessage = BaseDescriptions.WHAT_TO_TASTE };

            if (!string.IsNullOrEmpty(adventureEvent.Predicate.ErrorMessage))
            {
                containerObjectEventArgs.OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage;
            }

            this.universe.ActiveLocation.OnTaste(containerObjectEventArgs);

            return true;
        }
        catch (TasteException ex)
        {
            return printingSubsystem.Resource(ex.Message);
        }
    }
    
    private bool HandleTasteEventOnSingleObject(AdventureEvent adventureEvent)
    {
        var item = adventureEvent.ObjectOne;
        if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(item))
        {
            this.objectHandler.StoreAsActiveObject(item);
                
            try
            {
                var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage};
                    
                item.OnTaste(containerObjectEventArgs);
                    
                return true;
            }
            catch (TasteException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return printingSubsystem.ItemNotVisible();
    }
    
    internal bool Ways(AdventureEvent adventureEvent)
    {
        if (VerbKeys.WAYS == adventureEvent.Predicate.Key)
        {
            if (this.universe.LocationMap.ContainsKey(this.universe.ActiveLocation) 
                && this.universe.LocationMap[this.universe.ActiveLocation].Any(l => !l.IsHidden))
            {
                return printingSubsystem.DestinationNode(this.universe.ActiveLocation, this.universe.LocationMap);
            }

            return printingSubsystem.Resource(BaseDescriptions.NO_WAYS);
        }

        return false;
    }

    private bool HandleTakeEventOnSinglePickableAndUnhiddenItems(AdventureEvent adventureEvent)
    {
        var objectOne = adventureEvent.ObjectOne;
        if (this.objectHandler.IsObjectUnhiddenAndInInventoryOrActiveLocation(objectOne))
        {
            if (objectOne is Character character)
            {
                return printingSubsystem.ImpossiblePickup(character);
            }

            if (objectOne is Item item)
            {
                try
                {
                    this.objectHandler.StoreAsActiveObject(objectOne);

                    var eventArgs = new ContainerObjectEventArgs()
                        { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
                    objectOne.OnBeforeTake(eventArgs);
                    universe.PickObject(item);
                    objectOne.OnTake(eventArgs);
                    objectOne.OnAfterTake(eventArgs);
                }
                catch (TakeException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }   
            }
            
            return printingSubsystem.ImpossiblePickup(objectOne);
        }

        return printingSubsystem.ItemNotVisible();;
    }

    private bool HandleTakeEventOnAllPickableAndUnhiddenItems(AdventureEvent adventureEvent)
    {
        var subjects = this.universe.ActiveLocation.GetAllPickableAndUnHiddenItems();
        if (subjects.Any())
        {
            var eventArgs = new ContainerObjectEventArgs() { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };
            var result = true;
            foreach (var item in subjects)
            {
                try
                {
                    item.OnBeforeTake(eventArgs);
                    result = result && universe.PickObject(item);
                    item.OnTake(eventArgs);
                    item.OnAfterTake(eventArgs);
                }
                catch (TakeException ex)
                {
                    result = result && printingSubsystem.Resource(ex.Message);
                }
            }

            return result;
        }

        return printingSubsystem.NothingToTake();
    }

    private bool HandleLookEventOnActiveLocation(AdventureEvent adventureEvent)
    {
        try
        {
            var eventArgs = new ContainerObjectEventArgs() { OptionalErrorMessage = adventureEvent.Predicate.ErrorMessage };

            this.universe.ActiveLocation.OnBeforeLook(eventArgs);

            this.universe.UnveilFirstLevelObjects(this.universe.ActiveLocation);
            this.universe.ActiveLocation.OnLook(eventArgs);

            this.universe.ActiveLocation.OnAfterLook(eventArgs);
            return printingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
        }
        catch (LookException ex)
        {
            return printingSubsystem.Resource(ex.Message);
        }
    }
    
    private bool ChangeLocation(Directions direction, string optionalErrorMessage)
    {
        if (this.universe.LocationMap.ContainsKey(this.universe.ActiveLocation))
        {
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != null)
            {
                return printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
            }

            if (this.universe.ActivePlayer.IsSitting && this.universe.ActivePlayer.Seat != null)    
            {
                return printingSubsystem.Resource(BaseDescriptions.ALREADY_SITTING);
            }
            
            var mappings = this.universe.LocationMap[this.universe.ActiveLocation];
            var newLocationMap = mappings.Where(i => !i.IsHidden).SingleOrDefault(x => x.Direction == direction);
            if (newLocationMap != default)
            {
                try
                {
                    var changeLocationEventArgs = new ChangeLocationEventArgs(newLocationMap) {OptionalErrorMessage = optionalErrorMessage};
                    
                    this.universe.ActiveLocation.OnBeforeChangeLocation(changeLocationEventArgs);
                    if (!newLocationMap.Location.IsLocked)
                    {
                        if (!newLocationMap.Location.IsClosed)
                        {
                            this.universe.ActiveLocation = newLocationMap.Location;
                            this.objectHandler.ClearActiveObjectIfNotInInventory();
                            this.universe.ActiveLocation.OnChangeLocation(changeLocationEventArgs);

                            return printingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
                        }
                    }

                    //TODO - maybe OnUnsuccessfulLocationChange... 
                    var status = this.universe.ActiveLocation.OnAfterChangeLocation(changeLocationEventArgs);

                    if (status == ChangeLocationStatus.IsLocked)
                    {
                        return printingSubsystem.WayIsLocked(newLocationMap.Location);
                    }
                    else if (status == ChangeLocationStatus.IsClosed)
                    {
                        return printingSubsystem.WayIsClosed(newLocationMap.Location);
                    }
                }
                catch (BeforeChangeLocationException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }
            else
            {
                return printingSubsystem.Resource(BaseDescriptions.NO_WAY);
            }
        }
        else
        {
            return printingSubsystem.Resource(BaseDescriptions.NO_WAY);
        }

        return false;
    }
}