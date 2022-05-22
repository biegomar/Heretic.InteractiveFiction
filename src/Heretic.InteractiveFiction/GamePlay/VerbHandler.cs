﻿using System.Collections.Specialized;
using System.Runtime.InteropServices;
using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class VerbHandler
{
    private readonly Universe universe;
    private readonly IPrintingSubsystem PrintingSubsystem;
    private readonly ObjectHandler objectHandler;
    private bool isHintActive;

    internal VerbHandler(Universe universe, IPrintingSubsystem printingSubsystem)
    {
        this.PrintingSubsystem = printingSubsystem;
        this.universe = universe;
        this.objectHandler = new ObjectHandler(universe);
        this.isHintActive = false;
    }

    internal bool Quit(string input)
    {
        return this.universe.VerbResources[VerbKeys.QUIT].Contains(input, StringComparer.InvariantCultureIgnoreCase);
    }

    internal bool Credits(string verb)
    {
        if (this.universe.VerbResources[VerbKeys.CREDITS].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            return PrintingSubsystem.Credits();
        }

        return false;
    }
    
    internal bool Look(string verb)
    {
        if (this.universe.VerbResources[VerbKeys.LOOK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            this.universe.UnveilFirstLevelObjects(this.universe.ActiveLocation);
            return PrintingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
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
                var result = PrintingSubsystem.PrintObject(item);

                item.OnAfterLook(new ContainerObjectEventArgs());

                return result;
            }

            // lets have a look at surroundings.
            var itemKey = this.objectHandler.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                var result = PrintingSubsystem.Resource(this.universe.ActiveLocation.Surroundings[itemKey]());
                
                // surroundings only exist within the active location.
                this.universe.ActiveLocation.OnAfterLook(new ContainerObjectEventArgs() {ExternalItemKey = itemKey});

                return result;
            }

            return PrintingSubsystem.ItemNotVisible();
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
            if (item is { IsReadable: true })
            {
                try
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    item.OnBeforeRead(new ContainerObjectEventArgs());

                    var result = string.IsNullOrWhiteSpace(item.LetterContentDescription) ? 
                        PrintingSubsystem.Resource(BaseDescriptions.NO_LETTER_CONTENT) : 
                        PrintingSubsystem.FormattedResource(BaseDescriptions.LETTER_CONTENT, item.LetterContentDescription);
                    
                    item.OnAfterRead(new ContainerObjectEventArgs());

                    return result;
                }
                catch (ReadException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
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
                return PrintingSubsystem.Resource(BaseDescriptions.HINT_ON);
            }
            
            if (subject == BaseDescriptions.OFF.ToLower())
            {
                isHintActive = false;
                return PrintingSubsystem.Resource(BaseDescriptions.HINT_OFF);
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
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
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
                return PrintingSubsystem.CanNotUseObject(subjectName);
            }
            
            this.objectHandler.StoreAsActiveObject(subject);

            var item = this.objectHandler.GetUnhiddenObjectByName(objectName);

            if (item == default)
            {
                return PrintingSubsystem.CanNotUseObject(objectName);
            }

            try
            {
                subject.OnPull(new PullItemEventArgs() {ItemToUse = item});

                return true;
            }
            catch (PullException ex)
            {
                return PrintingSubsystem.Resource(ex.Message);
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
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }
            
            // lets have a look at surroundings.
            var itemKey = this.objectHandler.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                try
                {
                    this.universe.ActiveLocation.OnPush(new PushItemEventArgs() {ExternalItemKey = itemKey});
                    return true;
                }
                catch (Exception e)
                {
                    return PrintingSubsystem.Resource(e.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
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
                return PrintingSubsystem.CanNotUseObject(subjectName);
            }
            
            this.objectHandler.StoreAsActiveObject(subject);

            var item = this.objectHandler.GetUnhiddenObjectByName(objectName);

            if (item == default)
            {
                return PrintingSubsystem.CanNotUseObject(objectName);
            }

            try
            {
                subject.OnPush(new PushItemEventArgs() {ItemToUse = item});

                return true;
            }
            catch (PushException ex)
            {
                return PrintingSubsystem.Resource(ex.Message);
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
                var result = PrintingSubsystem.AlterEgo(item);
                return result;
            }

            var itemKey = this.objectHandler.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                var joint = string.Join('|', this.universe.ItemResources[itemKey]);
                return PrintingSubsystem.AlterEgo(joint);
            }

            return PrintingSubsystem.ItemNotVisible();

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
                return PrintingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }

    internal bool Help(string input)
    {
        if (this.universe.VerbResources[VerbKeys.HELP].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            return PrintingSubsystem.Help(this.universe.VerbResources);
        }

        return false;
    }

    internal bool History(string input, ICollection<string> historyCollection)
    {
        if (this.universe.VerbResources[VerbKeys.HISTORY].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            return PrintingSubsystem.History(historyCollection);
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

                return PrintingSubsystem.FormattedResource(BaseDescriptions.GAME_SAVED, combine);
            }

            return false;
        }
        catch (Exception e)
        {
            PrintingSubsystem.Resource(e.Message);
            return false;
        }
    }

    internal bool Score(string input)
    {
        if (this.universe.VerbResources[VerbKeys.SCORE].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            return PrintingSubsystem.Score(universe.Score, universe.MaxScore);
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
                return PrintingSubsystem.DestinationNode(this.universe.ActiveLocation, this.universe.LocationMap);
            }

            return PrintingSubsystem.Resource(BaseDescriptions.NO_WAYS);
        }

        return false;
    }

    internal bool Use(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.USE].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var key = this.objectHandler.GetItemKeyByName(subject);
            var item = this.universe.ActivePlayer.GetUnhiddenItemByKey(key);

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
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotOwned();
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
                return PrintingSubsystem.CanNotUseObject(subjectName);
            }

            this.objectHandler.StoreAsActiveObject(subject);
            
            var item = this.objectHandler.GetUnhiddenObjectByName(objectName);

            if (item == default)
            {
                return PrintingSubsystem.CanNotUseObject(objectName);
            }

            try
            {
                subject.OnUse(new UseItemEventArgs() {ItemToUse = item});

                return true;
            }
            catch (UseException ex)
            {
                return PrintingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }

    internal bool Buy(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.BUY].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            if (!this.universe.ActivePlayer.HasPaymentMethod)
            {
                return PrintingSubsystem.PayWithWhat();
            }

            var key = this.objectHandler.GetItemKeyByName(subject);
            var item = this.universe.ActivePlayer.GetUnhiddenItemByKey(key);
            if (item != default)
            {
                return PrintingSubsystem.ItemAlreadyOwned();
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
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
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
                    item.OnTurn(new ContainerObjectEventArgs());

                    return true;
                }
                catch (Exception ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
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
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Climb(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.CLIMB].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != null)
            {
                return PrintingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
            }
            
            var item = this.objectHandler.GetUnhiddenItemByNameActive(subject);
            this.objectHandler.StoreAsActiveObject(item);

            if (item is { IsClimbAble: true })
            {
                this.universe.ActivePlayer.HasClimbed = true;
                this.universe.ActivePlayer.ClimbedObject = item;
                return PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_CLIMBED, item.AccusativeArticleName, true);
            }

            // lets have a look at surroundings.
            if (item == default)
            {
                var itemKey = this.objectHandler.GetItemKeyByName(subject);
                if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
                {
                    try
                    {
                        this.universe.ActiveLocation.OnClimb(new ContainerObjectEventArgs() {ExternalItemKey = itemKey});
                        return true;
                    }
                    catch (Exception e)
                    {
                        return PrintingSubsystem.Resource(e.Message);
                    }
                }
            }
            
            return PrintingSubsystem.ItemNotVisible();
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
            
            return PrintingSubsystem.ItemNotVisible();
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
                        return PrintingSubsystem.ItemAlreadyClosed(item);
                    }

                    item.OnBeforeClose(new ContainerObjectEventArgs());

                    item.IsClosed = true;
                    this.objectHandler.HideItemsOnClose(item);
                    var result = PrintingSubsystem.ItemClosed(item);

                    item.OnAfterClose(new ContainerObjectEventArgs());

                    return result;
                }

                return false;
            }

            // lets have a look at surroundings.
            var itemKey = this.objectHandler.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                try
                {
                    this.universe.ActiveLocation.OnClose(new ContainerObjectEventArgs() {ExternalItemKey = itemKey});
                    return true;
                }
                catch (Exception e)
                {
                    return PrintingSubsystem.Resource(e.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
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
                        return PrintingSubsystem.ItemStillLocked(item);
                    }

                    if (!item.IsClosed)
                    {
                        return PrintingSubsystem.ItemAlreadyOpen(item);
                    }

                    try
                    {
                        item.OnBeforeOpen(new ContainerObjectEventArgs());
                    
                        item.IsClosed = false;
                        this.universe.UnveilFirstLevelObjects(item);
                        var result = PrintingSubsystem.ItemOpen(item);

                        item.OnAfterOpen(new ContainerObjectEventArgs());

                        return result;
                    }
                    catch (OpenException e)
                    {
                        return PrintingSubsystem.Resource(e.Message);
                    }
                }

                return PrintingSubsystem.Resource(BaseDescriptions.IMPOSSIBLE_OPEN);
            }
            
            // lets have a look at surroundings.
            var itemKey = this.objectHandler.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                // surroundings only exist within the active location.
                try
                {
                    this.universe.ActiveLocation.OnOpen(new ContainerObjectEventArgs() {ExternalItemKey = itemKey});
                    return true;
                }
                catch (OpenException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
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
                return PrintingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            var result = PrintingSubsystem.Talk(character);

            character.OnAfterTalk(new ContainerObjectEventArgs());

            return result;
        }

        return false;
    }

    internal bool Eat(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.EAT].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var key = this.objectHandler.GetItemKeyByName(subject);
            var item = this.universe.ActivePlayer.GetUnhiddenItemByKey(key);

            if (item == default)
            {
                item = this.universe.ActiveLocation.GetUnhiddenItemByKey(key);
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

                    var result = PrintingSubsystem.ItemEaten(item);

                    item.OnAfterEat(new ContainerObjectEventArgs());

                    return result;
                }
                
                return PrintingSubsystem.ItemNotEatable(item);
            }

            return PrintingSubsystem.ItemNotOwned();
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
                try
                {
                    // surroundings only exist within the active location.
                    this.universe.ActiveLocation.OnSitDown(new SitDownEventArgs());
                    return true;
                }
                catch (Exception e)
                {
                    return PrintingSubsystem.Resource(e.Message);
                }
            }
            
            if (seatCount == 1)
            {
                var onlySeat = this.universe.ActiveLocation.Items.Single(x => x.IsSeatAble);
                this.objectHandler.StoreAsActiveObject(onlySeat);
                
                this.universe.ActivePlayer.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = onlySeat});
                onlySeat.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                this.universe.ActivePlayer.SitDownOnSeat(onlySeat);
                var result = PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_ONLY_SEAT, onlySeat.DativeArticleName, true);
                onlySeat.OnAfterSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                this.universe.ActivePlayer.OnAfterSitDown(new SitDownEventArgs {ItemToSitOn = onlySeat});

                return result;
            }

            return PrintingSubsystem.Resource(BaseDescriptions.MORE_SEATS);
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
                    this.universe.ActivePlayer.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = item});
                    item.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                    this.universe.ActivePlayer.SitDownOnSeat(item);
                    var result = PrintingSubsystem.ItemSeated(item);
                    item.OnAfterSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                    this.universe.ActivePlayer.OnAfterSitDown(new SitDownEventArgs {ItemToSitOn = item});
                    
                    return result;
                }    
                return PrintingSubsystem.ItemNotSeatable(item);
            }
            
            // lets have a look at surroundings.
            var itemKey = this.objectHandler.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                try
                {
                    this.universe.ActiveLocation.OnSitDown(new SitDownEventArgs {ExternalItemKey = itemKey});
                }
                catch (Exception e)
                {
                    return PrintingSubsystem.Resource(e.Message);
                }
            }

            return PrintingSubsystem.ItemNotVisible();
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
            
            return PrintingSubsystem.ItemNotVisible();
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
                var result = PrintingSubsystem.Resource(BaseDescriptions.STANDING_UP);
                item.OnAfterStandUp(eventArgs);
                this.universe.ActivePlayer.OnAfterStandUp(eventArgs);
                return result;
            }
            return PrintingSubsystem.Resource(BaseDescriptions.NOT_SITTING);
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
                var result = PrintingSubsystem.Resource(BaseDescriptions.DESCENDING);
                item.OnAfterDescend(eventArgs);
                this.universe.ActivePlayer.OnAfterDescend(eventArgs);
                return result;
            }
            return PrintingSubsystem.Resource(BaseDescriptions.NOT_SITTING);
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
                return PrintingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
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
                        return PrintingSubsystem.NoAnswerToInvisibleObject(character);
                    }
                }
            }

            try
            {
                character.OnAsk(new ConversationEventArgs() { Item = item });
            }
            catch (Exception ex)
            {
                PrintingSubsystem.NoAnswerToQuestion(ex.Message);

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
                return PrintingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            var key = this.objectHandler.GetConversationAnswerKeyByName(phrase);
            if (string.IsNullOrEmpty(key))
            {
                return PrintingSubsystem.NoAnswer(phrase);
            }

            try
            {
                character.OnSay(new ConversationEventArgs { Phrase = key });
            }
            catch (Exception ex)
            {
                PrintingSubsystem.Resource(ex.Message);
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
                return PrintingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            //...and I can give only items that i own.
            var key = this.objectHandler.GetItemKeyByName(itemName);
            var item = this.universe.ActivePlayer.GetItemByKey(key);
            if (item == default)
            {
                return PrintingSubsystem.ItemNotOwned();
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
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }
            
            // lets have a look at surroundings.
            var itemKey = this.objectHandler.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                try
                {
                    this.universe.ActiveLocation.OnBreak(new BreakItemEventArgs());
                    return true;
                }
                catch (BreakException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
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
                                return PrintingSubsystem.Resource(ex.Message);
                            }
                        }
                        return PrintingSubsystem.ItemAlreadyBroken(item);
                    }
                    return PrintingSubsystem.ItemUnbreakable(item);
                }
                return PrintingSubsystem.ToolNotVisible();
            }

            // lets have a look at surroundings.
            var itemKey = this.objectHandler.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                if (toolItem != default)
                {
                    // surroundings only exist within the active location.
                    try
                    {
                        this.universe.ActiveLocation.OnBreak(new BreakItemEventArgs() { ItemToUse = toolItem });
                        return true;
                    }
                    catch (BreakException ex)
                    {
                        return PrintingSubsystem.Resource(ex.Message);
                    }
                }
                return PrintingSubsystem.ToolNotVisible();
            }
            
            return PrintingSubsystem.ItemNotVisible();
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
                return PrintingSubsystem.ImpossibleUnlock(item);
            }
            
            // lets have a look at surroundings.
            var itemKey = this.objectHandler.GetItemKeyByName(unlockObject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                return PrintingSubsystem.Resource(BaseDescriptions.IMPOSSIBLE_UNLOCK_SURROUNDINGS);
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
                                    return PrintingSubsystem.Resource(e.Message);
                                }
                                
                            }
                        }
                        return PrintingSubsystem.ItemAlreadyUnlocked(item);
                    }
                    return PrintingSubsystem.ItemNotLockAble(item);
                }
                return PrintingSubsystem.KeyNotVisible();
            }

            // lets have a look at surroundings.
            var itemKey = this.objectHandler.GetItemKeyByName(unlockObject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                if (key != default)
                {
                    // surroundings only exist within the active location.
                    this.universe.ActiveLocation.OnUnlock(new UnlockContainerEventArgs {Key = key, ExternalItemKey = itemKey});
                    return true;
                }
                return PrintingSubsystem.KeyNotVisible();
            }
            
            return PrintingSubsystem.ItemNotVisible();
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
                        result = result && PrintingSubsystem.ImpossiblePickup(character);
                    }
                    else
                    {
                        // lets have a look at surroundings.
                        var itemKey = this.objectHandler.GetItemKeyByName(subject);
                        if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
                        {
                            try
                            {
                                this.universe.ActiveLocation.OnTake(new ContainerObjectEventArgs() {ExternalItemKey = itemKey});
                            }
                            catch (Exception e)
                            {
                                result = result && PrintingSubsystem.Resource(e.Message);
                            }
                        }
                        else
                        {
                            result = result && PrintingSubsystem.ItemNotVisible();    
                        }
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
                        result = result && PrintingSubsystem.Resource(ex.Message);
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
                        result = result && PrintingSubsystem.Resource(ex.Message);
                    }
                }

                return result;
            }
            else
            {
                return PrintingSubsystem.NothingToTake();
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
                return PrintingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
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
                                PrintingSubsystem.ItemDropSuccess(item);
                                item.OnAfterDrop(new DropItemEventArgs());
                            }
                            else
                            {
                                PrintingSubsystem.ImpossibleDrop(item);
                            }
                        }
                        catch (DropException e)
                        {
                            PrintingSubsystem.Resource(e.Message);
                        }
                    }
                    else
                    {
                        PrintingSubsystem.ImpossibleDrop(item);
                        result = result && true;
                    }
                }
                else
                {
                    PrintingSubsystem.ItemNotOwned();
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
                                        PrintingSubsystem.ItemDropSuccess(itemToDrop, itemContainer);
                                        itemContainer.OnAfterDrop(new DropItemEventArgs() {ItemContainer = itemContainer});
                                        return true;
                                    }

                                    return PrintingSubsystem.ImpossibleDrop(itemContainer);
                                }
                                catch (DropException e)
                                {
                                    return PrintingSubsystem.Resource(e.Message);
                                }
                            }
                            
                            return PrintingSubsystem.ItemStillClosed(itemContainer);
                        }
                        
                        return PrintingSubsystem.ItemIsNotAContainer(itemContainer);
                    }
                }

                return PrintingSubsystem.ImpossibleDrop(itemToDrop);
            }
            return PrintingSubsystem.ItemNotOwned();
        }
        
        return false;
    }

    internal bool Name(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.NAME].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            this.universe.ActivePlayer.Name = subject;
            this.universe.ActivePlayer.IsStranger = false;
            PrintingSubsystem.ActivePlayer(this.universe.ActivePlayer);

            return true;
        }

        return false;
    }

    internal bool Inventory(string input)
    {
        if (this.universe.VerbResources[VerbKeys.INV].Contains(input, StringComparer.InvariantCultureIgnoreCase))
        {
            PrintingSubsystem.ActivePlayer(this.universe.ActivePlayer);
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
                PrintingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
                return;
            }

            if (this.universe.ActivePlayer.IsSitting && this.universe.ActivePlayer.Seat != null)    
            {
                PrintingSubsystem.Resource(BaseDescriptions.ALREADY_SITTING);
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

                            PrintingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
                        }
                    }

                    //TODO - maybe OnUnsuccessfulLocationChange... 
                    var status = this.universe.ActiveLocation.OnAfterChangeLocation(new ChangeLocationEventArgs(newLocationMap));

                    if (status == ChangeLocationStatus.IsLocked)
                    {
                        PrintingSubsystem.WayIsLocked(newLocationMap.Location);
                    }
                    else if (status == ChangeLocationStatus.IsClosed)
                    {
                        PrintingSubsystem.WayIsClosed(newLocationMap.Location);
                    }
                }
                catch (BeforeChangeLocationException ex)
                {
                    PrintingSubsystem.Resource(ex.Message);
                }
            }
            else
            {
                PrintingSubsystem.Resource(BaseDescriptions.NO_WAY);
            }
        }
        else
        {
            PrintingSubsystem.Resource(BaseDescriptions.NO_WAY);
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
                    return PrintingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
                }
                
                if (this.universe.ActivePlayer.IsSitting && this.universe.ActivePlayer.Seat != null)    
                {
                    return PrintingSubsystem.Resource(BaseDescriptions.ALREADY_SITTING);
                }
                
                var mappings = this.universe.LocationMap[this.universe.ActiveLocation];
                var newLocation = mappings.Where(i => !i.IsHidden).SingleOrDefault(x => x.Location.Key == locationKey);
                if (newLocation != default)
                {
                    if (!newLocation.Location.IsLocked)
                    {
                        this.universe.ActiveLocation = newLocation.Location;
                        this.objectHandler.ClearActiveObjectIfNotInInventory();
                        return PrintingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
                    }

                    return PrintingSubsystem.Resource(BaseDescriptions.ROOM_LOCKDESCRIPTION);
                }
            }

            return PrintingSubsystem.Resource(BaseDescriptions.NO_WAY);
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
}
