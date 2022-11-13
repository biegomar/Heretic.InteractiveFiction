using Xunit;

namespace Heretic.InteractiveFiction.TestGame.Tests;

public class AHereticObjectTest: IClassFixture<TestFixture>
{
    private readonly TestFixture testFixture;
    
    public AHereticObjectTest(TestFixture testFixture)
    {
        this.testFixture = testFixture;
    }
    
    [Fact]
    public void GetItemTest()
    {
        var expected = Keys.NOTE;
        var sut = this.testFixture.Table;
        
        var actual = sut.GetItem(Keys.NOTE);
        
        Assert.Equal(expected, actual.Key);
    }

    [Fact]
    public void GetItemFromCharacterTest()
    {
        
    }
    
    [Fact]
    public void GetCharacterFromCharacterTest()
    {
        //A child on the arm of an adult.
        
    }
    
    [Fact]
    public void GetCharacterFromItemFromCharacterTest()
    {
        //A baby in a baby sling, carried by one person.
        
    }
    
    [Fact]
    public void RemoveItemTest()
    {
        var sut = this.testFixture.Table;
        var item = sut.GetItem(Keys.CANDLE);

        var actual = sut.RemoveItem(item);
        
        Assert.True(actual);
    }

    [Fact]
    public void RemoveItemFromCharacterTest()
    {
        
    }
}