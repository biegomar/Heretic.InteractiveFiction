using System.Threading;
using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
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

    public InputProcessor(IPrintingSubsystem printingSubsystem, Universe universe)
    {
        this.PrintingSubsystem = printingSubsystem;
        this.universe = universe;
        this.verbHandler = new VerbHandler(this.universe, printingSubsystem);
        this.inputAnalyzer = new InputAnalyzer(this.universe);
        this.historyAdministrator = new HistoryAdministrator(this.inputAnalyzer);
    }

    internal bool Process(string input)
    {
        if (verbHandler.Quit(input))
        {
            throw new QuitGameException(BaseDescriptions.QUIT_GAME);
        }
        
        if (verbHandler.Save(input, this.historyAdministrator.All))
        {
            return true;
        }

        var sentence = this.inputAnalyzer.Analyze(input);

        this.historyAdministrator.Add(input);

        if (this.universe.IsVerb(VerbKeys.REM, sentence[0]))
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

    private bool ProcessSingleVerb(string input)
    {
        var processingInput = input.ToLower();

        var result = verbHandler.Directions(processingInput);
        result = result || verbHandler.ChangeLocationByName(processingInput);
        result = result || verbHandler.Look(processingInput);
        result = result || verbHandler.Take(processingInput);
        result = result || verbHandler.Inventory(processingInput);
        result = result || verbHandler.Ways(processingInput);
        result = result || verbHandler.Score(processingInput);
        result = result || verbHandler.Help(processingInput);
        result = result || verbHandler.Credits(processingInput);
        result = result || verbHandler.SitDown(processingInput);
        result = result || verbHandler.StandUp(processingInput);
        result = result || verbHandler.Descend(processingInput);
        result = result || verbHandler.Remark(processingInput);
        result = result || verbHandler.Wait(processingInput);
        result = result || verbHandler.History(processingInput, this.historyAdministrator.All);

        if (!result)
        {
            this.PrintingSubsystem.Misconcept();
        }

        return true;
    }

    private bool ProcessTwoWords(string verb, string subject)
    {
        var processingVerb = verb.ToLower();
        var processingSubject = subject.ToLower();

        var commaSeparatedList = processingSubject.Split(",");

        var result = verbHandler.Look(processingVerb, processingSubject);
        result = result || verbHandler.Go(processingVerb, subject);
        result = result || verbHandler.Name(processingVerb, subject);
        result = result || verbHandler.Take(processingVerb, commaSeparatedList);
        result = result || verbHandler.Talk(processingVerb, processingSubject);
        result = result || verbHandler.Use(processingVerb, processingSubject);
        result = result || verbHandler.Buy(processingVerb, processingSubject);
        result = result || verbHandler.Open(processingVerb, processingSubject);
        result = result || verbHandler.Close(processingVerb, processingSubject);
        result = result || verbHandler.Drop(processingVerb, commaSeparatedList);
        result = result || verbHandler.Pull(processingVerb, processingSubject);
        result = result || verbHandler.Push(processingVerb, processingSubject);
        result = result || verbHandler.Turn(processingVerb, processingSubject);
        result = result || verbHandler.Jump(processingVerb, processingSubject);
        result = result || verbHandler.Cut(processingVerb, processingSubject);
        result = result || verbHandler.Climb(processingVerb, processingSubject);
        result = result || verbHandler.AlterEgo(processingVerb, processingSubject);
        result = result || verbHandler.Unlock(processingVerb, processingSubject);
        result = result || verbHandler.Break(processingVerb, processingSubject);
        result = result || verbHandler.SitDown(processingVerb, processingSubject);
        result = result || verbHandler.Eat(processingVerb, processingSubject);
        result = result || verbHandler.Drink(processingVerb, processingSubject);
        result = result || verbHandler.Read(processingVerb, processingSubject);
        result = result || verbHandler.Write(processingVerb, processingSubject);
        result = result || verbHandler.Hint(processingVerb, processingSubject);

        if (!result)
        {
            this.PrintingSubsystem.Misconcept();
        }

        return true;
    }

    private bool ProcessThreeWords(string verb, string subject, string objectName)
    {
        var processingVerb = verb.ToLower();
        var processingSubject = subject.ToLower();
        var processingObject = objectName.ToLower();
        
        var commaSeparatedList = processingObject.Split(",");

        var result = verbHandler.Ask(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Look(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Take(processingVerb, processingSubject, commaSeparatedList);
        result = result || verbHandler.Say(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Give(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Unlock(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Cut(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Use(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Pull(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Push(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Break(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.SitDown(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Climb(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Drop(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Buy(processingVerb, processingSubject, processingObject);

        if (!result)
        {
            this.PrintingSubsystem.Misconcept();
        }

        return true;
    }
}
