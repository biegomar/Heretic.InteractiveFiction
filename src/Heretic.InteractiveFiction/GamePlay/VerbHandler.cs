using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class VerbHandler
{
    private readonly Universe universe;
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly ObjectHandler objectHandler;
    private bool isHintActive;

    internal VerbHandler(Universe universe, IPrintingSubsystem printingSubsystem)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
        this.objectHandler = new ObjectHandler(universe);
        this.isHintActive = false;
    }

    internal bool Quit(string verb)
    {
        return this.IsVerb(VerbKeys.QUIT, verb);
    }

    internal bool Credits(string verb)
    {
        if (this.universe.VerbResources[VerbKeys.CREDITS].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            return printingSubsystem.Credits();
        }

        return false;
    }
    
    internal bool Look(string verb)
    {
        if (this.universe.VerbResources[VerbKeys.LOOK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            this.universe.UnveilFirstLevelObjects(this.universe.ActiveLocation);
            return printingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
        }

        return false;
    }

    internal bool Look(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.LOOK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenObjectByName(subject);
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                this.universe.UnveilFirstLevelObjects(item);
                var result = printingSubsystem.PrintObject(item);

                item.OnAfterLook(new ContainerObjectEventArgs());
                this.universe.ActiveLocation.OnAfterLook(new ContainerObjectEventArgs() {ExternalItemKey = item.Key});

                return result;
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Look(string verb, string processingSubject, string processingObject)
    {
        if (this.universe.VerbResources[VerbKeys.LOOK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            if (this.objectHandler.GetUnhiddenObjectByName(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Look(verb, processingObject);
            }
        }

        return false;
    }
    
    internal bool Read(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.READ].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenObjectByName(subject);
            if (item != default)
            {
                if (item.IsReadable)
                {
                    try
                    {
                        this.objectHandler.StoreAsActiveObject(item);
                        item.OnBeforeRead(new ReadItemEventArgs());

                        var result = string.IsNullOrWhiteSpace(item.LetterContentDescription) ? 
                            printingSubsystem.Resource(BaseDescriptions.NO_LETTER_CONTENT) : 
                            printingSubsystem.FormattedResource(BaseDescriptions.LETTER_CONTENT, item.LetterContentDescription);
                    
                        item.OnAfterRead(new ReadItemEventArgs());

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

        return false;
    }
    
    internal bool Hint(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.HINT].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            if (subject == BaseDescriptions.ON.ToLower())
            {
                isHintActive = true;
                return printingSubsystem.Resource(BaseDescriptions.HINT_ON);
            }
            
            if (subject == BaseDescriptions.OFF.ToLower())
            {
                isHintActive = false;
                return printingSubsystem.Resource(BaseDescriptions.HINT_OFF);
            }
        }

        return false;
    }

    internal bool Remark(string verb)
    {
        if (this.universe.VerbResources[VerbKeys.REM].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }
    
    internal bool Pull(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.PULL].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenObjectByName(subject);
            if (item != default)
            {
                try
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    item.OnPull(new PullItemEventArgs());

                    return true;
                }
                catch (PullException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Pull(string verb, string subjectName, string objectName)
    {
        if (this.universe.VerbResources[VerbKeys.PULL].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var subject = this.objectHandler.GetUnhiddenObjectByName(subjectName);

            if (subject == default)
            {
                return printingSubsystem.CanNotUseObject(subjectName);
            }
            
            this.objectHandler.StoreAsActiveObject(subject);

            var item = this.objectHandler.GetUnhiddenObjectByName(objectName);

            if (item == default)
            {
                return printingSubsystem.CanNotUseObject(objectName);
            }

            try
            {
                subject.OnPull(new PullItemEventArgs() {ItemToUse = item});

                return true;
            }
            catch (PullException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }
    
    internal bool Push(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.PUSH].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenObjectByName(subject);
            if (item != default)
            {
                try
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    item.OnPush(new PushItemEventArgs());

                    return true;
                }
                catch (PushException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }
            
            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Push(string verb, string subjectName, string objectName)
    {
        if (this.universe.VerbResources[VerbKeys.PUSH].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var subject = this.objectHandler.GetUnhiddenObjectByName(subjectName);

            if (subject == default)
            {
                return printingSubsystem.CanNotUseObject(subjectName);
            }
            
            this.objectHandler.StoreAsActiveObject(subject);

            var item = this.objectHandler.GetUnhiddenObjectByName(objectName);

            if (item == default)
            {
                return printingSubsystem.CanNotUseObject(objectName);
            }

            try
            {
                subject.OnPush(new PushItemEventArgs() {ItemToUse = item});

                return true;
            }
            catch (PushException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }


    internal bool AlterEgo(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.ALTER_EGO].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenObjectByName(subject);
            if (item != default)
            {
                var result = printingSubsystem.AlterEgo(item);
                return result;
            }

            return printingSubsystem.ItemNotVisible();

        }

        return false;
    }
    
    internal bool Write(string verb, string text)
    {
        if (this.universe.VerbResources[VerbKeys.WRITE].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            try
            {
                this.universe.ActiveLocation.OnWrite(new WriteEventArgs() {Text = text});
                return true;
            }
            catch (WriteException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }

    internal bool Help(string input)
    {
        if (this.universe.VerbResources[VerbKeys.HELP].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            return printingSubsystem.Help(this.universe.VerbResources);
        }

        return false;
    }

    internal bool History(string input, ICollection<string> historyCollection)
    {
        if (this.universe.VerbResources[VerbKeys.HISTORY].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            return printingSubsystem.History(historyCollection);
        }

        return false;
    }
    
    internal bool Save(string input, ICollection<string> historyCollection)
    {
        try
        {
            if (this.universe.VerbResources[VerbKeys.SAVE].Contains(input, StringComparer.InvariantCultureIgnoreCase))
            {
                var history = new StringBuilder(historyCollection.Count);
                history.AppendJoin(Environment.NewLine, historyCollection);

                var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                var combine = Path.Combine(docPath, BaseDescriptions.SAVE_NAME);
                using (StreamWriter outputFile = new StreamWriter(combine))
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

    internal bool Score(string input)
    {
        if (this.universe.VerbResources[VerbKeys.SCORE].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            return printingSubsystem.Score(universe.Score, universe.MaxScore);
        }

        return false;
    }

    internal bool Ways(string input)
    {
        if (this.universe.VerbResources[VerbKeys.WAYS].Contains(input, StringComparer.InvariantCultureIgnoreCase))
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

    internal bool Use(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.USE].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameFromActivePlayer(subject);
            
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                try
                {
                    item.OnUse(new UseItemEventArgs());

                    return true;
                }
                catch (UseException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.ItemNotOwned();
        }

        return false;
    }

    internal bool Use(string verb, string subjectName, string objectName)
    {
        if (this.universe.VerbResources[VerbKeys.USE].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var subject = this.objectHandler.GetUnhiddenObjectByName(subjectName);

            if (subject == default)
            {
                return printingSubsystem.CanNotUseObject(subjectName);
            }

            this.objectHandler.StoreAsActiveObject(subject);
            
            var item = this.objectHandler.GetUnhiddenItemByNameFromActivePlayer(objectName);
            
            if (item != default)
            {
                try
                {
                    subject.OnUse(new UseItemEventArgs() {ItemToUse = item});

                    return true;
                }
                catch (UseException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.ItemNotOwned();
        }

        return false;
    }

    internal bool Buy(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.BUY].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            if (!this.universe.ActivePlayer.HasPaymentMethod)
            {
                return printingSubsystem.PayWithWhat();
            }

            var key = this.objectHandler.GetItemKeyByName(subject);
            var item = this.universe.ActivePlayer.GetUnhiddenItemByKey(key);
            if (item != default)
            {
                return printingSubsystem.ItemAlreadyOwned();
            }

            item = this.universe.ActiveLocation.GetItemByKey(key);
            if (item != default)
            {
                try
                {
                    item.OnBuy(new ContainerObjectEventArgs());
                    this.objectHandler.StoreAsActiveObject(item);

                    return true;
                }
                catch (Exception ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Buy(string verb, string processingSubject, string processingObject)
    {
        if (this.universe.VerbResources[VerbKeys.BUY].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            if (this.objectHandler.GetUnhiddenObjectByName(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Buy(verb, processingObject);
            }
        }

        return false;
    }

    internal bool Turn(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.TURN].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                try
                {
                    item.OnTurn(new TurnItemEventArgs());

                    return true;
                }
                catch (Exception ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }
            
            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Jump(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.JUMP].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);

            if (item != default)
            {
                try
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    item.OnJump(new ContainerObjectEventArgs());

                    return true;
                }
                catch (JumpException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Climb(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.CLIMB].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != null)
            {
                return printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
            }
            
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);
            this.objectHandler.StoreAsActiveObject(item);

            if (item is { IsClimbAble: true })
            {
                this.universe.ActivePlayer.HasClimbed = true;
                this.universe.ActivePlayer.ClimbedObject = item;
                return printingSubsystem.FormattedResource(BaseDescriptions.ITEM_CLIMBED, item.AccusativeArticleName, true);
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }

    internal bool Climb(string verb, string processingSubject, string processingObject)
    {
        if (this.universe.VerbResources[VerbKeys.CLIMB].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            
            if (this.objectHandler.GetUnhiddenObjectByName(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Climb(verb, processingObject);
            }
            
            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Close(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.CLOSE].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);
            this.objectHandler.StoreAsActiveObject(item);

            if (item != default)
            {
                if (item.IsCloseAble)
                {
                    if (item.IsClosed)
                    {
                        return printingSubsystem.ItemAlreadyClosed(item);
                    }

                    item.OnBeforeClose(new ContainerObjectEventArgs());

                    item.IsClosed = true;
                    this.objectHandler.HideItemsOnClose(item);
                    var result = printingSubsystem.ItemClosed(item);

                    item.OnAfterClose(new ContainerObjectEventArgs());

                    return result;
                }

                return false;
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }

    internal bool Open(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.OPEN].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                if (item.IsCloseAble)
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
                        item.OnBeforeOpen(new ContainerObjectEventArgs());
                    
                        item.IsClosed = false;
                        this.universe.UnveilFirstLevelObjects(item);
                        
                        item.OnOpen(new ContainerObjectEventArgs());
                        
                        var result = printingSubsystem.ItemOpen(item);

                        item.OnAfterOpen(new ContainerObjectEventArgs());

                        return result;
                    }
                    catch (OpenException e)
                    {
                        return printingSubsystem.Resource(e.Message);
                    }
                }

                return printingSubsystem.Resource(BaseDescriptions.IMPOSSIBLE_OPEN);
            }
            
            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }

    internal bool Talk(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.TALK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var character = this.objectHandler.GetUnhiddenCharacterByNameFromActiveLocation(subject);

            if (character == default)
            {
                return printingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            var result = printingSubsystem.Talk(character);

            character.OnAfterTalk(new ContainerObjectEventArgs());

            return result;
        }

        return false;
    }

    internal bool Eat(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.EAT].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameFromActivePlayer(subject);

            if (item == default)
            {
                item = this.objectHandler.GetUnhiddenItemByNameFromActiveLocation(subject);
                if (item is { IsEatable: true })
                {
                    this.universe.PickObject(item, true);
                }
            }

            if (item != default)
            {
                if (item.IsEatable)
                {
                    item.OnBeforeEat(new ContainerObjectEventArgs());

                    var result = printingSubsystem.ItemEaten(item);

                    item.OnAfterEat(new ContainerObjectEventArgs());

                    return result;
                }
                
                return printingSubsystem.ItemNotEatable(item);
            }

            return printingSubsystem.ItemNotOwned();
        }

        return false;
    }

    internal bool SitDown(string verb)
    {
        if (this.universe.VerbResources[VerbKeys.SIT].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var seatCount = this.universe.ActiveLocation.Items.Count(x => x.IsSeatAble);

            if (seatCount == 0)
            {
                return printingSubsystem.Resource(BaseDescriptions.NO_SEAT);
            }
            
            if (seatCount == 1)
            {
                var onlySeat = this.universe.ActiveLocation.Items.Single(x => x.IsSeatAble);
                this.objectHandler.StoreAsActiveObject(onlySeat);

                try
                {
                    this.universe.ActivePlayer.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = onlySeat});
                    onlySeat.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                
                    this.universe.ActivePlayer.SitDownOnSeat(onlySeat);
                    this.universe.ActivePlayer.OnSitDown(new SitDownEventArgs {ItemToSitOn = onlySeat});
                    onlySeat.OnSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                
                    var result = printingSubsystem.FormattedResource(BaseDescriptions.ITEM_ONLY_SEAT, onlySeat.DativeArticleName, true);
                
                    onlySeat.OnAfterSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                    this.universe.ActivePlayer.OnAfterSitDown(new SitDownEventArgs {ItemToSitOn = onlySeat});
                    
                    return result;
                }
                catch (SitDownException ex)
                {
                    this.universe.ActivePlayer.StandUp();
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.Resource(BaseDescriptions.MORE_SEATS);
        }
        
        return false;
    }
    
    internal bool SitDown(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.SIT].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            if (this.objectHandler.GetUnhiddenObjectByName(subject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.SitDown(verb);
            }
            
            var key = this.objectHandler.GetItemKeyByName(subject);
            var item = this.universe.ActiveLocation.GetUnhiddenItemByKey(key);

            if (item == default)
            {
                item = this.universe.ActivePlayer.GetUnhiddenItemByKey(key);
            }

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                if (item.IsSeatAble)
                {
                    try
                    {
                        this.universe.ActivePlayer.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = item});
                        item.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                    
                        this.universe.ActivePlayer.SitDownOnSeat(item);
                        this.universe.ActivePlayer.OnSitDown(new SitDownEventArgs {ItemToSitOn = item});
                        item.OnSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                    
                        var result = printingSubsystem.ItemSeated(item);
                    
                        item.OnAfterSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                        this.universe.ActivePlayer.OnAfterSitDown(new SitDownEventArgs {ItemToSitOn = item});
                    
                        return result;
                    }
                    catch (SitDownException ex)
                    {
                        this.universe.ActivePlayer.StandUp();
                        return printingSubsystem.Resource(ex.Message);
                    }
                }    
                return printingSubsystem.ItemNotSeatable(item);
            }
            
            return printingSubsystem.ItemNotVisible();
        }
        return false;
    }

    internal bool SitDown(string verb, string processingSubject, string processingObject)
    {
        if (this.universe.VerbResources[VerbKeys.SIT].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            
            if (this.objectHandler.GetUnhiddenObjectByName(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.SitDown(verb, processingObject);
            }
            
            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    
    internal bool StandUp(string input)
    {
        if (this.universe.VerbResources[VerbKeys.STANDUP].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            if (this.universe.ActivePlayer.IsSitting && this.universe.ActivePlayer.Seat != default)
            {
                var item = this.universe.ActivePlayer.Seat;
                this.objectHandler.StoreAsActiveObject(item);
                var eventArgs = new ContainerObjectEventArgs();
                
                this.universe.ActivePlayer.OnBeforeStandUp(eventArgs);
                item.OnBeforeStandUp(eventArgs);
                this.universe.ActivePlayer.StandUp();
                var result = printingSubsystem.Resource(BaseDescriptions.STANDING_UP);
                item.OnAfterStandUp(eventArgs);
                this.universe.ActivePlayer.OnAfterStandUp(eventArgs);
                return result;
            }
            return printingSubsystem.Resource(BaseDescriptions.NOT_SITTING);
        }

        return false;
    }
    
    internal bool Descend(string input)
    {
        if (this.universe.VerbResources[VerbKeys.DESCEND].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != default)
            {
                var item = this.universe.ActivePlayer.ClimbedObject;
                var eventArgs = new ContainerObjectEventArgs();
                
                this.universe.ActivePlayer.OnBeforeDescend(eventArgs);
                item.OnBeforeDescend(eventArgs);
                this.universe.ActivePlayer.Descend();
                var result = printingSubsystem.Resource(BaseDescriptions.DESCENDING);
                item.OnAfterDescend(eventArgs);
                this.universe.ActivePlayer.OnAfterDescend(eventArgs);
                return result;
            }
            return printingSubsystem.Resource(BaseDescriptions.NOT_SITTING);
        }

        return false;
    }
    
    internal bool Ask(string verb, string characterName, string subjectName)
    {
        if (this.universe.VerbResources[VerbKeys.ASK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            //I can only speak to visible people in the active location
            var character = this.objectHandler.GetUnhiddenCharacterByNameFromActiveLocation(characterName);

            if (character == default)
            {
                return printingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            //but I can speak about every unhidden or virtual item or character in the world
            var item = this.objectHandler.GetUnhiddenObjectFromWorldByName(subjectName);
            if (item == default)
            {
                item = this.objectHandler.GetUnhiddenCharacterByName(subjectName);
                if (item == default)
                {
                    item = this.objectHandler.GetVirtualItemByName(subjectName);
                    if (item == default)
                    {
                        return printingSubsystem.NoAnswerToInvisibleObject(character);
                    }
                }
            }

            try
            {
                character.OnAsk(new ConversationEventArgs() { Item = item });
            }
            catch (Exception ex)
            {
                printingSubsystem.NoAnswerToQuestion(ex.Message);

            }

            return true;
        }

        return false;
    }

    internal bool Say(string verb, string characterName, string phrase)
    {
        if (this.universe.VerbResources[VerbKeys.SAY].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            //I can only speak to visible people
            var character = this.objectHandler.GetUnhiddenCharacterByNameFromActiveLocation(characterName);
            if (character == default)
            {
                return printingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            var key = this.objectHandler.GetConversationAnswerKeyByName(phrase);
            if (string.IsNullOrEmpty(key))
            {
                return printingSubsystem.NoAnswer(phrase);
            }

            try
            {
                character.OnSay(new ConversationEventArgs { Phrase = key });
            }
            catch (Exception ex)
            {
                printingSubsystem.Resource(ex.Message);
            }

            return true;
        }

        return false;
    }

    internal bool Give(string verb, string characterName, string itemName)
    {
        if (this.universe.VerbResources[VerbKeys.GIVE].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            //I can only give things to visible people.
            var character = this.objectHandler.GetUnhiddenCharacterByNameFromActiveLocation(characterName);
            if (character == default)
            {
                return printingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            //...and I can give only items that i own.
            var key = this.objectHandler.GetItemKeyByName(itemName);
            var item = this.universe.ActivePlayer.GetItemByKey(key);
            if (item == default)
            {
                return printingSubsystem.ItemNotOwned();
            }

            this.objectHandler.StoreAsActiveObject(item);
            character.Items.Add(item);
            this.universe.ActivePlayer.RemoveItem(item);

            var eventArgs = new ContainerObjectEventArgs();
            this.universe.ActivePlayer.OnAfterGive(eventArgs);
            character.OnAfterGive(eventArgs);
            item.OnAfterGive(eventArgs);

            return true;
        }

        return false;
    }

    internal bool Break(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.BREAK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);
            if (item != default)
            {
                try
                {
                    item.OnBreak(new BreakItemEventArgs());
                    return true;
                }
                catch (BreakException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }
        }

        return false;
    }
    
    internal bool Break(string verb, string subject, string tool)
    {
        if (this.universe.VerbResources[VerbKeys.BREAK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);
            var toolItem = this.objectHandler.GetUnhiddenItemByNameActive(tool);

            if (item != default)
            {
                if (toolItem != default)
                {
                    if (item.IsBreakable)
                    {
                        if (!item.IsBroken)
                        {
                            try
                            {
                                item.OnBreak(new BreakItemEventArgs() { ItemToUse = toolItem });
                                return true;
                            }
                            catch (BreakException ex)
                            {
                                return printingSubsystem.Resource(ex.Message);
                            }
                        }
                        return printingSubsystem.ItemAlreadyBroken(item);
                    }
                    return printingSubsystem.ItemUnbreakable(item);
                }
                return printingSubsystem.ToolNotVisible();
            }

            return printingSubsystem.ItemNotVisible();
        }
        
        return false;
    }

    internal bool Unlock(string verb, string unlockObject)
    {
        //TODO noch nicht fertig!
        if (this.universe.VerbResources[VerbKeys.UNLOCK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(unlockObject);
            if (item != default)
            {
                return printingSubsystem.ImpossibleUnlock(item);
            }
        }

        return false;
    }

    internal bool Unlock(string verb, string unlockObject, string unlockKey)
    {
        if (this.universe.VerbResources[VerbKeys.UNLOCK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(unlockObject);
            var key = this.objectHandler.GetUnhiddenItemByNameActive(unlockKey);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                if (key != default)
                {
                    if (item.IsLockAble)
                    {
                        if (item.IsLocked)
                        {
                            if (!item.IsCloseAble || item.IsCloseAble && item.IsClosed)
                            {
                                try
                                {
                                    item.OnUnlock(new UnlockContainerEventArgs { Key = key });

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
                return printingSubsystem.KeyNotVisible();
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }

    internal bool Take(string verb, IEnumerable<string> subjects)
    {
        var result = false;
        if (this.universe.VerbResources[VerbKeys.TAKE].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            result = true;
            foreach (var subject in subjects)
            {
                var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);
                if (item == default)
                {
                    var characterKey = this.objectHandler.GetCharacterKeyByName(subject);
                    var character = this.universe.ActiveLocation.GetCharacterByKey(characterKey);
                    if (character != default)
                    {
                        result = result && printingSubsystem.ImpossiblePickup(character);
                    }
                    else
                    {
                        result = result && printingSubsystem.ItemNotVisible();
                    }
                }
                else
                {
                    try
                    {
                        item.OnBeforeTake(new ContainerObjectEventArgs());
                        result = result && universe.PickObject(item);
                        this.objectHandler.StoreAsActiveObject(item);
                        item.OnAfterTake(new ContainerObjectEventArgs());
                    }
                    catch (TakeException ex)
                    {
                        result = result && printingSubsystem.Resource(ex.Message);
                    }
                }
            }
        }

        return result;
    }
    
    internal bool Take(string verb, string processingSubject, IEnumerable<string> processingObjects)
    {
        if (this.universe.VerbResources[VerbKeys.TAKE].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            if (this.objectHandler.GetUnhiddenObjectByName(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Take(verb, processingObjects);
            }
        }

        return false;
    }

    internal bool Take(string verb)
    {
        if (this.universe.VerbResources[VerbKeys.TAKE].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var subjects = this.universe.ActiveLocation.GetAllPickableAndUnHiddenItems();
            if (subjects.Any())
            {
                var result = true;
                foreach (var item in subjects)
                {
                    try
                    {
                        item.OnBeforeTake(new ContainerObjectEventArgs());
                        result = result && universe.PickObject(item);
                        item.OnAfterTake(new ContainerObjectEventArgs());
                    }
                    catch (TakeException ex)
                    {
                        result = result && printingSubsystem.Resource(ex.Message);
                    }
                }

                return result;
            }
            else
            {
                return printingSubsystem.NothingToTake();
            }
        }

        return false;
    }

    internal bool Go(string verb, string location)
    {
        if (this.universe.VerbResources[VerbKeys.GO].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != null)
            {
                return printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
            }
            
            return this.ChangeLocationByName(location);
        }

        return false;
    }

    internal bool Drop(string verb, IEnumerable<string> subjects)
    {
        if (this.universe.VerbResources[VerbKeys.DROP].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var result = true;
            foreach (var subject in subjects)
            {
                var key = this.objectHandler.GetItemKeyByName(subject);

                var isPlayerItem = this.universe.ActivePlayer.Items.Any(x => x.Key == key);
                var isPlayerCloths = this.universe.ActivePlayer.Clothes.Any(x => x.Key == key);
                if (isPlayerItem || isPlayerCloths)
                {
                    var item = isPlayerItem ? 
                        this.universe.ActivePlayer.Items.Single(i => i.Key == key): 
                        this.universe.ActivePlayer.Clothes.Single(x => x.Key == key);

                    if (item.IsDropAble)
                    {
                        try
                        {
                            item.OnBeforeDrop(new DropItemEventArgs());

                            var singleDropResult = this.universe.ActivePlayer.RemoveItem(item);
                            result = result && singleDropResult;
                            if (singleDropResult)
                            {
                                this.universe.ActiveLocation.Items.Add(item);
                                printingSubsystem.ItemDropSuccess(item);
                                item.OnAfterDrop(new DropItemEventArgs());
                            }
                            else
                            {
                                printingSubsystem.ImpossibleDrop(item);
                            }
                        }
                        catch (DropException e)
                        {
                            printingSubsystem.Resource(e.Message);
                        }
                    }
                    else
                    {
                        printingSubsystem.ImpossibleDrop(item);
                        result = result && true;
                    }
                }
                else
                {
                    printingSubsystem.ItemNotOwned();
                }

            }

            return result;
        }

        return false;
    }

    internal bool Drop(string verb, string subjectName, string objectName)
    {
        if (this.universe.VerbResources[VerbKeys.DROP].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var subjectKey = this.objectHandler.GetItemKeyByName(subjectName);

            var isPlayerItem = this.universe.ActivePlayer.Items.Any(x => x.Key == subjectKey);
            var isPlayerCloths = this.universe.ActivePlayer.Clothes.Any(x => x.Key == subjectKey);
            
            if (isPlayerItem || isPlayerCloths)
            {
                var itemToDrop = isPlayerItem ? 
                    this.universe.ActivePlayer.Items.Single(i => i.Key == subjectKey): 
                    this.universe.ActivePlayer.Clothes.Single(x => x.Key == subjectKey);

                this.objectHandler.StoreAsActiveObject(itemToDrop);
                
                if (itemToDrop.IsDropAble)
                {
                    var objectKey = this.objectHandler.GetItemKeyByName(objectName);

                    isPlayerItem = this.universe.ActivePlayer.Items.Any(x => x.Key == objectKey);
                    isPlayerCloths = this.universe.ActivePlayer.Clothes.Any(x => x.Key == objectKey);
                    var isPlayerLinkedToObject = this.universe.ActivePlayer.LinkedTo.Any(x => x.Key == objectKey);
                    var isItemInLocation = this.universe.ActiveLocation.Items.Any(x => x.Key == objectKey);

                    if (isPlayerItem || isPlayerLinkedToObject || isPlayerCloths || isItemInLocation)
                    {
                        var itemContainer = isPlayerItem
                            ?
                            this.universe.ActivePlayer.Items.Single(i => i.Key == objectKey)
                            : isPlayerCloths
                                ? this.universe.ActivePlayer.Clothes.Single(x => x.Key == objectKey)
                                : isPlayerLinkedToObject
                                    ? this.universe.ActivePlayer.LinkedTo.Single(x => x.Key == objectKey)
                                    :
                                    this.universe.ActiveLocation.Items.Single(x => x.Key == objectKey);

                        if (itemContainer.IsContainer)
                        {
                            if (!itemContainer.IsCloseAble || itemContainer.IsCloseAble && !itemContainer.IsClosed)
                            {
                                try
                                {
                                    itemToDrop.OnBeforeDrop(new DropItemEventArgs() {ItemContainer = itemContainer});

                                    var removeSuccess = this.universe.ActivePlayer.RemoveItem(itemToDrop);
                            
                                    if (removeSuccess)
                                    {
                                        itemContainer.Items.Add(itemToDrop);
                                        printingSubsystem.ItemDropSuccess(itemToDrop, itemContainer);
                                        itemContainer.OnAfterDrop(new DropItemEventArgs() {ItemContainer = itemContainer});
                                        return true;
                                    }

                                    return printingSubsystem.ImpossibleDrop(itemContainer);
                                }
                                catch (DropException e)
                                {
                                    return printingSubsystem.Resource(e.Message);
                                }
                            }
                            
                            return printingSubsystem.ItemStillClosed(itemContainer);
                        }
                        
                        return printingSubsystem.ItemIsNotAContainer(itemContainer);
                    }
                }

                return printingSubsystem.ImpossibleDrop(itemToDrop);
            }
            return printingSubsystem.ItemNotOwned();
        }
        
        return false;
    }

    internal bool Name(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.NAME].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            this.universe.ActivePlayer.Name = subject;
            this.universe.ActivePlayer.IsStranger = false;
            printingSubsystem.ActivePlayer(this.universe.ActivePlayer);

            return true;
        }

        return false;
    }

    internal bool Inventory(string input)
    {
        if (this.universe.VerbResources[VerbKeys.INV].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            printingSubsystem.ActivePlayer(this.universe.ActivePlayer);
            return true;
        }

        return false;
    }
    internal void ChangeLocation(Directions direction)
    {
        if (this.universe.LocationMap.ContainsKey(this.universe.ActiveLocation))
        {
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != null)
            {
                printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
                return;
            }

            if (this.universe.ActivePlayer.IsSitting && this.universe.ActivePlayer.Seat != null)    
            {
                printingSubsystem.Resource(BaseDescriptions.ALREADY_SITTING);
                return;
            }
            
            var mappings = this.universe.LocationMap[this.universe.ActiveLocation];
            var newLocationMap = mappings.Where(i => !i.IsHidden).SingleOrDefault(x => x.Direction == direction);
            if (newLocationMap != default)
            {
                try
                {
                    this.universe.ActiveLocation.OnBeforeChangeLocation(new ChangeLocationEventArgs(newLocationMap));
                    if (!newLocationMap.Location.IsLocked)
                    {
                        if (!newLocationMap.Location.IsClosed)
                        {
                            this.universe.ActiveLocation = newLocationMap.Location;
                            this.objectHandler.ClearActiveObjectIfNotInInventory();

                            printingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
                        }
                    }

                    //TODO - maybe OnUnsuccessfulLocationChange... 
                    var status = this.universe.ActiveLocation.OnAfterChangeLocation(new ChangeLocationEventArgs(newLocationMap));

                    if (status == ChangeLocationStatus.IsLocked)
                    {
                        printingSubsystem.WayIsLocked(newLocationMap.Location);
                    }
                    else if (status == ChangeLocationStatus.IsClosed)
                    {
                        printingSubsystem.WayIsClosed(newLocationMap.Location);
                    }
                }
                catch (BeforeChangeLocationException ex)
                {
                    printingSubsystem.Resource(ex.Message);
                }
            }
            else
            {
                printingSubsystem.Resource(BaseDescriptions.NO_WAY);
            }
        }
        else
        {
            printingSubsystem.Resource(BaseDescriptions.NO_WAY);
        }
    }

    internal bool ChangeLocationByName(string input)
    {
        var locationKey = this.objectHandler.GetLocationKeyByName(input);
        if (!string.IsNullOrEmpty(locationKey))
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
                var newLocation = mappings.Where(i => !i.IsHidden).SingleOrDefault(x => x.Location.Key == locationKey);
                if (newLocation != default)
                {
                    if (!newLocation.Location.IsLocked)
                    {
                        this.universe.ActiveLocation = newLocation.Location;
                        this.objectHandler.ClearActiveObjectIfNotInInventory();
                        return printingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
                    }

                    return printingSubsystem.Resource(BaseDescriptions.ROOM_LOCKDESCRIPTION);
                }
            }

            return printingSubsystem.Resource(BaseDescriptions.NO_WAY);
        }

        return false;
    }

    internal bool Directions(string input)
    {
        var result = true;

        if (this.universe.VerbResources[VerbKeys.E].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            this.ChangeLocation(Objects.Directions.E);
        }
        else if (this.universe.VerbResources[VerbKeys.W].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            this.ChangeLocation(Objects.Directions.W);
        }
        else if (this.universe.VerbResources[VerbKeys.N].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            this.ChangeLocation(Objects.Directions.N);
        }
        else if (this.universe.VerbResources[VerbKeys.S].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            this.ChangeLocation(Objects.Directions.S);
        }
        else if (this.universe.VerbResources[VerbKeys.SE].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            this.ChangeLocation(Objects.Directions.SE);
        }
        else if (this.universe.VerbResources[VerbKeys.SW].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            this.ChangeLocation(Objects.Directions.SW);
        }
        else if (this.universe.VerbResources[VerbKeys.NE].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            this.ChangeLocation(Objects.Directions.NE);
        }
        else if (this.universe.VerbResources[VerbKeys.NW].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            this.ChangeLocation(Objects.Directions.NW);
        }
        else if (this.universe.VerbResources[VerbKeys.UP].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            this.ChangeLocation(Objects.Directions.UP);
        }
        else if (this.universe.VerbResources[VerbKeys.DOWN].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            this.ChangeLocation(Objects.Directions.DOWN);
        }
        else
        {
            result = false;
        }

        return result;
    }

    private bool IsVerb(string verbKey, string verbToCheck)
    {
        var verbOverrides = this.universe.ActiveLocation.GetVerbAlternatives(verbKey);
        var mergedVerbs = verbOverrides.Count > 0
            ? this.universe.VerbResources[verbKey].Union(second: verbOverrides)
            : this.universe.VerbResources[verbKey];
        
        return mergedVerbs.Contains(verbToCheck, StringComparer.InvariantCultureIgnoreCase);
    }
}
