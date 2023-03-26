using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class ContainerObjectEventArgs : System.EventArgs
{
    public string ExternalItemKey { get; init; } = string.Empty;
    public Description OptionalErrorMessage { get; init; } = string.Empty;
}
