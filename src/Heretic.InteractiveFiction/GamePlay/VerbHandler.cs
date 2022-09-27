using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class VerbHandler
{
    private readonly Universe universe;
    private readonly IGrammar grammar;
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly ObjectHandler objectHandler;
    private bool isHintActive;

    internal VerbHandler(Universe universe, IGrammar grammar, IPrintingSubsystem printingSubsystem)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
        this.grammar = grammar;
        this.objectHandler = new ObjectHandler(universe);
        this.isHintActive = false;
    }

    internal bool Quit(string verb)
    {
        if (VerbKeys.QUIT == verb)
        {
            throw new QuitGameException(BaseDescriptions.QUIT_GAME);
        }

        return false;
    }

    internal bool Credits(string verb)
    {
        if (VerbKeys.CREDITS == verb)
        {
            return printingSubsystem.Credits();
        }

        return false;
    }
    
    internal bool Look(string verb)
    {
        if (VerbKeys.LOOK == verb)
        {
            try
            {
                var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                
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

    internal bool Look(string verb, string processingObject)
    {
        if (VerbKeys.LOOK == verb)
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(processingObject);
            if (item != default)
            {
                try
                {
                    this.objectHandler.StoreAsActiveObject(item);
                    
                    var eventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    var eventArgsForActiveLocation = new ContainerObjectEventArgs() { ExternalItemKey = item.Key, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };

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
        if (VerbKeys.LOOK == verb)
        {
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Look(verb, processingObject);
            }
        }

        return false;
    }
    
    internal bool Read(string verb, string processingObject)
    {
        if (VerbKeys.READ == verb)
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(processingObject);
            if (item != default)
            {
                if (item.IsReadable)
                {
                    try
                    {
                        this.objectHandler.StoreAsActiveObject(item);
                        
                        var readItemEventArgs = new ReadItemEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                        
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
    
    internal bool Eat(string verb, string processingObject)
    {
        if (VerbKeys.EAT == verb)
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(processingObject);
            if (item != default)
            {
                if (item.IsEatable)
                {
                    try
                    {
                        var itemEventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                        
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

                        return printingSubsystem.FormattedResource(BaseDescriptions.ITEM_EATEN, item.AccusativeArticleName, true);
                    }
                    catch (EatException ex)
                    {
                        return printingSubsystem.Resource(ex.Message);
                    }
                }

                this.objectHandler.StoreAsActiveObject(item);
                
                return printingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_EAT, item.AccusativeArticleName, true);
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool SwitchOn(string verb, string processingSubject, string processingObjects)
    {
        if (VerbKeys.SWITCHON == verb)
        {
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.SwitchOn(verb, processingObjects);
            }
        }

        return false;
    }
    
    internal bool SwitchOn(string verb, string processingObject)
    {
        if (VerbKeys.SWITCHON == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                if (item.IsSwitchable)
                {
                    if (!item.IsSwitchedOn)
                    {
                        try
                        {
                            var itemEventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                        
                            item.OnBeforeSwitchOn(itemEventArgs);

                            item.IsSwitchedOn = true;
                            item.OnSwitchOn(itemEventArgs);
                    
                            item.OnAfterSwitchOn(itemEventArgs);

                            return printingSubsystem.FormattedResource(BaseDescriptions.ITEM_SWITCHEDON, item.AccusativeArticleName, true);
                        }
                        catch (SwitchOnException ex)
                        {
                            return printingSubsystem.Resource(ex.Message);
                        }
                    }
                    
                    return printingSubsystem.FormattedResource(BaseDescriptions.ALREADY_SWITCHEDON, item.AccusativeArticleName, true);
                }

                return printingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_SWITCHON, item.AccusativeArticleName, true);
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }

    internal bool SwitchOff(string verb, string processingSubject, string processingObjects)
    {
        if (VerbKeys.SWITCHOFF == verb)
        {
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.SwitchOff(verb, processingObjects);
            }
        }

        return false;
    }
    
    internal bool SwitchOff(string verb, string processingObject)
    {
        if (VerbKeys.SWITCHOFF == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                if (item.IsSwitchable)
                {
                    if (item.IsSwitchedOn)
                    {
                        try
                        {
                            var itemEventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                        
                            item.OnBeforeSwitchOff(itemEventArgs);

                            item.IsSwitchedOn = false;
                            item.OnSwitchOff(itemEventArgs);
                    
                            item.OnAfterSwitchOff(itemEventArgs);

                            return printingSubsystem.FormattedResource(BaseDescriptions.ITEM_SWITCHEDOFF, item.AccusativeArticleName, true);
                        }
                        catch (SwitchOffException ex)
                        {
                            return printingSubsystem.Resource(ex.Message);
                        }
                    }
                    
                    return printingSubsystem.FormattedResource(BaseDescriptions.ALREADY_SWITCHEDOFF, item.AccusativeArticleName, true);
                }

                return printingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_SWITCHOFF, item.AccusativeArticleName, true);
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Wear(string verb, string processingSubject, string processingObjects)
    {
        if (VerbKeys.WEAR == verb)
        {
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Wear(verb, processingObjects);
            }
        }

        return false;
    }
    
    internal bool Wear(string verb, string processingObject)
    {
        void SwapItem(Item item)
        {
            this.universe.ActivePlayer.Items.Remove(item);
            this.universe.ActivePlayer.Clothes.Add(item);
        }
        
        if (VerbKeys.WEAR == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                if (item.IsWearable)
                {
                    if (!this.universe.ActivePlayer.Clothes.Contains(item))
                    {
                        try
                        {
                            var itemEventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                        
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

                            return printingSubsystem.FormattedResource(BaseDescriptions.PULLON_WEARABLE, item.AccusativeArticleName, true);
                        }
                        catch (WearException ex)
                        {
                            return printingSubsystem.Resource(ex.Message);
                        } 
                    }
                    printingSubsystem.Resource(BaseDescriptions.ALREADY_WEARING);
                }

                return printingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_WEAR, item.AccusativeArticleName, true);
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Drink(string verb, string processingObject)
    {
        if (VerbKeys.DRINK == verb)
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(processingObject);
            if (item != default)
            {
                if (item.IsDrinkable)
                {
                    try
                    {
                        var itemEventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                        
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

                        return printingSubsystem.FormattedResource(BaseDescriptions.ITEM_DRUNK, item.AccusativeArticleName, true);
                    }
                    catch (DrinkException ex)
                    {
                        return printingSubsystem.Resource(ex.Message);
                    }
                }

                this.objectHandler.StoreAsActiveObject(item);
                
                return printingSubsystem.FormattedResource(BaseDescriptions.NOTHING_TO_DRINK, item.AccusativeArticleName, true);
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Hint(string verb, string processingObject)
    {
        if (VerbKeys.HINT == verb)
        {
            if (String.Equals(processingObject, BaseDescriptions.ON, StringComparison.CurrentCultureIgnoreCase))
            {
                isHintActive = true;
                return printingSubsystem.Resource(BaseDescriptions.HINT_ON);
            }
            
            if (String.Equals(processingObject, BaseDescriptions.OFF, StringComparison.CurrentCultureIgnoreCase))
            {
                isHintActive = false;
                return printingSubsystem.Resource(BaseDescriptions.HINT_OFF);
            }
        }

        return false;
    }

    internal bool Pull(string verb, string processingObject)
    {
        if (VerbKeys.PULL == verb)
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(processingObject);
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var pullItemEventArgs = new PullItemEventArgs() {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
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

        return false;
    }
    
    internal bool Pull(string verb, string subjectName, string objectName)
    {
        if (VerbKeys.PULL == verb)
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
                var eventArgs = new PullItemEventArgs() { ItemToUse = item, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };
                
                subject.OnPull(eventArgs);
                
                return true;
            }
            catch (PullException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }
    
    internal bool Push(string verb, string processingObject)
    {
        if (VerbKeys.PUSH == verb)
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(processingObject);
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var pushItemEventArgs = new PushItemEventArgs() {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
                    item.OnPush(pushItemEventArgs);
                    
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
        if (VerbKeys.PUSH == verb)
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
                var pushItemEventArgs = new PushItemEventArgs() {ItemToUse = item, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
                subject.OnPush(pushItemEventArgs);
                
                return true;
            }
            catch (PushException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }


    internal bool AlterEgo(string verb, string processingObject)
    {
        if (VerbKeys.ALTER_EGO == verb)
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(processingObject);
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
        if (VerbKeys.WRITE == verb)
        {
            try
            {
                var writeEventArgs = new WriteEventArgs() {Text = text, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                
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

    internal bool Help(string verb)
    {
        if (VerbKeys.HELP == verb)
        {
            return printingSubsystem.Help(this.grammar.Verbs);
        }

        return false;
    }

    internal bool History(string verb, ICollection<string> historyCollection)
    {
        if (VerbKeys.HISTORY == verb)
        {
            return printingSubsystem.History(historyCollection);
        }

        return false;
    }
    
    internal bool Save(string verb, ICollection<string> historyCollection)
    {
        try
        {
            if (VerbKeys.SAVE == verb)
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
        if (VerbKeys.SCORE == verb)
        {
            return printingSubsystem.Score(universe.Score, universe.MaxScore);
        }

        return false;
    }

    internal bool Ways(string verb)
    {
        if (VerbKeys.WAYS == verb)
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

    internal bool Use(string verb, string processingObject)
    {
        if (VerbKeys.USE == verb)
        {
            var item = this.objectHandler.GetUnhiddenObjectByNameActive(processingObject);
            
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                try
                {
                    string optionalErrorMessage = string.Empty;
                    var errorMessage = this.GetVerb(verb).ErrorMessage;
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                       optionalErrorMessage = string.Format(errorMessage, item.DativeArticleName.LowerFirstChar()); 
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

        return false;
    }

    internal bool Use(string verb, string subjectName, string objectName)
    {
        if (VerbKeys.USE == verb)
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
                    var useItemEventArgs = new UseItemEventArgs() {ItemToUse = item, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
                    subject.OnUse(useItemEventArgs);

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

    internal bool Buy(string verb, string processingObject)
    {
        if (VerbKeys.BUY == verb)
        {
            if (!this.universe.ActivePlayer.HasPaymentMethod)
            {
                return printingSubsystem.PayWithWhat();
            }

            var key = this.objectHandler.GetItemKeyByName(processingObject);
            if (this.universe.ActivePlayer.GetUnhiddenItem(key) != default)
            {
                return printingSubsystem.ItemAlreadyOwned();
            }

            var item = this.universe.ActiveLocation.GetItem(key);
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    item.OnBuy(containerObjectEventArgs);
                    
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
        if (VerbKeys.BUY == verb)
        {
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Buy(verb, processingObject);
            }
        }

        return false;
    }

    internal bool Turn(string verb, string processingObject)
    {
        if (VerbKeys.TURN == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                try
                {
                    var turnItemEventArgs = new TurnItemEventArgs() {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
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

        return false;
    }
    
    internal bool Jump(string verb, string processingObject)
    {
        if (VerbKeys.JUMP == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
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

        return false;
    }
    
    internal bool Kindle(string verb, string processingObject)
    {
        if (VerbKeys.KINDLE == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var containerObjectEventArgs = new KindleItemEventArgs() {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
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

        return false;
    }
    
    internal bool Kindle(string verb, string subjectName, string objectName)
    {
        if (VerbKeys.KINDLE == verb)
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
                    var cutItemEventArgs = new KindleItemEventArgs {ItemToUse = item, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
                    subject.OnKindle(cutItemEventArgs);

                    return true;
                }
                catch (KindleException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.ItemNotOwned();
        }

        return false;
    }

    internal bool Cut(string verb, string processingObject)
    {
        if (VerbKeys.CUT == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var containerObjectEventArgs = new CutItemEventArgs {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
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

        return false;
    }
    
    internal bool Cut(string verb, string subjectName, string objectName)
    {
        if (VerbKeys.CUT == verb)
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
                    var cutItemEventArgs = new CutItemEventArgs() {ItemToUse = item, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
                    subject.OnCut(cutItemEventArgs);

                    return true;
                }
                catch (CutException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.ItemNotOwned();
        }

        return false;
    }

    internal bool Wait(string verb)
    {
        if (VerbKeys.WAIT == verb)
        {
            try
            {
                var containerObjectEventArgs = new ContainerObjectEventArgs()
                    { OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };

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
    
    internal bool Smell(string verb)
    {
        if (VerbKeys.SMELL == verb)
        {
            try
            {
                var containerObjectEventArgs = new ContainerObjectEventArgs()
                    { OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };

                this.universe.ActiveLocation.OnSmell(containerObjectEventArgs);

                return true;
            }
            catch (SmellException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }
    
    internal bool Smell(string verb, string processingObject)
    {
        if (VerbKeys.SMELL == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
                    item.OnSmell(containerObjectEventArgs);
                    
                    return true;
                }
                catch (SmellException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Sleep(string verb)
    {
        if (VerbKeys.SLEEP == verb)
        {
            try
            {
                var containerObjectEventArgs = new ContainerObjectEventArgs()
                    { OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };

                this.universe.ActiveLocation.OnSleep(containerObjectEventArgs);

                return true;
            }
            catch (SleepException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }
    
    internal bool Sleep(string verb, string processingObject)
    {
        if (VerbKeys.SLEEP == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
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

        return false;
    }
    
    internal bool Taste(string verb)
    {
        if (VerbKeys.TASTE == verb)
        {
            try
            {
                var containerObjectEventArgs = new ContainerObjectEventArgs {OptionalErrorMessage = BaseDescriptions.WHAT_TO_TASTE};
                
                if (!string.IsNullOrEmpty(this.GetVerb(verb).ErrorMessage))
                {
                    containerObjectEventArgs.OptionalErrorMessage = this.GetVerb(verb).ErrorMessage;
                }

                this.universe.ActiveLocation.OnTaste(containerObjectEventArgs);

                return true;
            }
            catch (TasteException ex)
            {
                return printingSubsystem.Resource(ex.Message);
            }
        }

        return false;
    }
    
    internal bool Taste(string verb, string processingObject)
    {
        if (VerbKeys.TASTE == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                
                try
                {
                    var containerObjectEventArgs = new ContainerObjectEventArgs() {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
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

        return false;
    }

    internal bool Climb(string verb, string processingObject)
    {
        if (VerbKeys.CLIMB == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
            this.objectHandler.StoreAsActiveObject(item);

            if (item != default)
            {
                if (item.IsClimbable)
                {
                    if (!this.universe.ActivePlayer.HasClimbed)
                    {
                        try
                        {
                            var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
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

                    return this.universe.ActivePlayer.ClimbedObject == item ? 
                        printingSubsystem.FormattedResource(BaseDescriptions.ALREADY_CLIMBED_ITEM, item.AccusativeArticleName.LowerFirstChar()) : 
                        printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
                }

                return printingSubsystem.Resource(BaseDescriptions.IMPOSSIBLE_CLIMB);
            }
            
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != null)
            {
                return printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }

    internal bool Climb(string verb, string processingSubject, string processingObject)
    {
        if (VerbKeys.CLIMB == verb)
        {
            
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingSubject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Climb(verb, processingObject);
            }
            
            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }
    
    internal bool Close(string verb, string processingObject)
    {
        if (VerbKeys.CLOSE == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
            this.objectHandler.StoreAsActiveObject(item);

            if (item != default)
            {
                if (item.IsCloseable)
                {
                    if (item.IsClosed)
                    {
                        return printingSubsystem.ItemAlreadyClosed(item);
                    }

                    try
                    {
                        var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
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

        return false;
    }

    internal bool Open(string verb, string processingObject)
    {
        if (VerbKeys.OPEN == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);

            if (item != default)
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
                        var containerObjectEventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                        
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

        return false;
    }

    internal bool Talk(string verb, string processingObject)
    {
        if (VerbKeys.TALK == verb)
        {
            var character = this.objectHandler.GetUnhiddenCharacterByNameFromActiveLocation(processingObject);

            if (character == default)
            {
                return printingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }

            try
            {
                var containerObjectEventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                
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

        return false;
    }

    internal bool SitDown(string verb)
    {
        if (VerbKeys.SIT == verb)
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
                    var sitDownEventArgs = new SitDownEventArgs {ItemToSitOn = onlySeat, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    var downEventArgs = new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
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
                    this.universe.ActivePlayer.StandUpFromSeat();
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.Resource(BaseDescriptions.MORE_SEATS);
        }
        
        return false;
    }
    
    internal bool SitDown(string verb, string processingObject)
    {
        if (VerbKeys.SIT == verb)
        {
            if (this.objectHandler.GetUnhiddenObjectByNameActive(processingObject) is { } player && player.Key == this.universe.ActivePlayer.Key)
            {
                return this.SitDown(verb);
            }
            
            var key = this.objectHandler.GetItemKeyByName(processingObject);
            var item = this.universe.ActiveLocation.GetUnhiddenItem(key) ?? this.universe.ActivePlayer.GetUnhiddenItem(key);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                if (item.IsSeatable)
                {
                    try
                    {
                        var sitDownEventArgs = new SitDownEventArgs {ItemToSitOn = item, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                        var downEventArgs = new SitDownEventArgs {ItemToSitOn = this.universe.ActivePlayer, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                        
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
        return false;
    }

    internal bool SitDown(string verb, string processingSubject, string processingObject)
    {
        if (VerbKeys.SIT == verb)
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
        if (VerbKeys.STANDUP == verb)
        {
            if (this.universe.ActivePlayer.IsSitting && this.universe.ActivePlayer.Seat != default)
            {
                var item = this.universe.ActivePlayer.Seat;
                try
                {
                    this.objectHandler.StoreAsActiveObject(item);
                
                    var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                
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
    
    internal bool Descend(string verb)
    {
        if (VerbKeys.DESCEND == verb)
        {
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != default)
            {
                var item = this.universe.ActivePlayer.ClimbedObject;
                try
                {
                    var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                
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

        return false;
    }
    
    internal bool Descend(string verb, string processingObject)
    {
        if (VerbKeys.DESCEND == verb)
        {
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != default)
            {
                var compareItem = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
                var item = this.universe.ActivePlayer.ClimbedObject;
                if (item.Key == compareItem.Key)
                {
                    try
                    {
                        var eventArgs = new ContainerObjectEventArgs() { OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };

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

                return printingSubsystem.FormattedResource(BaseDescriptions.NOT_CLIMBED_ON_ITEM, compareItem.DativeArticleName.LowerFirstChar());
            }
            
            return printingSubsystem.Resource(BaseDescriptions.NOT_CLIMBED);
        }

        return false;
    }
    
    internal bool Ask(string verb, string characterName, string subjectName)
    {
        if (VerbKeys.ASK == verb)
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
                var conversationEventArgs = new ConversationEventArgs() { Item = item, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };
                
                character.OnAsk(conversationEventArgs);
            }
            catch (AskException ex)
            {
                printingSubsystem.NoAnswerToQuestion(ex.Message);

            }

            return true;
        }

        return false;
    }

    internal bool Say(string verb, string characterName, string phrase)
    {
        if (VerbKeys.SAY == verb)
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
                var conversationEventArgs = new ConversationEventArgs { Phrase = key, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };
                
                character.OnSay(conversationEventArgs);
            }
            catch (SayException ex)
            {
                printingSubsystem.Resource(ex.Message);
            }

            return true;
        }

        return false;
    }

    internal bool Give(string verb, string characterName, string itemName)
    {
        if (VerbKeys.GIVE == verb)
        {
            //I can only give things to visible people.
            var character = this.objectHandler.GetUnhiddenCharacterByNameFromActiveLocation(characterName);
            if (character == default)
            {
                return printingSubsystem.Resource(BaseDescriptions.CHARACTER_NOT_VISIBLE);
            }
            
            //...and I can give only items that i own.
            var item = this.universe.ActivePlayer.GetUnhiddenItem(this.objectHandler.GetItemKeyByName(itemName));
            if (item == default)
            {
                return printingSubsystem.ItemNotOwned();
            }

            this.objectHandler.StoreAsActiveObject(item);

            try
            {
                var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                
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

    internal bool Break(string verb, string processingObject)
    {
        if (VerbKeys.BREAK == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
            
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                if (item.IsBreakable)
                {
                    if (!item.IsBroken)
                    {
                        try
                        {
                            var eventArgs = new BreakItemEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
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
    
    internal bool Break(string verb, string processingObject, string tool)
    {
        if (VerbKeys.BREAK == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
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
                                var eventArgs = new BreakItemEventArgs() { ItemToUse = toolItem, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };
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

    internal bool Unlock(string verb, string processingObject)
    {
        if (VerbKeys.UNLOCK == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                if (!string.IsNullOrEmpty(item.UnlockWithKey) && this.universe.ActivePlayer.OwnsItem(item.UnlockWithKey))
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
                                    var unlockContainerEventArgs = new LockContainerEventArgs { Key = key, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };
                                    
                                    item.OnBeforeUnlock(unlockContainerEventArgs);

                                    item.IsLocked = false;
                                    item.OnUnlock(unlockContainerEventArgs);
                                    printingSubsystem.Resource(string.Format(BaseDescriptions.ITEM_UNLOCKED_WITH_KEY_FROM_INVENTORY, key.AccusativeArticleName.LowerFirstChar(), item.Name));
                                    
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
        }

        return false;
    }

    internal bool Unlock(string verb, string processingObject, string unlockKey)
    {
        if (VerbKeys.UNLOCK == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
            var key = this.objectHandler.GetUnhiddenItemByNameActive(unlockKey);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                if (key != default)
                {
                    if (item.IsLockable)
                    {
                        if (item.IsLocked)
                        {
                            if (!item.IsCloseable || item.IsCloseable && item.IsClosed)
                            {
                                if (this.universe.ActivePlayer.OwnsItem(key))
                                {
                                    try
                                    {
                                        var unlockContainerEventArgs = new LockContainerEventArgs { Key = key, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };
                                    
                                        item.OnBeforeUnlock(unlockContainerEventArgs);

                                        if (!string.IsNullOrEmpty(item.UnlockWithKey) && item.UnlockWithKey == key.Key)
                                        {
                                            item.IsLocked = false;
                                            item.OnUnlock(unlockContainerEventArgs);
                                            printingSubsystem.ItemUnlocked(item);
                                        }
                                        else
                                        {
                                            item.OnUnlock(unlockContainerEventArgs);
                                            printingSubsystem.Resource(string.Format(BaseDescriptions.IMPOSSIBLE_UNLOCK_WITH_WRONG_KEY, item.Name, key.Name));
                                        }
                                    
                                        item.OnAfterUnlock(unlockContainerEventArgs);
                                    
                                        return true;
                                    }
                                    catch (UnlockException e)
                                    {
                                        return printingSubsystem.Resource(e.Message);
                                    }
                                }
                                printingSubsystem.ItemNotOwned(key);
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
    
    internal bool Lock(string verb, string processingObject)
    {
        if (VerbKeys.LOCK == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                if (!string.IsNullOrEmpty(item.UnlockWithKey) && this.universe.ActivePlayer.OwnsItem(item.UnlockWithKey))
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
                                    var lockContainerEventArgs = new LockContainerEventArgs { Key = key, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };
                                    
                                    item.OnBeforeLock(lockContainerEventArgs);

                                    item.IsLocked = true;
                                    item.OnLock(lockContainerEventArgs);
                                    printingSubsystem.Resource(string.Format(BaseDescriptions.ITEM_LOCKED_WITH_KEY_FROM_INVENTORY, key.AccusativeArticleName.LowerFirstChar(), item.Name));
                                    
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
        }

        return false;
    }

    internal bool Lock(string verb, string processingObject, string lockKey)
    {
        if (VerbKeys.LOCK == verb)
        {
            var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
            var key = this.objectHandler.GetUnhiddenItemByNameActive(lockKey);

            if (item != default)
            {
                this.objectHandler.StoreAsActiveObject(item);
                if (key != default)
                {
                    if (item.IsLockable)
                    {
                        if (!item.IsLocked)
                        {
                            if (!item.IsCloseable || item.IsCloseable && item.IsClosed)
                            {
                                if (this.universe.ActivePlayer.OwnsItem(key))
                                {
                                    try
                                    {
                                        var lockContainerEventArgs = new LockContainerEventArgs { Key = key, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };
                                    
                                        item.OnBeforeLock(lockContainerEventArgs);

                                        if (!string.IsNullOrEmpty(item.UnlockWithKey) && item.UnlockWithKey == key.Key)
                                        {
                                            item.IsLocked = true;
                                            item.OnLock(lockContainerEventArgs);
                                            printingSubsystem.ItemLocked(item);
                                        }
                                        else
                                        {
                                            item.OnLock(lockContainerEventArgs);
                                            printingSubsystem.Resource(string.Format(BaseDescriptions.IMPOSSIBLE_UNLOCK_WITH_WRONG_KEY, item.Name, key.Name));
                                        }
                                    
                                        item.OnAfterLock(lockContainerEventArgs);
                                    
                                        return true;
                                    }
                                    catch (LockException e)
                                    {
                                        return printingSubsystem.Resource(e.Message);
                                    }
                                }
                                printingSubsystem.ItemNotOwned(key);
                            }
                        }
                        return printingSubsystem.ItemAlreadyLocked(item);
                    }
                    return printingSubsystem.ItemNotLockAble(item);
                }
                return printingSubsystem.KeyNotVisible();
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }

    internal bool Take(string verb, IEnumerable<string> processingObjects)
    {
        if (VerbKeys.TAKE == verb)
        {
            var result = true;
            foreach (var processingObject in processingObjects)
            {
                var item = this.objectHandler.GetUnhiddenItemByNameActive(processingObject);
                if (item == default)
                {
                    var character = this.objectHandler.GetUnhiddenCharacterByNameFromActiveLocation(processingObject);
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

                        var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
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

            return result;
        }

        return false;
    }
    
    internal bool Take(string verb, string processingSubject, IEnumerable<string> processingObjects)
    {
        if (VerbKeys.TAKE == verb)
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
        if (VerbKeys.TAKE == verb)
        {
            var subjects = this.universe.ActiveLocation.GetAllPickableAndUnHiddenItems();
            if (subjects.Any())
            {
                var result = true;
                foreach (var item in subjects)
                {
                    try
                    {
                        var eventArgs = new ContainerObjectEventArgs(){OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                        
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
        if (VerbKeys.GO == verb)
        {
            if (this.universe.ActivePlayer.HasClimbed && this.universe.ActivePlayer.ClimbedObject != null)
            {
                return printingSubsystem.Resource(BaseDescriptions.ALREADY_CLIMBED);
            }
            
            return this.ChangeLocationByName(location);
        }

        return false;
    }

    internal bool Drop(string verb, IEnumerable<string> processingObjects)
    {
        if (VerbKeys.DROP == verb)
        {
            var result = true;
            foreach (var processingObject in processingObjects)
            {
                if (this.objectHandler.GetUnhiddenObjectByNameActive(processingObject) is { } player &&
                    player.Key == this.universe.ActivePlayer.Key)
                {
                    result = result && this.Sleep(VerbKeys.SLEEP);
                }
                else
                {
                    var key = this.objectHandler.GetItemKeyByName(processingObject);

                    var isPlayerItem = this.universe.ActivePlayer.Items.Any(x => x.Key == key);
                    var isPlayerCloths = this.universe.ActivePlayer.Clothes.Any(x => x.Key == key);
                    if (isPlayerItem || isPlayerCloths)
                    {
                        var item = isPlayerItem
                            ? this.universe.ActivePlayer.Items.Single(i => i.Key == key)
                            : this.universe.ActivePlayer.Clothes.Single(x => x.Key == key);

                        if (item.IsDropable)
                        {
                            try
                            {
                                var dropItemEventArgs = new DropItemEventArgs()
                                    { OptionalErrorMessage = this.GetVerb(verb).ErrorMessage };

                                item.OnBeforeDrop(dropItemEventArgs);

                                var singleDropResult = this.universe.ActivePlayer.RemoveItem(item);
                                result = result && singleDropResult;

                                if (singleDropResult)
                                {
                                    this.universe.ActiveLocation.Items.Add(item);

                                    item.OnDrop(dropItemEventArgs);
                                    printingSubsystem.ItemDropSuccess(item);

                                    item.OnAfterDrop(dropItemEventArgs);
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
                        }
                    }
                    else
                    {
                        printingSubsystem.ItemNotOwned();
                    }
                }
            }

            return result;
        }

        return false;
    }

    internal bool Drop(string verb, string subjectName, string objectName)
    {
        if (VerbKeys.DROP == verb)
        {
            if (this.objectHandler.GetUnhiddenObjectByNameActive(subjectName) is { } player &&
                player.Key == this.universe.ActivePlayer.Key)
            {
                return this.Sleep(VerbKeys.SLEEP, objectName);
            }
            
            var subjectKey = this.objectHandler.GetItemKeyByName(subjectName);
            
            if (this.universe.ActivePlayer.OwnsItem(subjectKey))
            {
                var itemToDrop = (Item)this.objectHandler.GetObjectFromWorldByKey(subjectKey);

                this.objectHandler.StoreAsActiveObject(itemToDrop);
                
                if (itemToDrop.IsDropable)
                {
                    var objectKey = this.objectHandler.GetItemKeyByName(objectName);

                    var isPlayerOwnerOfItem = this.universe.ActivePlayer.OwnsItem(objectKey);
                    var isActiveLocationOwnerOfItem = this.universe.ActiveLocation.OwnsItem(objectKey);

                    if (isPlayerOwnerOfItem || isActiveLocationOwnerOfItem)
                    {
                        var itemContainer = this.objectHandler.GetObjectFromWorldByKey(objectKey);
                        
                        if (itemContainer.IsContainer)
                        {
                            if (!itemContainer.IsCloseable || itemContainer.IsCloseable && !itemContainer.IsClosed)
                            {
                                try
                                {
                                    var dropItemEventArgs = new DropItemEventArgs() {ItemContainer = itemContainer, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                                    
                                    itemToDrop.OnBeforeDrop(dropItemEventArgs);

                                    var removeSuccess = this.universe.ActivePlayer.RemoveItem(itemToDrop);
                            
                                    if (removeSuccess)
                                    {
                                        itemContainer.Items.Add(itemToDrop);
                                        itemContainer.OnDrop(dropItemEventArgs);
                                        printingSubsystem.ItemDropSuccess(itemToDrop, itemContainer);
                                        
                                        itemContainer.OnAfterDrop(dropItemEventArgs);
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

    internal bool ToBe(string verb, string subjectName, string processingObject)
    {
        if (VerbKeys.TOBE == verb)
        {
            var subject = this.objectHandler.GetObjectFromWorldByName(subjectName);

            if (subject != default)
            {
                this.objectHandler.StoreAsActiveObject(subject);
                
                try
                {
                    var containerObjectEventArgs = new ContainerObjectEventArgs() {ExternalItemKey = processingObject, OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    subject.OnToBe(containerObjectEventArgs);
                    
                    return true;
                }
                catch (ToBeException ex)
                {
                    return printingSubsystem.Resource(ex.Message);
                }
            }

            return printingSubsystem.ItemNotVisible();
        }

        return false;
    }

    internal bool Inventory(string verb)
    {
        if (VerbKeys.INV == verb)
        {
            printingSubsystem.ActivePlayer(this.universe.ActivePlayer);
            return true;
        }

        return false;
    }

    private void ChangeLocation(string verb, Directions direction)
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
                    var changeLocationEventArgs = new ChangeLocationEventArgs(newLocationMap) {OptionalErrorMessage = this.GetVerb(verb).ErrorMessage};
                    
                    this.universe.ActiveLocation.OnBeforeChangeLocation(changeLocationEventArgs);
                    if (!newLocationMap.Location.IsLocked)
                    {
                        if (!newLocationMap.Location.IsClosed)
                        {
                            this.universe.ActiveLocation = newLocationMap.Location;
                            this.objectHandler.ClearActiveObjectIfNotInInventory();
                            this.universe.ActiveLocation.OnChangeLocation(changeLocationEventArgs);

                            printingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
                        }
                    }

                    //TODO - maybe OnUnsuccessfulLocationChange... 
                    var status = this.universe.ActiveLocation.OnAfterChangeLocation(changeLocationEventArgs);

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

        if (VerbKeys.E == verb)
        {
            this.ChangeLocation(verb, Objects.Directions.E);
        }
        else if (VerbKeys.W == verb)
        {
            this.ChangeLocation(verb,Objects.Directions.W);
        }
        else if (VerbKeys.N == verb)
        {
            this.ChangeLocation(verb,Objects.Directions.N);
        }
        else if (VerbKeys.S == verb)
        {
            this.ChangeLocation(verb,Objects.Directions.S);
        }
        else if (VerbKeys.SE == verb)
        {
            this.ChangeLocation(verb,Objects.Directions.SE);
        }
        else if (VerbKeys.SW == verb)
        {
            this.ChangeLocation(verb,Objects.Directions.SW);
        }
        else if (VerbKeys.NE == verb)
        {
            this.ChangeLocation(verb,Objects.Directions.NE);
        }
        else if (VerbKeys.NW == verb)
        {
            this.ChangeLocation(verb,Objects.Directions.NW);
        }
        else if (VerbKeys.UP == verb)
        {
            this.ChangeLocation(verb,Objects.Directions.UP);
        }
        else if (VerbKeys.DOWN == verb)
        {
            this.ChangeLocation(verb,Objects.Directions.DOWN);
        }
        else
        {
            result = false;
        }

        return result;
    }

    private Verb GetVerb(string word)
    {
        return this.grammar.GetVerb(word, this.universe.ActiveLocation);
    }
}
