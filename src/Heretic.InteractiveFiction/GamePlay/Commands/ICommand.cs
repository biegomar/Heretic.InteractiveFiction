using Heretic.InteractiveFiction.Grammars;

namespace Heretic.InteractiveFiction.GamePlay.Commands;

internal interface ICommand
{
    bool Execute(AdventureEvent adventureEvent);
    
    public static AdventureEvent GetAdventureEventWithoutPlayer(AdventureEvent adventureEvent)
    {
        var adventureEventWithoutPlayer = new AdventureEvent();
        adventureEventWithoutPlayer.Predicate = adventureEvent.Predicate;
        adventureEventWithoutPlayer.AllObjects.AddRange(adventureEvent.AllObjects.Skip(1));
        return adventureEventWithoutPlayer;
    }
}