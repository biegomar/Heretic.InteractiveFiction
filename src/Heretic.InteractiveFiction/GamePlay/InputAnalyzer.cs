using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class InputAnalyzer
{
    private readonly Universe universe;
    private readonly ObjectHandler objectHandler;
    private readonly IGrammar grammar;
    
    private sealed class ObjectAndAssociatedWord
    {
        public AHereticObject? HereticObject { get; init; }
        public string AssociatedWord { get; init; } = string.Empty;
    }

    internal InputAnalyzer(Universe universe, IGrammar grammar)
    {
        this.universe = universe;
        this.objectHandler = new ObjectHandler(universe);
        this.grammar = grammar;
    }

    internal AdventureEvent AnalyzeInput(string input)
    {
        var stringToAnalyze = input;
        
        var normalizedInput = stringToAnalyze.Trim().Replace(", ", ",").Replace(",", " ");
        var sentence = normalizedInput.Split(' ');
        sentence = sentence.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        sentence = this.SubstitutePronoun(sentence).ToArray();
        sentence = this.SubstituteCombinedPrepositionsAndArticles(sentence).ToArray();

        return this.AnalyzeSentence(sentence);
    }

    private AdventureEvent AnalyzeSentence(IReadOnlyList<string> sentence)
    {
        AdventureEvent result = new();
        var parts = sentence.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        if (parts.Any())
        {
            var objectOne = this.GetObjectForRequestAndRemoveFromParts<Player>(parts);
            if (objectOne.HereticObject == default)
            {
                objectOne = this.GetObjectForRequestAndRemoveFromParts<Character>(parts);
                if (objectOne.HereticObject == default)
                {
                    objectOne = this.GetObjectForRequestAndRemoveFromParts<Item>(parts);
                    if (objectOne.HereticObject == default)
                    {
                        objectOne = this.GetObjectForRequestAndRemoveFromParts<Location>(parts);
                    }
                }
            }

            ObjectAndAssociatedWord? objectTwo = null;
            
            if (objectOne.HereticObject != default)
            {
                result.AllObjects.Add(objectOne.HereticObject);
                
                if (parts.Any())
                {
                    ObjectAndAssociatedWord? singleObject;

                    do
                    {
                        singleObject = this.GetObjectForRequestAndRemoveFromParts<Item>(parts);
                        if (singleObject.HereticObject == default)
                        {
                            singleObject = this.GetObjectForRequestAndRemoveFromParts<Player>(parts);
                            if (singleObject.HereticObject == default)
                            {
                                singleObject = this.GetObjectForRequestAndRemoveFromParts<Character>(parts);
                                if (singleObject.HereticObject == default)
                                {
                                    //objectTwo = this.GetConversationAnswer(parts);
                                }
                            }
                        }

                        if (singleObject.HereticObject != default)
                        {
                            objectTwo ??= singleObject;
                            result.AllObjects.Add(singleObject.HereticObject);
                        }

                    } while (singleObject.HereticObject != default);
                }
            }

            if (parts.Any())
            {
                (var predicate, parts) = this.GetVerbAndRemoveFromParts(sentence, parts, objectOne, objectTwo);
                result.Predicate = predicate;

                if (parts.Any())
                {
                    foreach (var hereticObject in result.AllObjects)
                    {
                        if (parts.Any())
                        {
                            RemoveObjectArticlesFromParts(hereticObject, parts);
                        }
                    }   
                }

                if (parts.Any())
                {
                    RemovePrepositionsFromParts(parts);
                }

                if (parts.Any())
                {
                    result.UnidentifiedSentenceParts.AddRange(parts);
                }
            }
            else
            {
                throw new NoVerbException();
            }
        }

        return result;
    }
    
    private void RemoveObjectArticlesFromParts(AHereticObject processingObject, ICollection<string> parts)
    {
        var partToRemove = parts.FirstOrDefault(p => p.Equals(ArticleHandler.GetArticleForObject(processingObject, GrammarCase.Nominative), StringComparison.InvariantCultureIgnoreCase));
        parts.Remove(partToRemove);
        partToRemove = parts.FirstOrDefault(p => p.Equals(ArticleHandler.GetArticleForObject(processingObject, GrammarCase.Nominative, ArticleState.Indefinite), StringComparison.InvariantCultureIgnoreCase));
        parts.Remove(partToRemove);
            
        partToRemove = parts.FirstOrDefault(p => p.Equals(ArticleHandler.GetArticleForObject(processingObject, GrammarCase.Genitive), StringComparison.InvariantCultureIgnoreCase));
        parts.Remove(partToRemove);
        partToRemove = parts.FirstOrDefault(p => p.Equals(ArticleHandler.GetArticleForObject(processingObject, GrammarCase.Genitive, ArticleState.Indefinite), StringComparison.InvariantCultureIgnoreCase));
        parts.Remove(partToRemove);
            
        partToRemove = parts.FirstOrDefault(p => p.Equals(ArticleHandler.GetArticleForObject(processingObject, GrammarCase.Dative), StringComparison.InvariantCultureIgnoreCase));
        parts.Remove(partToRemove);
        partToRemove = parts.FirstOrDefault(p => p.Equals(ArticleHandler.GetArticleForObject(processingObject, GrammarCase.Dative, ArticleState.Indefinite), StringComparison.InvariantCultureIgnoreCase));
        parts.Remove(partToRemove);
            
        partToRemove = parts.FirstOrDefault(p => p.Equals(ArticleHandler.GetArticleForObject(processingObject, GrammarCase.Accusative), StringComparison.InvariantCultureIgnoreCase));
        parts.Remove(partToRemove);
        partToRemove = parts.FirstOrDefault(p => p.Equals(ArticleHandler.GetArticleForObject(processingObject, GrammarCase.Accusative, ArticleState.Indefinite), StringComparison.InvariantCultureIgnoreCase));
        parts.Remove(partToRemove);
    }

    private void RemovePrepositionsFromParts(ICollection<string> parts)
    {
        var allPrepositions = this.grammar.Prepositions.Values.SelectMany(x => x);
        foreach (var preposition in allPrepositions)
        {
            parts.Remove(preposition);
        }
    }

    private (Verb verb, List<string> newParts) GetVerbAndRemoveFromParts(IReadOnlyList<string> sentence, ICollection<string> parts, ObjectAndAssociatedWord? objectOne, ObjectAndAssociatedWord? objectTwo)
    {
        Verb? verb = null;
        var result = parts.ToList();
        
        void SetVerb(Verb possibleVerb, string word)
        {
            verb = possibleVerb;
            result.Remove(word);
        }
        
        var isPrepositionOrPrefixPresentInSentence = this.grammar.HasPrepositionOrPrefix(sentence);

        foreach (var word in parts)
        {
            var possibleVerbsAndVariants = ExtractPossibleVerbs(word);
            
            if (possibleVerbsAndVariants.Any())
            {
                if (possibleVerbsAndVariants.Count == 1)
                {
                    SetVerb(possibleVerbsAndVariants.Single(), word);
                }
                else if (isPrepositionOrPrefixPresentInSentence)
                {
                    foreach (var possibleVerb in possibleVerbsAndVariants)
                    {
                        var onlyPossiblePrefix = GetOnlyPossiblePrefix(parts, possibleVerb, word);
                        var onlyPossiblePreposition = GetOnlyPossiblePreposition(parts, possibleVerb);

                        var isPrefixOnly = !string.IsNullOrEmpty(onlyPossiblePrefix) && string.IsNullOrEmpty(onlyPossiblePreposition);
                        var isPrepositionOnly = string.IsNullOrEmpty(onlyPossiblePrefix) && !string.IsNullOrEmpty(onlyPossiblePreposition);
                        var isPrefixAndPreposition = !string.IsNullOrEmpty(onlyPossiblePrefix) && !string.IsNullOrEmpty(onlyPossiblePreposition);
                        
                        if (objectOne != null)
                        {
                            bool isNoArticlePresent = true;
                            bool isObjectInCorrectCaseForPreposition = false;
                            var isPrepositionInFrontOfObject =
                                this.IsPrepositionInFrontOfObject(onlyPossiblePreposition, objectOne, sentence);
                            if (isPrepositionInFrontOfObject)
                            {
                                isObjectInCorrectCaseForPreposition =
                                    this.IsObjectInCorrectCaseForPreposition(possibleVerb, onlyPossiblePreposition,
                                        objectOne, sentence);
                                if (objectOne.HereticObject != null)
                                {
                                    isNoArticlePresent = !ArticleHandler.HasArticleInFrontOfObject(sentence,
                                        objectOne.HereticObject, objectOne.AssociatedWord);
                                }
                            }
                            else
                            {
                                if (objectTwo != null)
                                {
                                    isPrepositionInFrontOfObject =
                                        this.IsPrepositionInFrontOfObject(onlyPossiblePreposition, objectTwo, sentence);
                                    isObjectInCorrectCaseForPreposition =
                                        this.IsObjectInCorrectCaseForPreposition(possibleVerb, onlyPossiblePreposition,
                                            objectTwo, sentence);
                                    if (objectTwo.HereticObject != null)
                                    {
                                        isNoArticlePresent = !ArticleHandler.HasArticleInFrontOfObject(sentence,
                                            objectTwo.HereticObject, objectTwo.AssociatedWord);
                                    }
                                }
                            }

                            if (isPrepositionOnly)
                            {
                                if ((isObjectInCorrectCaseForPreposition || isNoArticlePresent) &&
                                    isPrepositionInFrontOfObject)
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
                                if (isObjectInCorrectCaseForPreposition &&
                                    isPrepositionInFrontOfObject &&
                                    this.IsPrefixTheLastWordInSentence(onlyPossiblePrefix, sentence))
                                {
                                    SetVerb(possibleVerb, word);
                                    break;
                                }
                            }
                        }
                    }
                }
                else 
                {
                    var onlyPossibleVerbWithoutPrefix = possibleVerbsAndVariants.SingleOrDefault(v => v.Variants.Count(x => x.Prefix == string.Empty) > 0);
                    if (onlyPossibleVerbWithoutPrefix != default)
                    {
                        SetVerb(onlyPossibleVerbWithoutPrefix, word);
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
        else
        {
            throw new NoVerbException();
        }

        return (verb, result);
    }
    
    private bool IsObjectInCorrectCaseForPreposition(Verb possibleVerb, string preposition, ObjectAndAssociatedWord objectOne, IReadOnlyList<string> sentence)
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
            if (prepositionCase.Equals("DATIVE", StringComparison.InvariantCultureIgnoreCase))
            {
                if (objectOne is { HereticObject: { } })
                {
                    var article = ArticleHandler.GetArticleForObject(objectOne.HereticObject, GrammarCase.Dative);
                    if (!string.IsNullOrEmpty(article) &&
                        sentence.Contains(article, StringComparer.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            else if (prepositionCase.Equals("ACCUSATIVE", StringComparison.InvariantCultureIgnoreCase))
            {
                if (objectOne is { HereticObject: { } })
                {
                    var article = ArticleHandler.GetArticleForObject(objectOne.HereticObject, GrammarCase.Accusative);
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

    private bool IsPrepositionInFrontOfObject(string preposition, ObjectAndAssociatedWord objectOne, IEnumerable<string> sentence)
    {
        if (string.IsNullOrEmpty(preposition) || objectOne == default || objectOne.HereticObject == default)
        {
            return false;
        }
        var singleWords = sentence.ToList();
        var positionOfPreposition = singleWords.IndexOf(preposition);
        var positionOfNomen = singleWords.IndexOf(objectOne.AssociatedWord);

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
        return onlyPossiblePrefix ?? string.Empty;
    }
    
    private string GetOnlyPossiblePreposition(ICollection<string> parts, Verb possibleVerb)
    {
        var prepositions = possibleVerb.Prepositions.Select(p => p.Name);
        var onlyPossiblePreposition = parts.Intersect(prepositions).FirstOrDefault();
        return onlyPossiblePreposition ?? string.Empty;
    }

    private ObjectAndAssociatedWord GetObjectForRequestAndRemoveFromParts<T>(ICollection<string> sentence) where T: AHereticObject?
    {
        T? discoveredObject = default;
        string associatedWord = string.Empty;
        
        foreach (var word in sentence)
        {
            var key = this.objectHandler.GetObjectKeyByNameAndAdjectives<T>(word, sentence);
            if (!string.IsNullOrEmpty(key))
            {
                discoveredObject = this.objectHandler.GetObjectFromWorldByKey<T>(key);
                if (discoveredObject != default)
                {
                    associatedWord = word;
                    break;    
                }
            }
        }
        
        if (associatedWord != string.Empty)
        {
            sentence.Remove(associatedWord);
        }

        if (discoveredObject != null)
        {
            AdjectiveDeclinationHandler.RemoveAdjectivesFromParts(discoveredObject, sentence);
        }
        
        return new ObjectAndAssociatedWord
        {
            HereticObject = discoveredObject,
            AssociatedWord = associatedWord
        };
    }

    private IList<string> SubstitutePronoun(IList<string> sentence)
    {
        var result = new List<string>();
        foreach (var word in sentence)
        {
            if (PronounHandler.IsPronounRepresentingActiveObject(this.universe.ActivePlayer, word))
            {
                result.Add(word);
            }
            else if (PronounHandler.IsPronounRepresentingActiveObject(this.universe.ActiveObject, word))
            {
                if (GetFirstObjectNameWithoutWhitespace(this.universe.ActiveObject) is {} objectName)
                {
                    result.Add(objectName);
                }
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

    private string? GetFirstObjectNameWithoutWhitespace(AHereticObject? item)
    {
        if (item != null)
        {
            return item.GetNames().FirstOrDefault(i => !i.Contains(" "));    
        }

        return string.Empty;
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
