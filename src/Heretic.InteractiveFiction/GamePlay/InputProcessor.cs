using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

public sealed class InputProcessor
{
    private readonly Universe universe;
    private readonly CommandExecutor commandExecutor;
    private readonly HistoryAdministrator historyAdministrator;
    private readonly InputAnalyzer inputAnalyzer;
    private readonly IPrintingSubsystem printingSubsystem;

    public InputProcessor(IPrintingSubsystem printingSubsystem, Universe universe, IGrammar grammar)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
        this.historyAdministrator = new HistoryAdministrator();
        this.commandExecutor = new CommandExecutor(this.universe, grammar, printingSubsystem, this.historyAdministrator);
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

            printingSubsystem.TitleAndScore(universe.Score, universe.MaxScore);
        
            if (this.universe.Quests.Count == this.universe.NumberOfSolvedQuests)
            {
                throw new GameWonException();
            }
        
            return result;
        }
        catch (NoVerbException ex)
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
