using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Grammars;

public class GermanGrammar: IGrammar
{
    public bool IsPronounActiveObject(AHereticObject activeObject, string pronoun)
    {
        var upperItemName = pronoun.ToUpperInvariant();
        
        if (activeObject != null && (upperItemName == activeObject.Grammar.GetAccusativePronoun().ToUpperInvariant() 
                                     || upperItemName == activeObject.Grammar.GetDativePronoun().ToUpperInvariant()  
                                     || upperItemName == activeObject.Grammar.GetNominativePronoun().ToUpperInvariant()))
        {
            return true;
        }

        return false;
    }
}