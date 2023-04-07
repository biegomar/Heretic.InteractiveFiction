using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using Heretic.InteractiveFiction.TestGame.GamePlay;
using Heretic.InteractiveFiction.TestGame.Printing;

IResourceProvider resourceProvider = new ResourceProvider();
IPrintingSubsystem printingSubsystem = new ConsolePrinting();
var universe = new Universe(printingSubsystem, resourceProvider);
var scoreBoard = new ScoreBoard(printingSubsystem);
var eventProvider = new EventProvider(universe, printingSubsystem, scoreBoard);
IGamePrerequisitesAssembler gamePrerequisitesAssembler = new GamePrerequisitesAssembler(eventProvider);
IVerbHandler verbHandler = new GermanVerbHandler(universe, resourceProvider);
IGrammar grammar = new GermanGrammar(resourceProvider, verbHandler);
IHelpSubsystem helpSubsystem = new BaseHelpSubsystem(grammar, printingSubsystem);
var gameLoop = new GameLoop(printingSubsystem, helpSubsystem, universe, gamePrerequisitesAssembler, grammar, scoreBoard);

gameLoop.Run();