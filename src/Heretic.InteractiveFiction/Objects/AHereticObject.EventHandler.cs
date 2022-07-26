using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public partial class AHereticObject
{
    public event EventHandler<ChangeLocationEventArgs> BeforeChangeLocation;
    public event EventHandler<ChangeLocationEventArgs> ChangeLocation;
    public event EventHandler<ChangeLocationEventArgs> AfterChangeLocation;
    
    public event EventHandler<BreakItemEventArgs> BeforeBreak;
    public event EventHandler<BreakItemEventArgs> Break;
    public event EventHandler<BreakItemEventArgs> AfterBreak;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeClimb;
    public event EventHandler<ContainerObjectEventArgs> Climb;
    public event EventHandler<ContainerObjectEventArgs> AfterClimb;

    public event EventHandler<ContainerObjectEventArgs> BeforeClose;
    public event EventHandler<ContainerObjectEventArgs> Close;
    public event EventHandler<ContainerObjectEventArgs> AfterClose;

    public event EventHandler<DropItemEventArgs> BeforeDrop;
    public event EventHandler<DropItemEventArgs> Drop;
    public event EventHandler<DropItemEventArgs> AfterDrop;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeGive;
    public event EventHandler<ContainerObjectEventArgs> Give;
    public event EventHandler<ContainerObjectEventArgs> AfterGive;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeOpen;
    public event EventHandler<ContainerObjectEventArgs> Open;
    public event EventHandler<ContainerObjectEventArgs> AfterOpen;
    
    public event EventHandler<ReadItemEventArgs> BeforeRead;
    public event EventHandler<ReadItemEventArgs> Read;
    public event EventHandler<ReadItemEventArgs> AfterRead;
    
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
    
    public event EventHandler<ContainerObjectEventArgs> BeforeBuy;
    public event EventHandler<ContainerObjectEventArgs> Buy;
    public event EventHandler<ContainerObjectEventArgs> AfterBuy;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeJump;
    public event EventHandler<ContainerObjectEventArgs> Jump;
    public event EventHandler<ContainerObjectEventArgs> AfterJump;
    
    public event EventHandler<PullItemEventArgs> BeforePull;
    public event EventHandler<PullItemEventArgs> Pull;
    public event EventHandler<PullItemEventArgs> AfterPull;
    
    public event EventHandler<PushItemEventArgs> BeforePush;
    public event EventHandler<PushItemEventArgs> Push;
    public event EventHandler<PushItemEventArgs> AfterPush;
    
    public event EventHandler<TurnItemEventArgs> BeforeTurn;
    public event EventHandler<TurnItemEventArgs> Turn;
    public event EventHandler<TurnItemEventArgs> AfterTurn;
    
    public event EventHandler<UnlockContainerEventArgs> BeforeUnlock;
    public event EventHandler<UnlockContainerEventArgs> Unlock;
    public event EventHandler<UnlockContainerEventArgs> AfterUnlock;
    
    public event EventHandler<UseItemEventArgs> BeforeUse;
    public event EventHandler<UseItemEventArgs> Use;
    public event EventHandler<UseItemEventArgs> AfterUse;
    
    public event EventHandler<WriteEventArgs> BeforeWrite;
    public event EventHandler<WriteEventArgs> Write;
    public event EventHandler<WriteEventArgs> AfterWrite;
    
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
    
        public virtual void OnBeforeJump(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.BeforeJump;
            localEventHandler?.Invoke(this, eventArgs);
        }
        
        public virtual void OnJump(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.Jump;
            localEventHandler?.Invoke(this, eventArgs);
        }
        
        public virtual void OnAfterJump(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.AfterJump;
            localEventHandler?.Invoke(this, eventArgs);
        }

        public virtual void OnBeforePull(PullItemEventArgs eventArgs)
        {
            var localEventHandler = this.BeforePull;
            localEventHandler?.Invoke(this, eventArgs);
        }
        
        public virtual void OnPull(PullItemEventArgs eventArgs)
        {
            var localEventHandler = this.Pull;
            localEventHandler?.Invoke(this, eventArgs);
        }
        
        public virtual void OnAfterPull(PullItemEventArgs eventArgs)
        {
            var localEventHandler = this.AfterPull;
            localEventHandler?.Invoke(this, eventArgs);
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
                throw new PushException(BaseDescriptions.DOES_NOT_WORK);
            }
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
                throw new WriteException(BaseDescriptions.NOTHING_HAPPENS);
            }
        }

        public virtual void OnBeforeBuy(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.BeforeBuy;
            localEventHandler?.Invoke(this, eventArgs);
        }
        
        public virtual void OnBuy(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.Buy;
            localEventHandler?.Invoke(this, eventArgs);
        }
        
        public virtual void OnAfterBuy(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.AfterBuy;
            localEventHandler?.Invoke(this, eventArgs);
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
    
        public virtual void OnTurn(TurnItemEventArgs eventArgs)
        {
            var localEventHandler = this.Turn;
            if (localEventHandler != null)
            {
                localEventHandler.Invoke(this, eventArgs);
            }
            else
            {
                throw new BreakException(BaseDescriptions.DOES_NOT_WORK);
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

        public virtual void OnBeforeChangeLocation(ChangeLocationEventArgs eventArgs)
        {
            var localEventHandler = this.BeforeChangeLocation;
            localEventHandler?.Invoke(this, eventArgs);
        }

        public virtual ChangeLocationStatus OnAfterChangeLocation(ChangeLocationEventArgs eventArgs)
        {
            var localEventHandler = this.AfterChangeLocation;
            if (localEventHandler != null)
            {
                localEventHandler(this, eventArgs);
            }
            else
            {
                if (!eventArgs.NewDestinationNode.Location.IsLocked)
                {
                    if (eventArgs.NewDestinationNode.Location.IsClosed)
                    {
                        return ChangeLocationStatus.IsClosed;
                    }
                }
                else
                {
                    return ChangeLocationStatus.IsLocked;
                }
            }

            return ChangeLocationStatus.Ok;
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

        public virtual void OnUnlock(UnlockContainerEventArgs eventArgs)
        {
            var localEventHandler = this.Unlock;
            if (localEventHandler != null)
            {
                localEventHandler.Invoke(this, eventArgs);
            }
            else
            {
                throw new UnlockException(string.Format(BaseDescriptions.IMPOSSIBLE_UNLOCK_WITH_WRONG_KEY, this.Name, eventArgs.Key.Name));
            }
        }
}