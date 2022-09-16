using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class InputAnalyzer
{
    private readonly Universe universe;
    private readonly ObjectHandler objectHandler;
    private readonly IGrammar grammar;

    internal InputAnalyzer(Universe universe, IGrammar grammar)
    {
        this.universe = universe;
        this.objectHandler = new ObjectHandler(universe);
        this.grammar = grammar;
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
        var objectTwo = string.Empty;
        var orderedSentence = new List<string>();
        var parts = sentence.ToList();

        if (parts.Any())
        {
            var normList = this.NormalizeSentence(parts);
            var objectOne = this.GetCharacter(normList.Keys.ToList());
            if (string.IsNullOrEmpty(objectOne))
            {  
                objectOne = this.GetItem(normList.Keys.ToList());
                if (string.IsNullOrEmpty(objectOne))
                {
                    objectOne = this.GetLocation(normList.Keys.ToList());
                }
            }
            if (!string.IsNullOrEmpty(objectOne))
            {
                RemoveNormlistItemsFromParts(normList[objectOne], parts);
            }


            if (parts.Any())
            {
                normList = this.NormalizeSentence(parts);
                objectTwo = this.GetItem(normList.Keys.ToList());
                if (string.IsNullOrEmpty(objectTwo))
                {
                    objectTwo = this.GetCharacter(normList.Keys.ToList());
                    if (string.IsNullOrEmpty(objectTwo))
                    {
                        objectTwo = this.GetConversationAnswer(normList.Keys.ToList());
                    }
                }

                if (!string.IsNullOrEmpty(objectTwo))
                {
                    RemoveNormlistItemsFromParts(normList[objectTwo], parts);
                }

                if (parts.Any())
                {
                    var predicate = this.GetVerb(parts);
                    orderedSentence.Add(predicate);
                    
                    if (!string.IsNullOrEmpty(objectOne))
                    {
                        orderedSentence.Add(objectOne);    
                    }
            
                    if (!string.IsNullOrEmpty(objectTwo))
                    {
                        orderedSentence.Add(objectTwo);    
                    }
                    
                    if (parts.Any() && string.IsNullOrEmpty(objectOne) && string.IsNullOrEmpty(objectOne))
                    {
                        foreach (var part in parts)
                        {
                            orderedSentence.Add(part);
                        }
                    }
                }
                else
                {
                    throw new NoVerbException();
                }
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
            if (!this.universe.IsVerb(word))
            {
                if (this.universe.LocationResources.Values.SelectMany(x => x).Contains(word, StringComparer.InvariantCultureIgnoreCase))
                {
                    return word;
                }   
            }
        }

        return string.Empty;
    }
    
    private string GetItem(IList<string> sentence)
    {
        foreach (var word in sentence)
        {
            if (!this.universe.IsVerb(word))
            {
                if (this.universe.ItemResources.Values.SelectMany(x => x).Contains(word, StringComparer.InvariantCultureIgnoreCase))
                {
                    return word;
                }    
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
            if (this.grammar.IsPronounActiveObject(this.universe.ActiveObject, word))
            {
                result.Add(GetFirstObjectNameWithoutWhitespace(this.universe.ActiveObject));
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
