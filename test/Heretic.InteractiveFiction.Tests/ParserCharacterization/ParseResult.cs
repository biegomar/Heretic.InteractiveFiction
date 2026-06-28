using Heretic.InteractiveFiction.Grammars;

namespace Heretic.InteractiveFiction.Tests.ParserCharacterization;

internal enum ParseOutcome
{
    /// <summary>Eingabe wurde zu einem AdventureEvent aufgelöst.</summary>
    Success,

    /// <summary>Objektauflösung war mehrdeutig – der Parser stellt eine Rückfrage.</summary>
    Ambiguous,

    /// <summary>Es konnte kein Verb erkannt werden (NoVerbException).</summary>
    NoVerb
}

/// <summary>
/// Gut prüfbare Projektion eines Parse-Ergebnisses für Charakterisierungstests.
/// Hält bewusst nur Schlüssel/Strings statt der vollen Objektgraphen.
/// </summary>
internal sealed record ParseResult(
    ParseOutcome Outcome,
    VerbKey? VerbKey = null,
    string? ObjectOneKey = null,
    string? ObjectTwoKey = null,
    IReadOnlyList<string>? AllObjectKeys = null,
    IReadOnlyList<string>? UnidentifiedParts = null,
    IReadOnlyList<string>? AmbiguousKeys = null);
