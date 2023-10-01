using Heretic.InteractiveFiction.Comparer;
using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.GamePlay.EventSystem;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.Objects;

public sealed class Universe
{ 
    public event EventHandler<ContainerObjectEventArgs>? NextGameLoop;
    
    public event EventHandler<PeriodicEventArgs>? PeriodicEvents;
    
    public int NumberOfSolvedQuests => this.solvedQuests.Count;
    
    public readonly IDictionary<string, IEnumerable<string>> ItemResources;
    public readonly IDictionary<string, IEnumerable<string>> CharacterResources;
    public readonly IDictionary<string, IEnumerable<string>> LocationResources;
    public readonly IDictionary<string, IEnumerable<string>> ConversationAnswersResources;

    public LocationMap LocationMap { get; set; }
    public Location ActiveLocation { get; set; } = default!;
    public Player ActivePlayer { get; set; } = default!;
    public AHereticObject? ActiveObject { get; set; }
    public ICollection<string>? Quests { get; set; }
    
    public bool IsPeriodicEventActivated
    {
        get => this.periodicEvent is {Active: true};
        set
        {
            if (this.periodicEvent != null)
            {
                this.periodicEvent.Active = value;
            }
        }
    }
    
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly IList<string> solvedQuests;
    
    private PeriodicEvent? periodicEvent;
    private bool gameWon;

    public Universe(IPrintingSubsystem printingSubsystem, IResourceProvider resourceProvider)
    {
        this.printingSubsystem = printingSubsystem;
        this.solvedQuests = new List<string>();
        this.gameWon = false;
        this.ItemResources = resourceProvider.GetItemsFromResources();
        this.CharacterResources = resourceProvider.GetCharactersFromResources();
        this.LocationResources = resourceProvider.GetLocationsFromResources();
        this.ConversationAnswersResources = resourceProvider.GetConversationsAnswersFromResources();
        this.LocationMap = new LocationMap(new LocationComparer());
        this.ActiveObject = null;
    }
    
    public void OnNextGameLoop(ContainerObjectEventArgs eventArgs)
    {
        var localEventHandler = this.NextGameLoop;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public void SetPeriodicEvent(PeriodicEvent? periodicEventToSet)
    {
        this.periodicEvent = periodicEventToSet;
        if (this.periodicEvent != null)
        {
            this.periodicEvent.Active = false;
            this.PeriodicEvents += this.periodicEvent.RaiseEvent;
        }
    }

    public void RaisePeriodicEvents(PeriodicEventArgs eventArgs)
    {
        var localEventHandler = this.PeriodicEvents;
        localEventHandler?.Invoke(this, eventArgs);
    }

    public bool IsQuestSolved(string quest)
    {
        return this.solvedQuests.Contains(quest);
    }

    public void SolveQuest(string quest)
    {
        if (!this.solvedQuests.Contains(quest))
        {
            this.solvedQuests.Add(quest);
            printingSubsystem.ForegroundColor = TextColor.Magenta;
            printingSubsystem.FormattedResource(BaseDescriptions.QUEST_SOLVED, quest);
            printingSubsystem.ResetColors();
        }
    }

    public void DidYouWin()
    {
        if (!this.gameWon && this.Quests != null && this.Quests.Count == this.NumberOfSolvedQuests)
        {
            this.gameWon = true;
            throw new GameWonException(BaseDescriptions.QUIT_GAME);
        }
    }

    public bool PickObject(Item item, bool suppressSuccessMessage = false)
    {
        var owner = this.ActiveLocation.GetOwnerOfUnhiddenItemByKey(item.Key);
        if (owner == default)
        {
            owner = this.ActivePlayer.GetOwnerOfUnhiddenItemByKey(item.Key);
        }

        if (owner == default)
        {
            return !item.IsPickable && printingSubsystem.ImpossiblePickup(item);
        }
            
        if (owner.Key == this.ActivePlayer.Key)
        {
            return printingSubsystem.ItemAlreadyOwned();
        }

        if (item.IsPickable)
        {
            var result = this.ActivePlayer.PickItem(item);
            if (result)
            {
                result = owner.Items.Remove(item);
                if (result)
                {
                    item.ContainmentDescription = string.Empty;
                    return suppressSuccessMessage || printingSubsystem.ItemPickupSuccess(item);
                }

                return false;
            }

            return printingSubsystem.ItemToHeavy();
        }

        return printingSubsystem.ImpossiblePickup(item);
    }
}
