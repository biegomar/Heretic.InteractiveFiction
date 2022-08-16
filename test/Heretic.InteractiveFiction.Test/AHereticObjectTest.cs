using Heretic.InteractiveFiction.Test.Mocks;
using Xunit;

namespace Heretic.InteractiveFiction.Test;

public class AHereticObjectTest
{
    [Fact]
    public void GetItemTest()
    {
        var sut = new HereticObjectMock();
        var actual = sut.GetItem("COOKTOP");
        var expected = "COOKTOP";

        Assert.Equal(expected, actual.Key);
    }

    [Fact]
    public void GetItemFromCharacterTest()
    {
        var sut = new HereticObjectMock();
        var actual = sut.GetItem("KNIFE");
        var expected = "KNIFE";

        Assert.Equal(expected, actual.Key);
    }
    
    [Fact]
    public void GetCharacterFromCharacterTest()
    {
        //A child on the arm of an adult.
        var sut = new HereticObjectMock();
        var actual = sut.GetCharacter("CHILD");
        var expected = "CHILD";

        Assert.Equal(expected, actual.Key);
    }
    
    [Fact]
    public void GetCharacterFromItemFromCharacterTest()
    {
        //A baby in a baby sling, carried by one person.
        var sut = new HereticObjectMock();
        var actual = sut.GetCharacter("BABY");
        var expected = "BABY";

        Assert.Equal(expected, actual.Key);
    }
    
    [Fact]
    public void RemoveItemTest()
    {
        var sut = new HereticObjectMock();
        var item = sut.GetItem("COOKTOP");

        var actual = sut.RemoveItem(item);
        
        Assert.True(actual);
    }

    [Fact]
    public void RemoveItemFromCharacterTest()
    {
        var sut = new HereticObjectMock();
        var item = sut.GetItem("KNIFE");

        var actual = sut.RemoveItem(item);
        
        Assert.True(actual);
    }
}