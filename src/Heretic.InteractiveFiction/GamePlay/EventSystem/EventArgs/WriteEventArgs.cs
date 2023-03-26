namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public sealed class WriteEventArgs: ContainerObjectEventArgs
{
    public string Text { get; init; } = string.Empty;
}