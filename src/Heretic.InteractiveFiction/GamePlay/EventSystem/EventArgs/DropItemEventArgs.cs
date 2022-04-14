using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class DropItemEventArgs: ContainerObjectEventArgs
{
    public AContainerObject ItemContainer { get; set; }
}