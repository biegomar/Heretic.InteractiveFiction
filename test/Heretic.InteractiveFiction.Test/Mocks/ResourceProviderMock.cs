using System.Collections.Generic;
using Heretic.InteractiveFiction.GamePlay;

namespace Heretic.Test.Mocks;

public class ResourceProviderMock: IResourceProvider
{
    public IDictionary<string, IEnumerable<string>> GetConversationsAnswersFromResources()
    {
        return new Dictionary<string, IEnumerable<string>>()
        {
            {
                "LOOK",
                new List<string>() { "Schaue", "Schau", "Lies", "Untersuche", "Zeige", "Zeig", "Look", "L", "Show" }
            },
        };
    }

    public IDictionary<string, IEnumerable<string>> GetItemsFromResources()
    {
        return new Dictionary<string, IEnumerable<string>>()
        {
            {
                "AIRLOCK_KEYPAD_GREEN_BUTTON",
                new List<string>() {"Der grüne Knopf", "DerGrüneKnopf", "Grüner Knopf", "GrünerKnopf"}
            }
        };
    }

    public IDictionary<string, IEnumerable<string>> GetCharactersFromResources()
    {
        return new Dictionary<string, IEnumerable<string>>()
        {
            {
                "PETROL_STATION_ATTENDANT",
                new List<string>() {"Der Tankwart", "DerTankwart","Tankwart","Mechaniker","Wart"}
            },
            {
                "OLD_FRIENDLY_MAN",
                new List<string>() {"Ein Mann", "EinMann","Mann", "Opa"}
            }
        };
    }

    public IDictionary<string, IEnumerable<string>> GetLocationsFromResources()
    {
        return new Dictionary<string, IEnumerable<string>>();
    }
}