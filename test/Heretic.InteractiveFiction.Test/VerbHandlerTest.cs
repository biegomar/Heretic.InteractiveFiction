using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using Heretic.InteractiveFiction.Test.Mocks;
using Moq;
using Xunit;

namespace Heretic.InteractiveFiction.Test;

public class VerbHandlerTest
{
    private VerbHandler SetUp()
    {
        var printingSubsystem = new Mock<IPrintingSubsystem>();
        printingSubsystem.SetReturnsDefault(true);
        
        var universe = new Universe(printingSubsystem.Object, new ResourceProviderMock());
        var smallWorldAssembler = new SmallWorldAssembler();
        var gamePrerequisites = smallWorldAssembler.AssembleGame();
        universe.LocationMap = gamePrerequisites.LocationMap;
        universe.ActiveLocation = gamePrerequisites.ActiveLocation;
        universe.ActivePlayer = gamePrerequisites.ActivePlayer;
        universe.Quests = gamePrerequisites.Quests;
        universe.SetPeriodicEvent(gamePrerequisites.PeriodicEvent);
        
        return new VerbHandler(universe, printingSubsystem.Object);
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
        var sut = SetUp();
        var actual = sut.Look(verb);
        
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
        var sut = SetUp();
        var actual = sut.Credits(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Absteigen", true)]
    [InlineData("Herabsteigen", true)]
    [InlineData("FALSCH", false)]
    public void DescendTest(string verb, bool expected)
    {
        var sut = SetUp();
        var actual = sut.Descend(verb);
        
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
        var sut = SetUp();
        var actual = sut.Ways(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Hilfe", true)]
    [InlineData("help", true)]
    [InlineData("FALSCH", false)]
    public void HelpTest(string verb, bool expected)
    {
        var sut = SetUp();
        var actual = sut.Help(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("I", true)]
    [InlineData("Inv", true)]
    [InlineData("Inventar", true)]
    [InlineData("FALSCH", false)]
    public void InventoryTest(string verb, bool expected)
    {
        var sut = SetUp();
        var actual = sut.Inventory(verb);
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Quit", true)]
    [InlineData("Q", true)]
    [InlineData("Ende", true)]
    [InlineData("FALSCH", false)]
    public void QuitTest(string verb, bool expected)
    {
        var sut = SetUp();
        var actual = sut.Quit(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Kommentar", true)]
    [InlineData("Comment", true)]
    [InlineData("Remark", true)]
    [InlineData("REM", true)]
    [InlineData("FALSCH", false)]
    public void RemarkTest(string verb, bool expected)
    {
        var sut = SetUp();
        var actual = sut.Remark(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Score", true)]
    [InlineData("Punkte", true)]
    [InlineData("Fortschritt", true)]
    [InlineData("FALSCH", false)]
    public void ScoreTest(string verb, bool expected)
    {
        var sut = SetUp();
        var actual = sut.Score(verb);
        
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
        var sut = SetUp();
        var actual = sut.SitDown(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Aufstehen", true)]
    [InlineData("Hinstellen", true)]
    [InlineData("FALSCH", false)]
    public void StandUpTest(string verb, bool expected)
    {
        var sut = SetUp();
        var actual = sut.StandUp(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Nimm", true)]
    [InlineData("Nehme", true)]
    [InlineData("Take", true)]
    [InlineData("FALSCH", false)]
    public void TakeTest(string verb, bool expected)
    {
        var sut = SetUp();
        var actual = sut.Take(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Schaue", "Kerze", true)]
    public void LookAtItemTest(string verb, string item, bool expected)
    {
        var sut = SetUp();
        
        var actual = sut.Look(verb, item);

        Assert.Equal(expected, actual);
    }
}