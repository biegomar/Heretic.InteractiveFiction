using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.GamePlay.EventSystem;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.Objects;

public class Universe
{
    public int Score;

    public int MaxScore
    {
        get
        {
            if (maxScore == 0)
            {
                maxScore = GetMaxScore();
            }
            return maxScore;
        }
    }

    public readonly IDictionary<string, IEnumerable<string>> VerbResources;
    public readonly IDictionary<string, IEnumerable<string>> ItemResources;
    public readonly IDictionary<string, IEnumerable<string>> CharacterResources;
    public readonly IDictionary<string, IEnumerable<string>> LocationResources;
    public readonly IDictionary<string, IEnumerable<string>> ConversationAnswersResources;
    public readonly IEnumerable<string> PackingWordsResources;

    public IDictionary<Location, IEnumerable<LocationMap>> LocationMap { get; set; }
    public Location ActiveLocation { get; set; }
    public Player ActivePlayer { get; set; }

    public bool IsPeriodicEventActivated { get => this.periodicEvent.Active; set => this.periodicEvent.Active = value; }

    public readonly IDictionary<string, int> ScoreBoard = new Dictionary<string, int>();

    public event EventHandler<PeriodicEventArgs> PeriodicEvents;

    private readonly IPrintingSubsystem printingSubsystem;
    private ICollection<string> SolvedQuests;
    private int maxScore;
    private PeriodicEvent periodicEvent;

    public Universe(IPrintingSubsystem printingSubsystem, IResourceProvider resourceProvider, PeriodicEvent periodicEvent)
    {
        this.periodicEvent = periodicEvent;
        this.periodicEvent.Active = false;
        this.PeriodicEvents += this.periodicEvent.RaiseEvent;
        this.printingSubsystem = printingSubsystem;
        this.SolvedQuests = new List<string>();
        this.Score = 0;
        this.VerbResources = resourceProvider.GetVerbsFromResources();
        this.ItemResources = resourceProvider.GetItemsFromResources();
        this.CharacterResources = resourceProvider.GetCharactersFromResources();
        this.LocationResources = resourceProvider.GetLocationsFromResources();
        this.PackingWordsResources = resourceProvider.GetPackingWordsFromResources();
        this.ConversationAnswersResources = resourceProvider.GetConversationsAnswersFromResources();
        this.LocationMap = new Dictionary<Location, IEnumerable<LocationMap>>();
    }

    public void RaisePeriodicEvents(PeriodicEventArgs eventArgs)
    {
        EventHandler<PeriodicEventArgs> localEventHandler = this.PeriodicEvents;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public bool MoveCharacter(Character person, Location newLocation)
    {
        if (person != default && newLocation != default)
        {
            var oldLocation = this.LocationMap.Keys.SingleOrDefault(l => l.Characters.Contains(person));
            if (oldLocation != default)
            {
                oldLocation.Characters.Remove(person);
                newLocation.Characters.Add(person);
                return true;
            }
        }

        return false;
    }

    public bool IsQuestSolved(string quest)
    {
        return this.SolvedQuests.Contains(quest);
    }

    public void SolveQuest(string quest)
    {
        if (!this.SolvedQuests.Contains(quest))
        {
            this.SolvedQuests.Add(quest);
        }
    }

    public bool PickObject(Item item)
    {
        if (item != default)
        {
            var owner = this.ActiveLocation.GetOwnerOfUnhiddenItemByKey(item.Key);
            if (owner == default)
            {
                owner = this.ActivePlayer.GetOwnerOfUnhiddenItemByKey(item.Key);
            }

            if (owner == default)
            {
                return false;
            }
            if (owner.Key == this.ActivePlayer.Key)
            {
                return printingSubsystem.ItemAlreadyOwned();
            }

            if (item.IsPickAble)
            {
                var result = this.ActivePlayer.PickItem(item);
                if (result)
                {
                    result = owner.Items.Remove(item);
                    if (result)
                    {
                        return printingSubsystem.ItemPickupSuccess(item);
                    }

                    return false;
                }

                return printingSubsystem.ItemToHeavy();
            }

            return printingSubsystem.ImpossiblePickup(item);
        }

        return printingSubsystem.ItemNotVisible();
    }

    public AContainerObject GetObjectFromWorldByKey(string key)
    {
        foreach (var location in this.LocationMap.Keys)
        {
            var result = findObject(key, location);
            if (result != default && result.Key == key)
            {
                return result;
            }
        }

        return findObject(key, this.ActivePlayer) ?? default;
    }

    private AContainerObject findObject(string key, AContainerObject containerObject)
    {
        if (containerObject.Key == key)
        {
            return containerObject;
        }

        AContainerObject result = containerObject.GetItemByKey(key);
        if (result != default)
        {
            return result;
        }

        result = containerObject.GetCharacterByKey(key);
        return result ?? default;
    }

    public void UnveilFirstLevelObjects(AContainerObject container)
    {
        if (container == default)
        {
            return;
        }

        if (container.IsCloseAble && container.IsClosed)
        {
            return;
        }

        var unveilAbleItems = container.Items.Where(i => i.IsUnveilAble).ToList();
        if (unveilAbleItems.Any())
        {
            foreach (var items in unveilAbleItems)
            {
                items.IsHidden = false;
            }
        }

        var unveilAbleCharacters = container.Characters.Where(c => c.IsUnveilAble).ToList();
        if (unveilAbleCharacters.Any())
        {
            foreach (var character in unveilAbleCharacters)
            {
                character.IsHidden = false;
            }
        }
    }

    public LocationMap GetLocationMapByEnumString(string key)
    {
        if (this.LocationMap.ContainsKey(this.ActiveLocation))
        {
            var result = Enum.TryParse<Directions>(key, true, out var direction);
            if (result)
            {
                return this.LocationMap[this.ActiveLocation].FirstOrDefault(l => l.Direction == direction);
            }
        }

        return default;
    }

    private int GetMaxScore()
    {
        return this.ScoreBoard.Sum(kv => kv.Value);
    }
}
