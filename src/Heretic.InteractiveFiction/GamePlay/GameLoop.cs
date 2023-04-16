using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

public sealed class GameLoop
{
    private readonly IGamePrerequisitesAssembler gamePrerequisitesAssembler;
    private IPrintingSubsystem printingSubsystem;
    private InputProcessor processor;
    private Queue<string> commands;
    private const string SAVE = "SAVE";

    public GameLoop(IGamePrerequisitesAssembler gamePrerequisitesAssembler, int consoleWidth = 0)
    {
        this.gamePrerequisitesAssembler = gamePrerequisitesAssembler;
        InitializeSystem(consoleWidth);
    }

    private void InitializeSystem(int consoleWidth)
    {
        printingSubsystem = this.gamePrerequisitesAssembler.PrintingSubsystem;
        printingSubsystem.ConsoleWidth = consoleWidth;

        processor = new InputProcessor(printingSubsystem, this.gamePrerequisitesAssembler.HelpSubsystem,
            this.gamePrerequisitesAssembler.Universe, this.gamePrerequisitesAssembler.Grammar,
            this.gamePrerequisitesAssembler.VerbHandler, this.gamePrerequisitesAssembler.ScoreBoard);

        commands = new Queue<string>();

        gamePrerequisitesAssembler.AssembleGame();

        InitializeScreen();
    }

    public void Run(string fileName = "")
    {
        if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
        {
            commands = GetCommandList(fileName);
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
                unfinished = processor.Process(input);
            } while (unfinished);
        }
        catch (QuitGameException ex)
        {
            printingSubsystem.Resource(ex.Message);
        }
        catch (GameWonException)
        {
            FinalizeGame();
        }
        catch (RestartException)
        {
            RestartSystem();
        }
        catch (Exception)
        {
            printingSubsystem.Resource(BaseDescriptions.SYSTEM_ERROR);
        }
    }

    private void RestartSystem()
    {
        this.gamePrerequisitesAssembler.Restart();
        InitializeSystem(printingSubsystem.ConsoleWidth);
        Run(); 
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
            printingSubsystem.Resource(result, wordWrap: false);
            return result;
        }

        return printingSubsystem.ReadInput();
    }

    private void InitializeScreen()
    {
        printingSubsystem.ClearScreen();
        printingSubsystem.TitleAndScore(gamePrerequisitesAssembler.ScoreBoard.Score,
            gamePrerequisitesAssembler.ScoreBoard.MaxScore);
        printingSubsystem.Opening();
        printingSubsystem.ForegroundColor = TextColor.DarkCyan;
        printingSubsystem.Resource(BaseDescriptions.START_THE_GAME);
        printingSubsystem.ResetColors();
        printingSubsystem.WaitForUserAction();
        printingSubsystem.ActiveLocation(gamePrerequisitesAssembler.Universe.ActiveLocation,
            gamePrerequisitesAssembler.Universe.LocationMap);
        printingSubsystem.ForegroundColor = TextColor.DarkCyan;
        printingSubsystem.Resource(BaseDescriptions.HELP_WANTED);
        printingSubsystem.ResetColors();
    }

    private void FinalizeGame()
    {
        printingSubsystem.Closing();
        printingSubsystem.Credits();
        processor.Process(SAVE);
        printingSubsystem.ForegroundColor = TextColor.DarkCyan;
        printingSubsystem.Resource(BaseDescriptions.END_THE_GAME);
        printingSubsystem.ResetColors();
        printingSubsystem.WaitForUserAction();
    }
}
