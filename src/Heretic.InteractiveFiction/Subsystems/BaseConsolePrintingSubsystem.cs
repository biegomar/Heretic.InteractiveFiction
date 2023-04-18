using System.Reflection;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Subsystems;

public abstract class BaseConsolePrintingSubsystem: IPrintingSubsystem
{
    private int consoleWidth;

    protected virtual string GetVersionNumber()
    {
        try
        {
            return string.Format(BaseDescriptions.FRAMEWORK_VERSION, Assembly.GetAssembly(typeof(BaseConsolePrintingSubsystem)).GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public bool IsInSilentMode { get; set; }

    public int ConsoleWidth
    {
        get
        {
            if (this.consoleWidth == 0)
            {
                return Console.WindowWidth;
            }
        
            return Math.Min(Console.WindowWidth, this.consoleWidth);
        }
        set => consoleWidth = value;
    }

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

    public string ReadInput()
    {
        return Console.ReadLine() ?? string.Empty;
    }

    public void WaitForUserAction()
    {
        Console.ReadKey();
    }

    public void ResetColors()
    {
        Console.ResetColor();
    }

    public virtual bool ActiveLocation(Location? activeLocation, IDictionary<Location, IEnumerable<DestinationNode>> locationMap)
    {
        if (activeLocation == default)
        {
            return Resource(BaseDescriptions.ITEM_NOT_VISIBLE);
        }
        
        Resource(activeLocation, endWithLineBreak: false);
        return DestinationNode(activeLocation, locationMap);
    }

    public virtual bool ActivePlayer(Player? activePlayer)
    {
        return activePlayer == default ? Resource(BaseDescriptions.ITEM_NOT_VISIBLE) : Resource(activePlayer, endWithLineBreak: false);
    }

    public virtual bool AlterEgo(AHereticObject? item)
    {
        return Resource(item == default ? BaseDescriptions.ITEM_NOT_VISIBLE : item.AlterEgo());
    }

    public virtual bool CanNotUseObject(string objectName)
    {
        return FormattedResource(BaseDescriptions.ITEM_UNKNOWN, objectName);
    }

    public virtual void ClearScreen()
    {
        Console.Clear();
    }

    public virtual bool PrintObject(AHereticObject? item)
    {
        return item == default ? Resource(BaseDescriptions.ITEM_NOT_VISIBLE) : Resource(item, endWithLineBreak: false);
    }

    public bool Hint(AHereticObject? item)
    {
        if (item == default)
        {
            return Resource(BaseDescriptions.ITEM_NOT_VISIBLE);
        }
        
        if (string.IsNullOrEmpty(item.Hint))
        {
            var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
            return FormattedResource(BaseDescriptions.HINT_NOT_AVAILABLE, itemName);
        }

        return Resource(item.Hint);
    }
    
    public virtual bool History(ICollection<string> historyCollection)
    {
        var history = new StringBuilder(historyCollection.Count);
        history.AppendJoin(Environment.NewLine, historyCollection);

        return Resource(history);
    }

    public virtual bool ItemNotVisible()
    {
        return Resource(BaseDescriptions.ITEM_NOT_VISIBLE);
    }
    
    public virtual bool ItemNotVisible(AHereticObject? item)
    {
        if (item == default)
        {
            return this.ItemNotVisible();
        }
        
        var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
        return FormattedResource(BaseDescriptions.ITEM_NAME_NOT_VISIBLE, itemName);
    }

    public bool KeyNotVisible()
    {
        return Resource(BaseDescriptions.KEY_NOT_VISIBLE);
    }

    public virtual bool ImpossiblePickup(AHereticObject? containerObject)
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

        return Resource(this.GetRandomPhrase(BaseDescriptions.IMPOSSIBLE_PICKUP));
    }

    public virtual bool ItemToHeavy()
    {
        return Resource(BaseDescriptions.TO_HEAVY);
    }

    public virtual bool ItemPickupSuccess(AHereticObject? item)
    {
        var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
        return FormattedResource(BaseDescriptions.ITEM_PICKUP, itemName);
    }

    public virtual bool ImpossibleDrop(AHereticObject? item)
    {
        if (item != default)
        {
            if (string.IsNullOrEmpty(item.UnDropAbleDescription))
            {
                var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
                return FormattedResource(BaseDescriptions.IMPOSSIBLE_DROP, itemName);
            }

            return this.Resource(item.UnDropAbleDescription);    
        }

        return true;
    }

    public virtual bool ItemAlreadyClosed(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.ALREADY_CLOSED, item?.Name);
    }

    public virtual bool ItemClosed(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.NOW_CLOSED, item?.Name);
    }

    public bool ItemStillClosed(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.ITEM_STILL_CLOSED, item?.Name);
    }

    public bool ItemAlreadyBroken(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.ALREADY_BROKEN, item?.Name);
    }

    public virtual bool ItemAlreadyOpen(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.ALREADY_OPEN, item?.Name);
    }

    public virtual bool ItemAlreadyLocked(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.ALREADY_LOCKED, item?.Name);
    }
    
    public virtual bool ItemAlreadyUnlocked(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.ALREADY_UNLOCKED, item?.Name);
    }

    public bool ItemUnbreakable(AHereticObject? item)
    {
        if (item == default || string.IsNullOrEmpty(item.UnbreakableDescription))
        {
            return FormattedResource(BaseDescriptions.ITEM_UNBREAKABLE, item?.Name, endWithLineBreak: false);
        }
        
        return Resource(item.UnbreakableDescription);
    }

    public bool ItemUnknown(AdventureEvent? adventureEvent)
    {
        if (adventureEvent != null && adventureEvent.UnidentifiedSentenceParts.Any())
        {
            return this.FormattedResource(BaseDescriptions.ITEM_UNKNOWN,
                string.Join(BaseDescriptions.BINDING_OR, adventureEvent.UnidentifiedSentenceParts));
        }
        return true;
    }

    public bool ItemSeated(AHereticObject? item)
    {
        var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Dative, lowerFirstCharacter: true);
        return FormattedResource(BaseDescriptions.ITEM_SEATED, itemName);
    }

    public bool ItemNotSeatable(AHereticObject? item)
    {
        var itemName =
            ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
        return FormattedResource(BaseDescriptions.ITEM_NOT_SEATABLE, itemName);
    }

    public virtual bool ItemStillLocked(AHereticObject? item)
    {
        if (item != default && !string.IsNullOrEmpty(item.LockDescription))
        {
            return Resource(item.LockDescription);
        }
        
        return FormattedResource(BaseDescriptions.ITEM_STILL_LOCKED, item?.Name);
    }

    public virtual bool ItemLocked(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.ITEM_LOCKED, item?.Name);
    }
    
    public virtual bool ItemUnlocked(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.ITEM_UNLOCKED, item?.Name);
    }

    public virtual bool ItemNotLockAble(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.ITEM_NOT_LOCKABLE, item?.Name);
    }

    public virtual bool ItemOpen(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.NOW_OPEN, item?.Name);
    }

    public virtual bool ItemDropSuccess(AHereticObject? item)
    {
        var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
        return FormattedResource(BaseDescriptions.ITEM_DROP, itemName);
    }

    public bool ItemDropSuccess(AHereticObject? itemToDrop, AHereticObject? containerItem)
    {
        var itemToDropName =
            ArticleHandler.GetNameWithArticleForObject(itemToDrop, GrammarCase.Accusative, lowerFirstCharacter: true);
        var containerItemName =
            ArticleHandler.GetNameWithArticleForObject(containerItem, GrammarCase.Accusative,
                lowerFirstCharacter: true);
        
        return Resource(string.Format(BaseDescriptions.ITEM_DROP_INTO, itemToDropName, containerItemName));
    }

    public bool ItemIsNotAContainer(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.ITEM_NOT_A_CONTAINER, item?.Name);
    }

    public virtual bool ItemNotOwned()
    {
        return Resource(BaseDescriptions.ITEM_NOT_OWNED);
    }
    
    public virtual bool ItemNotOwned(AHereticObject? item)
    {
        if (item == default)
        {
            return this.ItemNotOwned();
        }
        var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative);
        return FormattedResource(BaseDescriptions.ITEM_NOT_OWNED_FORMATTED, itemName, true);
    }

    public virtual bool ItemAlreadyOwned()
    {
        return Resource(BaseDescriptions.ITEM_ALREADY_OWNED);
    }

    public virtual bool DestinationNode(Location? activeLocation, IDictionary<Location, IEnumerable<DestinationNode>> locationMap)
    {
        if (activeLocation != default && locationMap.TryGetValue(activeLocation, out var value))
        {
            var unhiddenMappings = value.Where(l => !l.IsHidden).ToList();
            if (unhiddenMappings.Any())
            {
                foreach (var item in unhiddenMappings)
                {
                    if (item.ShowInDescription)
                    {
                        Resource(item, endWithLineBreak: false);
                    }
                }

                Resource(" ", wordWrap: false);
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
        return FormattedResource(BaseDescriptions.NO_ANSWER, phrase);
    }

    public virtual bool NoAnswerToInvisibleObject(Character? character)
    {
        string genderSwitch = character?.Grammar.Gender switch
        {
            Genders.Female => BaseDescriptions.GENDER_FEMALE,
            Genders.Male => BaseDescriptions.GENDER_MALE,
            _ => BaseDescriptions.GENDER_UNKNOWN
        };

        return FormattedResource(BaseDescriptions.ASK_FOR_INVISIBLE_OBJECT, genderSwitch);
    }

    public virtual bool NoAnswerToQuestion(string phrase)
    {
        return FormattedResource(BaseDescriptions.NO_ANSWER_TO_QUESTION, phrase, lowerFirstLetter: true);
    }

    public abstract bool Opening();
    
    public abstract bool Closing();

    public virtual bool Resource(string? resource, bool endWithLineBreak = true, bool wordWrap = true)
    {
        if (!IsInSilentMode)
        {
            if (!string.IsNullOrEmpty(resource))
            {
                if (wordWrap)
                {
                    Console.Write(WordWrap(resource, this.ConsoleWidth));    
                }
                else
                {
                    Console.Write(resource, this.ConsoleWidth);
                }
            
                if (endWithLineBreak)
                {
                    Console.WriteLine();    
                }
            }
        }
        
        return true;
    }

    public bool Resource(object? resource, bool endWithLineBreak = true, bool wordWrap = true)
    {
        return Resource(resource?.ToString(), endWithLineBreak, wordWrap);
    }

    public bool FormattedResource(string? resource, string? text, bool lowerFirstLetter = false, bool endWithLineBreak = true, bool wordWrap = true)
    {
        if (!string.IsNullOrEmpty(resource) && !string.IsNullOrEmpty(text))
        {
            var formattedString = string.Format(resource, lowerFirstLetter ? text.LowerFirstChar() : text);
            Resource(formattedString, endWithLineBreak, wordWrap);
        }
        return true;
    }

    public virtual bool Score(int score, int maxScore)
    {
        return Resource(string.Format(BaseDescriptions.ACTUAL_SCORE, score, maxScore));
    }
    
    public virtual bool Talk(Character? character)
    {
        if (character != default)
        {
            var talk = character.DoTalk();
            if (!string.IsNullOrEmpty(talk))
            {
                Resource(talk);
            }
            else
            {
                Resource(BaseDescriptions.WHAT);
            }
        }
        
        return true;
    }

    public abstract bool TitleAndScore(int score, int maxScore);
    
    public bool ToolNotVisible()
    {
        return Resource(BaseDescriptions.TOOL_NOT_VISIBLE);
    }

    public virtual bool WrongKey(AHereticObject? item)
    {
        return FormattedResource(BaseDescriptions.WRONG_KEY, item?.Name, lowerFirstLetter: true);
    }

    public virtual bool Prompt()
    {
        return Resource(@"> ", false, false);
    }

    public virtual bool PayWithWhat()
    {
        return Resource(BaseDescriptions.PAY_WITH_WHAT);
    }

    public virtual bool NoEvent()
    {
        return Resource(BaseDescriptions.NO_EVENT);
    }

    public virtual bool WayIsLocked(AHereticObject? item)
    {
        if (item != default && !string.IsNullOrEmpty(item.LockDescription))
        {
            return Resource(item.LockDescription);
        }

        return Resource(BaseDescriptions.WAY_IS_LOCKED);
    }

    public virtual bool WayIsClosed(AHereticObject? item)
    {
        if (item != default && !string.IsNullOrEmpty(item.CloseDescription))
        {
            return Resource(item.CloseDescription);
        }

        return Resource(BaseDescriptions.WAY_IS_CLOSED);
    }

    public virtual bool ImpossibleLock(AHereticObject? item)
    {
        var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
        return FormattedResource(BaseDescriptions.IMPOSSIBLE_LOCK, itemName);
    }
    
    public virtual bool ImpossibleUnlock(AHereticObject? item)
    {
        var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
        return FormattedResource(BaseDescriptions.IMPOSSIBLE_UNLOCK, itemName);
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

    private string GetRandomPhrase(string message)
    {
        var phrases = message.Split("|");

        var rand = new Random();
        var index = rand.Next(phrases.Length);

        return phrases[index];
    }

    public abstract bool Credits();
}