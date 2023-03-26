using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class UseItemEventArgs : ContainerObjectEventArgs
{
    public AHereticObject? ItemToUse { get; init; }
}
