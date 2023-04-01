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
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly ObjectHandler objectHandler;
    private readonly HistoryAdministrator historyAdministrator;
    private readonly ScoreBoard scoreBoard;
    
    internal CommandExecutor(Universe universe, IGrammar grammar, IPrintingSubsystem printingSubsystem, HistoryAdministrator historyAdministrator, ScoreBoard scoreBoard)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
        this.grammar = grammar;
        this.historyAdministrator = historyAdministrator;
        this.scoreBoard = scoreBoard;
        objectHandler = new ObjectHandler(universe);

        commands = InitCommands();
    }

    private IDictionary<VerbKey, ICommand> InitCommands()
    {
        var result = new Dictionary<VerbKey, ICommand>();

        var sleepCommand = new SleepCommand(universe, printingSubsystem, objectHandler);
        var dropCommand = new DropCommand(universe, grammar, printingSubsystem, sleepCommand);
        var climbCommand = new ClimbCommand(universe, printingSubsystem, objectHandler); 
        var standUpCommand = new StandUpCommand(universe, grammar, printingSubsystem, objectHandler, dropCommand);
        var putOnCommand = new PutOnCommand(universe, grammar, printingSubsystem, objectHandler, climbCommand);
        result.Add(VerbKey.SLEEP, sleepCommand);
        result.Add(VerbKey.DROP, dropCommand);
        result.Add(VerbKey.CLIMB, climbCommand);
        result.Add(VerbKey.STANDUP, standUpCommand);
        result.Add(VerbKey.PUTON, putOnCommand);
        
        result.Add(VerbKey.REM, new RemarkCommand());
        result.Add(VerbKey.QUIT, new QuitCommand());
        
        result.Add(VerbKey.SCORE, new ScoreCommand(scoreBoard));
        
        result.Add(VerbKey.CREDITS, new CreditsCommand(printingSubsystem));
        result.Add(VerbKey.HINT, new HintCommand(printingSubsystem));

        result.Add(VerbKey.HELP, new HelpCommand(grammar, printingSubsystem));
        
        result.Add(VerbKey.DESCEND, new DescendCommand(universe, printingSubsystem));
        result.Add(VerbKey.INV, new InventoryCommand(universe, printingSubsystem));
        result.Add(VerbKey.WAIT, new WaitCommand(universe, printingSubsystem));
        result.Add(VerbKey.WRITE, new WriteCommand(universe, printingSubsystem));
        result.Add(VerbKey.WAYS, new WaysCommand(universe, printingSubsystem));
        
        result.Add(VerbKey.HISTORY, new HistoryCommand(printingSubsystem, historyAdministrator));
        result.Add(VerbKey.SAVE, new SaveCommand(printingSubsystem, historyAdministrator));
        
        result.Add(VerbKey.ASK, new AskCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.CLOSE, new CloseCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.CUT, new CutCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.PULL, new PullCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.PUSH, new PushCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.READ, new ReadCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.SAY, new SayCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.TURN, new TurnCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.JUMP, new JumpCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.KINDLE, new KindleCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.CONNECT, new ConnectCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.DISCONNECT, new DisconnectCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.TOBE, new ToBeCommand(printingSubsystem, objectHandler));
        result.Add(VerbKey.TALK, new TalkCommand(printingSubsystem, objectHandler));
        
        result.Add(VerbKey.ALTER_EGO, new AlterEgoCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.BREAK, new BreakCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.DRINK, new DrinkCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.EAT, new EatCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.GIVE, new GiveCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.LOCK, new LockCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.LOOK, new LookCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.OPEN, new OpenCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.SMELL, new SmellCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.SWITCHOFF, new SwitchOffCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.SWITCHON, new SwitchOnCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.TAKEOFF, new TakeOffCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.TASTE, new TasteCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.UNLOCK, new UnlockCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.USE, new UseCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.WEAR, new WearCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.SIT, new SitDownCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.BUY, new BuyCommand(universe, printingSubsystem, objectHandler));
        result.Add(VerbKey.TAKE, new TakeCommand(universe, printingSubsystem, objectHandler));

        var northCommand = new ChangeLocationCommand(universe, printingSubsystem, objectHandler, Directions.N);
        var northEastCommand = new ChangeLocationCommand(universe, printingSubsystem, objectHandler, Directions.NE);
        var eastCommand = new ChangeLocationCommand(universe, printingSubsystem, objectHandler, Directions.E);
        var southEastCommand = new ChangeLocationCommand(universe, printingSubsystem, objectHandler, Directions.SE);
        var southCommand = new ChangeLocationCommand(universe, printingSubsystem, objectHandler, Directions.S);
        var southWestCommand = new ChangeLocationCommand(universe, printingSubsystem, objectHandler, Directions.SW);
        var westCommand = new ChangeLocationCommand(universe, printingSubsystem, objectHandler, Directions.W);
        var northWestCommand = new ChangeLocationCommand(universe, printingSubsystem, objectHandler, Directions.NW);
        var upCommand = new ChangeLocationCommand(universe, printingSubsystem, objectHandler, Directions.UP);
        var downCommand = new ChangeLocationCommand(universe, printingSubsystem, objectHandler, Directions.DOWN);
        
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
        if (adventureEvent.Predicate != null && commands.ContainsKey(adventureEvent.Predicate.Key))
        {
            var command = commands[adventureEvent.Predicate.Key];

            return command.Execute(adventureEvent);
        }

        return false;
    }
}