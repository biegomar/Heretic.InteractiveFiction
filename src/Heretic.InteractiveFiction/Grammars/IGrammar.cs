using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Grammars;

public interface IGrammar
{
    public IList<Verb> Verbs { get; }
    public IDictionary<string, IEnumerable<string>> Prepositions { get; }
    public bool IsPronounActiveObject(AHereticObject activeObject, string pronoun);
    public string GetArticleForObject(AHereticObject processingObject, GrammarCase grammarCase, ArticleState articleState = ArticleState.Definite);
    public string GetNominativePronounForObject(AHereticObject processingObject);
    public string GetDativePronounForObject(AHereticObject processingObject);
    public string GetAccusativePronounForObject(AHereticObject processingObject);

    public (string preposition, string article) GetPrepositionAndArticleFromCombinedWord(string word);

    public bool HasPrepositionOrPrefix(IEnumerable<string> sentence);
    public bool HasArticle(IEnumerable<string> sentence);

    public bool IsVerb(string verbToCheck, Location location);
    public Verb GetVerb(string verbToCheck, Location location);
}