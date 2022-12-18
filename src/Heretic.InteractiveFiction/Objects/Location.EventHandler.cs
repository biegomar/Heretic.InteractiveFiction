using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

namespace Heretic.InteractiveFiction.Objects;

public sealed partial class Location
{
    public event EventHandler<EnterLocationEventArgs> BeforeEnterLocation;
    public event EventHandler<EnterLocationEventArgs> EnterLocation;
    public event EventHandler<EnterLocationEventArgs> AfterEnterLocation;
    
    public event EventHandler<LeaveLocationEventArgs> BeforeLeaveLocation;
    public event EventHandler<LeaveLocationEventArgs> LeaveLocation;
    public event EventHandler<LeaveLocationEventArgs> AfterLeaveLocation;
    
    public void OnBeforeEnterLocation(EnterLocationEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeEnterLocation;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public void OnEnterLocation(EnterLocationEventArgs eventArgs)
    {
        var localEventHandler = this.EnterLocation;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public void OnAfterEnterLocation(EnterLocationEventArgs eventArgs)
    {
        var localEventHandler = this.AfterEnterLocation;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public void OnBeforeLeaveLocation(LeaveLocationEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeLeaveLocation;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public void OnLeaveLocation(LeaveLocationEventArgs eventArgs)
    {
        var localEventHandler = this.LeaveLocation;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public void OnAfterLeaveLocation(LeaveLocationEventArgs eventArgs)
    {
        var localEventHandler = this.AfterLeaveLocation;
        localEventHandler?.Invoke(this, eventArgs);
    }
}