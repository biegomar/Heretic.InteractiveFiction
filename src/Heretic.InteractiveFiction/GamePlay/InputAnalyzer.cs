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
        var normalizedInput = input.Trim().Replace(", ", ",");
        var sentence = normalizedInput.Split(' ');
        sentence = sentence.Where(x => !this.universe.PackingWordsResources.Contains(x, StringComparer.InvariantCultureIgnoreCase)).ToArray();

        sentence = this.OrderSentence(sentence);

        return sentence;
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
