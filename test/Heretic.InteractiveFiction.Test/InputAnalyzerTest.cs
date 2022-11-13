using System.Collections.Generic;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using Heretic.InteractiveFiction.Test.GamePlay;
using Heretic.InteractiveFiction.Test.Mocks;
using Moq;
using Xunit;

namespace Heretic.InteractiveFiction.Test;

public class InputAnalyzerTest
{
    private IPrintingSubsystem printingSubsystem => Mock.Of<IPrintingSubsystem>();
    private Player player;
    private Universe universe;
    private IGrammar grammar => new GermanGrammar(new ResourceProvider());
    private IGamePrerequisitesAssembler smallWorld;

    public InputAnalyzerTest()
    {
        this.smallWorld = new SmallWorldAssembler();
        this.universe = new Universe(printingSubsystem, new ResourceProvider());
        
        var gamePrerequisites = this.smallWorld.AssembleGame();
        this.universe.LocationMap = gamePrerequisites.LocationMap;
        this.universe.ActiveLocation = gamePrerequisites.ActiveLocation;
        this.universe.ActivePlayer = gamePrerequisites.ActivePlayer;
        this.universe.Quests = gamePrerequisites.Quests;
        this.universe.SetPeriodicEvent(gamePrerequisites.PeriodicEvent);
        this.player = gamePrerequisites.ActivePlayer;
    }
    
    [Theory]
    [InlineData("l", new[] { "LOOK" })]
    [InlineData("look", new[] { "LOOK" })]
    public void SingleWordsTest(string input, string[] expected)
    {
        var sut = new InputAnalyzer(this.universe, this.grammar);

        var actual = sut.Analyze(input);

        Assert.Equal(expected.Length, actual.Length);
        Assert.Equal(expected[0], actual[0]);
    }

    [Theory]
    [InlineData("l mann", new[] { "LOOK", "mann" })]
    [InlineData("look mann", new[] { "LOOK", "mann" }) ]
    [InlineData("mann look", new[] { "LOOK", "mann" })]
    [InlineData("schaue auf den Mann", new[] { "LOOK", "Mann" })]
    [InlineData("Mann schaue auf den ", new[] { "LOOK", "Mann" })]
    [InlineData("Rede mit dem Tankwart ", new[] { "TALK", "Tankwart" })]
    [InlineData("schaue dich an", new[] { "LOOK", "dich" })]
    [InlineData("lege die kerze hin", new[] { "DROP", "kerze" })]
    [InlineData("lege sie hin", new[] { "DROP", "sie" })]
    public void TwoWordsTest(string input, string[] expected)
    {
        var sut = new InputAnalyzer(universe, this.grammar);

        var actual = sut.Analyze(input);

        Assert.Equal(expected.Length, actual.Length);
        Assert.Equal(expected[0], actual[0]);
        Assert.Equal(expected[1], actual[1]);
    }
    
    [Theory]
    [InlineData("schaue dir den Mann an", new[] { "schaue", "dir", "Mann" })]
    [InlineData("schaue dir die kerze an", new[] { "schaue", "dir", "kerze" })]
    [InlineData("schaue sie dir an", new[] { "schaue", "dir", "sie" })]
    [InlineData("schaue dir sie an", new[] { "schaue", "dir", "sie" })]
    [InlineData("schaue ihn dir an", new[] { "schaue", "dir", "ihn" })]
    [InlineData("schaue dir ihn an", new[] { "schaue", "dir", "ihn" })]
    [InlineData("schaue es dir an", new[] { "schaue", "dir", "es" })]
    [InlineData("schaue dir es an", new[] { "schaue", "dir", "es" })]
    [InlineData("nimm dir die Kerze", new[] { "nimm", "dir", "Kerze" })]
    [InlineData("nimm sie dir", new[] { "nimm", "dir", "sie" })]
    [InlineData("nimm dir sie", new[] { "nimm", "dir", "sie" })]
    public void ThreeWordsTest(string input, string[] expected)
    {
        var sut = new InputAnalyzer(universe, this.grammar);

        var actual = sut.Analyze(input);

        Assert.Equal(expected.Length, actual.Length);
        Assert.Equal(expected[0], actual[0]);
        Assert.Equal(expected[1], actual[1]);
        Assert.Equal(expected[2], actual[2]);
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void OderSentenceTest(List<string> sentence, Request request)
    {
        
    }
    
    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] { new List<string>() {"a", "b"}, new Request() {Verb = new Verb()} },
        };
}
