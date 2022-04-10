using Heretic.InteractiveFiction.GamePlay.EventSystem;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

public class GamePrerequisites
{
    public GamePrerequisites(LocationMap LocationMap, Location ActiveLocation, Player ActivePlayer, PeriodicEvent PeriodicEvent ,ICollection<string> Quests)
    {
        this.LocationMap = LocationMap;
        this.ActiveLocation = ActiveLocation;
        this.ActivePlayer = ActivePlayer;
        this.PeriodicEvent = PeriodicEvent;
        this.Quests = Quests;
    }

    public LocationMap LocationMap { get; }
    public Location ActiveLocation { get; }
    public Player ActivePlayer { get; }
    public PeriodicEvent PeriodicEvent { get; }
    public ICollection<string> Quests { get; }

    public void Deconstruct(out LocationMap LocationMap, out Location ActiveLocation, out Player ActivePlayer, out PeriodicEvent PeriodicEvent , out ICollection<string> Quests)
    {
        LocationMap = this.LocationMap;
        ActiveLocation = this.ActiveLocation;
        ActivePlayer = this.ActivePlayer;
        PeriodicEvent = this.PeriodicEvent;
        Quests = this.Quests;
    }
}