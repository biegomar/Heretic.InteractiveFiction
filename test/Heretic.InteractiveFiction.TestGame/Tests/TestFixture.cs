using System.Runtime.CompilerServices;
using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using Heretic.InteractiveFiction.TestGame.GamePlay;
using Moq;

namespace Heretic.InteractiveFiction.TestGame.Tests;

public sealed class TestFixture
{
    private readonly IResourceProvider resourceProvider;
    private readonly IVerbHandler verbHandler;
    private readonly IPrintingSubsystem printingSubsystem;
    private readonly IHelpSubsystem helpSubsystem;
    private readonly HistoryAdministrator historyAdministrator;
    private readonly ScoreBoard scoreBoard;
    internal Universe Universe { get; }
    internal ObjectHandler ObjectHandler { get; }
    internal CommandExecutor CommandExecutor { get; }

    public Item? Table { get; }
    public Item? Lamp { get;  }
    
    public AHereticObject? ActiveObject => this.Universe.ActiveObject;

    public TestFixture()
    {
        this.resourceProvider = new ResourceProvider();
        this.printingSubsystem = InitializePrintingSubsystem();
        this.helpSubsystem = initializeHelpSubsystem();
        this.Universe = this.GetUniverse();
        this.verbHandler = this.GetVerbHandler();
        this.ObjectHandler = new ObjectHandler(this.Universe);
        this.historyAdministrator = new HistoryAdministrator();
        this.scoreBoard = new ScoreBoard(this.printingSubsystem);
        this.CommandExecutor = this.GetCommandExecutor();
        this.Table = this.ObjectHandler.GetObjectFromWorldByKey<Item>(Keys.TABLE);
        this.Lamp = this.ObjectHandler.GetObjectFromWorldByKey<Item>(Keys.PETROLEUM_LAMP);
    }
    
    public void SetActiveObject(string itemKey)
    {
        if (this.ObjectHandler.GetObjectFromWorldByKey(itemKey) is {} item)
        {
            this.ObjectHandler.StoreAsActiveObject(item);    
        }
    }

    public void ClearActiveObject()
    {
        this.ObjectHandler.ClearActiveObject();
    }

    private CommandExecutor GetCommandExecutor()
    {
        return new CommandExecutor(this.Universe, new GermanGrammar(this.resourceProvider, this.verbHandler), this.printingSubsystem,
            this.helpSubsystem, this.verbHandler, this.historyAdministrator, this.scoreBoard);
    }

    private IVerbHandler GetVerbHandler()
    {
        return new GermanVerbHandler(this.Universe, this.resourceProvider);
    }
    
    private Universe GetUniverse()
    {
        var universe = new Universe(printingSubsystem, resourceProvider);
        
        var eventProvider = new EventProvider(universe, printingSubsystem, scoreBoard);
        
        IGamePrerequisitesAssembler gamePrerequisitesAssembler = new GamePrerequisitesAssembler();
        gamePrerequisitesAssembler.AssembleGame();
        universe = gamePrerequisitesAssembler.Universe;

        return universe;
    }

    private static IPrintingSubsystem InitializePrintingSubsystem()
    {
        var printingSubsystemMock = new Mock<IPrintingSubsystem>();
        printingSubsystemMock.SetReturnsDefault(true);
        var printingSubsystem = printingSubsystemMock.Object;
        return printingSubsystem;
    }

    private static IHelpSubsystem initializeHelpSubsystem()
    {
        var helpSubsystemMock = new Mock<IHelpSubsystem>();
        helpSubsystemMock.SetReturnsDefault(true);
        var helpSubsystem = helpSubsystemMock.Object;
        return helpSubsystem;
    }
}