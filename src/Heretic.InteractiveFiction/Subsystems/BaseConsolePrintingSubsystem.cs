using System.Collections.ObjectModel;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Subsystems;

public abstract class BaseConsolePrintingSubsystem: IPrintingSubsystem
{
    public TextColor ForegroundColor
    {
        get => (TextColor)Console.ForegroundColor;
        set => Console.ForegroundColor = (ConsoleColor)value;
    }

    public TextColor BackgroundColor
    {
        get => (TextColor)Console.BackgroundColor;
        set => Console.BackgroundColor = (ConsoleColor)value;
    }

    public void ResetColors()
    {
        Console.ResetColor();
    }

    public virtual bool ActiveLocation(Location activeLocation, IDictionary<Location, IEnumerable<DestinationNode>> locationMap)
    {
        Console.Write(WordWrap(activeLocation, Console.WindowWidth));
        DestinationNode(activeLocation, locationMap);
        return true;
    }

    public virtual bool ActivePlayer(Player activePlayer)
    {
        Console.Write(WordWrap(activePlayer, Console.WindowWidth));
        return true;
    }

    public virtual bool AlterEgo(AContainerObject item)
    {
        if (item == default)
        {
            Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_VISIBLE, Console.WindowWidth));
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine(WordWrap(item.AlterEgo(), Console.WindowWidth));
        }
        return true;
    }

    public virtual bool AlterEgo(string itemName)
    {
        if (string.IsNullOrEmpty(itemName))
        {
            Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_VISIBLE, Console.WindowWidth));
            Console.WriteLine();

        }
        else
        {
            var result = new StringBuilder();
            result.AppendFormat(BaseDescriptions.ALTER_EGO_DESCRIPTION, this.GetObjectName(itemName));
            result.AppendLine(string.Join(", ", itemName.Split('|')));
            Console.WriteLine(WordWrap(result.ToString(), Console.WindowWidth));
        }

        return true;
    }

    public virtual bool CanNotUseObject(string objectName)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_UNKNOWN, Console.WindowWidth), objectName);
        Console.WriteLine();

        return true;
    }

    public virtual void ClearScreen()
    {
        Console.Clear();
    }

    public virtual bool PrintObject(AContainerObject item)
    {
        if (item == default)
        {
            Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_VISIBLE, Console.WindowWidth));
            Console.WriteLine();
        }
        else
        {
            Console.Write(WordWrap(item, Console.WindowWidth));
        }
        return true;
    }

    public virtual bool Help(IDictionary<string, IEnumerable<string>> verbResource)
    {
        GeneralHelp();
        VerbGroupDirections(verbResource);
        VerbTalks(verbResource);
        VerbInteractItems(verbResource);
        VerbContainers(verbResource);
        VerbMetaInfos(verbResource);
        
        return true;
    }

    private void VerbHelp(IDictionary<string, IEnumerable<string>> verbResource)
    {
        foreach (var verbs in verbResource)
        {
            Console.Write(BaseDescriptions.ResourceManager.GetString(verbs.Key));
            var index = 0;
            this.ForegroundColor = TextColor.Magenta;
            foreach (var value in verbs.Value)
            {
                Console.Write(index != 0 ? ", " : "...");

                Console.Write(value);
                index++;
            }

            this.ResetColors();

            Console.WriteLine();
        }
    }

    private IDictionary<string, IEnumerable<string>> GetDirectionVerbs(
        IDictionary<string, IEnumerable<string>> verbResource)
    {
        var directionVerbKeys = new Collection<string>()
        {
            nameof(Verbs.N),
            nameof(Verbs.S),
            nameof(Verbs.E),
            nameof(Verbs.W),
            nameof(Verbs.NE),
            nameof(Verbs.NW),
            nameof(Verbs.SE),
            nameof(Verbs.SW),
            nameof(Verbs.UP),
            nameof(Verbs.DOWN),
            nameof(Verbs.GO),
            nameof(Verbs.WAYS),
            nameof(Verbs.JUMP),
        };
            
        var result = verbResource.Where(x => directionVerbKeys.Contains(x.Key)).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        
        return result;
    }    
    
    
    private void VerbGroupDirections(IDictionary<string, IEnumerable<string>> verbResource)
    {
        Console.WriteLine(HelpDescriptions.VERBS_DIRECTIONS);
        Console.WriteLine(new string('-', HelpDescriptions.VERBS_DIRECTIONS.Length));
        VerbHelp(GetDirectionVerbs(verbResource));
        VerbGroupDirectionsExamples();
    }

    private void VerbGroupDirectionsExamples()
    {
        Console.WriteLine(HelpDescriptions.EXAMPLES);
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_DIRECTIONS_EXAMPLE_I);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_DIRECTIONS_EXAMPLE_DESCRIPTION);
        Console.WriteLine();
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_DIRECTIONS_EXAMPLE_II);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_DIRECTIONS_EXAMPLE_DESCRIPTION);
        Console.WriteLine();
    }

    private IDictionary<string, IEnumerable<string>> GetMetaInfoVerbs(
        IDictionary<string, IEnumerable<string>> verbResource)
    {
        var directionVerbKeys = new Collection<string>()
        {
            nameof(Verbs.INV),
            nameof(Verbs.CREDITS),
            nameof(Verbs.SCORE),
            nameof(Verbs.ALTER_EGO),
            nameof(Verbs.HISTORY),
            nameof(Verbs.NAME),
            nameof(Verbs.HELP),
            nameof(Verbs.QUIT)
        };
            
        var result = verbResource.Where(x => directionVerbKeys.Contains(x.Key)).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        
        return result;
    } 
    
    private void VerbMetaInfos(IDictionary<string, IEnumerable<string>> verbResource)
    {
        Console.WriteLine(HelpDescriptions.VERBS_METAINFO);
        Console.WriteLine(new string('-', HelpDescriptions.VERBS_METAINFO.Length));
        VerbHelp(GetMetaInfoVerbs(verbResource));
        VerbMetaInfosExamples();
    }

    private void VerbMetaInfosExamples()
    {
        Console.WriteLine(HelpDescriptions.EXAMPLES);
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_METAINFO_EXAMPLE_I);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_METAINFO_EXAMPLE_I_DESCRIPTION);
        Console.WriteLine();
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_METAINFO_EXAMPLE_II);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_METAINFO_EXAMPLE_II_DESCRIPTION);
        Console.WriteLine();
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_METAINFO_EXAMPLE_III);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_METAINFO_EXAMPLE_II_DESCRIPTION);
        Console.WriteLine();
    }

    private IDictionary<string, IEnumerable<string>> GetInteractVerbs(
        IDictionary<string, IEnumerable<string>> verbResource)
    {
        var directionVerbKeys = new Collection<string>()
        {
            nameof(Verbs.TAKE),
            nameof(Verbs.DROP),
            nameof(Verbs.PULL),
            nameof(Verbs.BUY),
            nameof(Verbs.BREAK),
            nameof(Verbs.USE),
            nameof(Verbs.LOOK),
            nameof(Verbs.EAT),
            nameof(Verbs.SIT),
            nameof(Verbs.STANDUP),
            nameof(Verbs.WRITE),
        };
            
        var result = verbResource.Where(x => directionVerbKeys.Contains(x.Key)).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        
        return result;
    } 
    
    private void VerbInteractItems(IDictionary<string, IEnumerable<string>> verbResource)
    {
        Console.WriteLine(HelpDescriptions.VERBS_INTERACT_ITEMS);
        Console.WriteLine(new string('-', HelpDescriptions.VERBS_INTERACT_ITEMS.Length));
        VerbHelp(GetInteractVerbs(verbResource));
        VerbInteractItemsExamples();
    }

    private void VerbInteractItemsExamples()
    {
        Console.WriteLine(HelpDescriptions.EXAMPLES);
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_I);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_I_DESCRIPTION);
        Console.WriteLine();
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_II);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_II_DESCRIPTION);
        Console.WriteLine();
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_III);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_III_DESCRIPTION);
        Console.WriteLine();
    }

    private IDictionary<string, IEnumerable<string>> GetTalkVerbs(
        IDictionary<string, IEnumerable<string>> verbResource)
    {
        var directionVerbKeys = new Collection<string>()
        {
            nameof(Verbs.TALK),
            nameof(Verbs.SAY),
            nameof(Verbs.ASK),
            nameof(Verbs.GIVE),
        };
            
        var result = verbResource.Where(x => directionVerbKeys.Contains(x.Key)).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        
        return result;
    }
    
    private void VerbTalks(IDictionary<string, IEnumerable<string>> verbResource)
    {
        Console.WriteLine(HelpDescriptions.VERBS_TALKS);
        Console.WriteLine(new string('-', HelpDescriptions.VERBS_TALKS.Length));
        VerbHelp(GetTalkVerbs(verbResource));
        VerbTalksExamples();
    }

    private void VerbTalksExamples()
    {
        Console.WriteLine(HelpDescriptions.EXAMPLES);
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_TALK_EXAMPLE_I);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_TALK_EXAMPLE_I_DESCRIPTION);
        Console.WriteLine();
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_TALK_EXAMPLE_II);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_TALK_EXAMPLE_II_DESCRIPTION);
        Console.WriteLine();
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_TALK_EXAMPLE_III);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_TALK_EXAMPLE_III_DESCRIPTION);
        Console.WriteLine();
    }

    private IDictionary<string, IEnumerable<string>> GetContainerVerbs(
        IDictionary<string, IEnumerable<string>> verbResource)
    {
        var directionVerbKeys = new Collection<string>()
        {
            nameof(Verbs.CLOSE),
            nameof(Verbs.OPEN),
            nameof(Verbs.UNLOCK),
            nameof(Verbs.LOCK),
            nameof(Verbs.TURN)
        };
            
        var result = verbResource.Where(x => directionVerbKeys.Contains(x.Key)).OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        
        return result;
    } 
   
    private void VerbContainers(IDictionary<string, IEnumerable<string>> verbResource)
    {
        Console.WriteLine(HelpDescriptions.VERBS_CONTAINER);
        Console.WriteLine(new string('-', HelpDescriptions.VERBS_CONTAINER.Length));
        VerbHelp(GetContainerVerbs(verbResource));
        VerbContainersExamples();
    }

    private void VerbContainersExamples()
    {
        Console.WriteLine(HelpDescriptions.EXAMPLES);
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_CONTAINER_EXAMPLE_I);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_CONTAINER_EXAMPLE_I_DESCRIPTION);
        Console.WriteLine();
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_CONTAINER_EXAMPLE_II);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_CONTAINER_EXAMPLE_II_DESCRIPTION);
        Console.WriteLine();
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.VERBS_TALK_EXAMPLE_III);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.VERBS_TALK_EXAMPLE_III_DESCRIPTION);
        Console.WriteLine();
    }

    private void GeneralHelp()
    {
        Console.WriteLine(HelpDescriptions.HELP_DESCRIPTION);
        Console.WriteLine(new string('-', HelpDescriptions.HELP_DESCRIPTION.Length));
        Console.Write(HelpDescriptions.HELP_GENERAL_INSTRUCTION_PART_I);
        this.ForegroundColor = TextColor.Green;
        Console.Write($@" {this.GetObjectName(Verbs.LOOK).ToLower()}");
        this.ResetColors();
        Console.WriteLine(".");
        Console.WriteLine(HelpDescriptions.EXAMPLES);
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.HELP_GENERAL_INSTRUCTION_LOOK);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.ROOM_DESCRIPTION);
        Console.WriteLine();
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.HELP_GENERAL_INSTRUCTION_LOOK_II);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.ITEM_DESCRIPTION);
        Console.WriteLine();
        Console.Write(HelpDescriptions.PROMPT);
        this.ForegroundColor = TextColor.Green;
        Console.WriteLine(HelpDescriptions.HELP_GENERAL_INSTRUCTION_LOOK_III);
        this.ResetColors();
        Console.WriteLine(HelpDescriptions.ITEM_DESCRIPTION_SHORT);
        Console.WriteLine();
        Console.WriteLine(HelpDescriptions.VERBS);
        Console.WriteLine(new string('-', HelpDescriptions.VERBS.Length));
    }

    public virtual bool History(ICollection<string> historyCollection)
    {
        var history = new StringBuilder(historyCollection.Count);
        history.AppendJoin(Environment.NewLine, historyCollection);

        Console.WriteLine(WordWrap(history, Console.WindowWidth));

        return true;
    }

    public virtual bool ItemNotVisible()
    {
        return Resource(BaseDescriptions.ITEM_NOT_VISIBLE);
    }

    public bool KeyNotVisible()
    {
        return Resource(BaseDescriptions.KEY_NOT_VISIBLE);
    }

    public virtual bool ImpossiblePickup(AContainerObject containerObject)
    {
        if (containerObject != default)
        {
            if (!string.IsNullOrEmpty(containerObject.UnPickAbleDescription))
            {
                return Resource(containerObject.UnPickAbleDescription);
            }

            if (containerObject is Character)
            {
                return Resource(BaseDescriptions.IMPOSSIBLE_CHARACTER_PICKUP);
            }
        }

        return Resource(BaseDescriptions.IMPOSSIBLE_PICKUP);
    }

    public virtual bool ItemToHeavy()
    {
        return Resource(BaseDescriptions.TO_HEAVY);
    }

    public virtual bool ItemPickupSuccess(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_PICKUP, Console.WindowWidth), this.LowerFirstChar(item.Name));
        Console.WriteLine();
        return true;
    }

    public virtual bool ImpossibleDrop(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.IMPOSSIBLE_DROP, Console.WindowWidth), this.LowerFirstChar(item.Name));
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemAlreadyClosed(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ALREADY_CLOSED, Console.WindowWidth), item.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemClosed(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.NOW_CLOSED, Console.WindowWidth), item.Name);
        Console.WriteLine();
        return true;
    }

    public bool ItemAlreadyBroken(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ALREADY_BROKEN, Console.WindowWidth), item.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemAlreadyOpen(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ALREADY_OPEN, Console.WindowWidth), item.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemAlreadyUnlocked(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ALREADY_UNLOCKED, Console.WindowWidth), item.Name);
        Console.WriteLine();
        return true;
    }

    public bool ItemEaten(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_EATEN, Console.WindowWidth), this.LowerFirstChar(item.Name));
        Console.WriteLine();
        return true;
    }

    public bool ItemUnbreakable(AContainerObject item)
    {
        if (string.IsNullOrEmpty(item.UnbreakableDescription))
        {
            Console.Write(WordWrap(BaseDescriptions.ITEM_UNBREAKABLE, Console.WindowWidth), item.Name);    
        }
        else
        {
            Console.Write(WordWrap(item.UnbreakableDescription, Console.WindowWidth));
        }
        
        Console.WriteLine();
        return true;
    }

    public bool ItemNotEatable(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_EATABLE, Console.WindowWidth));
        Console.WriteLine();
        return true;
    }

    public bool ItemSeated(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_SEATED, Console.WindowWidth), this.LowerFirstChar(item.Name));
        Console.WriteLine();
        return true;
    }

    public bool ItemNotSeatable(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_SEATABLE, Console.WindowWidth), this.LowerFirstChar(item.Name));
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemStillLocked(AContainerObject item)
    {
        Console.Write(!string.IsNullOrEmpty(item.LockDescription)
            ? WordWrap(item.LockDescription, Console.WindowWidth)
            : string.Format(WordWrap(BaseDescriptions.ITEM_STILL_LOCKED, Console.WindowWidth), item.Name));

        Console.WriteLine();
        return true;
    }

    public virtual bool ItemUnlocked(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_UNLOCKED, Console.WindowWidth), item.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemNotLockAble(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_LOCKABLE, Console.WindowWidth), item.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemOpen(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.NOW_OPEN, Console.WindowWidth), item.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemDropSuccess(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_DROP, Console.WindowWidth), this.LowerFirstChar(item.Name));
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemNotOwned()
    {
        return Resource(BaseDescriptions.ITEM_NOT_OWNED);
    }

    public virtual bool ItemAlreadyOwned()
    {
        return Resource(BaseDescriptions.ITEM_ALREADY_OWNED);
    }

    public virtual bool DestinationNode(Location activeLocation, IDictionary<Location, IEnumerable<DestinationNode>> locationMap)
    {
        if (locationMap.ContainsKey(activeLocation))
        {
            var unhiddenMappings = locationMap[activeLocation].Where(l => !l.IsHidden).ToList();
            if (unhiddenMappings.Any())
            {
                foreach (var item in unhiddenMappings)
                {
                    if (item.ShowInDescription)
                    {
                        Console.Write(WordWrap(item, Console.WindowWidth));    
                    }
                }

                Console.WriteLine();
            }
        }
        return true;
    }

    public virtual bool Misconcept()
    {
        return Resource(BaseDescriptions.MISCONCEPTION);
    }

    public virtual bool NothingToTake()
    {
        return Resource(BaseDescriptions.NOTHING_TO_TAKE);
    }

    public virtual bool NoAnswer(string phrase)
    {
        Console.WriteLine($@"{string.Format(WordWrap(BaseDescriptions.NO_ANSWER, Console.WindowWidth), phrase)}");
        return true;
    }

    public virtual bool NoAnswerToInvisibleObject(Character character)
    {
        string genderSwitch = character.Gender switch
        {
            Genders.Female => BaseDescriptions.GENDER_FEMALE,
            Genders.Male => BaseDescriptions.GENDER_MALE,
            _ => BaseDescriptions.GENDER_UNKNOWN
        };
        
        Console.WriteLine($@"{string.Format(WordWrap(BaseDescriptions.ASK_FOR_INVISIBLE_OBJECT, Console.WindowWidth), genderSwitch)}");
        return true;
    }

    public virtual bool NoAnswerToQuestion(string phrase)
    {
        Console.WriteLine($@"{string.Format(WordWrap(BaseDescriptions.NO_ANSWER_TO_QUESTION, Console.WindowWidth), phrase)}");
        return true;
    }

    public abstract bool Opening();

    public virtual bool Resource(string resource)
    {
        if (!string.IsNullOrEmpty(resource))
        {
            Console.Write(WordWrap(resource, Console.WindowWidth));
            Console.WriteLine();
        }
        return true;
    }

    public bool FormattedResource(string resource, string text)
    {
        if (!string.IsNullOrEmpty(resource) && !string.IsNullOrEmpty(text))
        {
            Console.Write(WordWrap(resource, Console.WindowWidth), text);
            Console.WriteLine();
        }
        return true;
    }

    public virtual bool Score(int score, int maxScore)
    {
        Console.WriteLine($@"{string.Format(BaseDescriptions.SCORE, score, maxScore)}");
        return true;
    }
    
    public virtual bool Talk(Character character)
    {
        if (character != default)
        {
            var talk = character.Talk();
            if (!string.IsNullOrEmpty(talk))
            {
                Console.WriteLine(WordWrap(talk, Console.WindowWidth));
            }
            else
            {
                Console.WriteLine(WordWrap(BaseDescriptions.WHAT, Console.WindowWidth));
            }
            Console.WriteLine();
        }
        return true;
    }

    public abstract bool TitleAndScore(int score, int maxScore);
    public bool ToolNotVisible()
    {
        return Resource(BaseDescriptions.TOOL_NOT_VISIBLE);
    }

    public virtual bool WrongKey(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.WRONG_KEY, Console.WindowWidth), this.LowerFirstChar(item.Name));
        Console.WriteLine();
        return true;
    }

    public virtual bool Prompt()
    {
        Console.Write(@"> ");
        return true;
    }

    public virtual bool PayWithWhat()
    {
        return Resource(BaseDescriptions.PAY_WITH_WHAT);
    }

    public virtual bool SameActionAgain()
    {
        return Resource(BaseDescriptions.SAME_ACTION_AGAIN);
    }

    public virtual bool NoEvent()
    {
        return Resource(BaseDescriptions.NO_EVENT);
    }

    public virtual bool WayIsLocked(AContainerObject item)
    {
        if (!string.IsNullOrEmpty(item.LockDescription))
        {
            return Resource(item.LockDescription);
        }

        return Resource(BaseDescriptions.WAY_IS_LOCKED);
    }

    public virtual bool WayIsClosed(AContainerObject item)
    {
        if (!string.IsNullOrEmpty(item.CloseDescription))
        {
            return Resource(item.CloseDescription);
        }

        return Resource(BaseDescriptions.WAY_IS_CLOSED);
    }

    public virtual bool ImpossibleUnlock(AContainerObject item)
    {
        Console.Write(WordWrap(BaseDescriptions.IMPOSSIBLE_UNLOCK, Console.WindowWidth), this.LowerFirstChar(item.Name));
        Console.WriteLine();
        return true;
    }

    private string LowerFirstChar(string description)
    {
        return description[..1].ToLower() + description[1..];
    }

    protected virtual string WordWrap(string message, int width)
    {
        var messageLines = message.Split(Environment.NewLine);
        var result = new StringBuilder();

        foreach (var messageLine in messageLines)
        {
            var newMessage = messageLine;

            while (!string.IsNullOrWhiteSpace(newMessage) && newMessage.Length > width)
            {
                int start;
                int last = 0;
                do
                {
                    start = last;
                    last = newMessage.IndexOf(' ', start + 1);
                } while (last > -1 && last < width);

                result.AppendLine(newMessage.Substring(0, start));
                newMessage = newMessage.Substring(start + 1);
            }

            result.AppendLine(newMessage);
        }

        return result.ToString();
    }

    protected virtual string WordWrap(object message, int width)
    {
        return WordWrap(message.ToString(), width);
    }

    private string GetObjectName(string itemName)
    {
        var sentence = itemName.Split('|');
        return sentence[0].Trim();
    }

    public abstract bool Credits();
}