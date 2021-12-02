using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

public class GameLoop
{
    private readonly Universe universe;
    private readonly InputProcessor processor;
    private readonly Queue<string> commands;
    private readonly IPrintingSubsystem printingSubsystem;

    public GameLoop(IPrintingSubsystem printingSubsystem, IResourceProvider resourceProvider, IGamePrerequisitesAssembler gamePrerequisitesAssembler, Universe universe, string fileName)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
        gamePrerequisitesAssembler.GetGameMap();
        this.processor = new InputProcessor(printingSubsystem, resourceProvider, this.universe);
        this.commands = new Queue<string>();
        if (!string.IsNullOrEmpty(fileName))
        {
            this.commands = GetCommandList(fileName);
        }

        InitializeScreen();
    }

    public void Run()
    {
        bool unfinished;
        do
        {
            printingSubsystem.Prompt();
            Console.ForegroundColor = ConsoleColor.Green;
            var input = GetInput();
            Console.ResetColor();
            unfinished = this.processor.Process(input);
        } while (unfinished);
    }

    private Queue<string> GetCommandList(string FileName)
    {
        return new(File.ReadAllLines(FileName).Where(x => !string.IsNullOrEmpty(x)).ToList());
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
        printingSubsystem.TitleAndScore(this.universe.Score, this.universe.MaxScore);
        printingSubsystem.Opening();
        printingSubsystem.ActiveLocation(this.universe.ActiveLocation, this.universe.LocationMap);
    }
}
