using System.Collections.Specialized;
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

    internal VerbHandler(Universe universe, IPrintingSubsystem printingSubsystem)
    {
        this.PrintingSubsystem = printingSubsystem;
        this.universe = universe;
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
            var item = this.GetUnhiddenObjectByName(subject);
            if (item != default)
            {
                this.universe.UnveilFirstLevelObjects(item);
                var result = PrintingSubsystem.PrintObject(item);

                item.OnAfterLook(new ContainerObjectEventArgs());

                return result;
            }

            // lets have a look at surroundings.
            var itemKey = this.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                var result = PrintingSubsystem.Resource(this.universe.ActiveLocation.Surroundings[itemKey]);
                
                // surroundings only exist within the active location.
                this.universe.ActiveLocation.OnAfterLook(new ContainerObjectEventArgs() {ExternalItemKey = itemKey});

                return result;
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Pull(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.PULL].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.GetUnhiddenObjectByName(subject);
            if (item != default)
            {
                try
                {
                    item.OnPull(new ContainerObjectEventArgs());

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

    internal bool AlterEgo(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.ALTER_EGO].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.GetUnhiddenObjectByName(subject);
            if (item != default)
            {
                var result = PrintingSubsystem.AlterEgo(item);
                return result;
            }

            var itemKey = this.GetItemKeyByName(subject);
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
            this.universe.ActiveLocation.OnWrite(new WriteEventArgs() {Text = text});
            return true;
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
            var key = this.GetItemKeyByName(subject);
            var item = this.universe.ActivePlayer.GetUnhiddenItemByKey(key);

            if (item != default)
            {
                // if (!universe.AreLocationPostProcessingEventsAvailable(Keys.USE, item) && !universe.AreItemPostProcessingEventsAvailable(Keys.USE, item))
                // {
                //     return PrintingSubsystem.NoEvent();
                // }

                return true;
            }

            return PrintingSubsystem.ItemNotOwned();
        }

        return false;
    }

    internal bool Use(string verb, string subjectName, string objectName)
    {
        if (this.universe.VerbResources[VerbKeys.USE].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var subject = this.GetUnhiddenObjectByName(subjectName);

            if (subject == default)
            {
                return PrintingSubsystem.CanNotUseObject(subjectName);
            }

            var item = this.GetUnhiddenObjectByName(objectName);

            if (item == default)
            {
                return PrintingSubsystem.CanNotUseObject(objectName);
            }

            try
            {
                subject.OnUse(new UseItemEventArg() {ItemToUse = item});

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

            var key = this.GetItemKeyByName(subject);
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

    internal bool Turn(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.TURN].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.GetUnhiddenItemByNameActive(subject);

            if (item != default)
            {
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

    internal bool Close(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.CLOSE].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.GetUnhiddenItemByNameActive(subject);

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
                    var result = PrintingSubsystem.ItemClosed(item);

                    item.OnAfterClose(new ContainerObjectEventArgs());

                    return result;
                }

                return false;
            }

            return PrintingSubsystem.ItemNotVisible();
        }

        return false;
    }

    internal bool Open(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.OPEN].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.GetUnhiddenItemByNameActive(subject);

            if (item != default)
            {
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

                    item.IsClosed = false;
                    var result = PrintingSubsystem.ItemOpen(item);

                    item.OnAfterOpen(new ContainerObjectEventArgs());

                    return result;
                }

                return false;
            }
            
            // lets have a look at surroundings.
            var itemKey = this.GetItemKeyByName(subject);
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
            var character = this.GetUnhiddenCharacterByNameFromActiveLocation(subject);

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
            var key = this.GetItemKeyByName(subject);
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
                return PrintingSubsystem.Resource(BaseDescriptions.NO_SEAT);
            }
            
            if (seatCount == 1)
            {
                var onlySeat = this.universe.ActiveLocation.Items.First(x => x.IsSeatAble);
                
                this.universe.ActivePlayer.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = onlySeat});
                onlySeat.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                this.universe.ActivePlayer.SitDown(onlySeat);
                var result = PrintingSubsystem.FormattedResource(BaseDescriptions.ITEM_ONLY_SEAT, onlySeat.Name);
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
            var key = this.GetItemKeyByName(subject);
            var item = this.universe.ActiveLocation.GetUnhiddenItemByKey(key);

            if (item == default)
            {
                item = this.universe.ActivePlayer.GetUnhiddenItemByKey(key);
            }

            if (item != default)
            {
                if (item.IsSeatAble)
                {
                    this.universe.ActivePlayer.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = item});
                    item.OnBeforeSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                    this.universe.ActivePlayer.SitDown(item);
                    var result = PrintingSubsystem.ItemSeated(item);
                    item.OnAfterSitDown(new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer});
                    this.universe.ActivePlayer.OnAfterSitDown(new SitDownEventArgs {ItemToSitOn = item});
                    
                    return result;
                }    
                return PrintingSubsystem.ItemNotSeatable(item);
            }
            
            // lets have a look at surroundings.
            var itemKey = this.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                return PrintingSubsystem.Resource(BaseDescriptions.SURROUNDING_NOT_SEATABLE);
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
    
    internal bool Ask(string verb, string characterName, string subjectName)
    {
        if (this.universe.VerbResources[VerbKeys.ASK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            //I can only speak to visible people in the active location
            var character = this.GetUnhiddenCharacterByNameFromActiveLocation(characterName);

            if (character == default)
            {
                return PrintingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            //but I can speak about every unhidden or virtual item or character in the world
            var item = this.GetUnhiddenObjectFromWorldByName(subjectName);
            if (item == default)
            {
                item = this.GetUnhiddenCharacterByName(subjectName);
                if (item == default)
                {
                    item = this.GetVirtualItemByName(subjectName);
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
            var character = this.GetUnhiddenCharacterByNameFromActiveLocation(characterName);
            if (character == default)
            {
                return PrintingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            var key = this.GetConversationAnswerKeyByPhrase(phrase);
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
            var character = this.GetUnhiddenCharacterByNameFromActiveLocation(characterName);
            if (character == default)
            {
                return PrintingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            //...and I can give only items that i own.
            var key = this.GetItemKeyByName(itemName);
            var item = this.universe.ActivePlayer.GetItemByKey(key);
            if (item == default)
            {
                return PrintingSubsystem.ItemNotOwned();
            }

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

    private string GetConversationAnswerKeyByPhrase(string phrase)
    {
        foreach (var (key, value) in this.universe.ConversationAnswersResources)
        {
            if (value.Contains(phrase, StringComparer.InvariantCultureIgnoreCase))
            {
                return key;
            }
        }

        return string.Empty;
    }
    
    internal bool Break(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.BREAK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.GetUnhiddenItemByNameActive(subject);
            if (item != default)
            {
                try
                {
                    item.OnBreak(new BreakItemEventArg());
                    return true;
                }
                catch (BreakException ex)
                {
                    return PrintingSubsystem.Resource(ex.Message);
                }
            }
            
            // lets have a look at surroundings.
            var itemKey = this.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                try
                {
                    this.universe.ActiveLocation.OnBreak(new BreakItemEventArg());
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
            var item = this.GetUnhiddenItemByNameActive(subject);
            var toolItem = this.GetUnhiddenItemByNameActive(tool);

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
                                item.OnBreak(new BreakItemEventArg() { ItemToUse = toolItem });
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
            var itemKey = this.GetItemKeyByName(subject);
            if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
            {
                if (toolItem != default)
                {
                    // surroundings only exist within the active location.
                    try
                    {
                        this.universe.ActiveLocation.OnBreak(new BreakItemEventArg() { ItemToUse = toolItem });
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
        if (this.universe.VerbResources[VerbKeys.UNLOCK].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            var item = this.GetUnhiddenItemByNameActive(unlockObject);
            if (item != default)
            {
                return PrintingSubsystem.ImpossibleUnlock(item);
            }
            
            // lets have a look at surroundings.
            var itemKey = this.GetItemKeyByName(unlockObject);
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
            var item = this.GetUnhiddenItemByNameActive(unlockObject);
            var key = this.GetUnhiddenItemByNameActive(unlockKey);

            if (item != default)
            {
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
            var itemKey = this.GetItemKeyByName(unlockObject);
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
                var item = this.GetUnhiddenItemByNameActive(subject);
                if (item == default)
                {
                    var characterKey = this.GetCharacterKeyByName(subject);
                    var character = this.universe.ActiveLocation.GetCharacterByKey(characterKey);
                    if (character != default)
                    {
                        result = result &&
                                 PrintingSubsystem.ImpossiblePickup(character);
                    }
                    else
                    {
                        var itemKey = this.GetItemKeyByName(subject);
                        if (!string.IsNullOrEmpty(itemKey) && this.universe.ActiveLocation.Surroundings.Any(x => x.Key == itemKey))
                        {
                            result = result && PrintingSubsystem.Resource(BaseDescriptions.IMPOSSIBLE_PICKUP);
                        }
                        else
                        {
                            result = result && PrintingSubsystem.ItemNotVisible();    
                        }
                    }
                }
                else
                {
                    result = result && universe.PickObject(item);
                    item.OnAfterTake(new ContainerObjectEventArgs());
                }
            }
        }

        return result;
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
                    result = result && universe.PickObject(item);
                    item.OnAfterTake(new ContainerObjectEventArgs());
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

    internal bool Go(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.GO].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            return this.ChangeLocationByName(subject);
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
                var key = this.GetItemKeyByName(subject);

                if (this.universe.ActivePlayer.Items.Any(x => x.Key == key))
                {
                    var item = this.universe.ActivePlayer.Items.Single(i => i.Key == key);

                    try
                    {
                        item.OnBeforeDrop(new ContainerObjectEventArgs());

                        var singleDropResult = this.universe.ActivePlayer.RemoveItem(item);
                        result = result && singleDropResult;
                        if (singleDropResult)
                        {
                            this.universe.ActiveLocation.Items.Add(item);
                            PrintingSubsystem.ItemDropSuccess(item);
                            item.OnAfterDrop(new ContainerObjectEventArgs());
                        }
                        else
                        {
                            PrintingSubsystem.ImpossibleDrop(item);
                        }
                    }
                    catch (BeforeDropException e)
                    {
                        PrintingSubsystem.Resource(e.Message);
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

    internal bool Name(string verb, string subject)
    {
        if (this.universe.VerbResources[VerbKeys.NAME].Contains(verb, StringComparer.InvariantCultureIgnoreCase))
        {
            this.universe.ActivePlayer.Name = subject;
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
        var locationKey = this.GetLocationKeyByName(input);
        if (!string.IsNullOrEmpty(locationKey))
        {
            if (this.universe.LocationMap.ContainsKey(this.universe.ActiveLocation))
            {
                var mappings = this.universe.LocationMap[this.universe.ActiveLocation];
                var newLocation = mappings.Where(i => !i.IsHidden).SingleOrDefault(x => x.Location.Key == locationKey);
                if (newLocation != default)
                {
                    if (!newLocation.Location.IsLocked)
                    {
                        this.universe.ActiveLocation = newLocation.Location;
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

    private Character GetUnhiddenCharacterByKeyFromActiveLocation(string key)
    {
        return this.universe.ActiveLocation.GetUnhiddenCharacterByKey(key);
    }

    private Character GetUnhiddenCharacterByKey(string key)
    {
        if (this.universe.GetObjectFromWorldByKey(key) is Character { IsHidden: false } character)
        {
            return character;
        }

        return default;
    }

    private Item GetUnhiddenItemByNameActive(string itemName)
    {
        return this.GetUnhiddenItemByKeyActive(this.GetItemKeyByName(itemName));
    }

    private AContainerObject GetUnhiddenObjectFromWorldByName(string itemName)
    {
        var item = this.universe.GetObjectFromWorldByKey(this.GetItemKeyByName(itemName));

        if (item == default || item.IsHidden)
        {
            return default;
        }

        return item;
    }

    private Item GetVirtualItemByName(string itemName)
    {
        return this.GetVirtualItemByKey(this.GetItemKeyByName(itemName));
    }

    private string GetItemKeyByName(string itemName)
    {
        var allActiveLocationItemKeys = this.GetItemKeysRecursive(this.universe.ActiveLocation.Items);
        var allActivePlayerItemKeys = this.GetItemKeysRecursive(this.universe.ActivePlayer.Items);
        var prioritizedKeysOfActiveLocationAndPlayer = allActiveLocationItemKeys.Union(allActivePlayerItemKeys).ToList();
        var prioritizedItemResources = this.universe.ItemResources.Where(x => prioritizedKeysOfActiveLocationAndPlayer.Contains(x.Key));
        
        foreach (var (key, value) in prioritizedItemResources)
        {
            if (value.Contains(itemName, StringComparer.InvariantCultureIgnoreCase))
            {
                return key;
            }
        }
        
        foreach (var (key, value) in this.universe.ItemResources)
        {
            if (value.Contains(itemName, StringComparer.InvariantCultureIgnoreCase))
            {
                return key;
            }
        }

        return string.Empty;
    }

    private IEnumerable<string> GetItemKeysRecursive(IEnumerable<Item> items)
    {
        var result = new List<string>();
        foreach (var item in items)
        {
            if (item.Items.Any())
            {
                result.AddRange(this.GetItemKeysRecursive(item.Items));
            }
            
            result.Add(item.Key);
        }

        return result;
    }

    private string GetLocationKeyByName(string locationName)
    {
        foreach (var (key, value) in this.universe.LocationResources)
        {
            if (value.Contains(locationName, StringComparer.InvariantCultureIgnoreCase))
            {
                return key;
            }
        }

        return string.Empty;
    }

    private Item GetUnhiddenItemByKeyActive(string key)
    {
        var result = this.universe.ActiveLocation.GetUnhiddenItemByKey(key);
        if (result == default)
        {
            result = this.universe.ActivePlayer.GetUnhiddenItemByKey(key);
        }

        return result;
    }

    private Item GetVirtualItemByKey(string key)
    {
        var result = this.universe.ActiveLocation.GetVirtualItemByKey(key);
        if (result == default)
        {
            result = this.universe.ActivePlayer.GetVirtualItemByKey(key);
        }

        return result;
    }

    private Character GetUnhiddenCharacterByNameFromActiveLocation(string itemName)
    {
        return this.GetUnhiddenCharacterByKeyFromActiveLocation(this.GetCharacterKeyByName(itemName));
    }

    private Character GetUnhiddenCharacterByName(string itemName)
    {
        return this.GetUnhiddenCharacterByKey(this.GetCharacterKeyByName(itemName));
    }

    private string GetCharacterKeyByName(string itemName)
    {
        foreach (var (key, value) in this.universe.CharacterResources)
        {
            if (value.Contains(itemName, StringComparer.InvariantCultureIgnoreCase))
            {
                return key;
            }
        }

        return string.Empty;
    }

    private AContainerObject GetUnhiddenObjectByName(string objectName)
    {
        AContainerObject containerObject = this.GetUnhiddenItemByNameActive(objectName);
        if (containerObject == default)
        {
            containerObject = this.GetUnhiddenCharacterByName(objectName);
        }

        if (containerObject == default)
        {
            var key = this.GetCharacterKeyByName(objectName);
            if (key == this.universe.ActivePlayer.Key)
            {
                containerObject = this.universe.ActivePlayer;
            }
        }

        return containerObject;
    }
}
