using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;

public class ChangeLocationEventArgs : ContainerObjectEventArgs
{
    public LocationMap NewLocationMap { get; set; }

    public ChangeLocationEventArgs(LocationMap newLocationMap)
    {
        this.NewLocationMap = newLocationMap;
    }
}
