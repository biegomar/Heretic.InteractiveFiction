using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public partial class AHereticObject
{
    public event EventHandler<ChangeLocationEventArgs> BeforeChangeLocation;
    public event EventHandler<ChangeLocationEventArgs> AfterChangeLocation;
    
    public event EventHandler<BreakItemEventArgs> Break;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeClimb;
    public event EventHandler<ContainerObjectEventArgs> AfterClimb;
    public event EventHandler<ContainerObjectEventArgs> Climb;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeClose;
    public event EventHandler<ContainerObjectEventArgs> AfterClose;
    public event EventHandler<ContainerObjectEventArgs> Close;
    
    public event EventHandler<DropItemEventArgs> BeforeDrop;
    public event EventHandler<DropItemEventArgs> AfterDrop;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeEat;
    public event EventHandler<ContainerObjectEventArgs> AfterEat;
    
    public event EventHandler<ContainerObjectEventArgs> AfterGive;
    
    public event EventHandler<ContainerObjectEventArgs> Open;
    public event EventHandler<ContainerObjectEventArgs> BeforeOpen;
    public event EventHandler<ContainerObjectEventArgs> AfterOpen;
    
    public event EventHandler<ReadItemEventArgs> Read;
    public event EventHandler<ReadItemEventArgs> BeforeRead;
    public event EventHandler<ReadItemEventArgs> AfterRead;
    
    public event EventHandler<ContainerObjectEventArgs> AfterLook;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeTake;
    public event EventHandler<ContainerObjectEventArgs> AfterTake;
    public event EventHandler<ContainerObjectEventArgs> Take;
    
    public event EventHandler<SitDownEventArgs> BeforeSitDown;
    public event EventHandler<SitDownEventArgs> AfterSitDown;
    public event EventHandler<SitDownEventArgs> SitDown;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeStandUp;
    public event EventHandler<ContainerObjectEventArgs> AfterStandUp;
    
    public event EventHandler<ContainerObjectEventArgs> BeforeDescend;
    public event EventHandler<ContainerObjectEventArgs> AfterDescend;
    
    public event EventHandler<ContainerObjectEventArgs> Buy;
    
    public event EventHandler<ContainerObjectEventArgs> Jump;
    
    public event EventHandler<PullItemEventArgs> Pull;
    
    public event EventHandler<PushItemEventArgs> Push;
    
    public event EventHandler<TurnItemEventArgs> Turn;
    
    public event EventHandler<UnlockContainerEventArgs> Unlock;
    
    public event EventHandler<UseItemEventArgs> Use;
    
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
                throw new UseException(BaseDescriptions.NOTHING_HAPPENS);
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
                throw new JumpException(BaseDescriptions.DOES_NOT_WORK);
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
                throw new PullException(BaseDescriptions.DOES_NOT_WORK);
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
                throw new PushException(BaseDescriptions.DOES_NOT_WORK);
            }
        }
    
        public virtual void OnOpen(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.Open;
            if (localEventHandler != null)
            {
                localEventHandler.Invoke(this, eventArgs);
            }
            else
            {
                throw new OpenException(BaseDescriptions.IMPOSSIBLE_OPEN);
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
        
        public virtual void OnRead(ReadItemEventArgs eventArgs)
        {
            var localEventHandler = this.Read;
            if (localEventHandler != null)
            {
                localEventHandler.Invoke(this, eventArgs);
            }
            else
            {
                throw new PushException(BaseDescriptions.NOTHING_TO_READ);
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
                throw new BuyException(BaseDescriptions.ON_BUY_EXCEPTION);
            }
        }
    
        public virtual void OnBeforeClimb(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.BeforeClimb;
            localEventHandler?.Invoke(this, eventArgs);
        }
    
        public virtual void OnAfterClimb(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.AfterClimb;
            localEventHandler?.Invoke(this, eventArgs);
        }
    
        public virtual void OnClimb(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.Climb;
            if (localEventHandler != null)
            {
                localEventHandler.Invoke(this, eventArgs);
            }
            else
            {
                throw new ClimbException(BaseDescriptions.IMPOSSIBLE_CLIMB);
            }
        }

        public virtual void OnBeforeDescend(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.BeforeDescend;
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

        public virtual void OnAfterDrop(DropItemEventArgs eventArgs)
        {
            var localEventHandler = this.AfterDrop;
            localEventHandler?.Invoke(this, eventArgs);
        }
    
        public virtual void OnBeforeEat(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.BeforeEat;
            localEventHandler?.Invoke(this, eventArgs);
        }
    
        public virtual void OnAfterEat(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.AfterEat;
            localEventHandler?.Invoke(this, eventArgs);
        }
    
        public virtual void OnBreak(BreakItemEventArgs eventArgses)
        {
            var localEventHandler = this.Break;
            if (localEventHandler != null)
            {
                localEventHandler.Invoke(this, eventArgses);
            }
            else
            {
                throw new BreakException(BaseDescriptions.IMPOSSIBLE_BREAK);
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
                throw new BreakException(BaseDescriptions.DOES_NOT_WORK);
            }
        }
    
        public virtual void OnBeforeSitDown(SitDownEventArgs eventArgs)
        {
            var localEventHandler = this.BeforeSitDown;
            localEventHandler?.Invoke(this, eventArgs);
        }
    
        public virtual void OnAfterSitDown(SitDownEventArgs eventArgs)
        {
            var localEventHandler = this.AfterSitDown;
            localEventHandler?.Invoke(this, eventArgs);
        }
    
        public virtual void OnSitDown(SitDownEventArgs eventArgs)
        {
            var localEventHandler = this.SitDown;
            localEventHandler?.Invoke(this, eventArgs);
        }
    
        public virtual void OnBeforeStandUp(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.BeforeStandUp;
            localEventHandler?.Invoke(this, eventArgs);
        }
    
        public virtual void OnAfterStandUp(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.AfterStandUp;
            localEventHandler?.Invoke(this, eventArgs);
        }

        public virtual void OnAfterGive(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.AfterGive;
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
    
        public virtual void OnAfterTake(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.AfterTake;
            localEventHandler?.Invoke(this, eventArgs);
        }
    
        public virtual void OnTake(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.Take;
            if (localEventHandler != null)
            {
                localEventHandler.Invoke(this, eventArgs);
            }
            else
            {
                throw new TakeException(BaseDescriptions.IMPOSSIBLE_SINGLE_ITEM_PICKUP);
            }
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

        public virtual void OnAfterClose(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.AfterClose;
            localEventHandler?.Invoke(this, eventArgs);
        }
    
        public virtual void OnClose(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.Close;
            if (localEventHandler != null)
            {
                localEventHandler.Invoke(this, eventArgs);
            }
            else
            {
                throw new CloseException(BaseDescriptions.IMPOSSIBLE_CLOSE);
            }
        }

        public virtual void OnBeforeOpen(ContainerObjectEventArgs eventArgs)
        {
            var localEventHandler = this.BeforeOpen;
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