using Heretic.InteractiveFiction.Comparer;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.LogCabin.Resources;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.LogCabin.GamePlay;

internal sealed class GamePrerequisitesAssembler: IGamePrerequisitesAssembler
{
    public GamePrerequisites AssembleGame()
    {
        var livingRoom = LivingRoomPrerequisites.Get();
        
        var map = new LocationMap(new LocationComparer());

        var activeLocation = livingRoom;
        var activePlayer = PlayerPrerequisites.Get();
        var actualQuests = GetQuests();
        
        return new GamePrerequisites(map, activeLocation, activePlayer, null, actualQuests);
    }

    private static ICollection<string> GetQuests()
    {
        var result = new List<string>
        {
            Descriptions.QUEST_I
        };

        return result;
    }
}