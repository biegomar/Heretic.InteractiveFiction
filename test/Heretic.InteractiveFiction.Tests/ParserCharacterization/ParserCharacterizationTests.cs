using FluentAssertions;
using Heretic.InteractiveFiction.Grammars;
using Xunit;

namespace Heretic.InteractiveFiction.Tests.ParserCharacterization;

/// <summary>
/// Charakterisierungstests (Phase 0): Sie nageln das <b>aktuelle</b> Verhalten des
/// Parsers fest – inklusive seiner Schwächen – damit eine künftige Neuimplementierung
/// verhaltensgleich starten kann. Verbesserungen nehmen wir danach bewusst vor und
/// passen die betroffenen Tests gezielt an.
///
/// Tests, die eine dokumentierte <b>Schwäche</b> festhalten, sind mit "WEAKNESS"
/// markiert (siehe docs/parser-architektur.md, Abschnitt 5).
/// </summary>
public sealed class ParserCharacterizationTests
{
    // Die Welt ist für reines Parsen zustandslos -> einmal bauen, überall nutzen (schnell).
    private static readonly MiniWorld World = new();

    private static ParseResult Parse(string input) => World.Parse(input);

    // ------------------------------------------------------------------
    // Objekterkennung: Artikel optional, Aliasnamen
    // ------------------------------------------------------------------

    [Theory]
    [InlineData("nimm das buch")]
    [InlineData("nimm buch")]
    public void Take_ResolvesDirectObject_WithOrWithoutArticle(string input)
    {
        var result = Parse(input);

        result.Outcome.Should().Be(ParseOutcome.Success);
        result.VerbKey.Should().Be(VerbKey.TAKE);
        result.ObjectOneKey.Should().Be(MiniWorldKeys.BUCH);
        result.ObjectTwoKey.Should().BeNull();
    }

    [Theory]
    [InlineData("untersuche die lampe")]
    [InlineData("untersuche laterne")] // Aliasname "Laterne" löst dasselbe Objekt auf
    public void Look_ResolvesObject_ByNameOrAlias(string input)
    {
        var result = Parse(input);

        result.Outcome.Should().Be(ParseOutcome.Success);
        result.VerbKey.Should().Be(VerbKey.LOOK);
        result.ObjectOneKey.Should().Be(MiniWorldKeys.LAMPE);
    }

    // ------------------------------------------------------------------
    // Satzrollen / Objektzuordnung
    // ------------------------------------------------------------------

    [Theory]
    [InlineData("gib dem zwerg das buch")]
    [InlineData("gib das buch dem zwerg")]
    public void Give_AssignsCharacterAsObjectOne_RegardlessOfWordOrder(string input)
    {
        // WEAKNESS (Abschnitt 5.1): obj1 = Zwerg ist hier NICHT das Ergebnis einer
        // Kasus-/Rollenanalyse (Dativ = indirektes Objekt), sondern entsteht nur, weil
        // der Analyzer Charaktere VOR Items als ObjectOne probiert. Für GIVE stimmt das
        // zufällig; ein rollenbasierter Parser sollte Zwerg über den Dativ als
        // indirektes Objekt erkennen – unabhängig von Wortreihenfolge UND Typ.
        var result = Parse(input);

        result.Outcome.Should().Be(ParseOutcome.Success);
        result.VerbKey.Should().Be(VerbKey.GIVE);
        result.ObjectOneKey.Should().Be(MiniWorldKeys.ZWERG);
        result.ObjectTwoKey.Should().Be(MiniWorldKeys.BUCH);
    }

    [Theory]
    [InlineData("lege das buch auf den tisch", MiniWorldKeys.BUCH, MiniWorldKeys.TISCH)]
    [InlineData("lege buch auf tisch", MiniWorldKeys.BUCH, MiniWorldKeys.TISCH)]
    // WEAKNESS (Abschnitt 5.1): Bei zwei gleichartigen Objekten (beide Items) entscheidet
    // allein die Wortreihenfolge über obj1/obj2 – nicht der Kasus. "den Tisch" ist hier
    // Akkusativ-Ziel der Präposition und müsste rollenbasiert das Präpositionalobjekt sein.
    [InlineData("lege den tisch auf das buch", MiniWorldKeys.TISCH, MiniWorldKeys.BUCH)]
    public void PutOn_TwoItems_RolesFollowWordOrder(string input, string expectedObjectOne, string expectedObjectTwo)
    {
        var result = Parse(input);

        result.Outcome.Should().Be(ParseOutcome.Success);
        result.VerbKey.Should().Be(VerbKey.PUTON);
        result.ObjectOneKey.Should().Be(expectedObjectOne);
        result.ObjectTwoKey.Should().Be(expectedObjectTwo);
    }

    // ------------------------------------------------------------------
    // Verb-Disambiguierung über Präposition + Kasus
    // ------------------------------------------------------------------

    [Fact]
    public void Lege_WithAccusativePreposition_DisambiguatesToPutOn_NotDrop()
    {
        // "Lege" ist sowohl PUTON (Lege) als auch DROP (Lege:hin/ab). Die Präposition
        // "auf" + Akkusativ ("den Tisch") führt korrekt zu PUTON statt DROP.
        var result = Parse("lege das buch auf den tisch");

        result.VerbKey.Should().Be(VerbKey.PUTON);
    }

    // ------------------------------------------------------------------
    // Mehrdeutigkeit / Adjektive
    // ------------------------------------------------------------------

    [Fact]
    public void Take_AmbiguousObject_ReportsBothCandidates()
    {
        var result = Parse("nimm den schlüssel");

        result.Outcome.Should().Be(ParseOutcome.Ambiguous);
        result.AmbiguousKeys.Should().BeEquivalentTo(
            new[] { MiniWorldKeys.SCHLUESSEL_ROT, MiniWorldKeys.SCHLUESSEL_BLAU });
    }

    [Fact]
    public void Take_AmbiguousObject_DisambiguatedByAdjective()
    {
        var result = Parse("nimm den roten schlüssel");

        result.Outcome.Should().Be(ParseOutcome.Success);
        result.VerbKey.Should().Be(VerbKey.TAKE);
        result.ObjectOneKey.Should().Be(MiniWorldKeys.SCHLUESSEL_ROT);
    }

    // ------------------------------------------------------------------
    // Verben ohne aufgelöstes Objekt
    // ------------------------------------------------------------------

    [Fact]
    public void Go_LeavesDirectionInUnidentifiedParts()
    {
        // WEAKNESS (Abschnitt 5.2): Richtungen sind keine Objekte; "norden" landet als
        // unidentifizierter Rest. Die Richtungsauflösung passiert erst im GoCommand.
        var result = Parse("geh nach norden");

        result.Outcome.Should().Be(ParseOutcome.Success);
        result.VerbKey.Should().Be(VerbKey.GO);
        result.AllObjectKeys.Should().BeEmpty();
        result.UnidentifiedParts.Should().Contain("norden");
    }

    [Fact]
    public void BareVerb_ParsesWithoutObject()
    {
        var result = Parse("öffne");

        result.Outcome.Should().Be(ParseOutcome.Success);
        result.VerbKey.Should().Be(VerbKey.OPEN);
        result.ObjectOneKey.Should().BeNull();
    }

    [Fact]
    public void UnknownVerb_EvenWithValidObject_YieldsNoVerb()
    {
        // WEAKNESS (Abschnitt 5.2): "tanze" ist kein bekanntes Verb -> NoVerbException,
        // obwohl "tisch" ein gültiges Objekt wäre. Es gibt keine differenzierte Diagnose.
        var result = Parse("tanze auf dem tisch");

        result.Outcome.Should().Be(ParseOutcome.NoVerb);
    }

    // ------------------------------------------------------------------
    // Multi-Objekt
    // ------------------------------------------------------------------

    [Fact]
    public void MultiObject_CollectsAllObjects_ButLeavesConjunctionAsNoise()
    {
        // Beide Objekte werden erkannt (AllObjects), aber nur obj1/obj2 sind exponiert,
        // und das "und" bleibt als unidentifizierter Rest stehen.
        var result = Parse("nimm das buch und die lampe");

        result.Outcome.Should().Be(ParseOutcome.Success);
        result.VerbKey.Should().Be(VerbKey.TAKE);
        result.AllObjectKeys.Should().Equal(MiniWorldKeys.BUCH, MiniWorldKeys.LAMPE);
        result.UnidentifiedParts.Should().Contain("und");
    }
}
