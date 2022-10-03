using System.Threading;
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
    private readonly VerbHandler verbHandler;
    private readonly HistoryAdministrator historyAdministrator;
    private readonly InputAnalyzer inputAnalyzer;
    private readonly IPrintingSubsystem PrintingSubsystem;

    public InputProcessor(IPrintingSubsystem printingSubsystem, Universe universe, IGrammar grammar)
    {
        this.PrintingSubsystem = printingSubsystem;
        this.universe = universe;
        this.verbHandler = new VerbHandler(this.universe, grammar, printingSubsystem);
        this.inputAnalyzer = new InputAnalyzer(this.universe, grammar);
        this.historyAdministrator = new HistoryAdministrator(this.inputAnalyzer);
    }

    internal bool Process(string input)
    {
        var sentence = this.inputAnalyzer.Analyze(input);

        this.historyAdministrator.Add(input);

        if (verbHandler.Save(sentence[0], this.historyAdministrator.All))
        {
            return true;
        }
        
        if (VerbKeys.REM == sentence[0])
        {
            return true;
        }
        
        var result = sentence.Length switch
        {
            1 => this.ProcessSingleVerb(sentence[0]),
            2 => this.ProcessTwoWords(sentence[0], sentence[1]),
            3 => this.ProcessThreeWords(sentence[0], sentence[1], sentence[2]),
            _ => PrintingSubsystem.Misconcept()
        };

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

    private bool ProcessSingleVerb(string verb)
    {
        var result = verbHandler.Quit(verb);
        result = result || verbHandler.Directions(verb);
        result = result || verbHandler.ChangeLocationByName(verb);
        result = result || verbHandler.Look(verb);
        result = result || verbHandler.Take(verb);
        result = result || verbHandler.Inventory(verb);
        result = result || verbHandler.Ways(verb);
        result = result || verbHandler.Score(verb);
        result = result || verbHandler.Help(verb);
        result = result || verbHandler.Credits(verb);
        result = result || verbHandler.SitDown(verb);
        result = result || verbHandler.StandUp(verb);
        result = result || verbHandler.Descend(verb);
        result = result || verbHandler.AlterEgo(verb);
        result = result || verbHandler.Wait(verb);
        result = result || verbHandler.Sleep(verb);
        result = result || verbHandler.Smell(verb);
        result = result || verbHandler.Taste(verb);
        result = result || verbHandler.History(verb, this.historyAdministrator.All);

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

        var result = verbHandler.Look(verb, processingSubject);
        result = result || verbHandler.Go(verb, subject);
        result = result || verbHandler.Take(verb, commaSeparatedList);
        result = result || verbHandler.Talk(verb, processingSubject);
        result = result || verbHandler.Use(verb, processingSubject);
        result = result || verbHandler.Buy(verb, processingSubject);
        result = result || verbHandler.Open(verb, processingSubject);
        result = result || verbHandler.Close(verb, processingSubject);
        result = result || verbHandler.Drop(verb, commaSeparatedList);
        result = result || verbHandler.Pull(verb, processingSubject);
        result = result || verbHandler.Push(verb, processingSubject);
        result = result || verbHandler.Turn(verb, processingSubject);
        result = result || verbHandler.Jump(verb, processingSubject);
        result = result || verbHandler.Sleep(verb, processingSubject);
        result = result || verbHandler.Smell(verb, processingSubject);
        result = result || verbHandler.Taste(verb, processingSubject);
        result = result || verbHandler.Cut(verb, processingSubject);
        result = result || verbHandler.Climb(verb, processingSubject);
        result = result || verbHandler.Descend(verb, processingSubject);
        result = result || verbHandler.Kindle(verb, processingSubject);
        result = result || verbHandler.AlterEgo(verb, processingSubject);
        result = result || verbHandler.Lock(verb, processingSubject);
        result = result || verbHandler.Unlock(verb, processingSubject);
        result = result || verbHandler.Break(verb, processingSubject);
        result = result || verbHandler.SitDown(verb, processingSubject);
        result = result || verbHandler.Eat(verb, processingSubject);
        result = result || verbHandler.Wear(verb, processingSubject);
        result = result || verbHandler.Drink(verb, processingSubject);
        result = result || verbHandler.Read(verb, processingSubject);
        result = result || verbHandler.SwitchOn(verb, processingSubject);
        result = result || verbHandler.SwitchOff(verb, processingSubject);
        result = result || verbHandler.Write(verb, processingSubject);
        result = result || verbHandler.Hint(verb, processingSubject);

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
        result = result || verbHandler.Look(verb, processingSubject, processingObject);
        result = result || verbHandler.Take(verb, processingSubject, commaSeparatedList);
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
