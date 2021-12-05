namespace Heretic.InteractiveFiction.Objects;

public class LocationMap : Dictionary<Location, IEnumerable<DestinationNode>>
{
    public LocationMap(IEqualityComparer<Location>? comparer) : base(0, comparer) { }
}