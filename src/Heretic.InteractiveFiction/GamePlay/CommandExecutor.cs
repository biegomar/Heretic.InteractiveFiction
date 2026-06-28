using System.Collections.ObjectModel;
using Heretic.InteractiveFiction.GamePlay.Commands;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

internal class CommandExecutor
{
    private readonly IDictionary<VerbKey, ICommand> commands;
    private readonly Universe universe;
    private readonly IGrammar grammar;
    private readonly IVerbHandler verbHandler;
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly IHelpSubsystem helpSubsystem;
    private readonly ObjectResolver resolver;
    private readonly ActiveObjectTracker tracker;
    private readonly WorldMutator mutator;
    private readonly HistoryAdministrator historyAdministrator;
    private readonly ScoreBoard scoreBoard;

    internal int Score => this.scoreBoard.Score;
    internal int MaxScore => this.scoreBoard.MaxScore;
    
    internal CommandExecutor(Universe universe, IGrammar grammar, IPrintingSubsystem printingSubsystem, IHelpSubsystem helpSubsystem, IVerbHandler verbHandler, ScoreBoard scoreBoard)
    {
        this.printingSubsystem = printingSubsystem;
        this.helpSubsystem = helpSubsystem;
        this.universe = universe;
        this.grammar = grammar;
        this.verbHandler = verbHandler;
        this.scoreBoard = scoreBoard;
        this.historyAdministrator = new HistoryAdministrator();
        this.resolver = new ObjectResolver(universe);
        this.tracker = new ActiveObjectTracker(universe);
        this.mutator = new WorldMutator(universe);

        commands = InitCommands();
    }

    internal void AddCommandToHistory(string command)
    {
        this.historyAdministrator.Add(command);
    }

    internal ReadOnlyCollection<string> CommandHistory => new(this.historyAdministrator.All);

    private IDictionary<VerbKey, ICommand> InitCommands()
    {
        var result = new Dictionary<VerbKey, ICommand>();

        var sleepCommand = new SleepCommand(universe, printingSubsystem, resolver, tracker);
        var dropCommand = new DropCommand(universe, grammar, printingSubsystem, sleepCommand);
        var climbCommand = new ClimbCommand(universe, printingSubsystem, resolver, tracker); 
        var standUpCommand = new StandUpCommand(universe, grammar, printingSubsystem, tracker, dropCommand);
        var putOnCommand = new PutOnCommand(universe, grammar, printingSubsystem, resolver, tracker, climbCommand);
        result.Add(VerbKey.SLEEP, sleepCommand);
        result.Add(VerbKey.DROP, dropCommand);
        result.Add(VerbKey.CLIMB, climbCommand);
        result.Add(VerbKey.STANDUP, standUpCommand);
        result.Add(VerbKey.PUTON, putOnCommand);
        
        result.Add(VerbKey.REM, new RemarkCommand());
        result.Add(VerbKey.RESTART, new RestartCommand());
        result.Add(VerbKey.QUIT, new QuitCommand());
        
        result.Add(VerbKey.SCORE, new ScoreCommand(scoreBoard));
        
        result.Add(VerbKey.CREDITS, new CreditsCommand(printingSubsystem));
        result.Add(VerbKey.LOAD, new LoadCommand(printingSubsystem));
        
        result.Add(VerbKey.REVERT, new RevertCommand(historyAdministrator));

        result.Add(VerbKey.HELP, new HelpCommand(helpSubsystem, verbHandler));
        
        result.Add(VerbKey.DESCEND, new DescendCommand(universe, printingSubsystem));
        result.Add(VerbKey.HINT, new HintCommand(universe, printingSubsystem));
        result.Add(VerbKey.INV, new InventoryCommand(universe, printingSubsystem));
        result.Add(VerbKey.WAIT, new WaitCommand(universe, printingSubsystem));
        result.Add(VerbKey.WRITE, new WriteCommand(universe, printingSubsystem));
        result.Add(VerbKey.WAYS, new WaysCommand(universe, printingSubsystem));
        
        result.Add(VerbKey.HISTORY, new HistoryCommand(printingSubsystem, historyAdministrator));
        result.Add(VerbKey.SAVE, new SaveCommand(printingSubsystem, historyAdministrator));
        
        result.Add(VerbKey.ASK, new AskCommand(printingSubsystem, resolver, tracker));
        result.Add(VerbKey.CLOSE, new CloseCommand(printingSubsystem, resolver, tracker, mutator));
        result.Add(VerbKey.CUT, new CutCommand(printingSubsystem, resolver, tracker));
        result.Add(VerbKey.PULL, new PullCommand(printingSubsystem, resolver, tracker));
        result.Add(VerbKey.PUSH, new PushCommand(printingSubsystem, resolver, tracker));
        result.Add(VerbKey.READ, new ReadCommand(printingSubsystem, resolver, tracker));
        result.Add(VerbKey.SAY, new SayCommand(printingSubsystem, resolver, tracker));
        result.Add(VerbKey.TURN, new TurnCommand(printingSubsystem, resolver, tracker));
        result.Add(VerbKey.JUMP, new JumpCommand(printingSubsystem, resolver, tracker));
        result.Add(VerbKey.KINDLE, new KindleCommand(printingSubsystem, resolver, tracker));
        result.Add(VerbKey.CONNECT, new ConnectCommand(printingSubsystem, resolver, tracker));
        result.Add(VerbKey.DISCONNECT, new DisconnectCommand(printingSubsystem, resolver, tracker));
        result.Add(VerbKey.TOBE, new ToBeCommand(printingSubsystem, tracker));
        result.Add(VerbKey.TALK, new TalkCommand(printingSubsystem, resolver, tracker));
        
        result.Add(VerbKey.ALTER_EGO, new AlterEgoCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.BREAK, new BreakCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.DRINK, new DrinkCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.EAT, new EatCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.GIVE, new GiveCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.LOCK, new LockCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.LOOK, new LookCommand(universe, printingSubsystem, resolver, tracker, mutator));
        result.Add(VerbKey.OPEN, new OpenCommand(universe, printingSubsystem, resolver, tracker, mutator));
        result.Add(VerbKey.SMELL, new SmellCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.SWITCHOFF, new SwitchOffCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.SWITCHON, new SwitchOnCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.TAKEOFF, new TakeOffCommand(universe, printingSubsystem, tracker));
        result.Add(VerbKey.TASTE, new TasteCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.UNLOCK, new UnlockCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.USE, new UseCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.WEAR, new WearCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.SIT, new SitDownCommand(universe, printingSubsystem, resolver, tracker));
        result.Add(VerbKey.BUY, new BuyCommand(universe, printingSubsystem, tracker));
        result.Add(VerbKey.TAKE, new TakeCommand(universe, printingSubsystem, resolver, tracker));

        var northCommand = new ChangeLocationCommand(universe, printingSubsystem, tracker, Directions.N);
        var northEastCommand = new ChangeLocationCommand(universe, printingSubsystem, tracker, Directions.NE);
        var eastCommand = new ChangeLocationCommand(universe, printingSubsystem, tracker, Directions.E);
        var southEastCommand = new ChangeLocationCommand(universe, printingSubsystem, tracker, Directions.SE);
        var southCommand = new ChangeLocationCommand(universe, printingSubsystem, tracker, Directions.S);
        var southWestCommand = new ChangeLocationCommand(universe, printingSubsystem, tracker, Directions.SW);
        var westCommand = new ChangeLocationCommand(universe, printingSubsystem, tracker, Directions.W);
        var northWestCommand = new ChangeLocationCommand(universe, printingSubsystem, tracker, Directions.NW);
        var upCommand = new ChangeLocationCommand(universe, printingSubsystem, tracker, Directions.UP);
        var downCommand = new ChangeLocationCommand(universe, printingSubsystem, tracker, Directions.DOWN);
        
        result.Add(VerbKey.N, northCommand);
        result.Add(VerbKey.NE, northEastCommand);
        result.Add(VerbKey.E, eastCommand);
        result.Add(VerbKey.SE, southEastCommand);
        result.Add(VerbKey.S, southCommand);
        result.Add(VerbKey.SW, southWestCommand);
        result.Add(VerbKey.W, westCommand);
        result.Add(VerbKey.NW, northWestCommand);
        result.Add(VerbKey.UP, upCommand);
        result.Add(VerbKey.DOWN, downCommand);
        
        var directionCommands = new Dictionary<Directions, ICommand>
        {
            { Directions.N, northCommand },
            { Directions.NE, northEastCommand },
            { Directions.E, eastCommand },
            { Directions.SE, southEastCommand },
            { Directions.S, southCommand },
            { Directions.SW, southWestCommand },
            { Directions.W, westCommand },
            { Directions.NW, northWestCommand },
            { Directions.UP, upCommand },
            { Directions.DOWN, downCommand }
        };
        
        result.Add(VerbKey.GO, new GoCommand(universe, printingSubsystem, directionCommands));

        return result;
    }

    internal bool Execute(AdventureEvent adventureEvent)
    {
        if (adventureEvent.Predicate != null && commands.TryGetValue(adventureEvent.Predicate.Key, out var command))
        {
            return command.Execute(adventureEvent);
        }

        return false;
    }
}