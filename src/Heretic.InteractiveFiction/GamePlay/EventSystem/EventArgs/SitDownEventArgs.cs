using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public sealed class SitDownEventArgs: ContainerObjectEventArgs
{
    public AHereticObject? ItemToSitOn { get; init; }
}