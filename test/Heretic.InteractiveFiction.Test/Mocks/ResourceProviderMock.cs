using System.Collections.Generic;
using Heretic.InteractiveFiction.GamePlay;

namespace Heretic.Test.Mocks;

public class ResourceProviderMock: IResourceProvider
{
    public IDictionary<string, IEnumerable<string>> GetConversationsAnswersFromResources()
    {
        return new Dictionary<string, IEnumerable<string>>();
    }

    public IDictionary<string, IEnumerable<string>> GetItemsFromResources()
    {
        return new Dictionary<string, IEnumerable<string>>();
    }

    public IDictionary<string, IEnumerable<string>> GetCharactersFromResources()
    {
        return new Dictionary<string, IEnumerable<string>>();
    }

    public IDictionary<string, IEnumerable<string>> GetLocationsFromResources()
    {
        return new Dictionary<string, IEnumerable<string>>();
    }
}