using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Resources;

namespace Heretic.InteractiveFiction.Subsystems;

public class BaseHelpSubsystem: IHelpSubsystem
{
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly IGrammar grammar;

    public BaseHelpSubsystem(IGrammar grammar, IPrintingSubsystem printingSubsystem)
    {
        this.grammar = grammar;
        this.printingSubsystem = printingSubsystem;
    }

    public virtual bool Help()
    {
        return this.General()
               && this.Directions()
               && this.Talks()
               && this.Interactions()
               && this.Container()
               && this.MetaInformation();
    }

    public virtual bool Help(HelpCategory helpCategory)
    {
        return helpCategory switch
        {
            HelpCategory.General => this.General(),
            HelpCategory.Directions => this.Directions(),
            HelpCategory.Talks => this.Talks(),
            HelpCategory.Interactions => this.Interactions(),
            HelpCategory.Containers => this.Container(),
            HelpCategory.MetaInformation => this.MetaInformation(),
            _ => throw new ArgumentOutOfRangeException(nameof(helpCategory), helpCategory, null)
        };
    }

    public virtual bool Help(IList<Verb> verbs)
    {
        if (verbs.Any())
        {
            printingSubsystem.Resource(HelpDescriptions.VERBS_INDIVIDUAL, wordWrap: false);
            printingSubsystem.Resource(new string('-', HelpDescriptions.VERBS_INDIVIDUAL.Length));
            VerbHelp(verbs);
            
            return true;
        }

        return printingSubsystem.Misconcept();
    }
    
    protected virtual bool General()
    {
        var look = this.grammar.Verbs.Single(x => x.Key == VerbKey.LOOK).Variants.First().Name.ToLower();
        
        printingSubsystem.Resource(HelpDescriptions.HELP_DESCRIPTION, wordWrap: false);
        printingSubsystem.Resource(new string('-', HelpDescriptions.HELP_DESCRIPTION.Length));
        printingSubsystem.Resource(HelpDescriptions.HELP_GENERAL_INSTRUCTION_PART_I, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(look, false, false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(".");
        printingSubsystem.Resource(HelpDescriptions.EXAMPLES, false);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.HELP_GENERAL_INSTRUCTION_LOOK, wordWrap: false );
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.ROOM_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.HELP_GENERAL_INSTRUCTION_LOOK_II, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.ITEM_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.HELP_GENERAL_INSTRUCTION_LOOK_III, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.ITEM_DESCRIPTION_SHORT);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.HELP_GENERAL_INSTRUCTION_LOOK_IV, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.ITEM_DESCRIPTION_ALTERNATIVE);
        printingSubsystem.Resource(HelpDescriptions.VERBS, wordWrap: false);
        printingSubsystem.Resource(new string('-', HelpDescriptions.VERBS.Length));

        return true;
    }
    
    protected virtual bool Directions()
    {
        printingSubsystem.Resource(HelpDescriptions.VERBS_DIRECTIONS, wordWrap: false);
        printingSubsystem.Resource(new string('-', HelpDescriptions.VERBS_DIRECTIONS.Length));
        VerbHelp(GetDirectionVerbs());
        DirectionExamples();

        return true;
    }
    
    protected virtual bool Talks()
    {
        printingSubsystem.Resource(HelpDescriptions.VERBS_TALKS, wordWrap: false);
        printingSubsystem.Resource(new string('-', HelpDescriptions.VERBS_TALKS.Length));
        VerbHelp(GetTalkVerbs());
        TalkExamples();

        return true;
    }
    
    protected virtual bool Interactions()
    {
        printingSubsystem.Resource(HelpDescriptions.VERBS_INTERACT_ITEMS, wordWrap: false);
        printingSubsystem.Resource(new string('-', HelpDescriptions.VERBS_INTERACT_ITEMS.Length));
        VerbHelp(GetInteractionVerbs());
        InteractionExamples();

        return true;
    }
    
    protected virtual bool Container()
    {
        printingSubsystem.Resource(HelpDescriptions.VERBS_CONTAINER, wordWrap: false);
        printingSubsystem.Resource(new string('-', HelpDescriptions.VERBS_CONTAINER.Length));
        VerbHelp(GetContainerVerbs());
        ContainerExamples();

        return true;
    }
    
    protected virtual bool MetaInformation()
    {
        printingSubsystem.Resource(HelpDescriptions.VERBS_METAINFO, wordWrap: false);
        printingSubsystem.Resource(new string('-', HelpDescriptions.VERBS_METAINFO.Length));
        VerbHelp(GetMetaInformationVerbs());
        MetaInformationExamples();

        return true;
    }
    
    private IEnumerable<Verb> GetDirectionVerbs()
    {
        var verbKeys = new List<VerbKey>()
        {
            VerbKey.N,
            VerbKey.S,
            VerbKey.E,
            VerbKey.W,
            VerbKey.NE,
            VerbKey.NW,
            VerbKey.SE,
            VerbKey.SW,
            VerbKey.UP,
            VerbKey.DOWN,
            VerbKey.GO,
            VerbKey.WAYS
        };
            
        var result = FilterVerbs(verbKeys);
        
        return result;
    }
    
    private void DirectionExamples()
    {
        printingSubsystem.Resource(" ", false);
        printingSubsystem.Resource(HelpDescriptions.EXAMPLES, false);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_DIRECTIONS_EXAMPLE_I, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_DIRECTIONS_EXAMPLE_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_DIRECTIONS_EXAMPLE_II, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_DIRECTIONS_EXAMPLE_DESCRIPTION);
    }
    
    private IEnumerable<Verb> GetTalkVerbs()
    {
        var verbKeys = new List<VerbKey>()
        {
            VerbKey.TALK,
            VerbKey.TELL,
            VerbKey.SAY,
            VerbKey.ASK,
            VerbKey.ANSWER,
            VerbKey.LISTEN,
            VerbKey.GIVE,
        };

        var result = FilterVerbs(verbKeys);
        
        return result;
    }
    
    private void TalkExamples()
    {
        printingSubsystem.Resource(" ", false);
        printingSubsystem.Resource(HelpDescriptions.EXAMPLES, false);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_TALK_EXAMPLE_I, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_TALK_EXAMPLE_I_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_TALK_EXAMPLE_II, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_TALK_EXAMPLE_II_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_TALK_EXAMPLE_III, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_TALK_EXAMPLE_III_DESCRIPTION);
    }
    
    private IEnumerable<Verb> GetInteractionVerbs()
    {
        var verbKeys = new List<VerbKey>()
        {
            VerbKey.TAKE,
            VerbKey.DROP,
            VerbKey.PULL,
            VerbKey.PUSH,
            VerbKey.BUY,
            VerbKey.BREAK,
            VerbKey.USE,
            VerbKey.LOOK,
            VerbKey.SIT,
            VerbKey.STANDUP,
            VerbKey.JUMP,
            VerbKey.CLIMB,
            VerbKey.DESCEND,
            VerbKey.WAIT,
            VerbKey.WRITE,
            VerbKey.CONNECT,
            VerbKey.DISCONNECT,
            VerbKey.CUT,
            VerbKey.THROW,
            VerbKey.DRINK,
            VerbKey.EAT,
            VerbKey.KINDLE,
            VerbKey.PUTON,
            VerbKey.READ,
            VerbKey.SLEEP,
            VerbKey.SMELL,
            VerbKey.TASTE,
            VerbKey.SWITCHOFF,
            VerbKey.SWITCHON,
            VerbKey.WEAR,
            VerbKey.TAKEOFF
        };

        var result = FilterVerbs(verbKeys);
        
        return result;
    }
    
    private void InteractionExamples()
    {
        printingSubsystem.Resource(" ", false);
        printingSubsystem.Resource(HelpDescriptions.EXAMPLES, false);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_I, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_I_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_II, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_II_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_III, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_III_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_IV, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_IV_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_V, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_INTERACT_ITEMS_EXAMPLE_V_DESCRIPTION);
    }
    
    private IEnumerable<Verb> GetContainerVerbs()
    {
        var verbKeys = new List<VerbKey>()
        {
            VerbKey.CLOSE,
            VerbKey.OPEN,
            VerbKey.UNLOCK,
            VerbKey.LOCK,
            VerbKey.TURN,
        };

        var result = FilterVerbs(verbKeys);
        
        return result;
    }
    
    private void ContainerExamples()
    {
        printingSubsystem.Resource(" ", false);
        printingSubsystem.Resource(HelpDescriptions.EXAMPLES, false);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_CONTAINER_EXAMPLE_I, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_CONTAINER_EXAMPLE_I_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_CONTAINER_EXAMPLE_II, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_CONTAINER_EXAMPLE_II_DESCRIPTION);
    }
    
    private IEnumerable<Verb> GetMetaInformationVerbs()
    {
        var verbKeys = new List<VerbKey>()
        {
            VerbKey.INV,
            VerbKey.CREDITS,
            VerbKey.SCORE,
            VerbKey.ALTER_EGO,
            VerbKey.HISTORY,
            VerbKey.HELP,
            VerbKey.HINT,
            VerbKey.REM,
            VerbKey.RESTART,
            VerbKey.SAVE,
            VerbKey.QUIT
        };
            
        var result = FilterVerbs(verbKeys);
        
        return result;
    }
    
    private void MetaInformationExamples()
    {
        printingSubsystem.Resource(" ", false);
        printingSubsystem.Resource(HelpDescriptions.EXAMPLES, false);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_METAINFO_EXAMPLE_I, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_METAINFO_EXAMPLE_I_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_METAINFO_EXAMPLE_II, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_METAINFO_EXAMPLE_II_DESCRIPTION);
        printingSubsystem.Resource(HelpDescriptions.PROMPT, false, false);
        printingSubsystem.ForegroundColor = TextColor.Green;
        printingSubsystem.Resource(HelpDescriptions.VERBS_METAINFO_EXAMPLE_III, wordWrap: false);
        printingSubsystem.ResetColors();
        printingSubsystem.Resource(HelpDescriptions.VERBS_METAINFO_EXAMPLE_II_DESCRIPTION);
    }

    private IEnumerable<Verb> FilterVerbs(IList<VerbKey> verbKeys)
    {
        var result = grammar.Verbs.Where(x => verbKeys.Contains(x.Key)).OrderBy(x => verbKeys.IndexOf(x.Key));
        return result;
    }

    private void VerbHelp(IEnumerable<Verb> verbs)
    {
        foreach (var verb in verbs)
        {
            printingSubsystem.Resource(BaseDescriptions.ResourceManager.GetString(verb.Key.ToString())!, false, false);
            var index = 0;
            printingSubsystem.ForegroundColor = TextColor.Magenta;
            var verbVariantsWithoutUmlauts = verb.Variants.Where(v => !v.IsUmlautVariant).ToList();
            foreach (var variant in verbVariantsWithoutUmlauts)
            {
                printingSubsystem.Resource(index != 0 ? ", " : "...", false, false);
                printingSubsystem.Resource(variant.Name, false, false);
                if (!string.IsNullOrEmpty(variant.Prefix))
                {
                    printingSubsystem.Resource($" [{variant.Prefix}]", false, false);    
                }
                index++;
            }

            printingSubsystem.ResetColors();
            printingSubsystem.Resource(" ", false);
        }
    }
}