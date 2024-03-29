using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public sealed class LockContainerEventArgs : ContainerObjectEventArgs
{
    public AHereticObject? Key { get; init; }
}
