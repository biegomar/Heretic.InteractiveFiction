using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using Heretic.InteractiveFiction.Test.GamePlay;
using Heretic.InteractiveFiction.Test.Mocks;
using Moq;
using Xunit;

namespace Heretic.InteractiveFiction.Test;

public class VerbHandlerTest
{
    private VerbHandler _sut;
    private Universe _universe;
    private IGrammar _grammar;

    public VerbHandlerTest()
    {
        var printingSubsystem = new Mock<IPrintingSubsystem>();
        printingSubsystem.SetReturnsDefault(true);
        
        this._universe = GetUniverse(printingSubsystem);
        this._grammar = new GermanGrammar(new ResourceProvider());
        this._sut = new VerbHandler(_universe, _grammar, printingSubsystem.Object);
    }

    private Universe GetUniverse(Mock<IPrintingSubsystem> printingSubsystem)
    {
        var universe = new Universe(printingSubsystem.Object, new ResourceProvider());
        var smallWorldAssembler = new SmallWorldAssembler();
        var gamePrerequisites = smallWorldAssembler.AssembleGame();
        universe.LocationMap = gamePrerequisites.LocationMap;
        universe.ActiveLocation = gamePrerequisites.ActiveLocation;
        universe.ActivePlayer = gamePrerequisites.ActivePlayer;
        universe.Quests = gamePrerequisites.Quests;
        universe.SetPeriodicEvent(gamePrerequisites.PeriodicEvent);
        
        return universe;
    }

    [Theory]
    [InlineData("Schaue", true)]
    [InlineData("Schau", true)]
    [InlineData("Untersuch", true)]
    [InlineData("Untersuche", true)]
    [InlineData("Untersuchen", true)]
    [InlineData("U", true)]
    [InlineData("Zeige", true)]
    [InlineData("Zeig", true)]
    [InlineData("Look", true)]
    [InlineData("L", true)]
    [InlineData("Show", true)]
    [InlineData("examine", true)]
    [InlineData("X", true)]
    [InlineData("FALSCH", false)]
    public void LookTest(string verb, bool expected)
    {
        var actual = _sut.Look(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Credits", true)]
    [InlineData("Info", true)]
    [InlineData("Contribute", true)]
    [InlineData("Contrib", true)]
    [InlineData("FALSCH", false)]
    public void CreditsTest(string verb, bool expected)
    {
        var actual = _sut.Credits(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Absteigen", true)]
    [InlineData("Herabsteigen", true)]
    [InlineData("FALSCH", false)]
    public void DescendTest(string verb, bool expected)
    {
        var actual = _sut.Descend(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Wege", true)]
    [InlineData("Auswege", true)]
    [InlineData("Weg", true)]
    [InlineData("Exits", true)]
    [InlineData("Exit", true)]
    [InlineData("Ausgänge", true)]
    [InlineData("Ausgang", true)]
    [InlineData("Ausweg", true)]
    [InlineData("FALSCH", false)]
    public void WaysTest(string verb, bool expected)
    {
        var actual = _sut.Ways(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Hilfe", true)]
    [InlineData("help", true)]
    [InlineData("FALSCH", false)]
    public void HelpTest(string verb, bool expected)
    {
        var actual = _sut.Help(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("I", true)]
    [InlineData("Inv", true)]
    [InlineData("Inventar", true)]
    [InlineData("FALSCH", false)]
    public void InventoryTest(string verb, bool expected)
    {
        var actual = _sut.Inventory(verb);
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Quit", true)]
    [InlineData("Q", true)]
    [InlineData("Ende", true)]
    [InlineData("FALSCH", false)]
    public void QuitTest(string verb, bool expected)
    {
        var actual = _sut.Quit(verb);
        
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("Score", true)]
    [InlineData("Punkte", true)]
    [InlineData("Fortschritt", true)]
    [InlineData("FALSCH", false)]
    public void ScoreTest(string verb, bool expected)
    {
        var actual = _sut.Score(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Setze", true)]
    [InlineData("Setzen", true)]
    [InlineData("Setz", true)]
    [InlineData("Hinsetzen", true)]
    [InlineData("Sitze", true)]
    [InlineData("FALSCH", false)]
    public void SitDownTest(string verb, bool expected)
    {
        var actual = _sut.SitDown(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Aufstehen", true)]
    [InlineData("Hinstellen", true)]
    [InlineData("FALSCH", false)]
    public void StandUpTest(string verb, bool expected)
    {
        var actual = _sut.StandUp(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Nimm", true)]
    [InlineData("Nehme", true)]
    [InlineData("Take", true)]
    [InlineData("FALSCH", false)]
    public void TakeTest(string verb, bool expected)
    {
        var actual = _sut.Take(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Schaue", "Kerze", true)]
    [InlineData("Schaue", "UnknownObject", true)]
    [InlineData("FALSCH", "Kerze", false)]
    public void LookAtItemTest(string verb, string item, bool expected)
    {
        var actual = _sut.Look(verb, item);

        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Kerze", "CANDLE", true)]
    public void SetPronoun_Look_Test(string subject, string expectedKey, bool expected)
    {
        _universe.ActiveObject = default;
        
        _sut.Look("Schaue", subject);
        var actualObject = _universe.ActiveObject;
        var actual = actualObject != default;
        var actualKey = actualObject?.Key;
        
        Assert.Equal(expected, actual);
        Assert.Equal(expectedKey, actualKey);
    }
}