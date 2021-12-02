using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class ConversationEventArgs : ContainerObjectEventArgs
{
    public AContainerObject Item { get; init; }
    public string Phrase { get; init; }
}
