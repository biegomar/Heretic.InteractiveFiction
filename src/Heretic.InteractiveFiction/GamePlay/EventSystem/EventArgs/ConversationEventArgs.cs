using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public sealed class ConversationEventArgs : ContainerObjectEventArgs
{
    public AHereticObject Item { get; set; }
    public string Phrase { get; set; }
}
