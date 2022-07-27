using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

namespace Heretic.InteractiveFiction.Objects;

public sealed partial class Location
{
    public event EventHandler<ChangeLocationEventArgs> BeforeChangeLocation;
    public event EventHandler<ChangeLocationEventArgs> ChangeLocation;
    public event EventHandler<ChangeLocationEventArgs> AfterChangeLocation;
    
    public void OnBeforeChangeLocation(ChangeLocationEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeChangeLocation;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public void OnChangeLocation(ChangeLocationEventArgs eventArgs)
    {
        var localEventHandler = this.ChangeLocation;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public ChangeLocationStatus OnAfterChangeLocation(ChangeLocationEventArgs eventArgs)
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
}