﻿using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Grammars;

public interface IGrammar
{
    public IList<Verb> Verbs { get; }
    public IDictionary<string, IEnumerable<string>> Prepositions { get; }
    public (string preposition, string article) GetPrepositionAndArticleFromCombinedWord(string word);
    public bool HasPrepositionOrPrefix(IEnumerable<string> sentence);
    public bool IsVerb(string verbKeyToCheck, Location location);
    public Verb GetVerb(string verbKeyToCheck, Location location);
}