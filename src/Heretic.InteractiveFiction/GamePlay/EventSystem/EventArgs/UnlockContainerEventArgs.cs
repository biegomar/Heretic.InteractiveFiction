using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class UnlockContainerEventArgs : ContainerObjectEventArgs
{
    public AContainerObject Key { get; init; }

    public UnlockContainerEventArgs(AContainerObject key)
    {
        this.Key = key;
    }
}
