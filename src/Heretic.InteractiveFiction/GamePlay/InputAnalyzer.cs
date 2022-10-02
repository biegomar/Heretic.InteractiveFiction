using System.Runtime.CompilerServices;
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
            sentence = this.SubstituteCombinedPrepositionsAndArticles(sentence).ToArray();
            sentence = this.OrderSentence(sentence).ToArray();

            return sentence;
        }
        catch (Exception)
        {
            return new []{string.Empty};
        }
    }

    private IEnumerable<string> OrderSentence(IReadOnlyList<string> sentence)
    {
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
                var objectTwo = this.GetItem(normList.Keys.ToList());
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
                    parts = this.ReplaceVerbInParts(sentence, parts, objectOne, objectTwo);
                    var predicate = this.GetVerb(sentence, parts);
                    orderedSentence.Add(predicate);
                    
                    if (!string.IsNullOrEmpty(objectOne))
                    {
                        orderedSentence.Add(objectOne);  
                        RemoveObjectArticlesFromParts(objectOne, parts);
                    }
            
                    if (!string.IsNullOrEmpty(objectTwo))
                    {
                        orderedSentence.Add(objectTwo);  
                        RemoveObjectArticlesFromParts(objectTwo, parts);
                    }

                    if (parts.Any())
                    {
                        RemovePrepositionsFromParts(parts);
                    }
                    
                    if (parts.Any() && (string.IsNullOrEmpty(objectOne) || string.IsNullOrEmpty(objectTwo)))
                    {
                        var partString = string.Join('|', parts);
                        orderedSentence.Add(partString);
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

    private void RemoveObjectArticlesFromParts(string processingObject, ICollection<string> parts)
    {
        var item = this.objectHandler.GetObjectFromWorldByName(processingObject);
        if (item != default)
        {
            var partToRemove = parts.FirstOrDefault(p => p.Equals(this.grammar.GetArticleForObject(item, GrammarCase.Nominative), StringComparison.InvariantCultureIgnoreCase));
            parts.Remove(partToRemove);
            partToRemove = parts.FirstOrDefault(p => p.Equals(this.grammar.GetArticleForObject(item, GrammarCase.Nominative, ArticleState.Indefinite), StringComparison.InvariantCultureIgnoreCase));
            parts.Remove(partToRemove);
            
            partToRemove = parts.FirstOrDefault(p => p.Equals(this.grammar.GetArticleForObject(item, GrammarCase.Genitive), StringComparison.InvariantCultureIgnoreCase));
            parts.Remove(partToRemove);
            partToRemove = parts.FirstOrDefault(p => p.Equals(this.grammar.GetArticleForObject(item, GrammarCase.Genitive, ArticleState.Indefinite), StringComparison.InvariantCultureIgnoreCase));
            parts.Remove(partToRemove);
            
            partToRemove = parts.FirstOrDefault(p => p.Equals(this.grammar.GetArticleForObject(item, GrammarCase.Dative), StringComparison.InvariantCultureIgnoreCase));
            parts.Remove(partToRemove);
            partToRemove = parts.FirstOrDefault(p => p.Equals(this.grammar.GetArticleForObject(item, GrammarCase.Dative, ArticleState.Indefinite), StringComparison.InvariantCultureIgnoreCase));
            parts.Remove(partToRemove);
            
            partToRemove = parts.FirstOrDefault(p => p.Equals(this.grammar.GetArticleForObject(item, GrammarCase.Accusative), StringComparison.InvariantCultureIgnoreCase));
            parts.Remove(partToRemove);
            partToRemove = parts.FirstOrDefault(p => p.Equals(this.grammar.GetArticleForObject(item, GrammarCase.Accusative, ArticleState.Indefinite), StringComparison.InvariantCultureIgnoreCase));
            parts.Remove(partToRemove);
        }
    }

    private void RemovePrepositionsFromParts(ICollection<string> parts)
    {
        var allPrepositions = this.grammar.Prepositions.Values.SelectMany(x => x);
        foreach (var preposition in allPrepositions)
        {
            parts.Remove(preposition);
        }
    }

    private List<string> ReplaceVerbInParts(IReadOnlyList<string> sentence, ICollection<string> parts, string objectOne, string objectTwo)
    {
        Verb verb = default;
        var result = parts.ToList();
        
        void SetVerb(Verb possibleVerb, string word)
        {
            verb = possibleVerb;
            var index = result.IndexOf(word);
            result[index] = verb.Key;
        }
        
        var isPrepositionOrPrefixPresentInSentence = this.grammar.HasPrepositionOrPrefix(sentence);

        foreach (var word in parts)
        {
            var possibleVerbsAndVariants = ExtractPossibleVerbs(word);
            
            if (possibleVerbsAndVariants.Any())
            {
                if (possibleVerbsAndVariants.Count == 1)
                {
                    verb = possibleVerbsAndVariants.First();
                    var index = result.IndexOf(word);
                    result[index] = verb.Key;
                }
                else if (!isPrepositionOrPrefixPresentInSentence)
                {
                    var onlyPossibleVerbWithoutPrefix = possibleVerbsAndVariants.SingleOrDefault(v => v.Variants.Count(x => x.Prefix == string.Empty) > 0);
                    if (onlyPossibleVerbWithoutPrefix != default)
                    {
                        SetVerb(onlyPossibleVerbWithoutPrefix, word);
                    }
                }
                else
                {
                    foreach (var possibleVerb in possibleVerbsAndVariants)
                    {
                        var onlyPossiblePrefix = GetOnlyPossiblePrefix(parts, possibleVerb, word);
                        var onlyPossiblePreposition = GetOnlyPossiblePreposition(parts, possibleVerb);

                        var isPrefixOnly = !string.IsNullOrEmpty(onlyPossiblePrefix) && string.IsNullOrEmpty(onlyPossiblePreposition);
                        var isPrepositionOnly = string.IsNullOrEmpty(onlyPossiblePrefix) && !string.IsNullOrEmpty(onlyPossiblePreposition);
                        var isPrefixAndPreposition = !string.IsNullOrEmpty(onlyPossiblePrefix) && !string.IsNullOrEmpty(onlyPossiblePreposition);
                        
                        if (isPrepositionOnly)
                        {
                            if ((this.IsObjectInCorrectCaseForPreposition(possibleVerb, onlyPossiblePreposition, objectOne, sentence) || !this.grammar.HasArticle(sentence)) &&
                                this.IsPrepositionInFrontOfObject(onlyPossiblePreposition, objectOne, sentence))
                            {
                                SetVerb(possibleVerb, word);
                                break;
                            }
                        }
                        else if (isPrefixOnly)
                        {
                            if (this.IsPrefixTheLastWordInSentence(onlyPossiblePrefix, sentence))
                            {
                                SetVerb(possibleVerb, word);
                                break;
                            }
                        }
                        else if (isPrefixAndPreposition)
                        {
                            if (this.IsObjectInCorrectCaseForPreposition(possibleVerb, onlyPossiblePreposition, objectOne, sentence) &&
                                this.IsPrepositionInFrontOfObject(onlyPossiblePreposition, objectOne, sentence) &&
                                this.IsPrefixTheLastWordInSentence(onlyPossiblePrefix, sentence))
                            {
                                SetVerb(possibleVerb, word);
                                break;
                            }
                        }
                    }
                }
            }

            if (verb != default)
            {
                break;
            }
        }
        
        if (verb != default)
        {
            foreach (var verbVariant in verb.Variants)
            {
                var partToRemove = result.FirstOrDefault(p => p.Equals(verbVariant.Prefix, StringComparison.InvariantCultureIgnoreCase));
                result.Remove(partToRemove);
            }
        }

        return result;
    }

    private bool IsObjectInCorrectCaseForPreposition(Verb possibleVerb, string preposition, string objectOne, IReadOnlyList<string> sentence)
    {
        var prepositionCaseFromVerb = possibleVerb.Prepositions
            .Where(c => c.Name.Equals(preposition, StringComparison.InvariantCultureIgnoreCase)).Select(p => p.Case).SingleOrDefault();

        IEnumerable<string> prepositionCases;
        if (!string.IsNullOrEmpty(prepositionCaseFromVerb))
        {
            prepositionCases = new List<string> { prepositionCaseFromVerb };
        }
        else
        {
            prepositionCases = this.grammar.Prepositions.Where(p =>
                p.Value.Contains(preposition, StringComparer.InvariantCultureIgnoreCase)).Select(x => x.Key);    
        }

        foreach (var prepositionCase in prepositionCases)
        {
            var itemOne = this.objectHandler.GetObjectFromWorldByName(objectOne);
            if (prepositionCase.Equals("DATIVE", StringComparison.InvariantCultureIgnoreCase))
            {
                if (itemOne != default)
                {
                    var article = this.grammar.GetArticleForObject(itemOne, GrammarCase.Dative);
                    if (!string.IsNullOrEmpty(article) &&
                        sentence.Contains(article, StringComparer.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            else if (prepositionCase.Equals("ACCUSATIVE", StringComparison.InvariantCultureIgnoreCase))
            {
                if (itemOne != default)
                {
                    var article = this.grammar.GetArticleForObject(itemOne, GrammarCase.Accusative);
                    if (!string.IsNullOrEmpty(article) &&
                        sentence.Contains(article, StringComparer.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private bool IsPrepositionInFrontOfObject(string preposition, string objectOne, IReadOnlyList<string> sentence)
    {
        var singleWords = sentence.ToList();
        var positionOfPreposition = singleWords.IndexOf(preposition);
        var positionOfNomen = singleWords.IndexOf(objectOne);

        return positionOfPreposition < positionOfNomen;
    }

    private bool IsPrefixTheLastWordInSentence(string prefix, IReadOnlyList<string> sentence)
    {
        var singleWords = sentence.ToList();
        return singleWords.IndexOf(prefix) == singleWords.Count() - 1;
    }
    
    private string GetOnlyPossiblePrefix(ICollection<string> parts, Verb possibleVerb, string verbToReplace)
    {
        var allPrefixes = possibleVerb.Variants
            .Where(v => v.Name.Equals(verbToReplace, StringComparison.InvariantCultureIgnoreCase) &&
                        !string.IsNullOrEmpty(v.Prefix)).Select(v => v.Prefix).Distinct();
        var intersect = parts.Intersect(allPrefixes);
        var onlyPossiblePrefix = intersect.FirstOrDefault();
        return onlyPossiblePrefix;
    }
    
    private string GetOnlyPossiblePreposition(ICollection<string> parts, Verb possibleVerb)
    {
        var onlyPossiblePreposition = parts.Intersect(possibleVerb.Prepositions.Select(p => p.Name)).FirstOrDefault();
        return onlyPossiblePreposition;
    }

    private string GetVerb(IReadOnlyList<string> sentence, ICollection<string> parts)
    {
        foreach (var word in parts)
        {
            if (this.grammar.IsVerb(word, this.universe.ActiveLocation))
            {
                parts.Remove(word);
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
            if (!this.grammar.IsVerb(word, this.universe.ActiveLocation))
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
            if (!this.grammar.IsVerb(word, this.universe.ActiveLocation))
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
    
    private IList<string> SubstituteCombinedPrepositionsAndArticles(IList<string> sentence)
    {
        var result = new List<string>();
        foreach (var word in sentence)
        {
            var (preposition, article) = this.grammar.GetPrepositionAndArticleFromCombinedWord(word);
            if (string.IsNullOrEmpty(preposition) || string.IsNullOrEmpty(article))
            {
                result.Add(word);
            }
            else
            {
                result.Add(preposition);
                result.Add(article);
            }
        }

        return result;
    }

    private string GetFirstObjectNameWithoutWhitespace(AHereticObject item)
    {
        return item.GetNames().FirstOrDefault(i => !i.Contains(" "));
    }
    
    private List<Verb> ExtractPossibleVerbs(string word)
    {
        var verbList = this.universe.ActiveLocation.OptionalVerbs.SelectMany(x => x.Value).ToList();
        
        return ExtractPossibleVerbsFromList(word, this.grammar.Verbs.ToList()).Union(ExtractPossibleVerbsFromList(word, verbList)).ToList();
    }

    private static List<Verb> ExtractPossibleVerbsFromList(string word, List<Verb> verbList)
    {
        var optionalVerbs = verbList
            .Where(v => v.Variants.Select(v => v.Name).Contains(word, StringComparer.InvariantCultureIgnoreCase)).Select(
                v => new Verb()
                {
                    Key = v.Key,
                    ErrorMessage = v.ErrorMessage,
                    Prepositions = v.Prepositions,
                    Variants = v.Variants.Where(vi => vi.Name.Equals(word, StringComparison.InvariantCultureIgnoreCase))
                        .ToList()
                }).ToList();
        return optionalVerbs;
    }
}
