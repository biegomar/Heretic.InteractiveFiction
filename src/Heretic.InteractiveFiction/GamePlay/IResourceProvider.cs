using System.Globalization;
using System.Resources;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.GamePlay;

public interface IResourceProvider
{
    public IList<Verb> GetVerbsFromResources()
    {
        var result = new List<Verb>();
        var resourceSet = Verbs.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        var umlauts = this.GetUmlauts();
        
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
                         prepositions.Add(new PrepositionVariant(Name: splitPair[0], Case: splitPair.Length > 1 ? splitPair[1] : string.Empty));
                     }
                 }
                 
                 var variantList = new List<VerbVariant>();
                 var wordList = entry.Value?.ToString()?.Split('|').ToList();

                 if (wordList != null)
                 {
                     var verb = new Verb
                     {
                         Key = Enum.Parse<VerbKey>(entry.Key.ToString(), true),
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
                         
                         if (umlauts.Keys.Any(verbAndPrefix[0].Contains))
                         {
                             var normalizedVerbName = verbAndPrefix[0];
                             normalizedVerbName = umlauts.Aggregate(normalizedVerbName, (current, umlaut) => current.Replace(umlaut.Key, umlaut.Value));
                             
                             var umlautVariant = new VerbVariant
                             {
                                 Name = normalizedVerbName,
                                 Prefix = verbAndPrefix.Count > 1 ? verbAndPrefix[1] : string.Empty,
                                 IsUmlautVariant = true
                             };
                             variantList.Add(umlautVariant);
                         }
                     }

                     verb.Variants = variantList;
                     result.Add(verb);
                 }
             }
         }

        return result;
    }

    private IDictionary<string,string> GetUmlauts()
    {
        var result = new Dictionary<string, string>();
        var umlauts = BaseGrammar.UMLAUTS.Split('|');
        foreach (var umlaut in umlauts)
        {
            var tranlation = umlaut.Split(':');
            result.Add(tranlation[0], tranlation[1]);
        }

        return result;
    }
    
    public IDictionary<string, IEnumerable<string>> ReadEntriesFromResources(ResourceManager resourceManager)
    {
        var result = new Dictionary<string, IEnumerable<string>>();
        
        var resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
        if (resourceSet != null)
        {
            foreach (DictionaryEntry entry in resourceSet)
            {
                var inputList = entry.Value?.ToString()?.Split('|', StringSplitOptions.RemoveEmptyEntries);
                if (inputList?.Any() == true)
                {
                    var normalizedList = this.NormalizeResourceList(inputList);
                    result.Add(entry.Key.ToString()!, normalizedList);
                }
            }
        }

        return result;
    }
    
    public IEnumerable<string> NormalizeResourceList(IEnumerable<string>? inputList)
    {
        var result = new List<string>();
        if (inputList != null)
        {
            foreach (var item in inputList)
            {
                result.Add(item);
                var trimmedItem = string.Concat(item.Where(c => !char.IsWhiteSpace(c)));
                if (item != trimmedItem)
                {
                    result.Add(trimmedItem);
                }
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
                if (inputList != null)
                {
                    result.Add(entry.Key.ToString()!, inputList);
                }
            }
        }

        return result;
    }
}
