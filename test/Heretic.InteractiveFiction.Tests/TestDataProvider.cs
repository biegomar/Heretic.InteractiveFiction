using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Tests;

internal sealed class TestDataProvider
{
    internal static readonly Item Candle = GetCandle();
    internal static readonly Item Stove = GetStove();
        
    private static Item GetCandle()
    {
        var candle = new Item()
        {
            Key = Keys.CANDLE,
            Name = Items.CANDLE,
            Description = Descriptions.CANDLE,
            ContainmentDescription = Descriptions.CANDLE_CONTAINMENT,
            IsLighter = true,
            IsLighterSwitchedOn = true,
            LighterSwitchedOnDescription = Descriptions.LIGHTER_ON
        };

        return candle;
    }
    
    private static Item GetStove()
    {
        var stove = new Item()
        {
            Key = Keys.STOVE,
            Name = Items.STOVE,
            Description = Descriptions.STOVE,
            IsPickable = false,
            IsClosed = true,
            IsCloseable = true,
            IsContainer = true,
            Grammar = new IndividualObjectGrammar(Genders.Male)
        };

        return stove;
    }
}