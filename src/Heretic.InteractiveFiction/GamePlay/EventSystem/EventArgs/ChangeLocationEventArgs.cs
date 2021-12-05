using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class ChangeLocationEventArgs : ContainerObjectEventArgs
{
    public DestinationNode NewDestinationNode { get; set; }

    public ChangeLocationEventArgs(DestinationNode newDestinationNode)
    {
        this.NewDestinationNode = newDestinationNode;
    }
}
