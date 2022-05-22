namespace Heretic.InteractiveFiction.Objects;

public sealed class LocationMap : Dictionary<Location, IEnumerable<DestinationNode>>
{
    public LocationMap(IEqualityComparer<Location> comparer) : base(0, comparer) { }
}