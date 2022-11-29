using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

public sealed class InputProcessor
{
    private readonly Universe universe;
    private readonly VerbHandler verbHandler;
    private readonly CommandExecutor commandExecutor;
    private readonly HistoryAdministrator historyAdministrator;
    private readonly InputAnalyzer inputAnalyzer;
    private readonly IPrintingSubsystem PrintingSubsystem;

    public InputProcessor(IPrintingSubsystem printingSubsystem, Universe universe, IGrammar grammar)
    {
        this.PrintingSubsystem = printingSubsystem;
        this.universe = universe;
        this.verbHandler = new VerbHandler(this.universe, grammar, printingSubsystem);
        this.commandExecutor = new CommandExecutor(this.universe, grammar, printingSubsystem);
        this.inputAnalyzer = new InputAnalyzer(this.universe, grammar);
        this.historyAdministrator = new HistoryAdministrator(this.inputAnalyzer);
    }

    internal bool Process(string input)
    {
        var sentence = this.inputAnalyzer.Analyze(input);
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

        var result =ProcessAdventureEvent(adventureEvent);

        if (!result)
        {
            result = sentence.Length switch
            {
                1 => true,
                2 => this.ProcessTwoWords(sentence[0], sentence[1]),
                3 => this.ProcessThreeWords(sentence[0], sentence[1], sentence[2]),
                _ => PrintingSubsystem.Misconcept()
            };
        }

        FirePeriodicEvent();

        PrintingSubsystem.TitleAndScore(universe.Score, universe.MaxScore);
        
        if (this.universe.Quests.Count == this.universe.NumberOfSolvedQuests)
        {
            throw new GameWonException();
        }
        
        return result;
    }

    private void FirePeriodicEvent()
    {
        var eventArgs = new PeriodicEventArgs();
        try
        {
            this.universe.RaisePeriodicEvents(eventArgs);
            if (!string.IsNullOrEmpty(eventArgs.Message))
            {
                PrintingSubsystem.ForegroundColor = TextColor.Magenta;
                PrintingSubsystem.Resource(eventArgs.Message);
                PrintingSubsystem.ResetColors();
            }
        }
        catch (PeriodicException ex)
        {
            PrintingSubsystem.ForegroundColor = TextColor.Magenta;
            PrintingSubsystem.Resource(ex.Message);
            PrintingSubsystem.ResetColors();
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
        result = result || commandExecutor.Ways(adventureEvent);
        result = result || commandExecutor.Help(adventureEvent);
        result = result || commandExecutor.History(adventureEvent, this.historyAdministrator.All);
        result = result || commandExecutor.Use(adventureEvent);
        result = result || commandExecutor.Buy(adventureEvent);
        result = result || commandExecutor.Open(adventureEvent);
        result = result || commandExecutor.Close(adventureEvent);
        result = result || commandExecutor.Talk(adventureEvent);
        result = result || commandExecutor.Pull(adventureEvent);
        result = result || commandExecutor.Push(adventureEvent);
        result = result || commandExecutor.Turn(adventureEvent);
        result = result || commandExecutor.Jump(adventureEvent);
        result = result || commandExecutor.Cut(adventureEvent);
        result = result || commandExecutor.Climb(adventureEvent);
        result = result || commandExecutor.Kindle(adventureEvent);
        result = result || commandExecutor.Lock(adventureEvent);
        result = result || commandExecutor.Unlock(adventureEvent);
        result = result || commandExecutor.Break(adventureEvent);
        result = result || commandExecutor.Eat(adventureEvent);
        result = result || commandExecutor.Wear(adventureEvent);
        result = result || commandExecutor.Read(adventureEvent);
        result = result || commandExecutor.Go(adventureEvent);
        result = result || commandExecutor.SwitchOn(adventureEvent);
        result = result || commandExecutor.SwitchOff(adventureEvent);
        result = result || commandExecutor.Write(adventureEvent);
        result = result || commandExecutor.Hint(adventureEvent);

        if (!result)
        {
            this.PrintingSubsystem.Misconcept();
        }
        
        return true;
    }

    private bool ProcessTwoWords(string verb, string subject)
    {
        var processingSubject = subject.ToLower();

        var commaSeparatedList = processingSubject.Split(",");
        var result = verbHandler.Take(verb, commaSeparatedList);
        result = result || verbHandler.Drop(verb, commaSeparatedList);

        if (!result)
        {
            this.PrintingSubsystem.Misconcept();
        }

        return true;
    }

    private bool ProcessThreeWords(string verb, string subject, string objectName)
    {
        var processingSubject = subject.ToLower();
        var processingObject = objectName.ToLower();
        
        var commaSeparatedList = processingObject.Split(",");

        var result = verbHandler.Ask(verb, processingSubject, processingObject);
        //result = result || verbHandler.Look(verb, processingSubject, processingObject);
        //result = result || verbHandler.Take(verb, processingSubject, commaSeparatedList);
        result = result || verbHandler.Say(verb, processingSubject, processingObject);
        result = result || verbHandler.Give(verb, processingSubject, processingObject);
        result = result || verbHandler.Lock(verb, processingSubject, processingObject);
        result = result || verbHandler.Unlock(verb, processingSubject, processingObject);
        result = result || verbHandler.Cut(verb, processingSubject, processingObject);
        result = result || verbHandler.Use(verb, processingSubject, processingObject);
        result = result || verbHandler.Pull(verb, processingSubject, processingObject);
        result = result || verbHandler.Push(verb, processingSubject, processingObject);
        result = result || verbHandler.Break(verb, processingSubject, processingObject);
        result = result || verbHandler.SitDown(verb, processingSubject, processingObject);
        result = result || verbHandler.Climb(verb, processingSubject, processingObject);
        result = result || verbHandler.Kindle(verb, processingSubject, processingObject);
        result = result || verbHandler.Drop(verb, processingSubject, processingObject);
        result = result || verbHandler.Buy(verb, processingSubject, processingObject);
        result = result || verbHandler.SwitchOn(verb, processingSubject, processingObject);
        result = result || verbHandler.SwitchOff(verb, processingSubject, processingObject);
        result = result || verbHandler.Wear(verb, processingSubject, processingObject);
        result = result || verbHandler.ToBe(verb, processingSubject, objectName);

        if (!result)
        {
            this.PrintingSubsystem.Misconcept();
        }

        return true;
    }
}
