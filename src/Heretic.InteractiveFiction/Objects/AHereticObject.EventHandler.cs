using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public abstract partial class AHereticObject
{
    public event EventHandler<BreakItemEventArgs> BeforeBreak;
    public event EventHandler<BreakItemEventArgs> Break;
    public event EventHandler<BreakItemEventArgs> AfterBreak;

    public event EventHandler<ContainerObjectEventArgs> BeforeClimb;
    public event EventHandler<ContainerObjectEventArgs> Climb;
    public event EventHandler<ContainerObjectEventArgs> AfterClimb;

    public event EventHandler<ContainerObjectEventArgs> BeforeClose;
    public event EventHandler<ContainerObjectEventArgs> Close;
    public event EventHandler<ContainerObjectEventArgs> AfterClose;

    public event EventHandler<ContainerObjectEventArgs> BeforeDrink;
    public event EventHandler<ContainerObjectEventArgs> Drink;
    public event EventHandler<ContainerObjectEventArgs> AfterDrink;
    
    public event EventHandler<DropItemEventArgs> BeforeDrop;
    public event EventHandler<DropItemEventArgs> Drop;
    public event EventHandler<DropItemEventArgs> AfterDrop;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeEat;
    public event EventHandler<ContainerObjectEventArgs> Eat;
    public event EventHandler<ContainerObjectEventArgs> AfterEat;

    public event EventHandler<ContainerObjectEventArgs> BeforeGive;
    public event EventHandler<ContainerObjectEventArgs> Give;
    public event EventHandler<ContainerObjectEventArgs> AfterGive;

    public event EventHandler<ContainerObjectEventArgs> BeforeOpen;
    public event EventHandler<ContainerObjectEventArgs> Open;
    public event EventHandler<ContainerObjectEventArgs> AfterOpen;

    public event EventHandler<ReadItemEventArgs> BeforeRead;
    public event EventHandler<ReadItemEventArgs> Read;
    public event EventHandler<ReadItemEventArgs> AfterRead;

    public event EventHandler<LockContainerEventArgs> BeforeLock;
    public event EventHandler<LockContainerEventArgs> Lock;
    public event EventHandler<LockContainerEventArgs> AfterLock;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeLook;
    public event EventHandler<ContainerObjectEventArgs> Look;
    public event EventHandler<ContainerObjectEventArgs> AfterLook;

    public event EventHandler<ContainerObjectEventArgs> BeforeTake;
    public event EventHandler<ContainerObjectEventArgs> Take;
    public event EventHandler<ContainerObjectEventArgs> AfterTake;

    public event EventHandler<SitDownEventArgs> BeforeSitDown;
    public event EventHandler<SitDownEventArgs> SitDown;
    public event EventHandler<SitDownEventArgs> AfterSitDown;

    public event EventHandler<ContainerObjectEventArgs> BeforeStandUp;
    public event EventHandler<ContainerObjectEventArgs> StandUp;
    public event EventHandler<ContainerObjectEventArgs> AfterStandUp;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeDescend;
    public event EventHandler<ContainerObjectEventArgs> Descend;
    public event EventHandler<ContainerObjectEventArgs> AfterDescend;
    
    public event EventHandler<LockContainerEventArgs> BeforeUnlock;
    public event EventHandler<LockContainerEventArgs> Unlock;
    public event EventHandler<LockContainerEventArgs> AfterUnlock;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeWear;
    public event EventHandler<ContainerObjectEventArgs> Wear;
    public event EventHandler<ContainerObjectEventArgs> AfterWear;
    
    public event EventHandler<PutOnEventArgs> BeforePutOn;
    public event EventHandler<PutOnEventArgs> PutOn;
    public event EventHandler<PutOnEventArgs> AfterPutOn;

    public event EventHandler<ContainerObjectEventArgs> Buy;
    public event EventHandler<CutItemEventArgs> Cut;
    public event EventHandler<ContainerObjectEventArgs> Jump;
    public event EventHandler<KindleItemEventArgs> Kindle;
    public event EventHandler<PullItemEventArgs> Pull;
    public event EventHandler<PushItemEventArgs> Push;
    public event EventHandler<ContainerObjectEventArgs> Sleep;
    public event EventHandler<ContainerObjectEventArgs> Smell;
    public event EventHandler<ContainerObjectEventArgs> Taste;
    public event EventHandler<ContainerObjectEventArgs> ToBe;
    public event EventHandler<TurnItemEventArgs> Turn;
    public event EventHandler<UseItemEventArgs> Use;
    public event EventHandler<ContainerObjectEventArgs> Wait;
    public event EventHandler<WriteEventArgs> Write;

    public virtual void OnUse(UseItemEventArgs eventArgs)
    {
        var localEventHandler = this.Use;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new UseException(eventArgs.OptionalErrorMessage);    
            }
            throw new UseException(BaseDescriptions.NOTHING_HAPPENS);
        }
    }
    
    public virtual void OnToBe(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.ToBe;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new ToBeException(eventArgs.OptionalErrorMessage);    
            }
            throw new ToBeException(BaseDescriptions.NOTHING_HAPPENS);
        }
    }
    
    public virtual void OnCut(CutItemEventArgs eventArgs)
    {
        var localEventHandler = this.Cut;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new CutException(eventArgs.OptionalErrorMessage);
            }
            
            var itemName = ArticleHandler.GetNameWithArticleForObject(this, GrammarCase.Accusative, lowerFirstCharacter: true);
            
            if (eventArgs.ItemToUse != default)
            {
                var itemToUseName = ArticleHandler.GetNameWithArticleForObject(eventArgs.ItemToUse, GrammarCase.Dative, lowerFirstCharacter: true);
                throw new CutException(string.Format(BaseDescriptions.IMPOSSIBLE_CUT, itemName, itemToUseName));    
            }
            
            throw new CutException(string.Format(BaseDescriptions.IMPOSSIBLE_CUT_WITHOUT_TOOL, itemName));
        }
    }
    
    public virtual void OnJump(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Jump;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new JumpException(eventArgs.OptionalErrorMessage);
            }
            throw new JumpException(BaseDescriptions.DOES_NOT_WORK);
        }
    }
    
    public virtual void OnKindle(KindleItemEventArgs eventArgs)
    {
        var localEventHandler = this.Kindle;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new KindleException(eventArgs.OptionalErrorMessage);
            }
            throw new KindleException(BaseDescriptions.DOES_NOT_WORK);
        }
    }
    
    public virtual void OnWait(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Wait;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new WaitException(eventArgs.OptionalErrorMessage);
            }
            throw new WaitException(BaseDescriptions.TIME_GOES_BY);
        }
    }

    public virtual void OnPull(PullItemEventArgs eventArgs)
    {
        var localEventHandler = this.Pull;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new PullException(eventArgs.OptionalErrorMessage);
            }
            throw new PullException(string.Format(BaseDescriptions.PULL_DOES_NOT_WORK, ArticleHandler.GetNameWithArticleForObject(this, GrammarCase.Dative, lowerFirstCharacter: true)));
        }
    }

    public virtual void OnPush(PushItemEventArgs eventArgs)
    {
        var localEventHandler = this.Push;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new PushException(eventArgs.OptionalErrorMessage);
            }
            throw new PushException(BaseDescriptions.DOES_NOT_WORK);
        }
    }
    
    public virtual void OnBeforePutOn(PutOnEventArgs eventArgs)
    {
        var localEventHandler = this.BeforePutOn;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnPutOn(PutOnEventArgs eventArgs)
    {
        var localEventHandler = this.PutOn;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterPutOn(PutOnEventArgs eventArgs)
    {
        var localEventHandler = this.AfterPutOn;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnWrite(WriteEventArgs eventArgs)
    {
        var localEventHandler = this.Write;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new WriteException(eventArgs.OptionalErrorMessage);    
            }
            throw new WriteException(BaseDescriptions.NOTHING_HAPPENS);
        }
    }
    
    public virtual void OnBuy(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Buy;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new BuyException(eventArgs.OptionalErrorMessage);
            }
            throw new BuyException(BaseDescriptions.ON_BUY_EXCEPTION);
        }
    }

    public virtual void OnBeforeClimb(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeClimb;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnClimb(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Climb;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterClimb(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterClimb;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeDescend(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeDescend;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnDescend(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Descend;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterDescend(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterDescend;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeDrop(DropItemEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeDrop;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnDrop(DropItemEventArgs eventArgs)
    {
        var localEventHandler = this.Drop;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterDrop(DropItemEventArgs eventArgs)
    {
        var localEventHandler = this.AfterDrop;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeBreak(BreakItemEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeBreak;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBreak(BreakItemEventArgs eventArgs)
    {
        var localEventHandler = this.Break;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterBreak(BreakItemEventArgs eventArgs)
    {
        var localEventHandler = this.AfterBreak;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBeforeEat(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeEat;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnEat(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Eat;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterEat(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterEat;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBeforeDrink(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeDrink;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnDrink(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Drink;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterDrink(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterDrink;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnSleep(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Sleep;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new SleepException(eventArgs.OptionalErrorMessage);    
            }
            throw new SleepException(BaseDescriptions.NOT_TIRED);
        }
    }
    
    public virtual void OnSmell(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Smell;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new SmellException(eventArgs.OptionalErrorMessage);    
            }

            if (this is Item)
            {
                throw new SmellException(string.Format(BaseDescriptions.ITEM_DOES_NOT_SMELL, ArticleHandler.GetNameWithArticleForObject(this, GrammarCase.Dative, lowerFirstCharacter: true)));    
            }
            
            throw new SmellException(BaseDescriptions.NOTHING_TO_SMELL);
        }
    }
    
    public virtual void OnTaste(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Taste;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new TasteException(eventArgs.OptionalErrorMessage);    
            }
            throw new TasteException(BaseDescriptions.NOTHING_TO_TASTE);
        }
    }

    public virtual void OnTurn(TurnItemEventArgs eventArgs)
    {
        var localEventHandler = this.Turn;
        if (localEventHandler != null)
        {
            localEventHandler.Invoke(this, eventArgs);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(eventArgs.OptionalErrorMessage))
            {
                throw new TurnException(eventArgs.OptionalErrorMessage);    
            }
            throw new TurnException(BaseDescriptions.DOES_NOT_WORK);
        }
    }

    public virtual void OnBeforeSitDown(SitDownEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeSitDown;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnSitDown(SitDownEventArgs eventArgs)
    {
        var localEventHandler = this.SitDown;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterSitDown(SitDownEventArgs eventArgs)
    {
        var localEventHandler = this.AfterSitDown;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeStandUp(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeStandUp;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnStandUp(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.StandUp;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterStandUp(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterStandUp;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeGive(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeGive;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnGive(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Give;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterGive(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterGive;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeLook(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeLook;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnLook(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Look;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterLook(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterLook;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeTake(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeTake;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnTake(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Take;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterTake(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterTake;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeClose(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeClose;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnClose(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Close;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterClose(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterClose;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBeforeOpen(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeOpen;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnOpen(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Open;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterOpen(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterOpen;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeRead(ReadItemEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeRead;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnRead(ReadItemEventArgs eventArgs)
    {
        var localEventHandler = this.Read;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterRead(ReadItemEventArgs eventArgs)
    {
        var localEventHandler = this.AfterRead;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnBeforeLock(LockContainerEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeLock;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnLock(LockContainerEventArgs eventArgs)
    {
        var localEventHandler = this.Lock;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterLock(LockContainerEventArgs eventArgs)
    {
        var localEventHandler = this.AfterLock;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBeforeUnlock(LockContainerEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeUnlock;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnUnlock(LockContainerEventArgs eventArgs)
    {
        var localEventHandler = this.Unlock;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnAfterUnlock(LockContainerEventArgs eventArgs)
    {
        var localEventHandler = this.AfterUnlock;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public virtual void OnBeforeWear(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeWear;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnWear(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.Wear;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public virtual void OnAfterWear(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterWear;
        localEventHandler?.Invoke(this, eventArgs);
    }
}