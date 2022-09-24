﻿using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Grammars;

public interface IGrammar
{
    public IDictionary<string, IEnumerable<string>> Prepositions { get; }
    public bool IsPronounActiveObject(AHereticObject activeObject, string pronoun);
    public string GetArticleForObject(AHereticObject processingObject);
    public string GetNominativeIndefiniteArticleForObject(AHereticObject processingObject);
    public string GetDativeIndefiniteArticleForObject(AHereticObject processingObject);
    public string GetDativeArticleForObject(AHereticObject processingObject);
    public string GetAccusativeIndefiniteArticleForObject(AHereticObject processingObject);
    public string GetAccusativeArticleForObject(AHereticObject processingObject);
    
    public string GetNominativePronounForObject(AHereticObject processingObject);
    public string GetDativePronounForObject(AHereticObject processingObject);
    public string GetAccusativePronounForObject(AHereticObject processingObject);

    public (string preposition, string article) GetPrepositionAndArticleFromCombinedWord(string word);
}