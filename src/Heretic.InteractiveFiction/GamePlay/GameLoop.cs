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
        InitializeScreen();
    }

    private void InitializeSystem(int consoleWidth)
    {
        printingSubsystem = gamePrerequisitesAssembler.PrintingSubsystem;
        printingSubsystem.ConsoleWidth = consoleWidth;

        processor = new InputProcessor(printingSubsystem, gamePrerequisitesAssembler.HelpSubsystem,
            gamePrerequisitesAssembler.Universe, gamePrerequisitesAssembler.Grammar,
            gamePrerequisitesAssembler.VerbHandler, gamePrerequisitesAssembler.ScoreBoard);

        commands = new Queue<string>();

        gamePrerequisitesAssembler.AssembleGame();
    }

    public void Run(string fileName = "", bool EnableSilentModeOnCommandsInList = false)
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
                if (EnableSilentModeOnCommandsInList)
                {
                    printingSubsystem.IsInSilentMode = commands.Any();
                }
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
        catch (RevertException ex)
        {
            printingSubsystem.Resource(ex.Message);
            RevertCommand();
        }
        catch (Exception)
        {
            printingSubsystem.Resource(BaseDescriptions.SYSTEM_ERROR);
        }
    }

    private void RestartSystem()
    {
        gamePrerequisitesAssembler.Restart();
        InitializeSystem(printingSubsystem.ConsoleWidth);
        InitializeScreen();
        Run(); 
    }
    
    private void RevertCommand()
    {
        var oldCommands = processor.HistoryAdministrator.All.Take(processor.HistoryAdministrator.All.Count - 2);
        gamePrerequisitesAssembler.Restart();
        InitializeSystem(printingSubsystem.ConsoleWidth);
        commands = new Queue<string>(oldCommands);
        Run(EnableSilentModeOnCommandsInList: true); 
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
