using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class InputAnalyzer
{
    private readonly Universe universe;
    private readonly ObjectHandler objectHandler;

    internal InputAnalyzer(Universe universe)
    {
        this.universe = universe;
        this.objectHandler = new ObjectHandler(universe);
    }

    internal string[] Analyze(string input)
    {
        var stringToAnalyze = input;
        
        try
        {
            var normalizedInput = stringToAnalyze.Trim().Replace(", ", ",");
            var sentence = normalizedInput.Split(' ');
            sentence = this.SubstitutePronoun(sentence).ToArray();
            sentence = this.NormalizeVerbInSentence(sentence);
            sentence = sentence.Where(x => !this.universe.PackingWordsResources.Contains(x, StringComparer.InvariantCultureIgnoreCase)).ToArray();

            sentence = this.OrderSentence(sentence).ToArray();

            return sentence;
        }
        catch (Exception)
        {
            return new []{string.Empty};
        }
    }

    private IList<string> OrderSentence(IReadOnlyList<string> sentence)
    {
        var orderedSentence = new List<string>();
        string itemObject;

        var parts = sentence.ToList();
        var verb = this.GetVerb(parts);
        orderedSentence.Add(verb);

        if (parts.Any())
        {
            var normList = this.NormalizeSentence(parts);
            itemObject = this.GetCharacter(normList.Keys.ToList());
            if (itemObject == string.Empty)
            {  
                itemObject = this.GetItem(normList.Keys.ToList());
                if (itemObject == string.Empty)
                {
                    itemObject = this.GetLocation(normList.Keys.ToList());
                    if (itemObject == string.Empty)
                    {
                        itemObject = parts[0];
                    }
                }
            }

            RemoveNormlistItemsFromParts(normList[itemObject], parts);

            orderedSentence.Add(itemObject);

            if (parts.Any())
            {
                normList = this.NormalizeSentence(parts);
                var subject = this.GetItem(normList.Keys.ToList());
                if (subject == string.Empty)
                {
                    subject = this.GetCharacter(normList.Keys.ToList());
                    if (subject == string.Empty)
                    {
                        subject = this.GetConversationAnswer(normList.Keys.ToList());
                        if (subject == string.Empty)
                        {
                            subject = parts[0];
                        }
                    }
                }

                orderedSentence.Add(subject);
            }
        }
        
        return orderedSentence;
    }

    private static void RemoveNormlistItemsFromParts(IEnumerable<string> normList, ICollection<string> parts)
    {
        foreach (var item in normList)
        {
            parts.Remove(item);
        }
    }
    
    public string[] NormalizeVerbInSentence(IList<string> sentence)
    {
        bool ReplaceVerb(string verbToReplace, ICollection<Verb> verbs, IList<string> resultingSentence)
        {
            Verb verb = default;
            if (verbs.Count == 1)
            {
                verb = verbs.First();
                var index = resultingSentence.IndexOf(verbToReplace);
                resultingSentence[index] = verb.Key;

                return true;
            }

            foreach (var possibleVerb in verbs)
            {
                var allPrefixes = possibleVerb.Variants.Where(v => v.Name.Equals(verbToReplace, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(v.Prefix)).Select(v => v.Prefix).Distinct();
                var intersect = sentence.Intersect(allPrefixes);
                var onlyPossiblePrefix = intersect.FirstOrDefault();
                if (onlyPossiblePrefix != default && sentence.Contains(onlyPossiblePrefix))
                {
                    verb = possibleVerb;
                    var index = resultingSentence.IndexOf(verbToReplace);
                    resultingSentence[index] = verb.Key;
                    resultingSentence.Remove(onlyPossiblePrefix);
                    return true;
                }
            }
            
            //only one verb without prefix is possible!
            var verbWithoutPrefix = verbs.SingleOrDefault(v => v.Variants.Count(x => x.Prefix == string.Empty) > 0);
            if (verbWithoutPrefix != default)
            {
                verb = verbWithoutPrefix;
                var index = resultingSentence.IndexOf(verbToReplace);
                resultingSentence[index] = verb.Key;
                return true;
            }

            return false;
        }

        var result = sentence.ToList();
        var isVerbReplaced = false;
        
        foreach (var word in sentence)
        {
            var possibleVerbsAndVariants =this.universe.Verbs.Where(v => v.Variants.Select(v => v.Name).Contains(word, StringComparer.InvariantCultureIgnoreCase)).Select(v => new Verb()
            {
                Key = v.Key,
                ErrorMessage = v.ErrorMessage,
                Variants = v.Variants.Where(vi => vi.Name.Equals(word, StringComparison.InvariantCultureIgnoreCase)).ToList()
            }).ToList();
            
            if (possibleVerbsAndVariants.Any())
            {
                isVerbReplaced = ReplaceVerb(word, possibleVerbsAndVariants, result);
            }
            else
            {
                var verbOverrides = this.universe.ActiveLocation.OptionalVerbs.SelectMany(x => x.Value).ToList();
                var optionalVerbs = verbOverrides.Where(v => v.Variants.Select(v => v.Name).Contains(word, StringComparer.InvariantCultureIgnoreCase)).Select(v => new Verb()
                {
                    Key = v.Key,
                    ErrorMessage = v.ErrorMessage,
                    Variants = v.Variants.Where(vi => vi.Name.Equals(word, StringComparison.InvariantCultureIgnoreCase)).ToList()
                }).ToList();
                
                if (optionalVerbs.Any())
                {
                    isVerbReplaced = ReplaceVerb(word, optionalVerbs, result);
                }
            }

            if (isVerbReplaced)
            {
                break;
            }
        }
        

        return result.ToArray();
    }


    private string GetVerb(IList<string> sentence)
    {
        foreach (var word in sentence)
        {
            if (this.universe.IsVerb(word))
            {
                sentence.Remove(word);
                return word;
            }
        }

        throw new NoVerbException();
    }
    
    private string GetCharacter(IList<string> sentence)
    {
        foreach (var word in sentence)
        {
            var key = this.objectHandler.GetCharacterKeyByName(word);
            if (!string.IsNullOrEmpty(key))
            {
                return word;
            }
        }

        return string.Empty;
    }
    
    private string GetLocation(IList<string> sentence)
    {
        foreach (var word in sentence)
        {
            if (this.universe.LocationResources.Values.SelectMany(x => x).Contains(word, StringComparer.InvariantCultureIgnoreCase))
            {
                return word;
            }
        }

        return string.Empty;
    }
    
    private string GetItem(IList<string> sentence)
    {
        foreach (var word in sentence)
        {
            if (this.universe.ItemResources.Values.SelectMany(x => x).Contains(word, StringComparer.InvariantCultureIgnoreCase))
            {
                return word;
            }
        }

        return string.Empty;
    }
    
    private string GetConversationAnswer(IList<string> sentence)
    {
        foreach (var word in sentence)
        {
            if (this.universe.ConversationAnswersResources.Values.SelectMany(x => x).Contains(word, StringComparer.InvariantCultureIgnoreCase))
            {
                return word;
            }
        }

        return string.Empty;
    }
    
    private IDictionary<string, IEnumerable<string>> NormalizeSentence(IList<string> sentence)
    {
        var result = new Dictionary<string, IEnumerable<string>>();
        
        for (var index = 0; index < sentence.Count; index++)
        {
            var concatWord = sentence[index];
            var concatWordList = new List<string>() { concatWord };
            if (!result.ContainsKey(concatWord))
            {
                result.Add(concatWord, concatWordList.ToList());    
            }
            
            for (var position = index + 1; position < sentence.Count(); position++)
            {
                concatWord += sentence[position];
                concatWordList.Add(sentence[position]);
                if (!result.ContainsKey(concatWord))
                {
                    result.Add(concatWord, concatWordList.ToList());    
                }
            }
        }

        return result;
    }

    private IList<string> SubstitutePronoun(IList<string> sentence)
    {
        var result = new List<string>();
        foreach (var word in sentence)
        {
            var item = this.objectHandler.GetActiveObjectViaPronoun(word);
            if (item != default)
            {
                result.Add(GetFirstObjectNameWithoutWhitespace(item));
            }
            else
            {
                result.Add(word);
            }
        }

        return result;
    }

    private string GetFirstObjectNameWithoutWhitespace(AHereticObject item)
    {
        return item.GetNames().FirstOrDefault(i => !i.Contains(" "));
    }
}
