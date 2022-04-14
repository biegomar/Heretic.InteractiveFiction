using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class BreakItemEventArgs: ContainerObjectEventArgs
{
    public AHereticObject ItemToUse { get; set; }
}