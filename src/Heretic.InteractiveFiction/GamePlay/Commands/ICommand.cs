using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal interface ICommand
{
    bool Execute(AdventureEvent adventureEvent);
    
    public static AdventureEvent GetAdventureEventWithoutPlayer(AdventureEvent adventureEvent)
    {
        var adventureEventWithoutPlayer = new AdventureEvent
        {
            Predicate = adventureEvent.Predicate,
            AllObjects = adventureEvent.AllObjects.Skip(1).ToList()
        };
        return adventureEventWithoutPlayer;
    }
}