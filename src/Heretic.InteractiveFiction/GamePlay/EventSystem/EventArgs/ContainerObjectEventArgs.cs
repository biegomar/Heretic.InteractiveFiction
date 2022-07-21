using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class ContainerObjectEventArgs : System.EventArgs
{
    public string ExternalItemKey { get; set; }
    public Description OptionalErrorMessage { get; set; }
}
