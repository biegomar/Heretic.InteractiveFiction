using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

namespace Heretic.InteractiveFiction.Objects;

public sealed partial class Item
{
    public event EventHandler<ContainerObjectEventArgs>? BeforeSwitchOn;
    public event EventHandler<ContainerObjectEventArgs>? SwitchOn;
    public event EventHandler<ContainerObjectEventArgs>? AfterSwitchOn;
    
    public event EventHandler<ContainerObjectEventArgs>? BeforeSwitchOff;
    public event EventHandler<ContainerObjectEventArgs>? SwitchOff;
    public event EventHandler<ContainerObjectEventArgs>? AfterSwitchOff;
    
    public void OnBeforeSwitchOn(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeSwitchOn;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public void OnSwitchOn(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.SwitchOn;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public void OnAfterSwitchOn(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterSwitchOn;
        localEventHandler?.Invoke(this, eventArgs);
    }
    
    public void OnBeforeSwitchOff(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.BeforeSwitchOff;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public void OnSwitchOff(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.SwitchOff;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public void OnAfterSwitchOff(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.AfterSwitchOff;
        localEventHandler?.Invoke(this, eventArgs);
    }
}