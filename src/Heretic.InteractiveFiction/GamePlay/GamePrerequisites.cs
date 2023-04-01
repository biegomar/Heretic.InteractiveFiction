using Heretic.InteractiveFiction.GamePlay.EventSystem;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

public class GamePrerequisites
{
    public LocationMap LocationMap { get; }
    public Location ActiveLocation { get; }
    public Player ActivePlayer { get; }
    public PeriodicEvent? PeriodicEvent { get; }
    public ICollection<string> Quests { get; }
    
    public GamePrerequisites(LocationMap locationMap, Location activeLocation, Player activePlayer, PeriodicEvent? periodicEvent ,ICollection<string> quests)
    {
        this.LocationMap = locationMap;
        this.ActiveLocation = activeLocation;
        this.ActivePlayer = activePlayer;
        this.PeriodicEvent = periodicEvent;
        this.Quests = quests;
    }
}