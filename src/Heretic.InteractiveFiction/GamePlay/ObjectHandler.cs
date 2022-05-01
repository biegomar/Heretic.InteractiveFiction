using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class ObjectHandler
{
    private readonly Universe universe;

    internal ObjectHandler(Universe universe)
    {
        this.universe = universe;
    }
    
    internal string GetCharacterKeyByName(string itemName)
    {
        foreach (var (key, value) in this.universe.CharacterResources)
        {
            if (value.Contains(itemName, StringComparer.InvariantCultureIgnoreCase))
            {
                return key;
            }
        }

        return string.Empty;
    }
}