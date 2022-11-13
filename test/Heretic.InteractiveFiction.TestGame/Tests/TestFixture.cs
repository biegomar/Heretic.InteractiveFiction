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
    private readonly IPrintingSubsystem printingSubsystem;
    internal Universe Universe { get; }
    internal ObjectHandler ObjectHandler { get; }
    internal VerbHandler VerbHandler { get; }

    public Item Table { get;}
    public AHereticObject? ActiveObject => this.Universe.ActiveObject;

    public TestFixture()
    {
        this.resourceProvider = new ResourceProvider();
        this.printingSubsystem = InitializePrintingSubsystem();
        this.Universe = this.GetUniverse();
        this.ObjectHandler = new ObjectHandler(this.Universe);
        this.VerbHandler = this.GetVerbHandler();
        this.Table = (Item)this.ObjectHandler.GetObjectFromWorldByKey(Keys.TABLE);
    }
    
    public void SetActiveObject(string itemKey)
    {
        
        var item = this.ObjectHandler.GetObjectFromWorldByKey(itemKey);
        this.ObjectHandler.StoreAsActiveObject(item);
    }

    public void ClearActiveObject()
    {
        this.ObjectHandler.ClearActiveObject();
    }

    private VerbHandler GetVerbHandler()
    {
        return new VerbHandler(this.Universe, new GermanGrammar(this.resourceProvider), this.printingSubsystem);
    }
    
    private Universe GetUniverse()
    {
        var printingSubsystem = InitializePrintingSubsystem();

        var universe = new Universe(printingSubsystem, resourceProvider);
        
        var eventProvider = new EventProvider(universe, printingSubsystem);
        
        IGamePrerequisitesAssembler gamePrerequisitesAssembler = new GamePrerequisitesAssembler(eventProvider);
        var gamePrerequisites = gamePrerequisitesAssembler.AssembleGame();

        universe.LocationMap = gamePrerequisites.LocationMap;
        universe.ActiveLocation = gamePrerequisites.ActiveLocation;
        universe.ActivePlayer = gamePrerequisites.ActivePlayer;
        universe.Quests = gamePrerequisites.Quests;
        universe.SetPeriodicEvent(gamePrerequisites.PeriodicEvent);

        return universe;
    }

    private static IPrintingSubsystem InitializePrintingSubsystem()
    {
        var printingSubsystemMock = new Mock<IPrintingSubsystem>();
        printingSubsystemMock.SetReturnsDefault(true);
        var printingSubsystem = printingSubsystemMock.Object;
        return printingSubsystem;
    }
}