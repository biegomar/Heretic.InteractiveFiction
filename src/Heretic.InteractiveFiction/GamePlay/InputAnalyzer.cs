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
        const string block = "##TEXT##";
        var stringToAnalyze = string.Empty;
        var quotedText = string.Empty;

        try
        {
            var quotedTextArray = input.Split('"');
            if (quotedTextArray.Length > 1)
            {
                for (int i = 0; i < quotedTextArray.Length; i++)
                {
                    if (i == 1)
                    {
                        quotedText = quotedTextArray[1].Trim();
                        stringToAnalyze += block;
                        stringToAnalyze += " ";
                    }
                    else
                    {
                        stringToAnalyze += quotedTextArray[i].Trim();
                        stringToAnalyze += " ";
                    }
                }
            }
            else
            {
                stringToAnalyze = input;
            }
        
            var normalizedInput = stringToAnalyze.Trim().Replace(", ", ",");
            var sentence = normalizedInput.Split(' ');
            sentence = sentence.Where(x => !this.universe.PackingWordsResources.Contains(x, StringComparer.InvariantCultureIgnoreCase)).ToArray();

            sentence = this.OrderSentence(sentence).ToArray();

            if (!string.IsNullOrEmpty(quotedText))
            {
                var index = Array.IndexOf(sentence, block);
                if (index > -1)
                {
                    sentence[index] = quotedText;
                }
            }
            
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
        parts.Remove(verb);
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

    private string GetVerb(IList<string> sentence)
    {
        foreach (var word in sentence)
        {
            if (this.universe.IsVerb(word))
            {
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
    
}
