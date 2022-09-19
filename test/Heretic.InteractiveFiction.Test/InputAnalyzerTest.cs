using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using Heretic.InteractiveFiction.Test.Mocks;
using Moq;
using Xunit;

namespace Heretic.InteractiveFiction.Test;

public class InputAnalyzerTest
{
    private IPrintingSubsystem printingSubsystem => Mock.Of<IPrintingSubsystem>();
    private Player player;
    private Universe universe;
    private IGrammar grammar => new GermanGrammar(new ResourceProviderMock());
    private IGamePrerequisitesAssembler smallWorld;

    public InputAnalyzerTest()
    {
        this.smallWorld = new SmallWorldAssembler();
        this.universe = new Universe(printingSubsystem, new ResourceProviderMock());
        
        var gamePrerequisites = this.smallWorld.AssembleGame();
        this.universe.LocationMap = gamePrerequisites.LocationMap;
        this.universe.ActiveLocation = gamePrerequisites.ActiveLocation;
        this.universe.ActivePlayer = gamePrerequisites.ActivePlayer;
        this.universe.Quests = gamePrerequisites.Quests;
        this.universe.SetPeriodicEvent(gamePrerequisites.PeriodicEvent);
        this.player = gamePrerequisites.ActivePlayer;
    }
    
    [Theory]
    [InlineData("l", new[] { "l" })]
    [InlineData("look", new[] { "look" })]
    public void SingleWordsTest(string input, string[] expected)
    {
        var sut = new InputAnalyzer(this.universe, this.grammar);

        var actual = sut.Analyze(input);

        Assert.Equal(expected[0], actual[0]);
    }

    [Theory]
    [InlineData("l mann", new[] { "l", "mann" })]
    [InlineData("look mann", new[] { "look", "mann" }) ]
    [InlineData("mann look", new[] { "look", "mann" })]
    [InlineData("schaue auf den Mann", new[] { "schaue", "Mann" })]
    [InlineData("Mann schaue auf den ", new[] { "schaue", "Mann" })]
    [InlineData("Rede mit dem Tankwart ", new[] { "Rede", "Tankwart" })]
    [InlineData("schaue dich an", new[] { "schaue", "dich" })]
    [InlineData("lege die kerze hin", new[] { "lege", "kerze" })]
    [InlineData("lege sie hin", new[] { "lege", "sie" })]
    public void TwoWordsTest(string input, string[] expected)
    {
        var sut = new InputAnalyzer(universe, this.grammar);

        var actual = sut.Analyze(input);

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

        Assert.Equal(expected[0], actual[0]);
        Assert.Equal(expected[1], actual[1]);
        Assert.Equal(expected[2], actual[2]);
    }
}
