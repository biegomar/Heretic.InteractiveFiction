using System.Collections.ObjectModel;
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
            Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_VISIBLE, this.ConsoleWidth));
            Console.WriteLine();
        }
        else
        {
            Console.Write(WordWrap(activeLocation, this.ConsoleWidth));
            DestinationNode(activeLocation, locationMap);    
        }
        
        return true;
    }

    public virtual bool ActivePlayer(Player? activePlayer)
    {
        if (activePlayer == default)
        {
            Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_VISIBLE, this.ConsoleWidth));
            Console.WriteLine();
        }
        else
        {
            Console.Write(WordWrap(activePlayer, this.ConsoleWidth));
        }
        
        return true;
    }

    public virtual bool AlterEgo(AHereticObject? item)
    {
        if (item == default)
        {
            Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_VISIBLE, this.ConsoleWidth));
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine(WordWrap(item.AlterEgo(), this.ConsoleWidth));
        }
        return true;
    }

    public virtual bool CanNotUseObject(string objectName)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_UNKNOWN, this.ConsoleWidth), objectName);
        Console.WriteLine();

        return true;
    }

    public virtual void ClearScreen()
    {
        Console.Clear();
    }

    public virtual bool PrintObject(AHereticObject? item)
    {
        if (item == default)
        {
            Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_VISIBLE, this.ConsoleWidth));
            Console.WriteLine();
        }
        else
        {
            Console.Write(WordWrap(item, this.ConsoleWidth));
        }
        return true;
    }

    public bool Hint(AHereticObject? item)
    {
        if (item == default)
        {
            Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_VISIBLE, this.ConsoleWidth));
            Console.WriteLine();
        }
        else
        {
            if (string.IsNullOrEmpty(item.Hint))
            {
                var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
                Console.Write(WordWrap(BaseDescriptions.HINT_NOT_AVAILABLE, this.ConsoleWidth), itemName);
                Console.WriteLine();
                return true;
            }
            Console.Write(WordWrap(item.Hint, this.ConsoleWidth));
        }
        return true;
    }
    
    public virtual bool History(ICollection<string> historyCollection)
    {
        var history = new StringBuilder(historyCollection.Count);
        history.AppendJoin(Environment.NewLine, historyCollection);

        Console.WriteLine(WordWrap(history, this.ConsoleWidth));

        return true;
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
        Console.Write(WordWrap(BaseDescriptions.ITEM_PICKUP, this.ConsoleWidth), itemName);
        Console.WriteLine();
        return true;
    }

    public virtual bool ImpossibleDrop(AHereticObject? item)
    {
        if (item != default)
        {
            if (string.IsNullOrEmpty(item.UnDropAbleDescription))
            {
                var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
                Console.Write(WordWrap(BaseDescriptions.IMPOSSIBLE_DROP, this.ConsoleWidth), itemName);
                Console.WriteLine();
            
                return true;
            }

            return this.Resource(item.UnDropAbleDescription);    
        }

        return true;
    }

    public virtual bool ItemAlreadyClosed(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.ALREADY_CLOSED, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemClosed(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.NOW_CLOSED, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
    }

    public bool ItemStillClosed(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_STILL_CLOSED, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
    }

    public bool ItemAlreadyBroken(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.ALREADY_BROKEN, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemAlreadyOpen(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.ALREADY_OPEN, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemAlreadyLocked(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.ALREADY_LOCKED, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
    }
    
    public virtual bool ItemAlreadyUnlocked(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.ALREADY_UNLOCKED, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
    }

    public bool ItemUnbreakable(AHereticObject? item)
    {
        if (item == default || string.IsNullOrEmpty(item.UnbreakableDescription))
        {
            Console.Write(WordWrap(BaseDescriptions.ITEM_UNBREAKABLE, this.ConsoleWidth), item?.Name);    
        }
        else
        {
            Console.Write(WordWrap(item.UnbreakableDescription, this.ConsoleWidth));
        }
        
        Console.WriteLine();
        return true;
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
        Console.Write(WordWrap(BaseDescriptions.ITEM_SEATED, this.ConsoleWidth), itemName);
        Console.WriteLine();
        return true;
    }

    public bool ItemNotSeatable(AHereticObject? item)
    {
        var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
        Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_SEATABLE, this.ConsoleWidth), itemName);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemStillLocked(AHereticObject? item)
    {
        if (item != default && !string.IsNullOrEmpty(item.LockDescription))
        {
            Console.Write(WordWrap(item.LockDescription, this.ConsoleWidth));
        }
        else
        {
            Console.Write(WordWrap(BaseDescriptions.ITEM_STILL_LOCKED, this.ConsoleWidth), item?.Name);
        }
        
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemLocked(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_LOCKED, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
    }
    
    public virtual bool ItemUnlocked(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_UNLOCKED, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemNotLockAble(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_LOCKABLE, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemOpen(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.NOW_OPEN, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
    }

    public virtual bool ItemDropSuccess(AHereticObject? item)
    {
        var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
        Console.Write(WordWrap(BaseDescriptions.ITEM_DROP, this.ConsoleWidth), itemName);
        Console.WriteLine();
        return true;
    }

    public bool ItemDropSuccess(AHereticObject? itemToDrop, AHereticObject? containerItem)
    {
        var itemToDropName = ArticleHandler.GetNameWithArticleForObject(itemToDrop, GrammarCase.Accusative, lowerFirstCharacter: true);
        var containerItemName = ArticleHandler.GetNameWithArticleForObject(containerItem, GrammarCase.Accusative, lowerFirstCharacter: true);
        Console.Write(WordWrap(BaseDescriptions.ITEM_DROP_INTO, this.ConsoleWidth), itemToDropName, containerItemName);
        Console.WriteLine();
        return true;
    }

    public bool ItemIsNotAContainer(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.ITEM_NOT_A_CONTAINER, this.ConsoleWidth), item?.Name);
        Console.WriteLine();
        return true;
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
        if (activeLocation != default && locationMap.ContainsKey(activeLocation))
        {
            var unhiddenMappings = locationMap[activeLocation].Where(l => !l.IsHidden).ToList();
            if (unhiddenMappings.Any())
            {
                foreach (var item in unhiddenMappings)
                {
                    if (item.ShowInDescription)
                    {
                        Console.Write(WordWrap(item, this.ConsoleWidth));    
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
        Console.WriteLine($@"{string.Format(WordWrap(BaseDescriptions.NO_ANSWER, this.ConsoleWidth), phrase)}");
        return true;
    }

    public virtual bool NoAnswerToInvisibleObject(Character? character)
    {
        string genderSwitch = character?.Grammar.Gender switch
        {
            Genders.Female => BaseDescriptions.GENDER_FEMALE,
            Genders.Male => BaseDescriptions.GENDER_MALE,
            _ => BaseDescriptions.GENDER_UNKNOWN
        };
        
        Console.WriteLine($@"{string.Format(WordWrap(BaseDescriptions.ASK_FOR_INVISIBLE_OBJECT, this.ConsoleWidth), genderSwitch)}");
        return true;
    }

    public virtual bool NoAnswerToQuestion(string phrase)
    {
        Console.WriteLine($@"{string.Format(WordWrap(BaseDescriptions.NO_ANSWER_TO_QUESTION, this.ConsoleWidth), phrase.LowerFirstChar())}");
        return true;
    }

    public abstract bool Opening();
    
    public abstract bool Closing();

    public virtual bool Resource(string resource, bool endWithLineBreak = true, bool wordWrap = true)
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
        return true;
    }

    public bool FormattedResource(string resource, string text, bool lowerFirstLetter = false)
    {
        if (!string.IsNullOrEmpty(resource) && !string.IsNullOrEmpty(text))
        {
            var formattedString = string.Format(resource, lowerFirstLetter ? text.LowerFirstChar() : text);
            Console.Write(WordWrap(formattedString, this.ConsoleWidth));
            Console.WriteLine();
        }
        return true;
    }

    public virtual bool Score(int score, int maxScore)
    {
        Console.WriteLine($@"{string.Format(BaseDescriptions.ACTUAL_SCORE, score, maxScore)}");
        return true;
    }
    
    public virtual bool Talk(Character? character)
    {
        if (character != default)
        {
            var talk = character.DoTalk();
            if (!string.IsNullOrEmpty(talk))
            {
                Console.WriteLine(WordWrap(talk, this.ConsoleWidth));
            }
            else
            {
                Console.WriteLine(WordWrap(BaseDescriptions.WHAT, this.ConsoleWidth));
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

    public virtual bool WrongKey(AHereticObject? item)
    {
        Console.Write(WordWrap(BaseDescriptions.WRONG_KEY, this.ConsoleWidth), item?.Name.LowerFirstChar());
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
        Console.Write(WordWrap(BaseDescriptions.IMPOSSIBLE_LOCK, this.ConsoleWidth), itemName);
        Console.WriteLine();
        return true;
    }
    
    public virtual bool ImpossibleUnlock(AHereticObject? item)
    {
        var itemName = ArticleHandler.GetNameWithArticleForObject(item, GrammarCase.Accusative, lowerFirstCharacter: true);
        Console.Write(WordWrap(BaseDescriptions.IMPOSSIBLE_UNLOCK, this.ConsoleWidth), itemName);
        Console.WriteLine();
        return true;
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