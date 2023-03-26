using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public sealed class ConversationEventArgs : ContainerObjectEventArgs
{
    public AHereticObject? Item { get; init; }
    public string Phrase { get; init; } = string.Empty;
}
