using Heretic.InteractiveFiction.Comparer;
using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using Moq;

namespace Heretic.InteractiveFiction.Tests.ParserCharacterization;

/// <summary>
/// Baut eine vollständige, aber winzige <see cref="Universe"/> für Parser-Tests
/// und treibt den <see cref="InputAnalyzer"/> über <see cref="Parse"/> an.
///
/// Welt:
///   Halle (Start) ── N ──&gt; Kammer
///   In der Halle: Buch (n), Tisch (m, Ablage), Lampe (f),
///                 roter + blauer Schlüssel (mehrdeutig), Zwerg (Charakter).
/// </summary>
internal sealed class MiniWorld
{
    private readonly InputAnalyzer analyzer;

    public Universe Universe { get; }
    public IGrammar Grammar { get; }

    public MiniWorld()
    {
        var resourceProvider = new MiniWorldResourceProvider();
        var printing = Mock.Of<IPrintingSubsystem>();

        this.Universe = new Universe(printing, resourceProvider);
        var verbHandler = new GermanVerbHandler(this.Universe, resourceProvider);
        this.Grammar = new GermanGrammar(resourceProvider, verbHandler);

        BuildWorld();

        this.analyzer = new InputAnalyzer(this.Universe, this.Grammar);
    }

    /// <summary>Analysiert eine Eingabe und projiziert das Ergebnis auf gut prüfbare Schlüssel.</summary>
    public ParseResult Parse(string input)
    {
        try
        {
            var result = this.analyzer.AnalyzeInput(input);
            return result switch
            {
                AnalysisResult.Ambiguous ambiguous => new ParseResult(
                    Outcome: ParseOutcome.Ambiguous,
                    AmbiguousKeys: ambiguous.Candidates.Select(c => c.Key).ToList()),

                AnalysisResult.Success success => new ParseResult(
                    Outcome: ParseOutcome.Success,
                    VerbKey: success.Event.Predicate?.Key,
                    ObjectOneKey: success.Event.ObjectOne?.Key,
                    ObjectTwoKey: success.Event.ObjectTwo?.Key,
                    AllObjectKeys: success.Event.AllObjects.Select(o => o.Key).ToList(),
                    UnidentifiedParts: success.Event.UnidentifiedSentenceParts.ToList()),

                _ => new ParseResult(ParseOutcome.Success)
            };
        }
        catch (NoVerbException)
        {
            return new ParseResult(ParseOutcome.NoVerb);
        }
    }

    private void BuildWorld()
    {
        var halle = new Location
        {
            Key = MiniWorldKeys.HALLE,
            Name = "Halle",
            Grammar = new IndividualObjectGrammar(Genders.Female)
        };

        halle.Items.Add(new Item
        {
            Key = MiniWorldKeys.BUCH,
            Name = "Buch",
            IsPickable = true,
            IsReadable = true,
            Grammar = new IndividualObjectGrammar(Genders.Neutrum)
        });
        halle.Items.Add(new Item
        {
            Key = MiniWorldKeys.TISCH,
            Name = "Tisch",
            IsPickable = false,
            IsSurfaceContainer = true,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        });
        halle.Items.Add(new Item
        {
            Key = MiniWorldKeys.LAMPE,
            Name = "Lampe",
            IsPickable = true,
            Grammar = new IndividualObjectGrammar(Genders.Female)
        });
        halle.Items.Add(new Item
        {
            Key = MiniWorldKeys.SCHLUESSEL_ROT,
            Name = "Schlüssel",
            Adjectives = "rot",
            IsPickable = true,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        });
        halle.Items.Add(new Item
        {
            Key = MiniWorldKeys.SCHLUESSEL_BLAU,
            Name = "Schlüssel",
            Adjectives = "blau",
            IsPickable = true,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        });

        halle.Characters.Add(new Character
        {
            Key = MiniWorldKeys.ZWERG,
            Name = "Zwerg",
            Grammar = new IndividualObjectGrammar(Genders.Male)
        });

        var kammer = new Location
        {
            Key = MiniWorldKeys.KAMMER,
            Name = "Kammer",
            Grammar = new IndividualObjectGrammar(Genders.Female)
        };

        var player = new Player
        {
            Key = MiniWorldKeys.SPIELER,
            Name = string.Empty,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };

        var locationMap = new LocationMap(new LocationComparer())
        {
            { halle, new List<DestinationNode> { new() { Direction = Directions.N, Location = kammer } } },
            { kammer, new List<DestinationNode> { new() { Direction = Directions.S, Location = halle } } }
        };

        this.Universe.LocationMap = locationMap;
        this.Universe.ActiveLocation = halle;
        this.Universe.ActivePlayer = player;
    }
}
