using Heretic.InteractiveFiction.Exceptions;
using Heretic.InteractiveFiction.GamePlay.EventSystem.EventArgs;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Resources;
using Heretic.InteractiveFiction.Subsystems;

namespace Heretic.InteractiveFiction.GamePlay;

internal sealed class InputProcessor
{
    private readonly Universe universe;
    private readonly CommandExecutor commandExecutor;
    private readonly InputAnalyzer inputAnalyzer;
    private readonly IPrintingSubsystem printingSubsystem;

    public IReadOnlyCollection<string> CommandHistory => commandExecutor.CommandHistory;

    public InputProcessor(IPrintingSubsystem printingSubsystem, IHelpSubsystem helpSubsystem, Universe universe, IGrammar grammar, IVerbHandler verbHandler, ScoreBoard scoreBoard)
    {
        this.printingSubsystem = printingSubsystem;
        this.universe = universe;
        this.commandExecutor = new CommandExecutor(this.universe, grammar, printingSubsystem, helpSubsystem, verbHandler, scoreBoard);
        this.inputAnalyzer = new InputAnalyzer(this.universe, grammar);
    }

    internal bool Process(string input)
    {
        try
        {
            if (string.IsNullOrEmpty(input))
                return true;

            this.commandExecutor.AddCommandToHistory(input);

            var analysisResult = this.inputAnalyzer.AnalyzeInput(input);

            switch (analysisResult)
            {
                case AnalysisResult.Ambiguous(var candidates):
                    return PrintAmbiguityQuestion(candidates);

                case AnalysisResult.Success(var adventureEvent):
                    var result = ProcessAdventureEvent(adventureEvent);
                    FirePeriodicEvent();
                    FireNextGameLoopEventsOnRegisteredItems();
                    printingSubsystem.TitleAndScore(this.commandExecutor.Score, this.commandExecutor.MaxScore);
                    this.universe.DidYouWin();
                    return result;

                default:
                    return true;
            }
        }
        catch (NoVerbException ex)
        {
            return printingSubsystem.Resource(ex.Message);
        }
        catch (AmbiguousHereticObjectException ex)
        {
            return printingSubsystem.Resource(ex.Message);
        }
    }

    private bool PrintAmbiguityQuestion(IReadOnlyList<AHereticObject> candidates)
    {
        var names = candidates
            .Select(c => ArticleHandler.GetNameWithArticleForObject(c, GrammarCase.Accusative, lowerFirstCharacter: true))
            .ToList();

        var joined = names.Count switch
        {
            0 => string.Empty,
            1 => names[0],
            _ => string.Join(BaseDescriptions.BINDING_OR, names)
        };

        return printingSubsystem.Resource(string.Format(BaseDescriptions.AMBIGUOUS_HERETICOBJECT_QUESTION, joined));
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

    private void FireNextGameLoopEventsOnRegisteredItems()
    {
        this.universe.OnNextGameLoop(new ContainerObjectEventArgs());
    }

    private bool ProcessAdventureEvent(AdventureEvent adventureEvent)
    {
        return this.commandExecutor.Execute(adventureEvent) || this.printingSubsystem.Misconcept();
    }
}
