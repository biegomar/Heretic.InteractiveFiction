using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public sealed class LeaveLocationEventArgs : ContainerObjectEventArgs
{
    public DestinationNode? NewDestinationNode { get; init; }
}
