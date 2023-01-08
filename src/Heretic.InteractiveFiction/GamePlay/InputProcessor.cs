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

    public InputProcessor(IPrintingSubsystem printingSubsystem, Universe universe, IGrammar grammar)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
        this.commandExecutor = new CommandExecutor(this.universe, grammar, printingSubsystem);
        this.inputAnalyzer = new InputAnalyzer(this.universe, grammar);
        this.historyAdministrator = new HistoryAdministrator();
    }

    internal bool Process(string input)
    {
        try
        {
            var adventureEvent = this.inputAnalyzer.AnalyzeInput(input);
            
            this.historyAdministrator.Add(input);

            if (commandExecutor.Save(adventureEvent, this.historyAdministrator.All))
            {
                return true;
            }
        
            if (VerbKeys.REM == adventureEvent.Predicate.Key)
            {
                return true;
            }

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
        var result = this.commandExecutor.Quit(adventureEvent);
        result = result || commandExecutor.Look(adventureEvent);
        result = result || commandExecutor.Directions(adventureEvent);
        result = result || commandExecutor.Take(adventureEvent);
        result = result || commandExecutor.Inventory(adventureEvent);
        result = result || commandExecutor.Score(adventureEvent);
        result = result || commandExecutor.Credits(adventureEvent);
        result = result || commandExecutor.Wait(adventureEvent);
        result = result || commandExecutor.Sleep(adventureEvent);
        result = result || commandExecutor.Smell(adventureEvent);
        result = result || commandExecutor.Taste(adventureEvent);
        result = result || commandExecutor.AlterEgo(adventureEvent);
        result = result || commandExecutor.SitDown(adventureEvent);
        result = result || commandExecutor.StandUp(adventureEvent);
        result = result || commandExecutor.Descend(adventureEvent);
        result = result || commandExecutor.Drink(adventureEvent);
        result = result || commandExecutor.Ways(adventureEvent);
        result = result || commandExecutor.Help(adventureEvent);
        result = result || commandExecutor.History(adventureEvent, this.historyAdministrator.All);
        result = result || commandExecutor.Use(adventureEvent);
        result = result || commandExecutor.Buy(adventureEvent);
        result = result || commandExecutor.Open(adventureEvent);
        result = result || commandExecutor.Give(adventureEvent);
        result = result || commandExecutor.Close(adventureEvent);
        result = result || commandExecutor.Talk(adventureEvent);
        result = result || commandExecutor.Pull(adventureEvent);
        result = result || commandExecutor.Push(adventureEvent);
        result = result || commandExecutor.PutOn(adventureEvent);
        result = result || commandExecutor.Turn(adventureEvent);
        result = result || commandExecutor.Jump(adventureEvent);
        result = result || commandExecutor.Cut(adventureEvent);
        result = result || commandExecutor.Climb(adventureEvent);
        result = result || commandExecutor.Connect(adventureEvent);
        result = result || commandExecutor.Disconnect(adventureEvent);
        result = result || commandExecutor.Kindle(adventureEvent);
        result = result || commandExecutor.Lock(adventureEvent);
        result = result || commandExecutor.Unlock(adventureEvent);
        result = result || commandExecutor.Break(adventureEvent);
        result = result || commandExecutor.Eat(adventureEvent);
        result = result || commandExecutor.Wear(adventureEvent);
        result = result || commandExecutor.TakeOff(adventureEvent);
        result = result || commandExecutor.Read(adventureEvent);
        result = result || commandExecutor.Go(adventureEvent);
        result = result || commandExecutor.SwitchOn(adventureEvent);
        result = result || commandExecutor.SwitchOff(adventureEvent);
        result = result || commandExecutor.Write(adventureEvent);
        result = result || commandExecutor.Hint(adventureEvent);
        result = result || commandExecutor.Drop(adventureEvent);
        result = result || commandExecutor.ToBe(adventureEvent);
        result = result || commandExecutor.Say(adventureEvent);
        result = result || commandExecutor.Ask(adventureEvent);

        if (!result)
        {
            this.printingSubsystem.Misconcept();
        }
        
        return true;
    }
}
