using System.Globalization;
using System.Resources;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.GamePlay;

public interface IResourceProvider
{
    public IDictionary<string, IEnumerable<string>> GetVerbsFromResources()
    {
        var result = new Dictionary<string, IEnumerable<string>>();

        ResourceSet resourceSet =
            Verbs.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        if (resourceSet != null)
        {
            foreach (DictionaryEntry entry in resourceSet)
            {
                var inputList = entry.Value?.ToString()?.Split('|').ToList();
                result.Add(entry.Key.ToString()!, inputList);
            }
        }

        return result;
    }

    public IEnumerable<string> GetPackingWordsFromResources()
    {
        var result = new List<string>();

        ResourceSet resourceSet =
            PackingWords.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        if (resourceSet != null)
        {
            foreach (DictionaryEntry entry in resourceSet)
            {
                var inputList = entry.Value?.ToString()?.Split('|').ToList();
                result.AddRange(inputList!);
            }
        }

        return result;
    }

    IDictionary<string, IEnumerable<string>> GetConversationsAnswersFromResources();
    IDictionary<string, IEnumerable<string>> GetItemsFromResources();
    IDictionary<string, IEnumerable<string>> GetCharactersFromResources();
    IDictionary<string, IEnumerable<string>> GetLocationsFromResources();
}
