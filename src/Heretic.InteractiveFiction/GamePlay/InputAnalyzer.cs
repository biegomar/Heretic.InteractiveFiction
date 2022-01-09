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

        var verb = sentence[0];
        var subject = string.Empty;
        var character = string.Empty;
        var isCharacterSet = false;

        if (sentence.Count == 2)
        {
            subject = sentence[1];
        }
        else if (sentence.Count == 3)
        {
            character = sentence[1];
            subject = sentence[2];
        }

        foreach (var word in sentence)
        {
            if (this.universe.VerbResources.Values.SelectMany(x => x).Contains(word, StringComparer.InvariantCultureIgnoreCase))
            {
                verb = word;
            }
            else if (this.universe.CharacterResources.Values.SelectMany(x => x).Contains(word, StringComparer.InvariantCultureIgnoreCase) && !isCharacterSet)
            {
                character = word;
                isCharacterSet = true;
            }
            else if (this.universe.ItemResources.Values.SelectMany(x => x).Contains(word, StringComparer.InvariantCultureIgnoreCase))
            {
                subject = word;
            }
            else if (this.universe.ConversationAnswersResources.Values.SelectMany(x => x).Contains(word, StringComparer.InvariantCultureIgnoreCase))
            {
                subject = word;
            }
            else if (sentence.Count == 3)
            {
                subject = word;
            }
        }

        orderedSentence[0] = verb;
        if (orderedSentence.Length == 2)
        {
            orderedSentence[1] = subject;
        }
        else if (sentence.Count == 3)
        {
            orderedSentence[1] = character;
            orderedSentence[2] = subject;
        }

        return orderedSentence;
    }
}
