using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Objects;
using Xunit;

namespace Heretic.InteractiveFiction.TestGame.Tests;

public class ObjectHandlerTest: IClassFixture<TestFixture>
{
    private ObjectHandler _sut;
    private Universe _universe;
    private TestFixture _testFixture;
    
    public ObjectHandlerTest(TestFixture testFixture)
    {
        this._testFixture = testFixture;
        this._universe = testFixture.Universe;
        this._sut = testFixture.ObjectHandler;
    }

    [Fact]
    public void StoreActiveObjectTest()
    {
        var item = _universe.ActiveLocation.Items.First(i => i.Key == "TABLE");
        var expected = item.Key;

        _sut.StoreAsActiveObject(item);
        var actual = _universe.ActiveObject.Key;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void ClearActiveObjectTest()
    {
        var expected = true;
        
        _sut.ClearActiveObject();
        var actual = _universe.ActiveObject == default;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void ClearActiveObjectIfNotInInventoryShouldFail()
    {
        
    }
    
    [Fact]
    public void ClearActiveObjectIfNotInInventoryShouldWork()
    {
        _universe.ActiveObject = _universe.ActiveLocation.Items.First(i => i.Key == "TABLE");
        
        _sut.ClearActiveObjectIfNotInInventory();
        var actual = _universe.ActiveObject == default;
        
        Assert.True(actual);
    }
}