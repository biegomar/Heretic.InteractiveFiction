using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class SitDownEventArgs: ContainerObjectEventArgs
{
    public AContainerObject ItemToSitOn { get; set; }
}