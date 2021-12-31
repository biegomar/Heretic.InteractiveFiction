using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class ChangeLocationEventArgs : ContainerObjectEventArgs
{
    public DestinationNode NewDestinationNode { get; }

    public ChangeLocationEventArgs(DestinationNode newDestinationNode)
    {
        this.NewDestinationNode = newDestinationNode;
    }
}
