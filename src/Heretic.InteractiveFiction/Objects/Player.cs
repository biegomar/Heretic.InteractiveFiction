using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Objects;

public class Player : AContainerObject
{
    public bool HasPaymentMethod { get; set; }
    public Item PaymentMethod { get; set; }

    public override string ToString()
    {
        var result = new StringBuilder();
        if (this.Name == string.Empty)
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

    public bool DropItem(Item item)
    {
        if (this.Items.Contains(item))
        {
            this.Items.Remove(item);
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

    public override string PrintItems(bool subItems = false)
    {
        var baseResult = base.PrintItems(subItems);

        var result = baseResult.Replace(BaseDescriptions.HERE_PLURAL, BaseDescriptions.INVENTORY);
        result = result.Replace(BaseDescriptions.HERE_SINGLE, BaseDescriptions.INVENTORY);

        return result;
    }
}
