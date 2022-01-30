using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

public sealed class InputProcessor
{
    private readonly Universe universe;
    private readonly VerbHandler verbHandler;
    private readonly HistoryAdministrator historyAdministrator;
    private readonly InputAnalyzer inputAnalyzer;
    private readonly IPrintingSubsystem PrintingSubsystem;

    public InputProcessor(IPrintingSubsystem printingSubsystem, IResourceProvider resourceProvider, Universe universe)
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
            return false;
        }

        var sentence = this.inputAnalyzer.Analyze(input);

        if (this.historyAdministrator.IsLastHistoryEntryTheSame(sentence))
        {
            PrintingSubsystem.SameActionAgain();
        }

        this.historyAdministrator.Add(input);

        var result = sentence.Length switch
        {
            1 => this.ProcessSingleVerb(sentence[0]),
            2 => this.ProcessTwoWords(sentence[0], sentence[1]),
            3 => this.ProcessThreeWords(sentence[0], sentence[1], sentence[2]),
            _ => PrintingSubsystem.Misconcept()
        };

        var eventArgs = new PeriodicEventArgs();
        this.universe.RaisePeriodicEvents(eventArgs);
        if (!string.IsNullOrEmpty(eventArgs.Message))
        {
            PrintingSubsystem.Resource(eventArgs.Message);
        }

        PrintingSubsystem.TitleAndScore(universe.Score, universe.MaxScore);

        return result;
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
        result = result || verbHandler.AlterEgo(processingVerb, processingSubject);
        result = result || verbHandler.Unlock(processingVerb, processingSubject);
        result = result || verbHandler.Break(processingVerb, processingSubject);
        result = result || verbHandler.Eat(processingVerb, processingSubject);
        result = result || verbHandler.SitDown(processingVerb, processingSubject);
        result = result || verbHandler.Write(processingVerb, processingSubject);

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

        var result = verbHandler.Ask(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Say(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Give(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Unlock(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Use(processingVerb, processingSubject, processingObject);
        result = result || verbHandler.Break(processingVerb, processingSubject, processingObject);

        if (!result)
        {
            this.PrintingSubsystem.Misconcept();
        }

        return true;
    }
}
