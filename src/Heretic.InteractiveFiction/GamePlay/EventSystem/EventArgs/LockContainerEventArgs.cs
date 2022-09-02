using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class LockContainerEventArgs : ContainerObjectEventArgs
{
    public AHereticObject Key { get; set; }
}
