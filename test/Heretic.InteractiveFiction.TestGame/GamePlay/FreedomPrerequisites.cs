using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.TestGame.Resources;

namespace Heretic.InteractiveFiction.TestGame.GamePlay;

internal static class FreedomPrerequisites
{
    internal static Location Get()
    {
        var freedom = new Location()
        {
            Key = Keys.FREEDOM,
            Name = Locations.FREEDOM,
            Description = Descriptions.FREEDOM,
            IsLocked = true,
            IsLockable = true,
            LockDescription = Descriptions.FREEDOM_LOCK_DESCRIPTION
        };
        
        return freedom;
    }
}