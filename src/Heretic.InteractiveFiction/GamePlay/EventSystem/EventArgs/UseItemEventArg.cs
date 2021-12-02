using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class UseItemEventArg : ContainerObjectEventArgs
{
    public AContainerObject ItemToUse { get; set; }

    public UseItemEventArg(AContainerObject item)
    {
        this.ItemToUse = item;
    }
}
