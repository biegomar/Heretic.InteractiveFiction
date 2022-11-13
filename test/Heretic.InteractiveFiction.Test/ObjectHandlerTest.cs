using System.Linq;
using System.Threading;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using Heretic.InteractiveFiction.Test.GamePlay;
using Heretic.InteractiveFiction.Test.Mocks;
using Moq;
using Xunit;

namespace Heretic.InteractiveFiction.Test;

public class ObjectHandlerTest
{
    private ObjectHandler _sut;
    private Universe _universe;
    
    public ObjectHandlerTest()
    {
        this._universe = this.GetUniverse();
        this._sut = new ObjectHandler(_universe);
    }
    
    private Universe GetUniverse()
    {
        var printingSubsystem = new Mock<IPrintingSubsystem>();
        printingSubsystem.SetReturnsDefault(true);
        
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
    public void ClearActiveObjectIfNotInInventory_FalseTest()
    {
        var expected = false;
        _universe.ActiveObject = _universe.ActivePlayer.Items.First(i => i.Key == "KNIFE");
        
        _sut.ClearActiveObjectIfNotInInventory();
        var actual = _universe.ActiveObject == default;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void ClearActiveObjectIfNotInInventory_TrueTest()
    {
        var expected = true;
        _universe.ActiveObject = _universe.ActiveLocation.Items.First(i => i.Key == "TABLE");
        
        _sut.ClearActiveObjectIfNotInInventory();
        var actual = _universe.ActiveObject == default;
        
        Assert.Equal(expected, actual);
    }
}