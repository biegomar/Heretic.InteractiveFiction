using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

public sealed class InputProcessor
{
    private readonly Universe universe;
    private readonly CommandExecutor commandExecutor;
    private readonly HistoryAdministrator historyAdministrator;
    private readonly InputAnalyzer inputAnalyzer;
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly ScoreBoard scoreBoard;

    public InputProcessor(IPrintingSubsystem printingSubsystem, IHelpSubsystem helpSubsystem, Universe universe, IGrammar grammar, IVerbHandler verbHandler, ScoreBoard scoreBoard)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
        this.historyAdministrator = new HistoryAdministrator();
        this.scoreBoard = scoreBoard;
        this.commandExecutor = new CommandExecutor(this.universe, grammar, printingSubsystem, helpSubsystem, verbHandler, this.historyAdministrator, this.scoreBoard);
        this.inputAnalyzer = new InputAnalyzer(this.universe, grammar);
    }

    internal bool Process(string input)
    {
        try
        {
            if (string.IsNullOrEmpty(input))
            {
                return true;
            }

            this.historyAdministrator.Add(input);

            var adventureEvent = this.inputAnalyzer.AnalyzeInput(input);
            var result = ProcessAdventureEvent(adventureEvent);

            FirePeriodicEvent();

            printingSubsystem.TitleAndScore(scoreBoard.Score, scoreBoard.MaxScore);
            
            this.universe.DidYouWin();

            return result;
        }
        catch (NoVerbException)
        {
            return printingSubsystem.Misconcept();
        }
        catch (AmbiguousHereticObjectException ex)
        {
            return printingSubsystem.Resource(ex.Message);
        }
    }

    private void FirePeriodicEvent()
    {
        var eventArgs = new PeriodicEventArgs();
        try
        {
            this.universe.RaisePeriodicEvents(eventArgs);
            if (!string.IsNullOrEmpty(eventArgs.Message))
            {
                printingSubsystem.ForegroundColor = TextColor.Magenta;
                printingSubsystem.Resource(eventArgs.Message);
                printingSubsystem.ResetColors();
            }
        }
        catch (PeriodicException ex)
        {
            printingSubsystem.ForegroundColor = TextColor.Magenta;
            printingSubsystem.Resource(ex.Message);
            printingSubsystem.ResetColors();
        }
    }

    private bool ProcessAdventureEvent(AdventureEvent adventureEvent)
    {
        return this.commandExecutor.Execute(adventureEvent) || this.printingSubsystem.Misconcept();
    }
}
