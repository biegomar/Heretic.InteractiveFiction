using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class ConversationEventArgs : ContainerObjectEventArgs
{
    public AContainerObject Item { get; set; }
    public string Phrase { get; set; }
}
