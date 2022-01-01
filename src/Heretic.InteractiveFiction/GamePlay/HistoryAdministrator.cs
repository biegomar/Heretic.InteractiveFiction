using System.Collections.ObjectModel;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class HistoryAdministrator
{
    private readonly ICollection<string> historyCollection;
    private readonly List<string> allRepeatableWords;
    private readonly InputAnalyzer inputAnalyzer;

    internal HistoryAdministrator(InputAnalyzer inputAnalyzer)
    {
        this.inputAnalyzer = inputAnalyzer;
        this.allRepeatableWords = this.GetAllRepeatableVerbs();
        this.historyCollection = new List<string>();
    }

    internal bool Any => this.historyCollection.Any();

    internal ReadOnlyCollection<string> All => new(this.historyCollection.ToList());
    internal void Add(string input)
    {
        this.historyCollection.Add(input);
    }

    internal bool IsLastHistoryEntryTheSame(string[] sentence)
    {
        if (!this.historyCollection.Any())
        {
            return false;
        }

        var last = this.historyCollection.Last();
        var lastSentence = this.inputAnalyzer.Analyze(last);

        if (!this.allRepeatableWords.Contains(lastSentence[0], StringComparer.CurrentCultureIgnoreCase))
        {
            return lastSentence.SequenceEqual(sentence);
        }

        return false;
    }

    private List<string> GetAllRepeatableVerbs()
    {
        var resourceManager = Verbs.ResourceManager;
        var verbs = new string[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW", "UP", "DOWN", "TALK", "WRITE" };
        List<string> allRepeatableVerbs = new List<string>();

        foreach (var verb in verbs)
        {
            var names = resourceManager.GetString(verb);
            var singleNames = names!.Split('|');
            allRepeatableVerbs.AddRange(singleNames);
        }

        return allRepeatableVerbs;
    }
}
