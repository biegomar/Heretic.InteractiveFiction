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
        if (this.IsVerb(VerbKeys.CREDITS, verb))
        {
            return printingSubsystem.Credits();
        }

        return false;
    }
    
    internal bool Look(string verb)
    {
        if (this.IsVerb(VerbKeys.LOOK, verb))
        {
            try
            {
                var eventArgs = new ContainerObjectEventArgs();
                
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

        return false;
    }

    internal bool Look(string verb, string subject)
    {
        if (this.IsVerb(VerbKeys.LOOK, verb))
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(subject);
            if (item != default)
            {
                try
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    
                    var eventArgs = new ContainerObjectEventArgs();
                    var eventArgsForActiveLocation = new ContainerObjectEventArgs() { ExternalItemKey = item.Key };

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

        return false;
    }
    
    internal bool Look(string verb, string processingSubject, string processingObject)
    {
        if (this.IsVerb(VerbKeys.LOOK, verb))
        {
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Look(verb, processingObject);
            }
        }

        return false;
    }
    
    internal bool Read(string verb, string subject)
    {
        if (this.IsVerb(VerbKeys.READ, verb))
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(subject);
            if (item != default)
            {
                if (item.IsReadable)
                {
                    try
                    {
                        this.objectHandler.StoreAsActiveObject(item);
                        
                        var readItemEventArgs = new ReadItemEventArgs();
                        
                        item.OnBeforeRead(readItemEventArgs);

                        var result = string.IsNullOrWhiteSpace(item.LetterContentDescription) ? 
                            printingSubsystem.Resource(BaseDescriptions.NO_LETTER_CONTENT) : 
                            printingSubsystem.FormattedResource(BaseDescriptions.LETTER_CONTENT, item.LetterContentDescription);
                        
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

        return false;
    }
    
    internal bool Hint(string verb, string subject)
    {
        if (this.IsVerb(VerbKeys.HINT, verb))
        {
            if (String.Equals(subject, BaseDescriptions.ON, StringComparison.CurrentCultureIgnoreCase))
            {
                isHintActive = true;
                return printingSubsystem.Resource(BaseDescriptions.HINT_ON);
            }
            
            if (String.Equals(subject, BaseDescriptions.OFF, StringComparison.CurrentCultureIgnoreCase))
            {
                isHintActive = false;
                return printingSubsystem.Resource(BaseDescriptions.HINT_OFF);
            }
        }

        return false;
    }

    internal bool Remark(string verb)
    {
        if (this.IsVerb(VerbKeys.REM, verb))
        {
            return true;
        }

        return false;
    }
    
    internal bool Pull(string verb, string subject)
    {
        if (this.IsVerb(VerbKeys.PULL, verb))
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(subject);
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
        if (this.IsVerb(VerbKeys.PULL, verb))
        {
            var subject = this.objectHandler.GetUnhiddenObjectByNameActive(subjectName);

            if (subject == default)
            {
                return printingSubsystem.CanNotUseObject(subjectName);
            }
            
            this.objectHandler.StoreAsActiveObject(subject);

            var item = this.objectHandler.GetUnhiddenObjectByNameActive(objectName);

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
        if (this.IsVerb(VerbKeys.PUSH, verb))
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(subject);
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
        if (this.IsVerb(VerbKeys.PUSH, verb))
        {
            var subject = this.objectHandler.GetUnhiddenObjectByNameActive(subjectName);

            if (subject == default)
            {
                return printingSubsystem.CanNotUseObject(subjectName);
            }
            
            this.objectHandler.StoreAsActiveObject(subject);

            var item = this.objectHandler.GetUnhiddenObjectByNameActive(objectName);

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
        if (this.IsVerb(VerbKeys.ALTER_EGO, verb))
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(subject);
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
        if (this.IsVerb(VerbKeys.WRITE, verb))
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

    internal bool Help(string verb)
    {
        if (this.IsVerb(VerbKeys.HELP, verb))
        {
            return printingSubsystem.Help(this.universe.Verbs);
        }

        return false;
    }

    internal bool History(string verb, ICollection<string> historyCollection)
    {
        if (this.IsVerb(VerbKeys.HISTORY, verb))
        {
            return printingSubsystem.History(historyCollection);
        }

        return false;
    }
    
    internal bool Save(string verb, ICollection<string> historyCollection)
    {
        try
        {
            if (this.IsVerb(VerbKeys.SAVE, verb))
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

    internal bool Score(string verb)
    {
        if (this.IsVerb(VerbKeys.SCORE, verb))
        {
            return printingSubsystem.Score(universe.Score, universe.MaxScore);
        }

        return false;
    }

    internal bool Ways(string verb)
    {
        if (this.IsVerb(VerbKeys.WAYS, verb))
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
        if (this.IsVerb(VerbKeys.USE, verb))
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(subject);
            
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                try
                {
                    item.OnUse(new UseItemEventArgs() {OptionalErrorMessage = this.universe.GetVerb(verb).ErrorMessage});

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
        if (this.IsVerb(VerbKeys.USE, verb))
        {
            var subject = this.objectHandler.GetUnhiddenObjectByNameActive(subjectName);

            if (subject == default)
            {
                return printingSubsystem.CanNotUseObject(subjectName);
            }

            this.objectHandler.StoreAsActiveObject(subject);
            
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(objectName);
            
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
        if (this.IsVerb(VerbKeys.BUY, verb))
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
                this.objectHandler.StoreAsActiveObject(item);
                
                try
                {
                    item.OnBeforeBuy(new ContainerObjectEventArgs());
                    item.OnBuy(new ContainerObjectEventArgs());
                    item.OnAfterBuy(new ContainerObjectEventArgs());
                    
                    return true;
                }
                catch (BuyException ex)
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
        if (this.IsVerb(VerbKeys.BUY, verb))
        {
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Buy(verb, processingObject);
            }
        }

        return false;
    }

    internal bool Turn(string verb, string subject)
    {
        if (this.IsVerb(VerbKeys.TURN, verb))
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
        if (this.IsVerb(VerbKeys.JUMP, verb))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var containerObjectEventArgs = new ContainerObjectEventArgs();
                    
                    item.OnBeforeJump(containerObjectEventArgs);
                    item.OnJump(containerObjectEventArgs);
                    item.OnAfterJump(containerObjectEventArgs);

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
        if (this.IsVerb(VerbKeys.CLIMB, verb))
        {
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != null)
            {
                return printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
            }
            
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);
            this.objectHandler.StoreAsActiveObject(item);

            if (item != default)
            {
                if (item.IsClimbAble)
                {
                    if (!this.universe.ActivePlayer.HasClimbed)
                    {
                        try
                        {
                            var eventArgs = new ContainerObjectEventArgs();
                            item.OnBeforeClimb(eventArgs);

                            this.universe.ActivePlayer.HasClimbed = true;
                            this.universe.ActivePlayer.ClimbedObject = item;
                            item.OnClimb(eventArgs);

                            item.OnAfterClimb(eventArgs);
                
                            return printingSubsystem.FormattedResource(BaseDescriptions.ITEM_CLIMBED, item.AccusativeArticleName, true);
                        }
                        catch (ClimbException ex)
                        {
                            this.universe.ActivePlayer.HasClimbed = false;
                            this.universe.ActivePlayer.ClimbedObject = default;
                            return printingSubsystem.Resource(ex.Message);
                        }    
                    }
                    
                    return printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
                }

                return printingSubsystem.Resource(BaseDescriptions.IMPOSSIBLE_CLIMB);
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }

    internal bool Climb(string verb, string processingSubject, string processingObject)
    {
        if (this.IsVerb(VerbKeys.CLIMB, verb))
        {
            
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Climb(verb, processingObject);
            }
            
            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Close(string verb, string subject)
    {
        if (this.IsVerb(VerbKeys.CLOSE, verb))
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

                    try
                    {
                        var eventArgs = new ContainerObjectEventArgs();
                    
                        item.OnBeforeClose(eventArgs);

                        item.IsClosed = true;
                        this.objectHandler.HideItemsOnClose(item);
                        item.OnClose(new ContainerObjectEventArgs());

                        item.OnAfterClose(new ContainerObjectEventArgs());

                        return printingSubsystem.ItemClosed(item);
                    }
                    catch (CloseException ex)
                    {
                        item.IsClosed = false;
                        this.objectHandler.UnhideItemsOnOpen(item);
                        return printingSubsystem.Resource(ex.Message);
                    }
                }

                return false;
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }

    internal bool Open(string verb, string subject)
    {
        if (this.IsVerb(VerbKeys.OPEN, verb))
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
                        item.IsClosed = true;
                        this.objectHandler.HideItemsOnClose(item);
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
        if (this.IsVerb(VerbKeys.TALK, verb))
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

    internal bool SitDown(string verb)
    {
        if (this.IsVerb(VerbKeys.SIT, verb))
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
                    var sitDownEventArgs = new SitDownEventArgs {ItemToSitOn = onlySeat};
                    var downEventArgs = new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer};
                    
                    this.universe.ActivePlayer.OnBeforeSitDown(sitDownEventArgs);
                    onlySeat.OnBeforeSitDown(downEventArgs);
                
                    this.universe.ActivePlayer.SitDownOnSeat(onlySeat);
                    this.universe.ActivePlayer.OnSitDown(sitDownEventArgs);
                    onlySeat.OnSitDown(downEventArgs);
                
                    var result = printingSubsystem.FormattedResource(BaseDescriptions.ITEM_ONLY_SEAT, onlySeat.DativeArticleName, true);
                
                    onlySeat.OnAfterSitDown(downEventArgs);
                    this.universe.ActivePlayer.OnAfterSitDown(sitDownEventArgs);
                    
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
        if (this.IsVerb(VerbKeys.SIT, verb))
        {
            if (this.objectHandler.GetUnhiddenObjectByNameActive(subject) is { } player && player.Key == this.universe.ActivePlayer.Key)
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
                        var sitDownEventArgs = new SitDownEventArgs {ItemToSitOn = item};
                        var downEventArgs = new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer};
                        
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
        if (this.IsVerb(VerbKeys.SIT, verb))
        {
            
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.SitDown(verb, processingObject);
            }
            
            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    
    internal bool StandUp(string verb)
    {
        if (this.IsVerb(VerbKeys.STANDUP, verb))
        {
            if (this.universe.ActivePlayer.IsSitting && this.universe.ActivePlayer.Seat != default)
            {
                var item = this.universe.ActivePlayer.Seat;
                try
                {
                    this.objectHandler.StoreAsActiveObject(item);
                
                    var eventArgs = new ContainerObjectEventArgs();
                
                    this.universe.ActivePlayer.OnBeforeStandUp(eventArgs);
                    item.OnBeforeStandUp(eventArgs);
                
                    this.universe.ActivePlayer.StandUp();
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
    
    internal bool Descend(string verb)
    {
        if (this.IsVerb(VerbKeys.DESCEND, verb))
        {
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != default)
            {
                var item = this.universe.ActivePlayer.ClimbedObject;
                try
                {
                    var eventArgs = new ContainerObjectEventArgs();
                
                    this.universe.ActivePlayer.OnBeforeDescend(eventArgs);
                    item.OnBeforeDescend(eventArgs);
                    
                    this.universe.ActivePlayer.Descend();
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
            return printingSubsystem.Resource(BaseDescriptions.NOT_SITTING);
        }

        return false;
    }
    
    internal bool Ask(string verb, string characterName, string subjectName)
    {
        if (this.IsVerb(VerbKeys.ASK, verb))
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
        if (this.IsVerb(VerbKeys.SAY, verb))
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
        if (this.IsVerb(VerbKeys.GIVE, verb))
        {
            //I can only give things to visible people.
            var character = this.objectHandler.GetUnhiddenCharacterByNameFromActiveLocation(characterName);
            if (character == default)
            {
                return printingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            //...and I can give only items that i own.
            var item = this.objectHandler.GetUnhiddenItemByNameFromActivePlayer(itemName);
            if (item == default)
            {
                return printingSubsystem.ItemNotOwned();
            }

            this.objectHandler.StoreAsActiveObject(item);

            try
            {
                var eventArgs = new ContainerObjectEventArgs();
                this.universe.ActivePlayer.OnBeforeGive(eventArgs);
                character.OnBeforeGive(eventArgs);
                item.OnBeforeGive(eventArgs);
                
                character.Items.Add(item);
                this.universe.ActivePlayer.RemoveItem(item);
                
                this.universe.ActivePlayer.OnGive(eventArgs);
                character.OnGive(eventArgs);
                item.OnGive(eventArgs);
                
                this.universe.ActivePlayer.OnAfterGive(eventArgs);
                character.OnAfterGive(eventArgs);
                item.OnAfterGive(eventArgs);

                return true;
            }
            catch (GiveException ex)
            {
                this.universe.ActivePlayer.Items.Add(item);
                character.RemoveItem(item);
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }

    internal bool Break(string verb, string subject)
    {
        if (this.IsVerb(VerbKeys.BREAK, verb))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);
            
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                if (item.IsBreakable)
                {
                    if (!item.IsBroken)
                    {
                        try
                        {
                            var eventArgs = new BreakItemEventArgs();
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

        }

        return false;
    }
    
    internal bool Break(string verb, string subject, string tool)
    {
        if (this.IsVerb(VerbKeys.BREAK, verb))
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);
            var toolItem = this.objectHandler.GetUnhiddenItemByNameActive(tool);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                if (toolItem != default)
                {
                    if (item.IsBreakable)
                    {
                        if (!item.IsBroken)
                        {
                            try
                            {
                                var eventArgs = new BreakItemEventArgs() { ItemToUse = toolItem };
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
                return printingSubsystem.ToolNotVisible();
            }

            return printingSubsystem.ItemNotVisible();
        }
        
        return false;
    }

    internal bool Unlock(string verb, string unlockObject)
    {
        //TODO noch nicht fertig!
        if (this.IsVerb(VerbKeys.UNLOCK, verb))
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
        if (this.IsVerb(VerbKeys.UNLOCK, verb))
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
        if (this.IsVerb(VerbKeys.TAKE, verb))
        {
            result = true;
            foreach (var subject in subjects)
            {
                var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);
                if (item == default)
                {
                    var character = this.objectHandler.GetUnhiddenCharacterByNameFromActiveLocation(subject);
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
                        this.objectHandler.StoreAsActiveObject(item);

                        var eventArgs = new ContainerObjectEventArgs();
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
            }
        }

        return result;
    }
    
    internal bool Take(string verb, string processingSubject, IEnumerable<string> processingObjects)
    {
        if (this.IsVerb(VerbKeys.TAKE, verb))
        {
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Take(verb, processingObjects);
            }
        }

        return false;
    }

    internal bool Take(string verb)
    {
        if (this.IsVerb(VerbKeys.TAKE, verb))
        {
            var subjects = this.universe.ActiveLocation.GetAllPickableAndUnHiddenItems();
            if (subjects.Any())
            {
                var result = true;
                foreach (var item in subjects)
                {
                    try
                    {
                        var eventArgs = new ContainerObjectEventArgs();
                        
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

        return false;
    }

    internal bool Go(string verb, string location)
    {
        if (this.IsVerb(VerbKeys.GO, verb))
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
        if (this.IsVerb(VerbKeys.DROP, verb))
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
                                
                                item.OnDrop(new DropItemEventArgs());
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
                            this.universe.PickObject(item, true);
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
        if (this.IsVerb(VerbKeys.DROP, verb))
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
        if (this.IsVerb(VerbKeys.NAME, verb))
        {
            this.universe.ActivePlayer.Name = subject;
            this.universe.ActivePlayer.IsStranger = false;
            printingSubsystem.ActivePlayer(this.universe.ActivePlayer);

            return true;
        }

        return false;
    }

    internal bool Inventory(string verb)
    {
        if (this.IsVerb(VerbKeys.INV, verb))
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

    internal bool Directions(string verb)
    {
        var result = true;

        if (this.IsVerb(VerbKeys.E, verb))
        {
            this.ChangeLocation(Objects.Directions.E);
        }
        else if (this.IsVerb(VerbKeys.W, verb))
        {
            this.ChangeLocation(Objects.Directions.W);
        }
        else if (this.IsVerb(VerbKeys.N, verb))
        {
            this.ChangeLocation(Objects.Directions.N);
        }
        else if (this.IsVerb(VerbKeys.S, verb))
        {
            this.ChangeLocation(Objects.Directions.S);
        }
        else if (this.IsVerb(VerbKeys.SE, verb))
        {
            this.ChangeLocation(Objects.Directions.SE);
        }
        else if (this.IsVerb(VerbKeys.SW, verb))
        {
            this.ChangeLocation(Objects.Directions.SW);
        }
        else if (this.IsVerb(VerbKeys.NE, verb))
        {
            this.ChangeLocation(Objects.Directions.NE);
        }
        else if (this.IsVerb(VerbKeys.NW, verb))
        {
            this.ChangeLocation(Objects.Directions.NW);
        }
        else if (this.IsVerb(VerbKeys.UP, verb))
        {
            this.ChangeLocation(Objects.Directions.UP);
        }
        else if (this.IsVerb(VerbKeys.DOWN, verb))
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
        return this.universe.IsVerb(verbKey, verbToCheck);
    }
}
