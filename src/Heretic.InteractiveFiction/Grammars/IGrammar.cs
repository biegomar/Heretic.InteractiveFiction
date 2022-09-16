using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Grammars;

public interface IGrammar
{
    bool IsPronounActiveObject(AHereticObject activeObject, string pronoun);
}