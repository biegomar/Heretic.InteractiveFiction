using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public class Player : AContainerObject
{
    public bool HasPaymentMethod { get; set; }
    public Item PaymentMethod { get; set; }

    public ICollection<Item> Clothes { get; set; }

    public bool IsSitting { get; set; }
    public bool IsStranger { get; set; }

    public bool HasClimbed { get; set; }

    public AContainerObject Seat { get; set; }

    public AContainerObject ClimbedObject { get; set; }

    public Player()
    {
        this.Clothes = new List<Item>();
        this.IsSitting = false;
        this.HasClimbed = false;
        this.IsStranger = true;
    }

    protected override string GetObjectName()
    {
        var sentence = this.name.Split('|');
        return sentence[0].Trim();
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        if (this.IsStranger)
        {
            result.AppendLine(BaseDescriptions.HELLO_STRANGER);
        }
        else
        {
            result.AppendLine(string.Format(BaseDescriptions.HELLO_NAME, this.Name));
        }
        
        result.AppendLine(this.Description);
        
        if (this.FirstLookDescription != string.Empty)
        {
            result.AppendLine(this.FirstLookDescription);
            this.FirstLookDescription = string.Empty;
        }

        result.AppendLine(this.PrintClothes());

        if (this.Items.Any())
        {
            var blockingItems = this.GetItemsThatBlockPickup();
            if (blockingItems.Any())
            {
                result.AppendLine(this.PrintItemsThatBlockPickup(blockingItems));
            }
            else
            {
                var actualPayload = this.GetActualPayload();

                result.AppendLine(string.Format(BaseDescriptions.ACTUAL_PAYLOAD, (float)actualPayload / 1000,
                    (float)(this.GetMaxPayload() - actualPayload) / 1000));
            }

            result.Append(this.PrintItems());
        }
        else
        {
            result.AppendLine(string.Format(BaseDescriptions.MAX_PAYLOAD, (float)this.GetMaxPayload() / 1000));
            result.AppendLine(BaseDescriptions.NOTHING);
        }

        if (this.IsSitting && this.Seat != default)
        {
            result.AppendLine(string.Format(BaseDescriptions.SITTING_ON, this.LowerFirstChar(this.Seat.DativeArticleName)));
        }
        
        if (this.HasClimbed && this.ClimbedObject != default)
        {
            if (string.IsNullOrEmpty(this.ClimbedObject.ClimbedDescription))
            {
                result.AppendLine(string.Format(BaseDescriptions.ITEM_CLIMBED, this.LowerFirstChar(this.ClimbedObject.AccusativeArticleName)));    
            }
            else
            {
                result.AppendLine(this.ClimbedObject.ClimbedDescription);
            }
        }


        return result.ToString();
    }

    public override bool RemoveItem(Item itemToRemove)
    {
        var baseResult = base.RemoveItem(itemToRemove);
        if (!baseResult)
        {
            foreach (var item in this.Clothes)
            {
                if (item.Key == itemToRemove.Key)
                {
                    return this.Clothes.Remove(itemToRemove);
                }
                var result = item.RemoveItem(itemToRemove);

                if (result)
                {
                    return true;
                }
            }
        }

        return baseResult;
    }

    protected override IList<Item> FilterUnhiddenItems()
    {
        var itemsFromCharacter = this.Characters.Where(c => c.IsHidden == false).SelectMany(c => c.Items).Where(i => i.IsHidden == false)
            .Union(this.Characters.Where(c => c.IsHidden == false).SelectMany(c => c.LinkedTo).Where(i => i.IsHidden == false));
        
        var unhiddenItems = this.Items.Where(i => i.IsHidden == false)
            .Union(this.LinkedTo.Where(i => i.IsHidden == false))
            .Union(this.Clothes.Where(i => i.IsHidden == false))
            .Union(itemsFromCharacter).ToList();

        return unhiddenItems;
    }
    
    private string PrintClothes()
    {
        var result = new StringBuilder();
        if (this.Clothes.Any())
        {
            result.Append(BaseDescriptions.WEARING_ITEMS);
            var itemIndex = 0;
            foreach (var cloth in Clothes)
            {
                if (itemIndex != 0)
                {
                    result.Append(", ");    
                }
                result.Append(this.LowerFirstChar(cloth.AccusativeIndefiniteArticleName));
                itemIndex++;
            }
        }

        return result.ToString();
    }

    private ICollection<Item> GetItemsThatBlockPickup()
    {
        return this.Items.Where(i => i.IsBlockingPickUp).ToList();
    }

    public string PrintItemsThatBlockPickup(ICollection<Item> blockingItems)
    {
        var description = new StringBuilder();

        if (blockingItems.Any())
        {
            var index = 0;
            foreach (var item in blockingItems)
            {
                if (index != 0)
                {
                    if (blockingItems.Count == 2)
                    {
                        description.Append($" {BaseDescriptions.AND} ");
                    }
                    else
                    {
                        description.Append(index == blockingItems.Count - 1 ? $" {BaseDescriptions.AND} " : ", ");
                    }
                }

                if (index != 0)
                {
                    var lowerName = item.Name.First().ToString().ToLower() + item.Name.Substring(1);
                    description.Append($"{lowerName}");
                }
                else
                {
                    description.Append($"{item.Name}");
                }

                index++;
            }

            description.AppendLine(index == 1
                ? $" {BaseDescriptions.BLOCK_PICKUP_SINGLE}"
                : $" {BaseDescriptions.BLOCK_PICKUP_PLURAL}");

        }

        return description.ToString();
    }

    public bool PickItem(Item item)
    {
        if (!this.Items.Contains(item))
        {
            var blockingItems = this.GetItemsThatBlockPickup();
            if (blockingItems.Any(i => !i.Items.Contains(item)))
            {
                return false;
            }

            if (item.GetActualPayload() + this.GetActualPayload() > this.GetMaxPayload() + item.MaxPayload)
            {
                return false;
            }

            this.Items.Add(item);
            return true;
        }

        return false;
    }
    
    public override int GetActualPayload()
    {
        var sum = base.GetActualPayload();
        if (this.Clothes.Any())
        {
            foreach (var item in this.Clothes)
            {
                if (item.Items.Any())
                {
                    sum += item.GetActualPayload();
                }
                else
                {
                    sum += item.Weight;
                }
            }
        }

        return sum;
    }

    public bool DropItem(Item item)
    {
        if (this.Items.Contains(item))
        {
            this.Items.Remove(item);
            return true;
        }

        return false;
    }

    public bool SitDownOnSeat(AContainerObject seat)
    {
        if (seat.IsSeatAble && !this.IsSitting)
        {
            this.IsSitting = true;
            this.Seat = seat;
            return true;
        }

        return false;
    }
    
    public bool StandUp()
    {
        if (this.IsSitting)
        {
            this.IsSitting = false;
            this.Seat = null;
            return true;
        }

        return false;
    }

    public bool Descend()
    {
        if (this.HasClimbed)
        {
            this.HasClimbed = false;
            this.ClimbedObject = null;
            return true;
        }

        return false;
    }

    public int GetMaxPayload()
    {
        var sum = MaxPayload;
        if (this.Items.Any())
        {
            var containerSum = this.Items.Sum(x => x.MaxPayload);
            if (containerSum > 0)
            {
                sum = containerSum;
            }
        }

        return sum;
    }

    protected override string PrintItems(bool subItems = false)
    {
        var baseResult = base.PrintItems(subItems);

        var result = baseResult.Replace(BaseDescriptions.HERE, BaseDescriptions.INVENTORY);
        
        return result;
    }
}
