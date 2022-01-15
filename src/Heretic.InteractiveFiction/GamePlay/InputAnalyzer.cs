using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class InputAnalyzer
{
    private readonly Universe universe;

    internal InputAnalyzer(Universe universe)
    {
        this.universe = universe;
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

            sentence = this.OrderSentence(sentence);

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

    private string[] OrderSentence(IReadOnlyList<string> sentence)
    {
        var orderedSentence = new string[sentence.Count];
        string itemObject;

        var parts = sentence.ToList();
        var verb = this.GetVerb(parts);
        parts.Remove(verb);
        orderedSentence[0] = verb;
        
        if (sentence.Count == 2)
        {
            itemObject = this.GetItem(parts);
            if (itemObject == string.Empty)
            {
                itemObject = this.GetCharacter(parts);
                if (itemObject== string.Empty)
                {
                    itemObject = this.GetConversationAnswer(parts);
                    if (itemObject== string.Empty)
                    {
                        itemObject = parts[0];
                    }
                }
            }
            
            orderedSentence[1] = itemObject;
        }
        else if (sentence.Count == 3)
        {
            itemObject = this.GetCharacter(parts);
            if (itemObject == string.Empty)
            {
                itemObject = this.GetItem(parts);
                if (itemObject== string.Empty)
                {
                    itemObject = parts[0];
                }
            }
            parts.Remove(itemObject);
            orderedSentence[1] = itemObject;
            
            
            var subject = this.GetItem(parts);
            if (subject == string.Empty)
            {
                subject = this.GetCharacter(parts);
                if (subject== string.Empty)
                {
                    subject = this.GetConversationAnswer(parts);
                    if (subject== string.Empty)
                    {
                        subject = parts[0];
                    }
                }
            }
            orderedSentence[2] = subject;
        }
        
        return orderedSentence;
    }
    
    private string GetVerb(IEnumerable<string> sentence)
    {
        foreach (var word in sentence)
        {
            if (this.universe.VerbResources.Values.SelectMany(x => x).Contains(word, StringComparer.InvariantCultureIgnoreCase))
            {
                return word;
            }
        }

        throw new NoVerbException();
    }
    
    private string GetCharacter(IEnumerable<string> sentence)
    {
        foreach (var word in sentence)
        {
            if (this.universe.CharacterResources.Values.SelectMany(x => x).Contains(word, StringComparer.InvariantCultureIgnoreCase))
            {
                return word;
            }
        }

        return string.Empty;
    }
    
    private string GetItem(IEnumerable<string> sentence)
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
    
    private string GetConversationAnswer(IEnumerable<string> sentence)
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
    
}
