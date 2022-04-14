using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class UnlockContainerEventArgs : ContainerObjectEventArgs
{
    public AHereticObject Key { get; set; }
}
