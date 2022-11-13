using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Objects;
using Xunit;

namespace Heretic.InteractiveFiction.TestGame.Tests;

public class VerbHandlerTest: IClassFixture<TestFixture>
{
    private VerbHandler sut;
    private TestFixture testFixture;
    
    public VerbHandlerTest(TestFixture testFixture)
    {
        this.testFixture = testFixture;
        this.sut = testFixture.VerbHandler;
    }

    [Theory]
    [InlineData("LOOK", true)]
    [InlineData("FALSCH", false)]
    public void LookTest(string verb, bool expected)
    {
        var actual = sut.Look(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("CREDITS", true)]
    [InlineData("FALSCH", false)]
    public void CreditsTest(string verb, bool expected)
    {
        var actual = sut.Credits(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("DESCEND", true)]
    [InlineData("FALSCH", false)]
    public void DescendTest(string verb, bool expected)
    {
        var actual = sut.Descend(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("WAYS", true)]
    [InlineData("FALSCH", false)]
    public void WaysTest(string verb, bool expected)
    {
        var actual = sut.Ways(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("HELP", true)]
    [InlineData("FALSCH", false)]
    public void HelpTest(string verb, bool expected)
    {
        var actual = sut.Help(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("INV", true)]
    [InlineData("FALSCH", false)]
    public void InventoryTest(string verb, bool expected)
    {
        var actual = sut.Inventory(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void QuitSTest()
    {
        var actual = sut.Quit("FALSCH");
        
        Assert.False(actual);
    }
    
    [Fact]
    public void QuitShouldThrowException()
    {
        Action action = () => sut.Quit(VerbKeys.QUIT);
        
        var caughtException = Assert.Throws<QuitGameException>(action);
        Assert.Equal("Das Spiel wird beendet.", caughtException.Message);
    }

    [Theory]
    [InlineData("SCORE", true)]
    [InlineData("FALSCH", false)]
    public void ScoreTest(string verb, bool expected)
    {
        var actual = sut.Score(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("SIT", true)]
    [InlineData("FALSCH", false)]
    public void SitDownTest(string verb, bool expected)
    {
        var actual = sut.SitDown(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("STANDUP", true)]
    [InlineData("FALSCH", false)]
    public void StandUpTest(string verb, bool expected)
    {
        var actual = sut.StandUp(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("TAKE", true)]
    [InlineData("FALSCH", false)]
    public void TakeTest(string verb, bool expected)
    {
        var actual = sut.Take(verb);
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("LOOK", "Kerze", true)]
    [InlineData("LOOK", "UnknownObject", true)]
    [InlineData("FALSCH", "Kerze", false)]
    public void LookAtItemTest(string verb, string item, bool expected)
    {
        var actual = sut.Look(verb, item);

        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void SetPronoun_Test()
    {
        this.testFixture.ClearActiveObject();
        var expectedKey = Keys.TABLE;

        sut.Look(VerbKeys.LOOK, "Holztisch");
        AHereticObject? actualObject = this.testFixture.ActiveObject;
        var actual = actualObject != default;
        var actualKey = actualObject?.Key;
        
        Assert.True(actual);
        Assert.Equal(expectedKey, actualKey);
    }
}