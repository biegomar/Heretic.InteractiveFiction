﻿using Heretic.InteractiveFiction.Comparer;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.GamePlay.EventSystem;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.Objects;

public sealed class Universe
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

    public int NumberOfSolvedQuests => this.SolvedQuests.Count;


    public readonly IList<Verb> Verbs;
    public readonly IDictionary<string, IEnumerable<string>> ItemResources;
    public readonly IDictionary<string, IEnumerable<string>> CharacterResources;
    public readonly IDictionary<string, IEnumerable<string>> LocationResources;
    public readonly IDictionary<string, IEnumerable<string>> ConversationAnswersResources;
    public readonly IEnumerable<string> PackingWordsResources;

    public LocationMap LocationMap { get; set; }
    public Location ActiveLocation { get; set; }
    public Player ActivePlayer { get; set; }
    public AHereticObject ActiveObject { get; set; }
    public bool IsPeriodicEventActivated { get => this.periodicEvent.Active; set => this.periodicEvent.Active = value; }
    public ICollection<string> Quests { get; set; }
    public readonly IDictionary<string, int> ScoreBoard = new Dictionary<string, int>();
    public event EventHandler<PeriodicEventArgs> PeriodicEvents;

    private readonly IPrintingSubsystem printingSubsystem;
    private readonly IList<string> SolvedQuests;
    private int maxScore;
    private PeriodicEvent periodicEvent;

    public Universe(IPrintingSubsystem printingSubsystem, IResourceProvider resourceProvider)
    {
        this.printingSubsystem = printingSubsystem;
        this.SolvedQuests = new List<string>();
        this.Score = 0;
        this.Verbs = resourceProvider.GetVerbsFromResources();
        this.ItemResources = resourceProvider.GetItemsFromResources();
        this.CharacterResources = resourceProvider.GetCharactersFromResources();
        this.LocationResources = resourceProvider.GetLocationsFromResources();
        this.PackingWordsResources = resourceProvider.GetPackingWordsFromResources();
        this.ConversationAnswersResources = resourceProvider.GetConversationsAnswersFromResources();
        this.LocationMap = new LocationMap(new LocationComparer());
        this.ActiveObject = null;
    }

    public bool IsVerb(string verbKey, string verbToCheck)
    {
        var verbOverrides = this.ActiveLocation.GetOptionalVerbs(verbKey);
        var optionalVerbNames = verbOverrides.SelectMany(verb => verb.Names).ToList();

        var mergedVerbs = verbOverrides.Count > 0
            ? this.Verbs.Single(verb => verb.Key == verbKey).Names.Union(optionalVerbNames)
            : this.Verbs.Single(verb => verb.Key == verbKey).Names;
        
        return mergedVerbs.Contains(verbToCheck, StringComparer.InvariantCultureIgnoreCase);
    }

    public bool IsVerb(string verbToCheck)
    {
        var verbOverrides = this.ActiveLocation.OptionalVerbs.Values.SelectMany(x => x);
        
        return verbOverrides.Any()
            ? this.Verbs.Select(v => v.Key).Contains(verbToCheck, StringComparer.InvariantCultureIgnoreCase)
              || verbOverrides.Select(v => v.Key).Contains(verbToCheck, StringComparer.InvariantCultureIgnoreCase)
            : this.Verbs.Select(v => v.Key).Contains(verbToCheck, StringComparer.InvariantCultureIgnoreCase);
    }
    
    public Verb GetVerb(string verbToCheck)
    {
        var result = this.Verbs.FirstOrDefault(v => v.Key.Equals(verbToCheck, StringComparison.InvariantCultureIgnoreCase));
        if (result != default)
        {
            return result;
        }
        
        var verbOverrides = this.ActiveLocation.OptionalVerbs.SelectMany(x => x.Value).ToList();
        return verbOverrides.SingleOrDefault(v => v.Key.Equals(verbToCheck, StringComparison.InvariantCultureIgnoreCase));
    }

    public string[] NormalizeVerbInSentence(IList<string> sentence)
    {
        bool ReplaceVerb(string verbToReplace, ICollection<Verb> verbs, IList<string> resultingSentence)
        {
            Verb verb = default;
            if (verbs.Count == 1)
            {
                verb = verbs.First();
                var index = resultingSentence.IndexOf(verbToReplace);
                resultingSentence[index] = verb.Key;

                return true;
            }
            else
            {
                foreach (var possibleVerb in verbs)
                {
                    if (sentence.Contains(possibleVerb.Prefix))
                    {
                        verb = possibleVerb;
                        var index = resultingSentence.IndexOf(verbToReplace);
                        resultingSentence[index] = verb.Key;
                        resultingSentence.Remove(verb.Prefix);
                        return true;
                    }
                }

                if (verb == default)
                {
                    //only one verb without prefix is possible
                    var verbWithoutPrefix = verbs.SingleOrDefault(v => v.Prefix == string.Empty);
                    if (verbWithoutPrefix != default)
                    {
                        verb = verbWithoutPrefix;
                        var index = resultingSentence.IndexOf(verbToReplace);
                        resultingSentence[index] = verb.Key;
                        return true;
                    }
                }
            }

            return false;
        }

        var result = sentence.ToList();
        var isVerbReplaced = false;
        
        foreach (var word in sentence)
        {
            var possibleVerbs =this.Verbs.Where(v => v.Names.Contains(word, StringComparer.InvariantCultureIgnoreCase)).ToList();
            if (possibleVerbs.Any())
            {
                isVerbReplaced = ReplaceVerb(word, possibleVerbs, result);
            }
            else
            {
                var verbOverrides = this.ActiveLocation.OptionalVerbs.SelectMany(x => x.Value).ToList();
                var optionalVerbs = verbOverrides.Where(v => v.Names.Contains(word, StringComparer.InvariantCultureIgnoreCase)).ToList();
                if (optionalVerbs.Any())
                {
                    isVerbReplaced = ReplaceVerb(word, optionalVerbs, result);
                }
            }

            if (isVerbReplaced)
            {
                break;
            }
        }
        

        return result.ToArray();
    }

    public void SetPeriodicEvent(PeriodicEvent periodicEvent)
    {
        if (periodicEvent != null)
        {
            this.periodicEvent = periodicEvent;
            this.periodicEvent.Active = false;
            this.PeriodicEvents += this.periodicEvent.RaiseEvent;    
        }
    }

    public void RaisePeriodicEvents(PeriodicEventArgs eventArgs)
    {
        var localEventHandler = this.PeriodicEvents;
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
            printingSubsystem.ForegroundColor = TextColor.Magenta;
            printingSubsystem.FormattedResource(BaseDescriptions.QUEST_SOLVED, quest);
            printingSubsystem.ResetColors();
        }
    }

    public bool PickObject(Item item, bool suppressSuccessMessage = false)
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

        return printingSubsystem.ItemNotVisible();
    }

    public void UnveilFirstLevelObjects(AHereticObject container)
    {
        if (container == default)
        {
            return;
        }

        var unveilAbleLinkedItems = container.LinkedTo.Where(i => i.IsUnveilable).ToList();
        if (unveilAbleLinkedItems.Any())
        {
            foreach (var linkedItem in unveilAbleLinkedItems)
            {
                linkedItem.IsHidden = false;
            }
        }
        
        if (container.IsCloseable && container.IsClosed)
        {
            return;
        }

        var unveilAbleItems = container.Items.Where(i => i.IsUnveilable).ToList();
        if (unveilAbleItems.Any())
        {
            foreach (var item in unveilAbleItems)
            {
                item.IsHidden = false;
            }
        }

        var unveilAbleCharacters = container.Characters.Where(c => c.IsUnveilable).ToList();
        if (unveilAbleCharacters.Any())
        {
            foreach (var character in unveilAbleCharacters)
            {
                character.IsHidden = false;
            }
        }
    }

    public DestinationNode GetDestinationNodeFromActiveLocationByDirection(Directions key)
    {
        if (this.LocationMap.ContainsKey(this.ActiveLocation))
        {
            return this.LocationMap[this.ActiveLocation].FirstOrDefault(l => l.Direction == key);
        }

        return default;
    }
    
    public Location GetLocationByKey(string key)
    {
        return this.LocationMap.Keys.SingleOrDefault(l => l.Key == key);
    }
    
    public AHereticObject GetObjectFromWorld(string key)
    {
        foreach (var location in this.LocationMap.Keys)
        {
            var result = location.GetObject(key);
            if (result != default && result.Key == key)
            {
                return result;
            }
        }

        return this.ActivePlayer.GetObject(key);
    }

    private int GetMaxScore()
    {
        return this.ScoreBoard.Sum(kv => kv.Value);
    }
}
