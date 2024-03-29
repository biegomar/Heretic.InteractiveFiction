using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public sealed class EnterLocationEventArgs: ContainerObjectEventArgs
{
    public DestinationNode? OldDestinationNode { get; init; }
}