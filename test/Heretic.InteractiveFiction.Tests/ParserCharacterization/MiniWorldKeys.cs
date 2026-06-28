namespace Heretic.InteractiveFiction.Tests.ParserCharacterization;

/// <summary>
/// Stabile Schlüssel der kontrollierten Parser-Test-Welt.
/// Bewusst klein und mit gezielt gewählten Genera/Mehrdeutigkeiten,
/// um die korrektheitskritischen Parser-Fälle abzudecken.
/// </summary>
internal static class MiniWorldKeys
{
    // Items
    internal const string BUCH = "BUCH";                    // das Buch  (Neutrum) – direktes Objekt (Akkusativ)
    internal const string TISCH = "TISCH";                  // der Tisch (Maskulin) – Ablagefläche, Ziel von "auf den Tisch"
    internal const string LAMPE = "LAMPE";                  // die Lampe (Feminin) – mit Alias "Laterne"
    internal const string SCHLUESSEL_ROT = "SCHLUESSEL_ROT";   // der rote Schlüssel  – Mehrdeutigkeit
    internal const string SCHLUESSEL_BLAU = "SCHLUESSEL_BLAU"; // der blaue Schlüssel – Mehrdeutigkeit

    // Characters
    internal const string ZWERG = "ZWERG";                  // der Zwerg (Maskulin) – indirektes Objekt (Dativ)

    // Locations
    internal const string HALLE = "HALLE";                  // die Halle (Feminin) – Startort
    internal const string KAMMER = "KAMMER";                // die Kammer (Feminin) – per Richtung erreichbar

    // Player
    internal const string SPIELER = "SPIELER";
}
