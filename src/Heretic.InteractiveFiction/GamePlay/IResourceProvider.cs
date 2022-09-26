using System.Globalization;
using System.Resources;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.GamePlay;

public interface IResourceProvider
{
    public IList<Verb> GetVerbsFromResources()
    {
        var result = new List<Verb>();
         ResourceSet resourceSet =
             Verbs.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

         if (resourceSet != null) 
         {
             foreach (DictionaryEntry entry in resourceSet)
             {
                 var prepositionPairs = VerbsAndPrepositions.ResourceManager.GetString(entry.Key.ToString())?.Split('|').ToList();
                 var prepositions = new List<PrepositionVariant>();
                 if (prepositionPairs != null)
                 {
                     foreach (var prepositionPair in prepositionPairs)
                     {
                         var splitPair = prepositionPair.Split(':');
                         prepositions.Add(new PrepositionVariant {Name = splitPair[0], Case = splitPair.Length > 1 ? splitPair[1] : string.Empty});
                     }
                 }
                 
                 var variantList = new List<VerbVariant>();
                 var wordList = entry.Value?.ToString()?.Split('|').ToList();

                 if (wordList != null)
                 {
                     var verb = new Verb
                     {
                         Key = entry.Key.ToString(),
                         Prepositions = prepositions
                     };

                     foreach (var word in wordList)
                     {
                         var verbAndPrefix = word.Split(':').ToList();
                         var variant = new VerbVariant
                         {
                             Name = verbAndPrefix[0],
                             Prefix = verbAndPrefix.Count > 1 ? verbAndPrefix[1] : string.Empty
                         };
                         variantList.Add(variant);
                     }

                     verb.Variants = variantList;
                     result.Add(verb);
                 }
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
    
    public IDictionary<string, (string, string)> PreparePrepositionsAndArticlesFromResource()
    {
        var result = new Dictionary<string, (string, string)>();
        var splitResource = BaseGrammar.COMBINED_PREPOSITIONS_AND_ARTICLES.Split('|');
        foreach (var resource in splitResource)
        {
            var combined = resource.Split(':');
            var word = combined[0];
            var prepositionAndArticle = combined[1].Split(' ');
            result.Add(word, (prepositionAndArticle[0], prepositionAndArticle[1]));

        }

        return result;
    }

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
    
    public IDictionary<string, IEnumerable<string>> GetPrepositionsFromResources()
    {
        var result = new Dictionary<string, IEnumerable<string>>();

        ResourceSet resourceSet =
            Prepositions.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
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
}
