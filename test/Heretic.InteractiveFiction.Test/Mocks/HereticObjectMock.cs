using Heretic.InteractiveFiction.Objects;

namespace Heretic.InteractiveFiction.Test.Mocks;

public class HereticObjectMock: AHereticObject
{
    public HereticObjectMock()
    {
        this.Key = "Mock";
        this.Name = "Mock";
        this.Description = "Mock";
        
        this.Items.Add(AddItems());
        this.Characters.Add(AddCharacter());
    }

    private Item AddItems()
    {
        var cookTop = new Item
        {
            Key = "COOKTOP",
            Name = "COOKTOP",
            Description = "COOKTOP",
            IsPickable = false,
            Grammar = new Objects.Grammars(Genders.Neutrum)
        };

        return cookTop;
    }

    private Character AddCharacter()
    {
        var claudius = new Character
        {
            Key = "CLAUDIUS",
            Name = "CLAUDIUS",
            Description = "CLAUDIUS",
        };
        
        claudius.Items.Add(AddKnifeForClaudius());
        claudius.Items.Add(AddKnapSackForClaudius());
        claudius.Characters.Add(AddChildForClaudius());

        return claudius;
    }

    private Item AddKnifeForClaudius()
    {
        var knife = new Item
        {
            Key = "KNIFE",
            Name = "KNIFE",
            Description = "KNIFE",
            Grammar = new Objects.Grammars(gender: Genders.Neutrum)
        };

        return knife;
    }
    
    private Item AddKnapSackForClaudius()
    {
        var knapSack = new Item
        {
            Key = "KNAPSACK",
            Name = "KNAPSACK",
            Description = "KNAPSACK",
            IsContainer = true
        };
        
        knapSack.Characters.Add(AddBabyForClaudius());

        return knapSack;
    }
    
    private Character AddBabyForClaudius()
    {
        var baby = new Character
        {
            Key = "BABY",
            Name = "BABY",
            Description = "BABY",
        };

        return baby;
    }
    
    private Character AddChildForClaudius()
    {
        var child = new Character
        {
            Key = "CHILD",
            Name = "CHILD",
            Description = "CHILD",
        };

        return child;
    }
}