using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class BreakItemEventArg: ContainerObjectEventArgs
{
    public AContainerObject ItemToUse { get; set; }
}