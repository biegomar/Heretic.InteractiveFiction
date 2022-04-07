using Heretic.InteractiveFiction.GamePlay.EventSystem;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

public record GamePrerequisites(LocationMap LocationMap, Location ActiveLocation, Player ActivePlayer, PeriodicEvent PeriodicEvent ,ICollection<string> Quests);