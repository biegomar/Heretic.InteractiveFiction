using Heretic.InteractiveFiction.GamePlay;
using Heretic.InteractiveFiction.Grammars;
using Heretic.InteractiveFiction.Objects;
using Heretic.InteractiveFiction.Subsystems;
using Heretic.InteractiveFiction.TestGame.GamePlay;
using Heretic.InteractiveFiction.TestGame.Printing;

IResourceProvider resourceProvider = new ResourceProvider();
IPrintingSubsystem printingSubsystem = new ConsolePrinting();
var universe = new Universe(printingSubsystem, resourceProvider);
var eventProvider = new EventProvider(universe, printingSubsystem);
IGamePrerequisitesAssembler gamePrerequisitesAssembler = new GamePrerequisitesAssembler(eventProvider);
IGrammar grammar = new GermanGrammar(resourceProvider);
var gameLoop = new GameLoop(printingSubsystem, universe, gamePrerequisitesAssembler, grammar);

gameLoop.Run();