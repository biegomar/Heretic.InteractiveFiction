using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Comparer;

public class LocationComparer : IEqualityComparer<Location>
{
    public bool Equals(Location x, Location y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return Equals(x.Items, y.Items) && Equals(x.Characters, y.Characters);
    }

    public int GetHashCode(Location obj)
    {
        return HashCode.Combine(obj.Items, obj.Characters);
    }
}
