using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

public sealed class GameLoop
{
    private readonly Universe universe;
    private readonly InputProcessor processor;
    private Queue<string> commands;
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly ScoreBoard scoreBoard;
    private const string SAVE = "SAVE";

    public GameLoop(
        IPrintingSubsystem printingSubsystem, 
        Universe universe, 
        IGamePrerequisitesAssembler gamePrerequisitesAssembler, 
        IGrammar grammar, 
        ScoreBoard scoreBoard, 
        int consoleWidth = 0)
    {
        this.printingSubsystem = printingSubsystem;
        this.printingSubsystem.ConsoleWidth = consoleWidth;
        this.scoreBoard = scoreBoard;

        this.universe = universe;
        
        this.processor = new InputProcessor(printingSubsystem, this.universe, grammar, this.scoreBoard);
        this.commands = new Queue<string>();
        
        AssemblyGame(gamePrerequisitesAssembler);
        
        InitializeScreen();
    }
    
    private void AssemblyGame(IGamePrerequisitesAssembler gamePrerequisitesAssembler)
    {
        var gamePrerequisites = gamePrerequisitesAssembler.AssembleGame();
        this.universe.LocationMap = gamePrerequisites.LocationMap;
        this.universe.ActiveLocation = gamePrerequisites.ActiveLocation;
        this.universe.ActivePlayer = gamePrerequisites.ActivePlayer;
        this.universe.Quests = gamePrerequisites.Quests;
        this.universe.SetPeriodicEvent(gamePrerequisites.PeriodicEvent);
    }

    public void Run(string fileName = "")
    {
        if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
        {
            this.commands = GetCommandList(fileName);
        }
        
        try
        {
            bool unfinished;
            do
            {
                printingSubsystem.Prompt();
                printingSubsystem.ForegroundColor = TextColor.Green;
                var input = GetInput();
                printingSubsystem.ResetColors();
                unfinished = this.processor.Process(input);
            } while (unfinished);
        }
        catch (QuitGameException ex)
        {
            printingSubsystem.Resource(ex.Message);
        }
        catch (GameWonException ex)
        {
            FinalizeGame();
        }
        catch (Exception ex)
        {
            printingSubsystem.Resource(BaseDescriptions.SYSTEM_ERROR);
        }
    }

    private Queue<string> GetCommandList(string fileName)
    {
        return new(File.ReadAllLines(fileName).Where(x => !string.IsNullOrEmpty(x)).ToList());
    }

    private string GetInput()
    {
        if (commands.Any())
        {
            var result = commands.Dequeue();
            Console.WriteLine(result);
            return result;
        }
        return Console.ReadLine();
    }

    private void InitializeScreen()
    {
        printingSubsystem.ClearScreen();
        printingSubsystem.TitleAndScore(this.scoreBoard.Score, this.scoreBoard.MaxScore);
        printingSubsystem.Opening();
        printingSubsystem.ForegroundColor = TextColor.DarkCyan;
        printingSubsystem.Resource(BaseDescriptions.START_THE_GAME);
        printingSubsystem.ResetColors();
        Console.ReadKey();
        printingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
        printingSubsystem.ForegroundColor = TextColor.DarkCyan;
        printingSubsystem.Resource(BaseDescriptions.HELP_WANTED);
        printingSubsystem.ResetColors();
    }

    private void FinalizeGame()
    {
        printingSubsystem.Closing();
        printingSubsystem.Credits();
        this.processor.Process(SAVE);
        Console.WriteLine();
        printingSubsystem.ForegroundColor = TextColor.DarkCyan;
        printingSubsystem.Resource(BaseDescriptions.END_THE_GAME);
        printingSubsystem.ResetColors();
        Console.ReadKey();
    }
}
