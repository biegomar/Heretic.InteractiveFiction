using Heretic.InteractiveFiction.GamePlay;

namespace Heretic.InteractiveFiction.Tests.ParserCharacterization;

/// <summary>
/// Liefert den Wortschatz (Schlüssel → Namen) der kontrollierten Parser-Test-Welt.
/// Verben, Präpositionen und verschmolzene Präposition+Artikel-Formen kommen
/// weiterhin aus den Framework-Ressourcen über die Default-Implementierungen
/// von <see cref="IResourceProvider"/> – nur die drei Inhalts-Methoden werden überschrieben.
/// </summary>
internal sealed class MiniWorldResourceProvider : IResourceProvider
{
    public IDictionary<string, IEnumerable<string>> GetItemsFromResources() =>
        new Dictionary<string, IEnumerable<string>>
        {
            { MiniWorldKeys.BUCH, new[] { "Buch" } },
            { MiniWorldKeys.TISCH, new[] { "Tisch" } },
            { MiniWorldKeys.LAMPE, new[] { "Lampe", "Laterne" } },
            { MiniWorldKeys.SCHLUESSEL_ROT, new[] { "Schlüssel" } },
            { MiniWorldKeys.SCHLUESSEL_BLAU, new[] { "Schlüssel" } },
        };

    public IDictionary<string, IEnumerable<string>> GetCharactersFromResources() =>
        new Dictionary<string, IEnumerable<string>>
        {
            { MiniWorldKeys.ZWERG, new[] { "Zwerg" } },
        };

    public IDictionary<string, IEnumerable<string>> GetLocationsFromResources() =>
        new Dictionary<string, IEnumerable<string>>
        {
            { MiniWorldKeys.HALLE, new[] { "Halle" } },
            { MiniWorldKeys.KAMMER, new[] { "Kammer" } },
        };
}
